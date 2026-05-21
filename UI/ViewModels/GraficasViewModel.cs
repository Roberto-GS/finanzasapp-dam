using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinanzasApp.Core.DTOs.Movimiento;
using FinanzasApp.UI.Services;
using Microcharts;
using SkiaSharp;
using System.Collections.ObjectModel;

namespace FinanzasApp.UI.ViewModels;

/// <summary>
/// ViewModel de la página de gráficas. Genera la gráfica circular
/// de distribución por categoría y la gráfica de barras de evolución mensual.
/// </summary>
public partial class GraficasViewModel : BaseViewModel
{
    private readonly IApiService _apiService;
    private readonly INavigationService _navigationService;

    [ObservableProperty]
    private bool _mostrandoGastos = true;

    [ObservableProperty]
    private bool _mostrandoIngresos;

    [ObservableProperty]
    private int _anioSeleccionado;

    [ObservableProperty]
    private int _mesSeleccionado;

    [ObservableProperty]
    private string _mesAnioTexto = string.Empty;

    [ObservableProperty]
    private DonutChart? _graficaCircular;

    [ObservableProperty]
    private BarChart? _graficaBarras;

    [ObservableProperty]
    private decimal _totalGeneral;

    [ObservableProperty]
    private bool _sinDatosCircular;

    [ObservableProperty]
    private bool _sinDatosBarras;

    // Para evitar llamadas simultáneas que causan duplicados
    private bool _cargando;

    public ObservableCollection<GraficaCategoriaDto> DatosCategorias { get; } = [];

    // Tipo actual en formato string para la API
    private string TipoActual => MostrandoGastos ? "gasto" : "ingreso";

    public GraficasViewModel(
        IApiService apiService,
        INavigationService navigationService)
    {
        _apiService = apiService;
        _navigationService = navigationService;
        Titulo = "Gráficas";

        DateTime ahora = DateTime.Now;
        AnioSeleccionado = ahora.Year;
        MesSeleccionado = ahora.Month;
        MostrandoIngresos = false;
        ActualizarTextoMes();
    }

    /// <summary>
    /// Carga las dos gráficas en paralelo: circular por categoría
    /// y barras de evolución mensual del año seleccionado.
    /// </summary>
    [RelayCommand]
    private async Task CargarGraficasAsync()
    {
        if (_cargando) return;
        _cargando = true;

        await EjecutarSeguro(async () =>
        {
            // Limpiamos antes de cargar para evitar datos residuales
            GraficaCircular = null;
            GraficaBarras = null;
            DatosCategorias.Clear();
            TotalGeneral = 0;
            SinDatosCircular = false;
            SinDatosBarras = false;

            Task<List<GraficaCategoriaDto>> categoriasTask =
                _apiService.ObtenerGraficaCategorias(
                    TipoActual, AnioSeleccionado, MesSeleccionado);

            Task<List<GraficaMensualDto>> mensualTask =
                _apiService.ObtenerGraficaMensual(TipoActual, AnioSeleccionado);

            await Task.WhenAll(categoriasTask, mensualTask);

            List<GraficaCategoriaDto> categorias = await categoriasTask;
            List<GraficaMensualDto> mensual = await mensualTask;

            SinDatosCircular = categorias.Count == 0;
            SinDatosBarras = mensual.All(m => m.Total == 0);
            TotalGeneral = categorias.Sum(c => c.Total);

            // Pequeña pausa para evitar colisiones en el renderifado de las gráficas
            await Task.Delay(80);

            if (DatosCategorias.Count == 0)
            {
                foreach (GraficaCategoriaDto c in categorias)
                    DatosCategorias.Add(c);
            }

            if (!SinDatosCircular)
                GraficaCircular = GenerarGraficaCircular(categorias);

            if (!SinDatosBarras)
                GraficaBarras = GenerarGraficaBarras(mensual);

        }, "Error al cargar las gráficas");

        _cargando = false;
    }

    /// <summary>
    /// Genera la gráfica circular de distribución de gastos o ingresos
    /// por categoría para el mes seleccionado.
    /// </summary>
    private static DonutChart GenerarGraficaCircular(List<GraficaCategoriaDto> datos)
    {
        ChartEntry[] entradas = datos.Select(d =>
        {
            SKColor color = SKColor.TryParse(d.CategoriaColor, out SKColor c)
                ? c
                : SKColor.Parse("#2563EB");

            return new ChartEntry((float)d.Total)
            {
                Label = d.CategoriaNombre,
                ValueLabel = $"{d.Porcentaje}%",
                Color = color,
                TextColor = SKColors.Gray,
                ValueLabelColor = color
            };
        }).ToArray();

        return new DonutChart
        {
            Entries = entradas,
            LabelTextSize = 28,
            BackgroundColor = SKColors.Transparent,
            HoleRadius = 0.4f,
            AnimationDuration = TimeSpan.FromMilliseconds(400)
        };
    }

    /// <summary>
    /// Genera la gráfica de barras de evolución mensual
    /// para el año seleccionado.
    /// </summary>
    private static BarChart GenerarGraficaBarras(List<GraficaMensualDto> datos)
    {
        SKColor colorPrimario = SKColor.Parse("#2563EB");
        SKColor colorVacio = SKColor.Parse("#E5E7EB");

        ChartEntry[] entradas = datos.Select(d => new ChartEntry((float)d.Total)
        {
            Label = d.MesNombre,
            ValueLabel = d.Total > 0 ? $"{d.Total:F0}" : string.Empty,
            Color = d.Total > 0 ? colorPrimario : colorVacio,
            TextColor = SKColors.Gray,
            ValueLabelColor = colorPrimario
        }).ToArray();

        var maxValor = entradas.Length > 0
            ? entradas.Max(e => e.Value)
            : 0f;

        return new BarChart
        {
            Entries = entradas,
            LabelTextSize = 28,
            ValueLabelTextSize = 24,
            BackgroundColor = SKColors.Transparent,
            LabelColor = SKColors.Gray,
            AnimationDuration = TimeSpan.FromMilliseconds(400),
            MaxValue = (float)(maxValor > 0 ? maxValor * 1.2f : 100f)
        };
    }

    [RelayCommand]
    private async Task SeleccionarGastoAsync()
    {
        if (MostrandoGastos) return;
        MostrandoGastos = true;
        MostrandoIngresos = false;
        await CargarGraficasAsync();
    }

    [RelayCommand]
    private async Task SeleccionarIngresoAsync()
    {
        if (MostrandoIngresos) return;
        MostrandoGastos = false;
        MostrandoIngresos = true;
        await CargarGraficasAsync();
    }

    [RelayCommand]
    private async Task MesAnteriorAsync()
    {
        DateTime fecha = new DateTime(
            AnioSeleccionado, MesSeleccionado, 1).AddMonths(-1);
        AnioSeleccionado = fecha.Year;
        MesSeleccionado = fecha.Month;
        ActualizarTextoMes();
        await CargarGraficasAsync();
    }

    [RelayCommand]
    private async Task MesSiguienteAsync()
    {
        DateTime ahora = DateTime.Now;
        DateTime siguiente = new DateTime(
            AnioSeleccionado, MesSeleccionado, 1).AddMonths(1);

        if (siguiente.Year > ahora.Year ||
           (siguiente.Year == ahora.Year && siguiente.Month > ahora.Month))
            return;

        AnioSeleccionado = siguiente.Year;
        MesSeleccionado = siguiente.Month;
        ActualizarTextoMes();
        await CargarGraficasAsync();
    }

    [RelayCommand]
    private async Task AnioAnteriorAsync()
    {
        AnioSeleccionado--;
        await CargarGraficasAsync();
    }

    [RelayCommand]
    private async Task AnioSiguienteAsync()
    {
        if (AnioSeleccionado >= DateTime.Now.Year) return;
        AnioSeleccionado++;
        await CargarGraficasAsync();
    }

    [RelayCommand]
    private async Task VolverAsync()
        => await Shell.Current.GoToAsync("//DashboardPage");

    private void ActualizarTextoMes()
    {
        DateTime fecha = new DateTime(AnioSeleccionado, MesSeleccionado, 1);
        MesAnioTexto = fecha.ToString("MMMM yyyy");
    }
}
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinanzasApp.Core.DTOs.Categoria;
using FinanzasApp.Core.DTOs.Movimiento;
using FinanzasApp.Core.Enums;
using FinanzasApp.UI.Services;
using FinanzasApp.UI.Views;
using System.Collections.ObjectModel;

namespace FinanzasApp.UI.ViewModels;

/// <summary>
/// ViewModel de la búsqueda avanzada de movimientos. Permite filtrar
/// por texto, mes, año, categoría y tipo simultáneamente.
/// </summary>
public partial class BuscarMovimientosViewModel : BaseViewModel
{
    private readonly IApiService _apiService;
    private readonly INavigationService _navigationService;

    [ObservableProperty]
    private string _textoBusqueda = string.Empty;

    [ObservableProperty]
    private int? _anioFiltro;

    [ObservableProperty]
    private int? _mesFiltro;

    [ObservableProperty]
    private CategoriaDto? _categoriaFiltro;

    [ObservableProperty]
    private TipoMovimiento? _tipoFiltro;

    [ObservableProperty]
    private bool _hayFiltrosActivos;

    [ObservableProperty]
    private string _mesAnioTexto = string.Empty;

    public ObservableCollection<MovimientoDto> Resultados { get; } = [];
    public ObservableCollection<CategoriaDto> Categorias { get; } = [];

    public List<string> Meses { get; } =
    [
        "Todos", "Enero", "Febrero", "Marzo", "Abril",
        "Mayo", "Junio", "Julio", "Agosto", "Septiembre",
        "Octubre", "Noviembre", "Diciembre"
    ];

    public List<string> Tipos { get; } = ["Todos", "Ingreso", "Gasto"];

    public BuscarMovimientosViewModel(
        IApiService apiService,
        INavigationService navigationService)
    {
        _apiService = apiService;
        _navigationService = navigationService;
        Titulo = "Buscar movimientos";

        // Por defecto carga el mes actual
        DateTime ahora = DateTime.Now;
        AnioFiltro = ahora.Year;
        MesFiltro = ahora.Month;
        ActualizarTextoMes();
    }

    /// <summary>
    /// Carga las categorías disponibles y lanza la búsqueda inicial.
    /// </summary>
    [RelayCommand]
    private async Task CargarAsync()
    {
        await EjecutarSeguro(async () =>
        {
            Task<List<CategoriaDto>> categoriasTask = _apiService.ObtenerCategorias();
            Task resultadosTask = BuscarAsync();
            await Task.WhenAll(categoriasTask, resultadosTask);

            List<CategoriaDto> cats = await categoriasTask;
            Categorias.Clear();
            foreach (CategoriaDto c in cats)
                Categorias.Add(c);
        }, "Error al cargar");
    }

    /// <summary>
    /// Ejecuta la búsqueda aplicando todos los filtros activos.
    /// </summary>
    [RelayCommand]
    private async Task BuscarAsync()
    {
        await EjecutarSeguro(async () =>
        {
            // Convertimos el enum a string en minúsculas para la API
            string? tipo = TipoFiltro.HasValue
                ? TipoFiltro.Value.ToString().ToLower()
                : null;

            List<MovimientoDto> resultados = await _apiService.BuscarMovimientos(
                string.IsNullOrWhiteSpace(TextoBusqueda) ? null : TextoBusqueda,
                AnioFiltro,
                MesFiltro,
                CategoriaFiltro?.Id,
                tipo);

            Resultados.Clear();
            foreach (MovimientoDto r in resultados)
                Resultados.Add(r);

            ActualizarHayFiltros();
        }, "Error al buscar");
    }

    /// <summary>
    /// Restablece todos los filtros al mes actual y relanza la búsqueda.
    /// </summary>
    [RelayCommand]
    private async Task LimpiarFiltrosAsync()
    {
        TextoBusqueda = string.Empty;
        AnioFiltro = DateTime.Now.Year;
        MesFiltro = DateTime.Now.Month;
        CategoriaFiltro = null;
        TipoFiltro = null;
        ActualizarTextoMes();
        await BuscarAsync();
    }

    [RelayCommand]
    private async Task SeleccionarMesAsync(string mes)
    {
        int indice = Meses.IndexOf(mes);
        // Índice 0 = "Todos", sin filtro de mes
        MesFiltro = indice == 0 ? null : indice;
        ActualizarTextoMes();
        await BuscarAsync();
    }

    [RelayCommand]
    private async Task SeleccionarTipoAsync(string tipo)
    {
        TipoFiltro = tipo switch
        {
            "Ingreso" => TipoMovimiento.Ingreso,
            "Gasto" => TipoMovimiento.Gasto,
            _ => null
        };
        await BuscarAsync();
    }

    [RelayCommand]
    private async Task VerDetalleAsync(MovimientoDto movimiento)
        => await _navigationService.Navegar(nameof(DetalleMovimientoPage),
            new Dictionary<string, object> { { "Movimiento", movimiento } });

    [RelayCommand]
    private async Task VolverAsync()
        => await _navigationService.NavigarAtras();

    private void ActualizarTextoMes()
    {
        if (!MesFiltro.HasValue)
        {
            MesAnioTexto = AnioFiltro.HasValue
                ? AnioFiltro.Value.ToString()
                : "Todos";
            return;
        }

        DateTime fecha = new DateTime(
            AnioFiltro ?? DateTime.Now.Year, MesFiltro.Value, 1);
        MesAnioTexto = fecha.ToString("MMMM yyyy");
    }

    private void ActualizarHayFiltros()
    {
        HayFiltrosActivos =
            !string.IsNullOrWhiteSpace(TextoBusqueda) ||
            MesFiltro != DateTime.Now.Month ||
            CategoriaFiltro is not null ||
            TipoFiltro is not null;
    }
}
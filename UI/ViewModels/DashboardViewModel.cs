using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinanzasApp.Core.DTOs.Movimiento;
using FinanzasApp.Core.DTOs.Notificacion;
using FinanzasApp.UI.Services;
using FinanzasApp.UI.Views;
using System.Collections.ObjectModel;

namespace FinanzasApp.UI.ViewModels;

public partial class DashboardViewModel : BaseViewModel
{
    private readonly IApiService _apiService;
    private readonly ISessionService _sessionService;
    private readonly INavigationService _navigationService;

    [ObservableProperty]
    private string _nombreUsuario = string.Empty;

    [ObservableProperty]
    private int _anioSeleccionado;

    [ObservableProperty]
    private int _mesSeleccionado;

    [ObservableProperty]
    private string _mesAnioTexto = string.Empty;

    [ObservableProperty]
    private decimal _totalIngresos;

    [ObservableProperty]
    private decimal _totalGastos;

    [ObservableProperty]
    private decimal _balance;

    [ObservableProperty]
    private bool _balancePositivo;

    public ObservableCollection<MovimientoDto> UltimosMovimientos { get; } = [];

    public DashboardViewModel(
        IApiService apiService,
        ISessionService sessionService,
        INavigationService navigationService)
    {
        _apiService = apiService;
        _sessionService = sessionService;
        _navigationService = navigationService;
        Titulo = "Inicio";

        DateTime ahora = DateTime.Now;
        AnioSeleccionado = ahora.Year;
        MesSeleccionado = ahora.Month;
        ActualizarTextoMes();
        NombreUsuario = _sessionService.ObtenerNombre();
    }

    [RelayCommand]
    private async Task CargarDatosAsync()
    {
        await EjecutarSeguro(async () =>
        {
            Task<ResumenMesDto> resumenTask =
                _apiService.ObtenerResumenMes(AnioSeleccionado, MesSeleccionado);
            Task<List<MovimientoDto>> ultimosTask =
                _apiService.ObtenerUltimosMovimientos(5);

            await Task.WhenAll(resumenTask, ultimosTask);

            ResumenMesDto resumen = await resumenTask;
            TotalIngresos = resumen.TotalIngresos;
            TotalGastos = resumen.TotalGastos;
            Balance = resumen.Balance;
            BalancePositivo = Balance >= 0;

            UltimosMovimientos.Clear();
            foreach (MovimientoDto m in await ultimosTask)
                UltimosMovimientos.Add(m);

        }, "Error al cargar el dashboard");
    }

    [RelayCommand]
    private async Task MesAnteriorAsync()
    {
        DateTime fecha = new DateTime(AnioSeleccionado, MesSeleccionado, 1).AddMonths(-1);
        AnioSeleccionado = fecha.Year;
        MesSeleccionado = fecha.Month;
        ActualizarTextoMes();
        await CargarDatosAsync();
    }

    [RelayCommand]
    private async Task MesSiguienteAsync()
    {
        DateTime ahora = DateTime.Now;
        DateTime siguiente = new DateTime(AnioSeleccionado, MesSeleccionado, 1).AddMonths(1);

        if (siguiente.Year > ahora.Year ||
           (siguiente.Year == ahora.Year && siguiente.Month > ahora.Month))
            return;

        AnioSeleccionado = siguiente.Year;
        MesSeleccionado = siguiente.Month;
        ActualizarTextoMes();
        await CargarDatosAsync();
    }

    [RelayCommand]
    private async Task VerDetalleMovimientoAsync(MovimientoDto movimiento)
        => await _navigationService.Navegar(nameof(DetalleMovimientoPage),
            new Dictionary<string, object> { { "Movimiento", movimiento } });

    private void ActualizarTextoMes()
    {
        DateTime fecha = new DateTime(AnioSeleccionado, MesSeleccionado, 1);
        MesAnioTexto = fecha.ToString("MMMM yyyy");
    }
}
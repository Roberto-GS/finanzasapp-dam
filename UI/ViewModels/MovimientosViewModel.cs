using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinanzasApp.Core.DTOs.Movimiento;
using FinanzasApp.Core.Enums;
using FinanzasApp.UI.Services;
using FinanzasApp.UI.Views;
using System.Collections.ObjectModel;

namespace FinanzasApp.UI.ViewModels;

/// <summary>
/// ViewModel de la página de movimientos. Muestra los movimientos del mes
/// seleccionado con filtrado por texto en tiempo real.
/// </summary>
public partial class MovimientosViewModel : BaseViewModel
{
    private readonly IApiService _apiService;
    private readonly INavigationService _navigationService;

    [ObservableProperty]
    private string _textoBusqueda = string.Empty;

    [ObservableProperty]
    private int _anioSeleccionado;

    [ObservableProperty]
    private int _mesSeleccionado;

    [ObservableProperty]
    private string _mesAnioTexto = string.Empty;

    private List<MovimientoDto> _todosLosMovimientos = [];

    public ObservableCollection<MovimientoDto> MovimientosFiltrados { get; } = [];

    public MovimientosViewModel(
        IApiService apiService,
        INavigationService navigationService)
    {
        _apiService = apiService;
        _navigationService = navigationService;
        Titulo = "Movimientos";

        DateTime ahora = DateTime.Now;
        AnioSeleccionado = ahora.Year;
        MesSeleccionado = ahora.Month;
        ActualizarTextoMes();
    }

    [RelayCommand]
    private async Task CargarMovimientosAsync()
    {
        await EjecutarSeguro(async () =>
        {
            _todosLosMovimientos = await _apiService
                .ObtenerMovimientosPorMes(AnioSeleccionado, MesSeleccionado);
            AplicarFiltros();
        }, "Error al cargar los movimientos");
    }

    // Se llama automáticamente al cambiar el texto de búsqueda
    partial void OnTextoBusquedaChanged(string value) => AplicarFiltros();

    /// <summary>
    /// Filtra la lista local por nombre o descripción sin hacer
    /// llamadas adicionales a la API.
    /// </summary>
    private void AplicarFiltros()
    {
        IEnumerable<MovimientoDto> filtrados = _todosLosMovimientos.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(TextoBusqueda))
            filtrados = filtrados.Where(m =>
                m.Nombre.Contains(TextoBusqueda, StringComparison.OrdinalIgnoreCase) ||
                (m.Descripcion?.Contains(
                    TextoBusqueda, StringComparison.OrdinalIgnoreCase) ?? false));

        MovimientosFiltrados.Clear();
        foreach (MovimientoDto m in filtrados)
            MovimientosFiltrados.Add(m);
    }

    [RelayCommand]
    private async Task MesAnteriorAsync()
    {
        DateTime fecha = new DateTime(
            AnioSeleccionado, MesSeleccionado, 1).AddMonths(-1);
        AnioSeleccionado = fecha.Year;
        MesSeleccionado = fecha.Month;
        ActualizarTextoMes();
        await CargarMovimientosAsync();
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
        await CargarMovimientosAsync();
    }

    [RelayCommand]
    private async Task VerDetalleAsync(MovimientoDto movimiento)
        => await _navigationService.Navegar(nameof(DetalleMovimientoPage),
            new Dictionary<string, object> { { "Movimiento", movimiento } });

    [RelayCommand]
    private async Task CrearMovimientoAsync()
        => await _navigationService.Navegar(nameof(CrearMovimientoPage));

    [RelayCommand]
    private async Task VerIngresosAsync()
        => await _navigationService.Navegar(nameof(MovimientosPage),
            new Dictionary<string, object> { { "Tipo", TipoMovimiento.Ingreso } });

    [RelayCommand]
    private async Task VerGastosAsync()
        => await _navigationService.Navegar(nameof(MovimientosPage),
            new Dictionary<string, object> { { "Tipo", TipoMovimiento.Gasto } });

    [RelayCommand]
    private async Task IrABuscarAsync()
        => await _navigationService.Navegar(nameof(BuscarMovimientosPage));

    private void ActualizarTextoMes()
    {
        DateTime fecha = new DateTime(AnioSeleccionado, MesSeleccionado, 1);
        MesAnioTexto = fecha.ToString("MMMM yyyy");
    }
}
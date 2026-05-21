using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinanzasApp.Core.DTOs.Categoria;
using FinanzasApp.Core.DTOs.Movimiento;
using FinanzasApp.UI.Services;
using FinanzasApp.UI.Views;
using System.Collections.ObjectModel;

namespace FinanzasApp.UI.ViewModels;

/// <summary>
/// ViewModel del detalle de una categoría. Muestra los últimos movimientos
/// del mes actual asociados a esa categoría.
/// </summary>
[QueryProperty(nameof(Categoria), "Categoria")]
public partial class DetalleCategoriaViewModel : BaseViewModel
{
    private readonly IApiService _apiService;
    private readonly INavigationService _navigationService;

    [ObservableProperty]
    private CategoriaDto? _categoria;

    public ObservableCollection<MovimientoDto> Movimientos { get; } = [];

    public DetalleCategoriaViewModel(
        IApiService apiService,
        INavigationService navigationService)
    {
        _apiService = apiService;
        _navigationService = navigationService;
        Titulo = "Categoría";
    }

    partial void OnCategoriaChanged(CategoriaDto? value)
    {
        if (value is null) return;
        CargarMovimientosCommand.Execute(null);
    }

    [RelayCommand]
    private async Task CargarMovimientosAsync()
    {
        if (Categoria is null) return;

        await EjecutarSeguro(async () =>
        {
            DateTime ahora = DateTime.Now;

            // Cargamos el mes actual y filtramos por la categoría
            List<MovimientoDto> todos =
                await _apiService.ObtenerMovimientosPorMes(ahora.Year, ahora.Month);

            List<MovimientoDto> filtrados = todos
                .Where(m => m.CategoriaId == Categoria.Id)
                .Take(10)
                .ToList();

            Movimientos.Clear();
            foreach (MovimientoDto m in filtrados)
                Movimientos.Add(m);

        }, "Error al cargar los movimientos de la categoría");
    }

    [RelayCommand]
    private async Task VolverAsync()
        => await _navigationService.NavigarAtras();

    [RelayCommand]
    private async Task VerDetalleMovimientoAsync(MovimientoDto movimiento)
        => await _navigationService.Navegar(nameof(DetalleMovimientoPage),
            new Dictionary<string, object> { { "Movimiento", movimiento } });
}
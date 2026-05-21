using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinanzasApp.Core.DTOs.Objetivo;
using FinanzasApp.UI.Services;
using FinanzasApp.UI.Views;
using System.Collections.ObjectModel;

namespace FinanzasApp.UI.ViewModels;

/// <summary>
/// ViewModel de la página de objetivos. Muestra todos los objetivos activos
/// con su progreso calculado en tiempo real.
/// </summary>
public partial class ObjetivosViewModel : BaseViewModel
{
    private readonly IApiService _apiService;
    private readonly INavigationService _navigationService;

    public ObservableCollection<ObjetivoConProgresoDto> Objetivos { get; } = [];

    public ObjetivosViewModel(
        IApiService apiService,
        INavigationService navigationService)
    {
        _apiService = apiService;
        _navigationService = navigationService;
        Titulo = "Objetivos";
    }

    [RelayCommand]
    private async Task CargarObjetivosAsync()
    {
        await EjecutarSeguro(async () =>
        {
            // Cargamos los objetivos con su progreso calculado en tiempo real
            List<ObjetivoConProgresoDto> objetivos =
                await _apiService.ObtenerObjetivosConProgreso();

            Objetivos.Clear();
            foreach (ObjetivoConProgresoDto objetivo in objetivos)
                Objetivos.Add(objetivo);
        }, "Error al cargar los objetivos");
    }

    [RelayCommand]
    private async Task VerDetalleAsync(ObjetivoConProgresoDto? objetivo)
    {
        // Protección contra null por si el binding no resuelve correctamente
        if (objetivo is null) return;

        await _navigationService.Navegar(nameof(DetalleObjetivoPage),
            new Dictionary<string, object> { { "ObjetivoId", objetivo.Id } });
    }

    [RelayCommand]
    private async Task CrearObjetivoAsync()
        => await _navigationService.Navegar(nameof(CrearObjetivoPage));
}
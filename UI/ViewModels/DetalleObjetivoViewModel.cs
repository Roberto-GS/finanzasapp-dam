using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinanzasApp.Core.DTOs.Objetivo;
using FinanzasApp.UI.Services;
using FinanzasApp.UI.Views;

namespace FinanzasApp.UI.ViewModels;

/// <summary>
/// ViewModel del detalle de un objetivo. Carga el objetivo con su progreso
/// calculado en tiempo real y permite editarlo o eliminarlo.
/// </summary>
[QueryProperty(nameof(ObjetivoId), "ObjetivoId")]
public partial class DetalleObjetivoViewModel : BaseViewModel
{
    private readonly IApiService _apiService;
    private readonly INavigationService _navigationService;

    [ObservableProperty]
    private int _objetivoId;

    [ObservableProperty]
    private ObjetivoConProgresoDto? _objetivo;

    public DetalleObjetivoViewModel(
        IApiService apiService,
        INavigationService navigationService)
    {
        _apiService = apiService;
        _navigationService = navigationService;
        Titulo = "Detalle objetivo";
    }

    partial void OnObjetivoIdChanged(int value)
    {
        // Cuando llega el ID por navegación cargamos el objetivo
        if (value > 0)
            CargarDatosCommand.Execute(null);
    }

    [RelayCommand]
    private async Task CargarDatosAsync()
    {
        if (ObjetivoId <= 0) return;

        await EjecutarSeguro(async () =>
        {
            // Obtenemos el objetivo con su progreso calculado en tiempo real
            Objetivo = await _apiService.ObtenerObjetivoConProgreso(ObjetivoId);
        }, "Error al cargar el objetivo");
    }

    [RelayCommand]
    private async Task EliminarAsync()
    {
        if (Objetivo is null) return;

        bool confirmar = await Shell.Current.DisplayAlert(
            "Eliminar objetivo",
            "¿Seguro que quieres eliminar este objetivo?",
            "Eliminar", "Cancelar");

        if (!confirmar) return;

        await EjecutarSeguro(async () =>
        {
            await _apiService.EliminarObjetivo(Objetivo.Id);
            await _navigationService.NavigarAtras();
        }, "Error al eliminar el objetivo");
    }

    [RelayCommand]
    private async Task EditarAsync()
    {
        if (Objetivo is null) return;

        // Navegamos al formulario de edición pasando el ID del objetivo
        await _navigationService.Navegar(nameof(CrearObjetivoPage),
            new Dictionary<string, object> { { "ObjetivoId", Objetivo.Id } });
    }

    [RelayCommand]
    private async Task VolverAsync()
        => await Shell.Current.GoToAsync("..");
}
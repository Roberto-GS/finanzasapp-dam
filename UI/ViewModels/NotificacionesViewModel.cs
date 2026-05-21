using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinanzasApp.Core.DTOs.Notificacion;
using FinanzasApp.UI.Services;
using System.Collections.ObjectModel;

namespace FinanzasApp.UI.ViewModels;

/// <summary>
/// ViewModel de la página de notificaciones. Gestiona la lista combinada
/// de notificaciones persistentes y dinámicas, y su eliminación.
/// </summary>
public partial class NotificacionesViewModel : BaseViewModel
{
    private readonly IApiService _apiService;
    private readonly INavigationService _navigationService;

    [ObservableProperty]
    private int _totalNotificaciones;

    public ObservableCollection<NotificacionDto> Notificaciones { get; } = [];

    public NotificacionesViewModel(
        IApiService apiService,
        INavigationService navigationService)
    {
        _apiService = apiService;
        _navigationService = navigationService;
        Titulo = "Notificaciones";
    }

    [RelayCommand]
    private async Task CargarAsync()
    {
        await EjecutarSeguro(async () =>
        {
            List<NotificacionDto> notificaciones =
                await _apiService.ObtenerNotificaciones();

            Notificaciones.Clear();
            foreach (NotificacionDto n in notificaciones)
                Notificaciones.Add(n);

            TotalNotificaciones = notificaciones.Count;
        }, "Error al cargar las notificaciones");
    }

    [RelayCommand]
    private async Task EliminarAsync(NotificacionDto notificacion)
    {
        if (notificacion.EsDinamica)
        {
            // Las dinámicas solo se eliminan de la lista local
            // Reaparecerán si la condición sigue existiendo en la próxima sesión
            Notificaciones.Remove(notificacion);
            TotalNotificaciones = Notificaciones.Count;
            return;
        }

        // Las persistentes se marcan como leídas en la BD
        await EjecutarSeguro(async () =>
        {
            await _apiService.EliminarNotificacion(notificacion.Id);
            Notificaciones.Remove(notificacion);
            TotalNotificaciones = Notificaciones.Count;
        }, "Error al eliminar la notificación");
    }

    [RelayCommand]
    private async Task EliminarTodasAsync()
    {
        bool confirmar = await Shell.Current.DisplayAlert(
            "Borrar todas",
            "¿Seguro que quieres eliminar todas las notificaciones?",
            "Eliminar", "Cancelar");

        if (!confirmar) return;

        await EjecutarSeguro(async () =>
        {
            // Marcamos todas las persistentes como leídas en BD
            await _apiService.EliminarTodasNotificaciones();

            // Las dinámicas se limpian solo de la lista local
            Notificaciones.Clear();
            TotalNotificaciones = 0;
        }, "Error al eliminar las notificaciones");
    }

    [RelayCommand]
    private async Task VolverAsync()
        => await Shell.Current.GoToAsync("//DashboardPage");
}
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinanzasApp.Core.DTOs.Grupo;
using FinanzasApp.UI.Services;
using System.Collections.ObjectModel;

namespace FinanzasApp.UI.ViewModels;

/// <summary>
/// ViewModel de la página de solicitudes de grupo. Muestra las invitaciones
/// pendientes recibidas por el usuario y permite aceptarlas o rechazarlas.
/// </summary>
public partial class SolicitudesGrupoViewModel : BaseViewModel
{
    private readonly IApiService _apiService;
    private readonly INavigationService _navigationService;

    public ObservableCollection<SolicitudGrupoDto> Solicitudes { get; } = [];

    public SolicitudesGrupoViewModel(
        IApiService apiService,
        INavigationService navigationService)
    {
        _apiService = apiService;
        _navigationService = navigationService;
        Titulo = "Solicitudes pendientes";
    }

    [RelayCommand]
    private async Task CargarAsync()
    {
        await EjecutarSeguro(async () =>
        {
            List<SolicitudGrupoDto> solicitudes =
                await _apiService.ObtenerSolicitudesPendientes();

            Solicitudes.Clear();
            foreach (SolicitudGrupoDto s in solicitudes)
                Solicitudes.Add(s);
        }, "Error al cargar las solicitudes");
    }

    [RelayCommand]
    private async Task AceptarAsync(SolicitudGrupoDto solicitud)
    {
        await EjecutarSeguro(async () =>
        {
            await _apiService.AceptarSolicitud(solicitud.Id);
            Solicitudes.Remove(solicitud);

            await Shell.Current.DisplayAlert(
                "¡Bienvenido!",
                $"Te has unido al grupo '{solicitud.GrupoNombre}'.",
                "Aceptar");
        }, "Error al aceptar la solicitud");
    }

    [RelayCommand]
    private async Task RechazarAsync(SolicitudGrupoDto solicitud)
    {
        await EjecutarSeguro(async () =>
        {
            await _apiService.RechazarSolicitud(solicitud.Id);
            Solicitudes.Remove(solicitud);
        }, "Error al rechazar la solicitud");
    }

    [RelayCommand]
    private async Task VolverAsync()
        => await _navigationService.NavigarAtras();
}
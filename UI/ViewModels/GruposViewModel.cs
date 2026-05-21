using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinanzasApp.Core.DTOs.Grupo;
using FinanzasApp.UI.Services;
using FinanzasApp.UI.Views;
using System.Collections.ObjectModel;

namespace FinanzasApp.UI.ViewModels;

/// <summary>
/// ViewModel de la página de grupos. Gestiona la lista de grupos,
/// la creación, eliminación y la visualización de solicitudes pendientes.
/// </summary>
public partial class GruposViewModel : BaseViewModel
{
    private readonly IApiService _apiService;
    private readonly INavigationService _navigationService;

    [ObservableProperty]
    private int _solicitudesPendientes;

    public ObservableCollection<GrupoDto> Grupos { get; } = [];

    public GruposViewModel(
        IApiService apiService,
        INavigationService navigationService)
    {
        _apiService = apiService;
        _navigationService = navigationService;
        Titulo = "Grupos";
    }

    [RelayCommand]
    private async Task CargarAsync()
    {
        await EjecutarSeguro(async () =>
        {
            // Cargamos grupos y solicitudes en paralelo
            Task<List<GrupoDto>> gruposTask = _apiService.ObtenerGrupos();
            Task<List<SolicitudGrupoDto>> solicitudesTask =
                _apiService.ObtenerSolicitudesPendientes();

            await Task.WhenAll(gruposTask, solicitudesTask);

            List<GrupoDto> grupos = await gruposTask;
            Grupos.Clear();
            foreach (GrupoDto grupo in grupos)
                Grupos.Add(grupo);

            SolicitudesPendientes = (await solicitudesTask).Count;

        }, "Error al cargar los grupos");
    }

    /// <summary>
    /// Muestra un diálogo para introducir el nombre del grupo y lo crea.
    /// </summary>
    [RelayCommand]
    private async Task CrearGrupoAsync()
    {
        string? nombre = await Shell.Current.DisplayPromptAsync(
            "Nuevo grupo",
            "¿Cómo se llamará el grupo?",
            "Crear", "Cancelar",
            placeholder: "Introduzca el nombre");

        if (string.IsNullOrWhiteSpace(nombre)) return;

        await EjecutarSeguro(async () =>
        {
            CrearGrupoDto dto = new CrearGrupoDto { Nombre = nombre };
            GrupoDto grupo = await _apiService.CrearGrupo(dto);
            Grupos.Add(grupo);
        }, "Error al crear el grupo");
    }

    [RelayCommand]
    private async Task VerDetalleAsync(GrupoDto grupo)
        => await _navigationService.Navegar(nameof(DetalleGrupoPage),
            new Dictionary<string, object> { { "Grupo", grupo } });

    [RelayCommand]
    private async Task VerSolicitudesAsync()
        => await _navigationService.Navegar(nameof(SolicitudesGrupoPage));

    [RelayCommand]
    private async Task EliminarGrupoAsync(GrupoDto grupo)
    {
        if (!grupo.SoyCreador)
        {
            await Shell.Current.DisplayAlert(
                "Sin permisos",
                "Solo el creador puede eliminar el grupo.",
                "Aceptar");
            return;
        }

        bool confirmar = await Shell.Current.DisplayAlert(
            "Eliminar grupo",
            $"¿Seguro que quieres eliminar '{grupo.Nombre}'? " +
            "Se eliminarán todos los datos del grupo.",
            "Eliminar", "Cancelar");

        if (!confirmar) return;

        await EjecutarSeguro(async () =>
        {
            await _apiService.EliminarGrupo(grupo.Id);
            Grupos.Remove(grupo);
        }, "Error al eliminar el grupo");
    }

    [RelayCommand]
    private async Task VolverAsync()
        => await Shell.Current.GoToAsync("//DashboardPage");
}
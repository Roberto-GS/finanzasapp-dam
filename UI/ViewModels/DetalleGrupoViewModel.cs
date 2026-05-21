using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinanzasApp.Core.DTOs.Grupo;
using FinanzasApp.UI.Services;
using System.Collections.ObjectModel;

namespace FinanzasApp.UI.ViewModels;

/// <summary>
/// ViewModel del detalle de un grupo. Muestra el resumen financiero
/// del mes actual y el ranking de gastos de sus miembros.
/// </summary>
[QueryProperty(nameof(Grupo), "Grupo")]
public partial class DetalleGrupoViewModel : BaseViewModel
{
    private readonly IApiService _apiService;
    private readonly INavigationService _navigationService;

    [ObservableProperty]
    private GrupoDto? _grupo;

    [ObservableProperty]
    private ResumenGrupoDto? _resumen;

    public ObservableCollection<MiembroGrupoDto> Ranking { get; } = [];

    public DetalleGrupoViewModel(
        IApiService apiService,
        INavigationService navigationService)
    {
        _apiService = apiService;
        _navigationService = navigationService;
        Titulo = "Detalle del grupo";
    }

    partial void OnGrupoChanged(GrupoDto? value)
    {
        if (value is null) return;
        CargarResumenCommand.Execute(null);
    }

    [RelayCommand]
    private async Task CargarResumenAsync()
    {
        if (Grupo is null) return;

        await EjecutarSeguro(async () =>
        {
            Resumen = await _apiService.ObtenerResumenGrupo(Grupo.Id);
            Ranking.Clear();
            foreach (MiembroGrupoDto miembro in Resumen.Ranking)
                Ranking.Add(miembro);
        }, "Error al cargar el resumen del grupo");
    }

    /// <summary>
    /// Muestra un diálogo para introducir el email del usuario a invitar
    /// y envía la solicitud a la API.
    /// </summary>
    [RelayCommand]
    private async Task InvitarUsuarioAsync()
    {
        if (Grupo is null) return;

        string? email = await Shell.Current.DisplayPromptAsync(
            "Invitar usuario",
            "Introduce el email del usuario a invitar:",
            "Invitar",
            "Cancelar",
            placeholder: "usuario@email.com",
            keyboard: Microsoft.Maui.Keyboard.Email);

        if (string.IsNullOrWhiteSpace(email)) return;

        await EjecutarSeguro(async () =>
        {
            await _apiService.InvitarUsuario(Grupo.Id, email);
            await Shell.Current.DisplayAlert(
                "Invitación enviada",
                $"Se ha enviado una invitación a {email}.",
                "Aceptar");
        }, "Error al enviar la invitación");
    }

    [RelayCommand]
    private async Task VolverAsync()
        => await _navigationService.NavigarAtras();
}
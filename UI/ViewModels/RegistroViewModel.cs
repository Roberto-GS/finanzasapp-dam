using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinanzasApp.Core.DTOs.Autentificacion;
using FinanzasApp.UI.Services;

namespace FinanzasApp.UI.ViewModels;

/// <summary>
/// ViewModel de la pantalla de registro. Gestiona el formulario de alta,
/// la validación de campos y la navegación tras el registro exitoso.
/// </summary>
public partial class RegistroViewModel : BaseViewModel
{
    private readonly IApiService _apiService;
    private readonly ISessionService _sessionService;
    private readonly INavigationService _navigationService;

    [ObservableProperty]
    private string _nombre = string.Empty;

    [ObservableProperty]
    private string _email = string.Empty;

    [ObservableProperty]
    private string _password = string.Empty;

    [ObservableProperty]
    private string _confirmarPassword = string.Empty;

    public RegistroViewModel(
        IApiService apiService,
        ISessionService sessionService,
        INavigationService navigationService)
    {
        _apiService = apiService;
        _sessionService = sessionService;
        _navigationService = navigationService;
        Titulo = "Crear cuenta";
    }

    [RelayCommand]
    private async Task RegistrarAsync()
    {
        if (string.IsNullOrWhiteSpace(Nombre) ||
            string.IsNullOrWhiteSpace(Email) ||
            string.IsNullOrWhiteSpace(Password))
        {
            MostrarError("Por favor, completa todos los campos.");
            return;
        }

        // Validamos que el email tiene un formato correcto antes de llamar a la API
        if (!EsEmailValido(Email))
        {
            MostrarError("El correo electrónico no tiene un formato válido.");
            return;
        }

        if (Password != ConfirmarPassword)
        {
            MostrarError("Las contraseñas no coinciden.");
            return;
        }

        if (Password.Length < 8)
        {
            MostrarError("La contraseña debe tener al menos 8 caracteres.");
            return;
        }

        await EjecutarSeguro(async () =>
        {
            RegistroDto dto = new RegistroDto
            {
                Nombre = Nombre,
                Email = Email,
                Password = Password
            };

            AuthResponseDto respuesta = await _apiService.Registro(dto);

            // Guardamos la sesión y navegamos al Shell principal
            _sessionService.GuardarSesion(
                respuesta.Token,
                respuesta.Usuario.Id,
                respuesta.Usuario.Nombre,
                respuesta.Usuario.Email);

            await _navigationService.NavegarAShell();
        }, "Error al crear la cuenta");
    }

    [RelayCommand]
    private async Task VolverALoginAsync()
    {
        // Fuera del Shell usamos NavigationPage para volver
        if (Application.Current?.Windows[0].Page is NavigationPage navPage)
            await navPage.PopAsync();
    }
}
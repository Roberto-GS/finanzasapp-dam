using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinanzasApp.Core.DTOs.Autentificacion;
using FinanzasApp.UI.Services;
using FinanzasApp.UI.Views;

namespace FinanzasApp.UI.ViewModels;

/// <summary>
/// ViewModel de la pantalla de inicio de sesión. Gestiona las credenciales,
/// la validación básica y la navegación tras el login exitoso.
/// </summary>
public partial class LoginViewModel : BaseViewModel
{
    private readonly IApiService _apiService;
    private readonly ISessionService _sessionService;
    private readonly INavigationService _navigationService;

    [ObservableProperty]
    private string _email = string.Empty;

    [ObservableProperty]
    private string _password = string.Empty;

    [ObservableProperty]
    private bool _mantenerSesion;

    public LoginViewModel(
        IApiService apiService,
        ISessionService sessionService,
        INavigationService navigationService)
    {
        _apiService = apiService;
        _sessionService = sessionService;
        _navigationService = navigationService;
        Titulo = "Iniciar sesión";
    }

    [RelayCommand]
    private async Task LoginAsync()
    {
        if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
        {
            MostrarError("Por favor, completa todos los campos");
            return;
        }

        // Validamos que el email tiene un formato correcto antes de llamar a la API
        if (!EsEmailValido(Email))
        {
            MostrarError("El correo electrónico no tiene un formato válido.");
            return;
        }

        await EjecutarSeguro(async () =>
        {
            LoginDto dto = new LoginDto { Email = Email, Password = Password };
            AuthResponseDto respuesta = await _apiService.Login(dto);

            // Guardamos la sesión y navegamos al Shell principal
            _sessionService.GuardarSesion(
                respuesta.Token,
                respuesta.Usuario.Id,
                respuesta.Usuario.Nombre,
                respuesta.Usuario.Email);

            await _navigationService.NavegarAShell();
        }, "Error al iniciar sesión");
    }

    [RelayCommand]
    private async Task IrARegistroAsync()
    {
        // Fuera del Shell navegamos con NavigationPage directamente
        RegistroPage registroPage = IPlatformApplication.Current!.Services
            .GetRequiredService<RegistroPage>();

        if (Application.Current?.Windows[0].Page is NavigationPage navPage)
            await navPage.PushAsync(registroPage);
    }
}
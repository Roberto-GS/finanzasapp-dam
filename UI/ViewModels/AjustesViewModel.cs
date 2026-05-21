using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinanzasApp.UI.Services;

namespace FinanzasApp.UI.ViewModels;

/// <summary>
/// ViewModel de la página de ajustes. Gestiona el cambio de contraseña
/// y el cierre de sesión del usuario.
/// </summary>
public partial class AjustesViewModel : BaseViewModel
{
    private readonly ISessionService _sessionService;
    private readonly INavigationService _navigationService;
    private readonly IApiService _apiService;

    [ObservableProperty]
    private string _nombreUsuario = string.Empty;

    [ObservableProperty]
    private string _emailUsuario = string.Empty;

    [ObservableProperty]
    private string _passwordActual = string.Empty;

    [ObservableProperty]
    private string _passwordNueva = string.Empty;

    [ObservableProperty]
    private string _confirmarPasswordNueva = string.Empty;

    [ObservableProperty]
    private bool _mostrarCambiarPassword;

    [ObservableProperty]
    private bool _cambioExitoso;

    [ObservableProperty]
    private int _temaSeleccionado;

    [ObservableProperty]
    private string _textoTema = "El mismo que el del dispositivo";

    public AjustesViewModel(
        ISessionService sessionService,
        INavigationService navigationService,
        IApiService apiService)
    {
        _sessionService = sessionService;
        _navigationService = navigationService;
        _apiService = apiService;
        Titulo = "Ajustes";
    }

    /// <summary>
    /// Carga el nombre y email del usuario desde la sesión activa.
    /// </summary>
    [RelayCommand]
    private void CargarDatos()
    {
        NombreUsuario = _sessionService.ObtenerNombre();
        EmailUsuario = _sessionService.ObtenerEmail();
        CargarTemaActual();
    }

    /// <summary>
    /// Muestra u oculta el formulario de cambio de contraseña
    /// y limpia sus campos al hacerlo.
    /// </summary>
    [RelayCommand]
    private void ToggleCambiarPassword()
    {
        MostrarCambiarPassword = !MostrarCambiarPassword;
        LimpiarError();
        CambioExitoso = false;
        PasswordActual = string.Empty;
        PasswordNueva = string.Empty;
        ConfirmarPasswordNueva = string.Empty;
    }

    /// <summary>
    /// Valida los campos y envía la petición de cambio de contraseña a la API.
    /// </summary>
    [RelayCommand]
    private async Task CambiarPasswordAsync()
    {
        if (string.IsNullOrWhiteSpace(PasswordActual) ||
            string.IsNullOrWhiteSpace(PasswordNueva) ||
            string.IsNullOrWhiteSpace(ConfirmarPasswordNueva))
        {
            MostrarError("Por favor, completa todos los campos");
            return;
        }

        if (PasswordNueva != ConfirmarPasswordNueva)
        {
            MostrarError("Las contraseñas nuevas no coinciden");
            return;
        }

        if (PasswordNueva.Length < 8)
        {
            MostrarError("La contraseña debe tener al menos 8 caracteres");
            return;
        }

        if (PasswordNueva == PasswordActual)
        {
            MostrarError("La contraseña nueva debe ser diferente a la actual");
            return;
        }

        await EjecutarSeguro(async () =>
        {
            await _apiService.CambiarPassword(
                _sessionService.ObtenerUsuarioId(),
                PasswordActual,
                PasswordNueva);

            // Limpiamos el formulario tras el cambio exitoso
            CambioExitoso = true;
            MostrarCambiarPassword = false;
            PasswordActual = string.Empty;
            PasswordNueva = string.Empty;
            ConfirmarPasswordNueva = string.Empty;

        }, "Error al cambiar la contraseña");
    }

    /// <summary>
    /// Muestra un diálogo de confirmación y cierra la sesión del usuario.
    /// </summary>
    [RelayCommand]
    private async Task CerrarSesionAsync()
    {
        bool confirmar = await Shell.Current.DisplayAlert(
            "Cerrar sesión",
            "¿Seguro que quieres cerrar sesión?",
            "Cerrar sesión", "Cancelar");

        if (!confirmar) return;

        _sessionService.CerrarSesion();
        await _navigationService.NavegarALogin();
    }

    [RelayCommand]
    private async Task VolverAsync()
        => await Shell.Current.GoToAsync("//DashboardPage");

    /// <summary>
    /// Se dispara cuando cambia TemaSeleccionado. Aplica el tema
    /// en toda la app y lo guarda en Preferences.
    /// </summary>
    partial void OnTemaSeleccionadoChanged(int value)
    {
        switch (value)
        {
            case 1:
                Application.Current!.UserAppTheme = AppTheme.Light;
                Preferences.Default.Set("tema_app", "claro");
                TextoTema = "Claro ☀️";
                break;
            case 2:
                Application.Current!.UserAppTheme = AppTheme.Dark;
                Preferences.Default.Set("tema_app", "oscuro");
                TextoTema = "Oscuro 🌙";
                break;
            default:
                Application.Current!.UserAppTheme = AppTheme.Unspecified;
                Preferences.Default.Set("tema_app", "sistema");
                TextoTema = "El mismo que el del dispositivo";
                break;
        }
    }

    /// <summary>
    /// Comando que recibe 0, 1 o 2 desde el botón pulsado en la UI.
    /// </summary>
    [RelayCommand]
    private void CambiarTema(string parametro)
    {
        if (int.TryParse(parametro, out int valor))
            TemaSeleccionado = valor;
    }

    /// <summary>
    /// Carga el tema guardado para mostrar la opción seleccionada
    /// en el selector de Ajustes. No aplica el tema porque ya lo
    /// hizo App.xaml.cs al arrancar.
    /// </summary>
    private void CargarTemaActual()
    {
        string temaGuardado = Preferences.Default.Get("tema_app", "sistema");

        // Asignamos al campo privado directamente para NO disparar
        // OnTemaSeleccionadoChanged, que volvería a aplicar el tema
        _temaSeleccionado = temaGuardado switch
        {
            "claro" => 1,
            "oscuro" => 2,
            _ => 0
        };
        TextoTema = temaGuardado switch
        {
            "claro" => "Claro ☀️",
            "oscuro" => "Oscuro 🌙",
            _ => "El mismo que el del dispositivo"
        };

        OnPropertyChanged(nameof(TemaSeleccionado));
    }
}
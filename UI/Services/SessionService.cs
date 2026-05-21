namespace FinanzasApp.UI.Services;

/// <summary>
/// Gestionamos la sesión del usuario persistiendo el token JWT y los datos básicos en las preferencias del dispositivo.
/// </summary>
public class SessionService : ISessionService
{
    // Claves para las preferencias del dispositivo
    private const string ClaveToken = "auth_token";
    private const string ClaveUsuarioId = "usuario_id";
    private const string ClaveNombre = "usuario_nombre";
    private const string ClaveEmail = "usuario_email";

    /// <inheritdoc/>
    public bool HaySesionActiva()
        => !string.IsNullOrEmpty(
            Preferences.Default.Get(ClaveToken, string.Empty));

    /// <inheritdoc/>
    public void GuardarSesion(
        string token, int usuarioId, string nombre, string email)
    {
        Preferences.Default.Set(ClaveToken, token);
        Preferences.Default.Set(ClaveUsuarioId, usuarioId);
        Preferences.Default.Set(ClaveNombre, nombre);
        Preferences.Default.Set(ClaveEmail, email);
    }

    /// <inheritdoc/>
    public void CerrarSesion()
    {
        Preferences.Default.Remove(ClaveToken);
        Preferences.Default.Remove(ClaveUsuarioId);
        Preferences.Default.Remove(ClaveNombre);
        Preferences.Default.Remove(ClaveEmail);
    }

    /// <inheritdoc/>
    public string ObtenerToken()
        => Preferences.Default.Get(ClaveToken, string.Empty);

    /// <inheritdoc/>
    public int ObtenerUsuarioId()
        => Preferences.Default.Get(ClaveUsuarioId, 0);

    /// <inheritdoc/>
    public string ObtenerNombre()
        => Preferences.Default.Get(ClaveNombre, string.Empty);

    /// <inheritdoc/>
    public string ObtenerEmail()
        => Preferences.Default.Get(ClaveEmail, string.Empty);
}
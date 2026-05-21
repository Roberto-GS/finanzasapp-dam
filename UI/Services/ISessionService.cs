namespace FinanzasApp.UI.Services;

public interface ISessionService
{
    /// <summary>Compruebamos si hay una sesión activa con token guardado.</summary>
    bool HaySesionActiva();

    /// <summary>
    /// Guarda el token JWT y los datos del usuario en las preferencias.
    /// </summary>
    void GuardarSesion(string token, int usuarioId, string nombre, string email);

    /// <summary>Eliminamos todos los datos de sesión del dispositivo.</summary>
    void CerrarSesion();

    /// <summary>Devuelve el token JWT almacenado.</summary>
    string ObtenerToken();

    /// <summary>Devuelve el ID del usuario en sesión.</summary>
    int ObtenerUsuarioId();

    /// <summary>Devuelve el nombre del usuario en sesión.</summary>
    string ObtenerNombre();

    /// <summary>Devuelve el email del usuario en sesión.</summary>
    string ObtenerEmail();
}
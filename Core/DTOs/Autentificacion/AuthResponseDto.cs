using FinanzasApp.Core.DTOs.Usuario;

namespace FinanzasApp.Core.DTOs.Autentificacion;

/// <summary>
/// Es lo que devuelve el servidor al iniciar sesión o al registrarse, contiene el token de autenticación y la información del usuario.
/// </summary>
public class AuthResponseDto
{
    public string Token { get; init; } = string.Empty;
    public UsuarioDto Usuario { get; init; } = null!;
}
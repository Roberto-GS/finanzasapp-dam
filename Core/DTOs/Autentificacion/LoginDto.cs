namespace FinanzasApp.Core.DTOs.Autentificacion;

/// <summary>
/// Son los datos que envia el usuario para iniciar sesión
/// </summary>
public class LoginDto
{
    public string Email { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
}
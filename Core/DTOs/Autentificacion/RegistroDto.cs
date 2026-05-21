namespace FinanzasApp.Core.DTOs.Autentificacion;

/// <summary>
/// Son los datos que envia el usuario para crear una nueva cuenta, es decir, para registrarse
/// </summary>
public class RegistroDto
{
    public string Nombre { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
}
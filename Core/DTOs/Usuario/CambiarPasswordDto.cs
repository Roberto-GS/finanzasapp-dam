namespace FinanzasApp.Core.DTOs.Usuario;

/// <summary>
/// Son los datos necesarios para cambiar la contraseña de un usuario
/// </summary>
public class CambiarPasswordDto
{
    public string PasswordActual { get; init; } = string.Empty;
    public string PasswordNueva { get; init; } = string.Empty;
}
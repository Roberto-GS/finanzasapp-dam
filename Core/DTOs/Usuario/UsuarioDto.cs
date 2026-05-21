namespace FinanzasApp.Core.DTOs.Usuario;

/// <summary>
/// Son los datos básicos de un usuario
/// </summary>
public class UsuarioDto
{
    public int Id { get; init; }
    public string Nombre { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public DateTime FechaCreacion { get; init; }
}
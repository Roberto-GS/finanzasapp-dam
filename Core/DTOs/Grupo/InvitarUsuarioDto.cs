namespace FinanzasApp.Core.DTOs.Grupo;

/// <summary>
/// Datos necesarios para invitar a un usuario a un grupo
/// </summary>
public class InvitarUsuarioDto
{
    public string EmailInvitado { get; init; } = string.Empty;
}
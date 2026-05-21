namespace FinanzasApp.Core.DTOs.Grupo;

/// <summary>
/// Representa una solicitud de unión a un grupo
/// </summary>
public class SolicitudGrupoDto
{
    public int Id { get; init; }
    public int GrupoId { get; init; }
    public string GrupoNombre { get; init; } = string.Empty;
    public int SolicitanteId { get; init; }
    public string SolicitanteNombre { get; init; } = string.Empty;
    public string Estado { get; init; } = string.Empty;
    public DateTime FechaSolicitud { get; init; }
}
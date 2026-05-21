namespace FinanzasApp.Core.DTOs.Grupo;

/// <summary>
/// Representa la información de un grupo para mostrar en los listados o en detalles básicos
/// </summary>
public class GrupoDto
{
    public int Id { get; init; }
    public string Nombre { get; init; } = string.Empty;
    public int CreadorId { get; init; }
    public string CreadorNombre { get; init; } = string.Empty;
    public DateTime FechaCreacion { get; init; }
    public int TotalMiembros { get; init; }
    public bool SoyCreador { get; init; }
}
namespace FinanzasApp.Core.DTOs.Grupo;

/// <summary>
/// Representa un resumen financiero completo de un grupo con un ranking de gastos de sus miembros
/// </summary>
public class ResumenGrupoDto
{
    public int GrupoId { get; init; }
    public string GrupoNombre { get; init; } = string.Empty;
    public decimal TotalGastosGrupo { get; init; }
    public decimal TotalIngresosGrupo { get; init; }
    public decimal BalanceGrupo => TotalIngresosGrupo - TotalGastosGrupo;
    public List<MiembroGrupoDto> Ranking { get; init; } = [];
}
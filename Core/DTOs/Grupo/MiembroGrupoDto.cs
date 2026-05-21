namespace FinanzasApp.Core.DTOs.Grupo;

/// <summary>
/// Informaación sobre un miembro de un grupo, incluyendo su contribución total en gastos e ingresos, y si es el creador del grupo
/// </summary>
public class MiembroGrupoDto
{
    public int UsuarioId { get; init; }
    public string Nombre { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public decimal TotalGastos { get; init; }
    public decimal TotalIngresos { get; init; }
    public bool EsCreador { get; init; }
    public int Posicion { get; set; }
}
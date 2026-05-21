namespace FinanzasApp.Core.DTOs.Movimiento;

/// <summary>
/// Representa los datos de un mes para representar en la gráfica de evolución mensual
/// </summary>
public class GraficaMensualDto
{
    public int Mes { get; init; }
    public string MesNombre { get; init; } = string.Empty;
    public decimal Total { get; init; }
}
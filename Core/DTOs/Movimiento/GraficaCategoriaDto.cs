namespace FinanzasApp.Core.DTOs.Movimiento;

/// <summary>
/// Representa los datos de una categoría para representar en la gráfica circular de distribución de gastos o de ingresos
/// </summary>
public class GraficaCategoriaDto
{
    public string CategoriaNombre { get; init; } = string.Empty;
    public string CategoriaColor { get; init; } = "#2563EB";
    public decimal Total { get; init; }
    public double Porcentaje { get; init; }
}
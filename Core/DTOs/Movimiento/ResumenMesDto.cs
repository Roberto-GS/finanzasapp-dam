namespace FinanzasApp.Core.DTOs.Movimiento;

/// <summary>
/// Representa un resumen finnciero de un mes (los ingresos, los gastos y el balance)
/// </summary>
public class ResumenMesDto
{
    public int Anio { get; init; }
    public int Mes { get; init; }
    public decimal TotalIngresos { get; init; }
    public decimal TotalGastos { get; init; }
    public decimal Balance => TotalIngresos - TotalGastos;
}
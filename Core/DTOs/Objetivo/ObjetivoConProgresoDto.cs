using FinanzasApp.Core.Enums;

namespace FinanzasApp.Core.DTOs.Objetivo;

/// <summary>
/// Representaun objetivo con su progreso actual, incluyendo detalles como cantidad objetivo, cantidad actual, porcentaje de progreso, estado y fechas relevantes.
/// </summary>
public class ObjetivoConProgresoDto
{
    public int Id { get; init; }
    public string Descripcion { get; init; } = string.Empty;
    public int TipoId { get; init; }
    public string TipoNombre { get; init; } = string.Empty;
    public string? CategoriaNombre { get; init; }
    public int? CategoriaId { get; init; }
    public decimal? CantidadObjetivo { get; init; }
    public decimal CantidadActual { get; init; }
    public double Progreso { get; init; }
    public string TextoProgreso { get; init; } = string.Empty;
    public string Estado { get; init; } = string.Empty;
    public bool EstaVencido { get; init; }
    public bool EstaEnPeligro { get; init; }
    public bool SeHaCumplido { get; init; }
    public DateTime? FechaInicio { get; init; }
    public DateTime? FechaFin { get; init; }
    public bool Activo { get; init; }
    public PeriodoObjetivo Periodo { get; init; }
}
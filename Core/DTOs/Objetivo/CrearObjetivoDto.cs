using FinanzasApp.Core.Enums;

namespace FinanzasApp.Core.DTOs.Objetivo;

/// <summary>
/// Representa los datos que el usuario introduce para crear un objetivo o editarlo
/// </summary>
public class CrearObjetivoDto
{
    public int TipoId { get; init; }
    public int? CategoriaId { get; init; }
    public decimal? CantidadObjetivo { get; init; }
    public PeriodoObjetivo Periodo { get; init; }
    public string? Descripcion { get; init; }
    public DateTime? FechaInicio { get; init; }
    public DateTime? FechaFin { get; init; }
}
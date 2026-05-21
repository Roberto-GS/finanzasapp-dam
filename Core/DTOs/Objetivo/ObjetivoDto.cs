using FinanzasApp.Core.Enums;

namespace FinanzasApp.Core.DTOs.Objetivo;

/// <summary>
/// Representa un objetivo tal y como se almacena en la db, sin hacer calculos en el progreso
/// </summary>
public class ObjetivoDto
{
    public int Id { get; init; }
    public int UsuarioId { get; init; }
    public int TipoId { get; init; }
    public string? TipoNombre { get; init; }
    public int? CategoriaId { get; init; }
    public string? CategoriaNombre { get; init; }
    public decimal? CantidadObjetivo { get; init; }
    public PeriodoObjetivo Periodo { get; init; }
    public string? Descripcion { get; init; }
    public DateTime? FechaInicio { get; init; }
    public DateTime? FechaFin { get; init; }
    public bool Activo { get; init; }
    public bool EstaVencido { get; init; }
}
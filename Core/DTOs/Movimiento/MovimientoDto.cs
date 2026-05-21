using FinanzasApp.Core.Enums;

namespace FinanzasApp.Core.DTOs.Movimiento;

/// <summary>
/// Representa un movimiento
/// </summary>
public class MovimientoDto
{
    public int Id { get; init; }
    public int UsuarioId { get; init; }
    public TipoMovimiento Tipo { get; init; }
    public decimal Cantidad { get; init; }
    public DateTime Fecha { get; init; }
    public int? CategoriaId { get; init; }
    public string? CategoriaNombre { get; init; }
    public string? CategoriaColor { get; init; }
    public string Nombre { get; init; } = string.Empty;
    public string? Descripcion { get; init; }
    public string? Etiqueta { get; init; }
}
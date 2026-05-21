using FinanzasApp.Core.Enums;

namespace FinanzasApp.Core.DTOs.Movimiento;

/// <summary>
/// Representa los datos que el usuario introduce para editar un movimiento existente
/// </summary>
public class EditarMovimientoDto
{
    public TipoMovimiento Tipo { get; init; }
    public decimal Cantidad { get; init; }
    public int? CategoriaId { get; init; }
    public string Nombre { get; init; } = string.Empty;
    public string? Descripcion { get; init; }
    public string? Etiqueta { get; init; }
}
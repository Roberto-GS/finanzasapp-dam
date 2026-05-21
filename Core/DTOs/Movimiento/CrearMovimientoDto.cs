using FinanzasApp.Core.Enums;

namespace FinanzasApp.Core.DTOs.Movimiento;

/// <summary>
/// Son los datos que el usuario introduce para registrar un nuevo movimiento. No incluye el Id ni la FechaCreacion, ya que estos se generan automáticamente al crear el movimiento en la base de datos.
/// </summary>
public class CrearMovimientoDto
{
    public TipoMovimiento Tipo { get; init; }
    public decimal Cantidad { get; init; }
    public int? CategoriaId { get; init; }
    public string Nombre { get; init; } = string.Empty;
    public string? Descripcion { get; init; }
    public string? Etiqueta { get; init; }
}
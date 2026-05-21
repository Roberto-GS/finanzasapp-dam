namespace FinanzasApp.Core.DTOs.Categoria;

/// <summary>
/// Representa una categoría
/// </summary>
public class CategoriaDto
{
    public int Id { get; init; }
    public string Nombre { get; init; } = string.Empty;
    public string? Color { get; init; }
    public int? UsuarioId { get; init; }
}
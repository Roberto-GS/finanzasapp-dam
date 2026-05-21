namespace FinanzasApp.Core.DTOs.Categoria;

/// <summary>
/// Representa los datos necesarios para crear una nueva categoría o para editarla
/// </summary>
public class CrearCategoriaDto
{
    public string Nombre { get; init; } = string.Empty;
    public string? Color { get; init; }
}
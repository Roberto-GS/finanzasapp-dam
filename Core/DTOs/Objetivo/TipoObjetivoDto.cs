namespace FinanzasApp.Core.DTOs.Objetivo;

/// <summary>
/// Son los tipos de objetivos disponibles
/// </summary>
public class TipoObjetivoDto
{
    public int Id { get; init; }
    public string Nombre { get; init; } = string.Empty;
    public string Descripcion { get; init; } = string.Empty;
}
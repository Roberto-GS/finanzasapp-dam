namespace FinanzasApp.Core.DTOs.Notificacion;

/// <summary>
/// Representa una notificacion que se le muestra al usuario, ya sea persistente (db) o dinaminca (generada en tiempo real)
/// </summary>
public class NotificacionDto
{
    public int Id { get; init; }
    public string Titulo { get; init; } = string.Empty;
    public string Mensaje { get; init; } = string.Empty;
    public string Tipo { get; init; } = string.Empty;
    public string Icono { get; init; } = string.Empty;
    public bool Leida { get; init; }
    public DateTime FechaCreacion { get; init; }
    public bool EsDinamica { get; init; }
}
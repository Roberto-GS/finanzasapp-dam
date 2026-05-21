using FinanzasApp.Core.Models;

namespace FinanzasApp.Core.Interfaces;

public interface INotificacionRepository
{
    /// <summary>
    /// Obtenemos todas las notificaciones no leídas de un usuario
    /// </summary>
    Task<IEnumerable<Notificacion>> ObtenerPorUsuarioId(int usuarioId);

    /// <summary>
    /// Comprobamos si ya existe una notificación persistente para un objetivo y tipo concreto
    /// </summary>
    Task<bool> ExisteNotificacionObjetivo(
        int usuarioId, int objetivoId, string tipo);

    /// <summary>
    /// Creamos una nueva notificación presistente en la base de datos
    /// </summary>
    Task<Notificacion> Crear(Notificacion notificacion);

    /// <summary>
    /// Marcamos una notificación persistente como leída por su ID
    /// </summary>
    Task MarcarLeida(int id);

    /// <summary>
    /// Marca todas las notificaciones del usuario como leídas
    /// </summary>
    Task MarcarTodasLeidas(int usuarioId);
}
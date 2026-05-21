using FinanzasApp.Core.DTOs.Notificacion;

namespace FinanzasApp.API.Services;

public interface INotificacionService
{
    /// <summary>
    /// Obtenemos las notificaciones persistentes (almacenadas en la base de datos) y dinámicas (generadas en tiempo real) para un usuario específico, ordenadas por fecha de creación, y marcamos las notificaciones persistentes como leídas.
    /// </summary>
    /// <param name="usuarioId"></param>
    /// <returns></returns>
    Task<IEnumerable<NotificacionDto>> ObtenerNotificaciones(int usuarioId);
    /// <summary>
    /// Evaaluamos los objetivos financieros del usuario y generamos notificaciones dinámicas para aquellos objetivos que estén próximos a vencer o que hayan vencido, con el fin de mantener al usuario informado sobre el estado de sus objetivos y motivarlo a tomar acciones para alcanzarlos.
    /// </summary>
    /// <param name="usuarioId"></param>
    /// <returns></returns>
    Task GenerarNotificacionesObjetivos(int usuarioId);
}
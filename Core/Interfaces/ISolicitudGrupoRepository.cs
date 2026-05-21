using FinanzasApp.Core.Models;

namespace FinanzasApp.Core.Interfaces;

public interface ISolicitudGrupoRepository
{
    /// <summary>
    /// Obtenemos todas las solicitudes de grupo pendientes de un usuario
    /// </summary>
    /// <param name="usuarioId"></param>
    /// <returns></returns>
    Task<IEnumerable<SolicitudGrupo>> ObtenerSolicitudesPendientesPorUsuario(int usuarioId);
    /// <summary>
    /// Obtenemos una solicitud de grupo por su ID
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<SolicitudGrupo?> ObtenerPorId(int id);
    /// <summary>
    /// Creamos una nueva solicitud de grupo
    /// </summary>
    /// <param name="solicitud"></param>
    /// <returns></returns>
    Task<SolicitudGrupo> Crear(SolicitudGrupo solicitud);
    /// <summary>
    /// Actualizamos el estado de una solicitud de grupo
    /// </summary>
    /// <param name="solicitudId"></param>
    /// <param name="estado"></param>
    /// <returns></returns>
    Task ActualizarEstado(int solicitudId, string estado);
    /// <summary>
    /// Comprobamos si ya existe una solicitud de grupo pendiente para un grupo y un usuario concreto
    /// </summary>
    /// <param name="grupoId"></param>
    /// <param name="invitadoId"></param>
    /// <returns></returns>
    Task<bool> ExisteSolicitudPendiente(int grupoId, int invitadoId);
}
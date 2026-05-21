using FinanzasApp.Core.Models;

namespace FinanzasApp.Core.Interfaces;

public interface IObjetivoRepository : IRepository<Objetivo>
{
    /// <summary>
    /// Obtenemos todos los objetivos activos de un usuario
    /// </summary>
    /// <param name="usuarioId"></param>
    /// <returns></returns>
    Task<IEnumerable<Objetivo>> ObtenerPorUsuarioId(int usuarioId);
    /// <summary>
    /// Obtenemos los objetivos activos de un usuario que terminan en los roximos días
    /// </summary>
    /// <param name="usuarioId"></param>
    /// <param name="diasRestantes"></param>
    /// <returns></returns>
    Task<IEnumerable<Objetivo>> ObtenerProximosAVencer(int usuarioId, int diasRestantes);

    /// <summary>
    /// Marca un objetivo como inactivo cuando se cumple o vence
    /// para que no vuelva a evaluarse.
    /// </summary>
    Task MarcarInactivo(int id);
}
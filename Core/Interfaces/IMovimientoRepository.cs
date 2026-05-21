using FinanzasApp.Core.Enums;
using FinanzasApp.Core.Models;

namespace FinanzasApp.Core.Interfaces;

public interface IMovimientoRepository : IRepository<Movimiento>
{
    /// <summary>
    /// Obtenemos todos los movimientos de un usuario, ordenados por sus fechas
    /// </summary>
    /// <param name="usuarioId"></param>
    /// <returns></returns>
    Task<IEnumerable<Movimiento>> ObtenerPorIdUsuario(int usuarioId);
    /// <summary>
    /// Obtenemos los movimientos de un usuario filtrados por año y por mes
    /// </summary>
    /// <param name="usuarioId"></param>
    /// <param name="anio"></param>
    /// <param name="mes"></param>
    /// <returns></returns>
    Task<IEnumerable<Movimiento>> ObtenerPorUsuarioYMes(int usuarioId, int anio, int mes);
    /// <summary>
    /// Obtenemos los movimientos de un usuario filtrados por tipo
    /// </summary>
    /// <param name="usuarioId"></param>
    /// <param name="tipo"></param>
    /// <returns></returns>
    Task<IEnumerable<Movimiento>> ObtenerPorTipo(int usuarioId, TipoMovimiento tipo);
    /// <summary>
    /// Obtiene una determinada cantidad de los ultimos movimientos de un usuario
    /// </summary>
    /// <param name="usuarioId"></param>
    /// <param name="cantidad"></param>
    /// <returns></returns>
    Task<IEnumerable<Movimiento>> ObtenerUltimos(int usuarioId, int cantidad);
    /// <summary>
    /// Calculamos el total de un tipo de movimiento para un usuario en un mes y un año determinado
    /// </summary>
    /// <param name="usuarioId"></param>
    /// <param name="tipo"></param>
    /// <param name="anio"></param>
    /// <param name="mes"></param>
    /// <returns></returns>
    Task<decimal> ObtenerTotalPorTipoYMes(int usuarioId, TipoMovimiento tipo, int anio, int mes);
    /// <summary>
    /// Obtenemos todos los movimientos de un tipo concreto de un usuario en un año determinado
    /// </summary>
    /// <param name="usuarioId"></param>
    /// <param name="tipo"></param>
    /// <param name="anio"></param>
    /// <returns></returns>
    Task<IEnumerable<Movimiento>> ObtenerPorUsuarioTipoYAnio(int usuarioId, TipoMovimiento tipo, int anio);
    /// <summary>
    /// Obtenemos todos los movimientos de un tipo concreto de un usuario en un mes, año y tipo
    /// </summary>
    /// <param name="usuarioId"></param>
    /// <param name="anio"></param>
    /// <param name="mes"></param>
    /// <param name="tipo"></param>
    /// <returns></returns>
    Task<IEnumerable<Movimiento>> ObtenerPorUsuarioMesTipo(int usuarioId, int anio, int mes, TipoMovimiento tipo);
    /// <summary>
    /// Realizamos una busqueda avanzada aplicandodiferentes filtros
    /// </summary>
    /// <param name="usuarioId"></param>
    /// <param name="texto"></param>
    /// <param name="anio"></param>
    /// <param name="mes"></param>
    /// <param name="categoriaId"></param>
    /// <param name="tipo"></param>
    /// <returns></returns>
    Task<IEnumerable<Movimiento>> Buscar(int usuarioId, string? texto, int? anio, int? mes, int? categoriaId, TipoMovimiento? tipo);
}
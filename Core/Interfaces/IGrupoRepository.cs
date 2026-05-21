using FinanzasApp.Core.Models;

namespace FinanzasApp.Core.Interfaces;

public interface IGrupoRepository
{
    /// <summary>
    /// Obtenemos un grupo por su id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<Grupo?> ObtenerPorID(int id);
    /// <summary>
    /// Obtenemos todos los grupos a los que pertenece un usuario a traves de su ID
    /// </summary>
    /// <param name="usuarioId"></param>
    /// <returns></returns>
    Task<IEnumerable<Grupo>> ObtenerGruposPorUsuarioId(int usuarioId);
    /// <summary>
    /// Creamos un nuevo grupo
    /// </summary>
    /// <param name="grupo"></param>
    /// <returns></returns>
    Task<Grupo> Crear(Grupo grupo);
    /// <summary>
    /// Eliminamos un grupo y todos los datos relacionados
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task Eliminar(int id);
    /// <summary>
    /// Comprueba si un usuario es miembro de un grupo
    /// </summary>
    /// <param name="grupoId"></param>
    /// <param name="usuarioId"></param>
    /// <returns></returns>
    Task<bool> EsMiembro(int grupoId, int usuarioId);
    /// <summary>
    /// Añadimos un usuario a un grupo con un rol especifico, si el usuario es miembro no se hace nada
    /// </summary>
    /// <param name="grupoId"></param>
    /// <param name="usuarioId"></param>
    /// <param name="rolId"></param>
    /// <returns></returns>
    Task AgregarMiembro(int grupoId, int usuarioId, int rolId);
    /// <summary>
    /// Obtenemos todos los usuario que pertenecen a un grupo
    /// </summary>
    /// <param name="grupoId"></param>
    /// <returns></returns>
    Task<IEnumerable<Usuario>> ObtenerMiembros(int grupoId);
}
namespace FinanzasApp.Core.Interfaces;

public interface IRepository<T> where T : class
{
    /// <summary>
    /// Obtenemos una entidad por su ID
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<T?> ObtenerPorID(int id);
    /// <summary>
    /// Obtenemos todas las entidades de un tipo concreto
    /// </summary>
    /// <returns></returns>
    Task<IEnumerable<T>> ObtenerTodos();
    /// <summary>
    /// Añadimos una nueva entidad a la base de datos y devolvemos la entidad creada con su ID asignado
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    Task<T> Agregar(T entity);
    /// <summary>
    /// Actualizamos una entidad existente en la base de datos
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    Task Actualizar(T entity);
    /// <summary>
    /// Borramos una entidad de la base de datos por su ID
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task Eliminar(int id);
}
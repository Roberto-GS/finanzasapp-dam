using FinanzasApp.Core.Models;

namespace FinanzasApp.Core.Interfaces;

public interface IUsuarioRepository
{
    /// <summary>
    /// Obtenemos un usuario por su ID
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<Usuario?> ObtenerPorId(int id);
    /// <summary>
    /// Obtenemos un usuario por su email, que es único en la base de datos
    /// </summary>
    /// <param name="email"></param>
    /// <returns></returns>
    Task<Usuario?> ObtenerPorEmail(string email);
    /// <summary>
    /// Añadimos un nuevo usuario a la base de datos y devolvemos la entidad creada con su ID asignado
    /// </summary>
    /// <param name="usuario"></param>
    /// <returns></returns>
    Task<Usuario> Añadir(Usuario usuario);
    /// <summary>
    /// Actualizamos los datos de un usuario
    /// </summary>
    /// <param name="usuario"></param>
    /// <returns></returns>
    Task Actualizar(Usuario usuario);
}
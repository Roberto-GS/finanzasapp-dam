using FinanzasApp.Core.Models;

namespace FinanzasApp.Core.Interfaces;

public interface IRolRepository
{
    /// <summary>
    /// Obtenemos un rol por su nombre
    /// </summary>
    /// <param name="nombre"></param>
    /// <returns></returns>
    Task<Rol?> ObtenerPorNombre(string nombre);
    /// <summary>
    /// Obtenemos un rol por su ID
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<Rol?> ObtenerPorId(int id);
}
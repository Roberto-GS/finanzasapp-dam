using FinanzasApp.Core.Models;

namespace FinanzasApp.Core.Interfaces;

public interface ICategoriaRepository : IRepository<Categoria>
{
    /// <summary>
    /// Obtenemos todas las categorías dispoibles para un usuario
    /// </summary>
    /// <param name="usuarioId"></param>
    /// <returns></returns>
    Task<IEnumerable<Categoria>> ObtenerPorIdUsuario(int usuarioId);
}
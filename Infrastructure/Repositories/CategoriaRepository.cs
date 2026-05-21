using FinanzasApp.Core.Interfaces;
using FinanzasApp.Core.Models;
using FinanzasApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FinanzasApp.Infrastructure.Repositories;

/// <summary>
/// Repositorio en el que gestionamos las operaciones de base de datos relacionadas con las categorías de los usuarios.
/// </summary>
public class CategoriaRepository : ICategoriaRepository
{
    private readonly AppDbContext _contexto;

    public CategoriaRepository(AppDbContext contexto)
    {
        _contexto = contexto;
    }

    /// <inheritdoc/>
    public async Task<Categoria?> ObtenerPorID(int id)
        => await _contexto.Categorias.FindAsync(id);

    /// <inheritdoc/>
    public async Task<IEnumerable<Categoria>> ObtenerTodos()
        => await _contexto.Categorias.ToListAsync();

    /// <inheritdoc/>
    public async Task<IEnumerable<Categoria>> ObtenerPorIdUsuario(int usuarioId)
        => await _contexto.Categorias
            .Where(c => c.UsuarioId == usuarioId || c.UsuarioId == null)
            .OrderBy(c => c.Nombre)
            .ToListAsync();

    /// <inheritdoc/>
    public async Task<Categoria> Agregar(Categoria categoria)
    {
        _contexto.Categorias.Add(categoria);
        await _contexto.SaveChangesAsync();
        return categoria;
    }

    /// <inheritdoc/>
    public async Task Actualizar(Categoria categoria)
    {
        _contexto.Categorias.Update(categoria);
        await _contexto.SaveChangesAsync();
    }

    /// <inheritdoc/>
    public async Task Eliminar(int id)
    {
        Categoria? categoria = await _contexto.Categorias.FindAsync(id);
        if (categoria is not null)
        {
            _contexto.Categorias.Remove(categoria);
            await _contexto.SaveChangesAsync();
        }
    }
}
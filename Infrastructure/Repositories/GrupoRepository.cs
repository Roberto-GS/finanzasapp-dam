using FinanzasApp.Core.Interfaces;
using FinanzasApp.Core.Models;
using FinanzasApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FinanzasApp.Infrastructure.Repositories;

/// <summary>
/// Repositorio en el que gestionamos las operaciones de base de datos relacionadas con los grupos de usuarios.
/// </summary>
public class GrupoRepository : IGrupoRepository
{
    private readonly AppDbContext _contexto;

    public GrupoRepository(AppDbContext contexto)
    {
        _contexto = contexto;
    }

    /// <inheritdoc/>
    public async Task<Grupo?> ObtenerPorID(int id)
        => await _contexto.Grupos
            .Include(g => g.Creador)
            .FirstOrDefaultAsync(g => g.Id == id);

    /// <inheritdoc/>
    public async Task<IEnumerable<Grupo>> ObtenerGruposPorUsuarioId(int usuarioId)
    {
        // Primero obtenemos los IDs de los grupos donde el usuario es miembro
        List<int> grupoIds = await _contexto.UsuariosGrupos
            .Where(ug => ug.UsuarioId == usuarioId)
            .Select(ug => ug.GrupoId)
            .ToListAsync();

        // Luego cargamos esos grupos con su creador
        return await _contexto.Grupos
            .Include(g => g.Creador)
            .Where(g => grupoIds.Contains(g.Id))
            .ToListAsync();
    }

    /// <inheritdoc/>
    public async Task<Grupo> Crear(Grupo grupo)
    {
        _contexto.Grupos.Add(grupo);
        await _contexto.SaveChangesAsync();
        return grupo;
    }

    /// <inheritdoc/>
    public async Task Eliminar(int id)
    {
        Grupo? grupo = await _contexto.Grupos.FindAsync(id);
        if (grupo is not null)
        {
            _contexto.Grupos.Remove(grupo);
            await _contexto.SaveChangesAsync();
        }
    }

    /// <inheritdoc/>
    public async Task<bool> EsMiembro(int grupoId, int usuarioId)
        => await _contexto.UsuariosGrupos
            .AnyAsync(ug => ug.GrupoId == grupoId && ug.UsuarioId == usuarioId);

    /// <inheritdoc/>
    public async Task AgregarMiembro(int grupoId, int usuarioId, int rolId)
    {
        // Verificamos que el usuario no sea actualmente miembro del grupo
        bool yaEsMiembro = await _contexto.UsuariosGrupos
            .AnyAsync(ug => ug.GrupoId == grupoId && ug.UsuarioId == usuarioId);

        if (yaEsMiembro) return;

        UsuarioGrupo nuevoMiembro = new UsuarioGrupo
        {
            GrupoId = grupoId,
            UsuarioId = usuarioId,
            RolId = rolId
        };

        _contexto.UsuariosGrupos.Add(nuevoMiembro);
        await _contexto.SaveChangesAsync();
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<Usuario>> ObtenerMiembros(int grupoId)
        => await _contexto.UsuariosGrupos
            .Where(ug => ug.GrupoId == grupoId)
            .Include(ug => ug.Usuario)
            .Select(ug => ug.Usuario!)
            .Where(u => u != null)
            .ToListAsync();
}
using FinanzasApp.Core.Interfaces;
using FinanzasApp.Core.Models;
using FinanzasApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FinanzasApp.Infrastructure.Repositories;

/// <summary>
/// Repositorio en el que gestionamos las operaciones de base de datos relacionadas con los usuarios del sistema.
/// </summary>
public class UsuarioRepository : IUsuarioRepository
{
    private readonly AppDbContext _contexto;

    public UsuarioRepository(AppDbContext contexto)
    {
        _contexto = contexto;
    }

    /// <inheritdoc/>
    public async Task<Usuario?> ObtenerPorId(int id)
        => await _contexto.Usuarios.FindAsync(id);

    /// <inheritdoc/>
    public async Task<Usuario?> ObtenerPorEmail(string email)
        => await _contexto.Usuarios
            .FirstOrDefaultAsync(u => u.Email == email.ToLowerInvariant());

    /// <inheritdoc/>
    public async Task<Usuario> Añadir(Usuario usuario)
    {
        _contexto.Usuarios.Add(usuario);
        await _contexto.SaveChangesAsync();
        return usuario;
    }

    /// <inheritdoc/>
    public async Task Actualizar(Usuario usuario)
    {
        _contexto.Usuarios.Update(usuario);
        await _contexto.SaveChangesAsync();
    }
}
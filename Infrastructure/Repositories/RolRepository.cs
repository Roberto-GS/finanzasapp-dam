using FinanzasApp.Core.Interfaces;
using FinanzasApp.Core.Models;
using FinanzasApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FinanzasApp.Infrastructure.Repositories;

/// <summary>
/// Repositorio en el que gestionamos las operaciones de base de datos relacionadas con los roles de los usuarios en los grupos.
/// </summary>
public class RolRepository : IRolRepository
{
    private readonly AppDbContext _contexto;

    public RolRepository(AppDbContext contexto)
    {
        _contexto = contexto;
    }

    /// <inheritdoc/>
    public async Task<Rol?> ObtenerPorNombre(string nombre)
        => await _contexto.Roles
            .FirstOrDefaultAsync(r => r.Nombre == nombre);

    /// <inheritdoc/>
    public async Task<Rol?> ObtenerPorId(int id)
        => await _contexto.Roles.FindAsync(id);
}
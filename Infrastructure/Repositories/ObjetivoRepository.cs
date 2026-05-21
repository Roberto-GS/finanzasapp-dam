using FinanzasApp.Core.Interfaces;
using FinanzasApp.Core.Models;
using FinanzasApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FinanzasApp.Infrastructure.Repositories;

/// <summary>
/// Repositorio en el que gestionamos las operaciones de base de datos relacionadas con los objetivos financieros de los usuarios.
/// </summary>
public class ObjetivoRepository : IObjetivoRepository
{
    private readonly AppDbContext _contexto;

    public ObjetivoRepository(AppDbContext contexto)
    {
        _contexto = contexto;
    }

    /// <inheritdoc/>
    public async Task<Objetivo?> ObtenerPorID(int id)
        => await _contexto.Objetivos
            .Include(o => o.TipoObjetivo)
            .Include(o => o.Categoria)
            .FirstOrDefaultAsync(o => o.Id == id);

    /// <inheritdoc/>
    public async Task<IEnumerable<Objetivo>> ObtenerTodos()
        => await _contexto.Objetivos
            .Include(o => o.TipoObjetivo)
            .Include(o => o.Categoria)
            .ToListAsync();

    /// <inheritdoc/>
    public async Task<IEnumerable<Objetivo>> ObtenerPorUsuarioId(int usuarioId)
        => await _contexto.Objetivos
            .Include(o => o.TipoObjetivo)
            .Include(o => o.Categoria)
            .Where(o => o.UsuarioId == usuarioId)
            .OrderBy(o => o.FechaFin)
            .ToListAsync();

    /// <inheritdoc/>
    public async Task<IEnumerable<Objetivo>> ObtenerProximosAVencer(
        int usuarioId, int diasRestantes)
    {
        // Calculamos la fecha límite para el filtro de vencimiento
        DateTime fechaLimite = DateTime.UtcNow.AddDays(diasRestantes);

        return await _contexto.Objetivos
            .Include(o => o.TipoObjetivo)
            .Where(o => o.UsuarioId == usuarioId
                     && o.Activo
                     && o.FechaFin.HasValue
                     && o.FechaFin.Value <= fechaLimite
                     && o.FechaFin.Value >= DateTime.UtcNow)
            .OrderBy(o => o.FechaFin)
            .ToListAsync();
    }

    /// <inheritdoc/>
    public async Task<Objetivo> Agregar(Objetivo objetivo)
    {
        _contexto.Objetivos.Add(objetivo);
        await _contexto.SaveChangesAsync();
        return objetivo;
    }

    /// <inheritdoc/>
    public async Task Actualizar(Objetivo objetivo)
    {
        _contexto.Objetivos.Update(objetivo);
        await _contexto.SaveChangesAsync();
    }

    /// <inheritdoc/>
    public async Task Eliminar(int id)
    {
        Objetivo? objetivo = await _contexto.Objetivos.FindAsync(id);
        if (objetivo is not null)
        {
            _contexto.Objetivos.Remove(objetivo);
            await _contexto.SaveChangesAsync();
        }
    }

    /// <inheritdoc/>
    public async Task MarcarInactivo(int id)
    {
        Objetivo? objetivo = await _contexto.Objetivos.FindAsync(id);
        if (objetivo is not null)
        {
            objetivo.Activo = false;
            await _contexto.SaveChangesAsync();
        }
    }
}
using FinanzasApp.Core.Interfaces;
using FinanzasApp.Core.Models;
using FinanzasApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FinanzasApp.Infrastructure.Repositories;

/// <summary>
/// Repositorio en el que gestionamos las operaciones de base de datos relacionadas con las notificaciones persistentes de los usuarios.
/// </summary>
public class NotificacionRepository : INotificacionRepository
{
    private readonly AppDbContext _contexto;

    public NotificacionRepository(AppDbContext contexto)
    {
        _contexto = contexto;
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<Notificacion>> ObtenerPorUsuarioId(int usuarioId)
        => await _contexto.Notificaciones
            .Where(n => n.UsuarioId == usuarioId && !n.Leida)
            .OrderByDescending(n => n.FechaCreacion)
            .ToListAsync();

    /// <inheritdoc/>
    public async Task<bool> ExisteNotificacionObjetivo(
        int usuarioId, int objetivoId, string tipo)
        => await _contexto.Notificaciones
            .AnyAsync(n => n.UsuarioId == usuarioId
                        && n.ObjetivoId == objetivoId
                        && n.Tipo == tipo);

    /// <inheritdoc/>
    public async Task<Notificacion> Crear(Notificacion notificacion)
    {
        _contexto.Notificaciones.Add(notificacion);
        await _contexto.SaveChangesAsync();
        return notificacion;
    }

    /// <inheritdoc/>
    public async Task MarcarLeida(int id)
    {
        Notificacion? notificacion = await _contexto.Notificaciones.FindAsync(id);
        if (notificacion is not null)
        {
            notificacion.Leida = true;
            await _contexto.SaveChangesAsync();
        }
    }

    /// <inheritdoc/>
    public async Task MarcarTodasLeidas(int usuarioId)
    {
        List<Notificacion> notificaciones = await _contexto.Notificaciones
            .Where(n => n.UsuarioId == usuarioId && !n.Leida)
            .ToListAsync();

        foreach (Notificacion notificacion in notificaciones)
            notificacion.Leida = true;

        await _contexto.SaveChangesAsync();
    }
}
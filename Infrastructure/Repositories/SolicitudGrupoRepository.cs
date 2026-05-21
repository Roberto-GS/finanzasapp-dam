using FinanzasApp.Core.Interfaces;
using FinanzasApp.Core.Models;
using FinanzasApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FinanzasApp.Infrastructure.Repositories;

/// <summary>
/// Repositorio en el que gestionamos las operaciones de base de datos relacionadas con las solicitudes de unión a grupos.
/// </summary>
public class SolicitudGrupoRepository : ISolicitudGrupoRepository
{
    private readonly AppDbContext _contexto;

    public SolicitudGrupoRepository(AppDbContext contexto)
    {
        _contexto = contexto;
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<SolicitudGrupo>> ObtenerSolicitudesPendientesPorUsuario(
        int usuarioId)
        => await _contexto.SolicitudesGrupo
            .Include(s => s.Grupo)
            .Include(s => s.Solicitante)
            .Where(s => s.InvitadoId == usuarioId && s.Estado == "pendiente")
            .OrderByDescending(s => s.FechaSolicitud)
            .ToListAsync();

    /// <inheritdoc/>
    public async Task<SolicitudGrupo?> ObtenerPorId(int id)
        => await _contexto.SolicitudesGrupo
            .Include(s => s.Grupo)
            .Include(s => s.Solicitante)
            .Include(s => s.Invitado)
            .FirstOrDefaultAsync(s => s.Id == id);

    /// <inheritdoc/>
    public async Task<SolicitudGrupo> Crear(SolicitudGrupo solicitud)
    {
        _contexto.SolicitudesGrupo.Add(solicitud);
        await _contexto.SaveChangesAsync();
        return solicitud;
    }

    /// <inheritdoc/>
    public async Task ActualizarEstado(int solicitudId, string estado)
    {
        SolicitudGrupo? solicitud = await _contexto.SolicitudesGrupo.FindAsync(solicitudId);
        if (solicitud is not null)
        {
            solicitud.Estado = estado;
            await _contexto.SaveChangesAsync();
        }
    }

    /// <inheritdoc/>
    public async Task<bool> ExisteSolicitudPendiente(int grupoId, int invitadoId)
        => await _contexto.SolicitudesGrupo
            .AnyAsync(s => s.GrupoId == grupoId
                        && s.InvitadoId == invitadoId
                        && s.Estado == "pendiente");
}
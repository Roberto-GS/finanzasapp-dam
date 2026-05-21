using FinanzasApp.Core.Enums;
using FinanzasApp.Core.Interfaces;
using FinanzasApp.Core.Models;
using FinanzasApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FinanzasApp.Infrastructure.Repositories;

/// <summary>
/// Repositorio en el que gestionamos las operaciones de base de datos relacionadas con los movimientos económicos de los usuarios.
/// </summary>
public class MovimientoRepository : IMovimientoRepository
{
    private readonly AppDbContext _contexto;

    public MovimientoRepository(AppDbContext contexto)
    {
        _contexto = contexto;
    }

    /// <inheritdoc/>
    public async Task<Movimiento?> ObtenerPorID(int id)
        => await _contexto.Movimientos
            .Include(m => m.Categoria)
            .FirstOrDefaultAsync(m => m.Id == id);

    /// <inheritdoc/>
    public async Task<IEnumerable<Movimiento>> ObtenerTodos()
        => await _contexto.Movimientos
            .Include(m => m.Categoria)
            .ToListAsync();

    /// <inheritdoc/>
    public async Task<IEnumerable<Movimiento>> ObtenerPorIdUsuario(int usuarioId)
        => await _contexto.Movimientos
            .Include(m => m.Categoria)
            .Where(m => m.UsuarioId == usuarioId)
            .OrderByDescending(m => m.Fecha)
            .ToListAsync();

    /// <inheritdoc/>
    public async Task<IEnumerable<Movimiento>> ObtenerPorUsuarioYMes(
        int usuarioId, int anio, int mes)
        => await _contexto.Movimientos
            .Include(m => m.Categoria)
            .Where(m => m.UsuarioId == usuarioId
                     && m.Fecha.Year == anio
                     && m.Fecha.Month == mes)
            .OrderByDescending(m => m.Fecha)
            .ToListAsync();

    /// <inheritdoc/>
    public async Task<IEnumerable<Movimiento>> ObtenerPorTipo(
        int usuarioId, TipoMovimiento tipo)
        => await _contexto.Movimientos
            .Include(m => m.Categoria)
            .Where(m => m.UsuarioId == usuarioId && m.Tipo == tipo)
            .OrderByDescending(m => m.Fecha)
            .ToListAsync();

    /// <inheritdoc/>
    public async Task<IEnumerable<Movimiento>> ObtenerUltimos(
        int usuarioId, int cantidad)
        => await _contexto.Movimientos
            .Include(m => m.Categoria)
            .Where(m => m.UsuarioId == usuarioId)
            .OrderByDescending(m => m.Fecha)
            .Take(cantidad)
            .ToListAsync();

    /// <inheritdoc/>
    public async Task<decimal> ObtenerTotalPorTipoYMes(
        int usuarioId, TipoMovimiento tipo, int anio, int mes)
        => await _contexto.Movimientos
            .Where(m => m.UsuarioId == usuarioId
                     && m.Tipo == tipo
                     && m.Fecha.Year == anio
                     && m.Fecha.Month == mes)
            .SumAsync(m => m.Cantidad);

    /// <inheritdoc/>
    public async Task<IEnumerable<Movimiento>> ObtenerPorUsuarioTipoYAnio(
        int usuarioId, TipoMovimiento tipo, int anio)
        => await _contexto.Movimientos
            .Include(m => m.Categoria)
            .Where(m => m.UsuarioId == usuarioId
                     && m.Tipo == tipo
                     && m.Fecha.Year == anio)
            .OrderBy(m => m.Fecha)
            .ToListAsync();

    /// <inheritdoc/>
    public async Task<IEnumerable<Movimiento>> ObtenerPorUsuarioMesTipo(
        int usuarioId, int anio, int mes, TipoMovimiento tipo)
        => await _contexto.Movimientos
            .Include(m => m.Categoria)
            .Where(m => m.UsuarioId == usuarioId
                     && m.Tipo == tipo
                     && m.Fecha.Year == anio
                     && m.Fecha.Month == mes)
            .OrderByDescending(m => m.Fecha)
            .ToListAsync();

    /// <inheritdoc/>
    public async Task<IEnumerable<Movimiento>> Buscar(
        int usuarioId, string? texto,
        int? anio, int? mes,
        int? categoriaId, TipoMovimiento? tipo)
    {
        // Construimos la consulta aplicando los filtros opcionales
        IQueryable<Movimiento> consulta = _contexto.Movimientos
            .Include(m => m.Categoria)
            .Where(m => m.UsuarioId == usuarioId);

        if (!string.IsNullOrWhiteSpace(texto))
            consulta = consulta.Where(m =>
                m.Nombre.Contains(texto) ||
                (m.Descripcion != null && m.Descripcion.Contains(texto)) ||
                (m.Etiqueta != null && m.Etiqueta.Contains(texto)));

        if (anio.HasValue)
            consulta = consulta.Where(m => m.Fecha.Year == anio.Value);

        if (mes.HasValue)
            consulta = consulta.Where(m => m.Fecha.Month == mes.Value);

        if (categoriaId.HasValue)
            consulta = consulta.Where(m => m.CategoriaId == categoriaId.Value);

        if (tipo.HasValue)
            consulta = consulta.Where(m => m.Tipo == tipo.Value);

        return await consulta
            .OrderByDescending(m => m.Fecha)
            .ToListAsync();
    }

    /// <inheritdoc/>
    public async Task<Movimiento> Agregar(Movimiento movimiento)
    {
        _contexto.Movimientos.Add(movimiento);
        await _contexto.SaveChangesAsync();
        return movimiento;
    }

    /// <inheritdoc/>
    public async Task Actualizar(Movimiento movimiento)
    {
        _contexto.Movimientos.Update(movimiento);
        await _contexto.SaveChangesAsync();
    }

    /// <inheritdoc/>
    public async Task Eliminar(int id)
    {
        Movimiento? movimiento = await _contexto.Movimientos.FindAsync(id);
        if (movimiento is not null)
        {
            _contexto.Movimientos.Remove(movimiento);
            await _contexto.SaveChangesAsync();
        }
    }
}
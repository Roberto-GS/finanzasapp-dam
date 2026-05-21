using FinanzasApp.Core.DTOs.Objetivo;
using FinanzasApp.Core.Enums;
using FinanzasApp.Core.Interfaces;
using FinanzasApp.Core.Models;

namespace FinanzasApp.API.Services;

/// <summary>
/// Implementa la lógica de evaluación de objetivos financieros calculando
/// el progreso real en base a los movimientos del periodo definido.
/// </summary>
public class ObjetivoService : IObjetivoService
{
    private readonly IObjetivoRepository _objetivoRepository;
    private readonly IMovimientoRepository _movimientoRepository;

    public ObjetivoService(
        IObjetivoRepository objetivoRepository,
        IMovimientoRepository movimientoRepository)
    {
        _objetivoRepository = objetivoRepository;
        _movimientoRepository = movimientoRepository;
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<ObjetivoConProgresoDto>> EvaluarTodos(int usuarioId)
    {
        IEnumerable<Objetivo> objetivos =
            await _objetivoRepository.ObtenerPorUsuarioId(usuarioId);

        List<Objetivo> activos = objetivos.Where(o => o.Activo).ToList();
        List<ObjetivoConProgresoDto> resultado = new List<ObjetivoConProgresoDto>();

        if (!activos.Any()) return resultado;

        // Cargamos TODOS los movimientos una sola vez para todos los objetivos
        // Antes se hacía una llamada por objetivo, ahora solo una
        IEnumerable<Movimiento> todosLosMovimientos =
            await _movimientoRepository.ObtenerPorIdUsuario(usuarioId);

        foreach (Objetivo obj in activos)
            resultado.Add(EvaluarObjetivoConMovimientos(obj, todosLosMovimientos));

        return resultado;
    }

    /// <inheritdoc/>
    public async Task<ObjetivoConProgresoDto> EvaluarObjetivo(int objetivoId, int usuarioId)
    {
        Objetivo objetivo = await _objetivoRepository.ObtenerPorID(objetivoId)
            ?? throw new KeyNotFoundException("Objetivo no encontrado");

        IEnumerable<Movimiento> todosLosMovimientos =
            await _movimientoRepository.ObtenerPorIdUsuario(usuarioId);

        return EvaluarObjetivoConMovimientos(objetivo, todosLosMovimientos);
    }

    /// <summary>
    /// Evalúa un objetivo usando una lista de movimientos ya cargada en memoria.
    /// Evita llamadas repetidas a la base de datos cuando se evalúan varios objetivos.
    /// </summary>
    private ObjetivoConProgresoDto EvaluarObjetivoConMovimientos(
        Objetivo objetivo, IEnumerable<Movimiento> todosLosMovimientos)
    {
        // Convertimos las fechas del objetivo a DateOnly para comparar
        // con el campo Fecha del movimiento que también es DateOnly
        DateOnly fechaInicio = objetivo.FechaInicio.HasValue
            ? DateOnly.FromDateTime(objetivo.FechaInicio.Value)
            : DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(-1));

        DateOnly fechaFin = objetivo.FechaFin.HasValue
            ? DateOnly.FromDateTime(objetivo.FechaFin.Value)
            : DateOnly.FromDateTime(DateTime.UtcNow);

        // Filtramos los movimientos en memoria por el rango del objetivo
        IEnumerable<Movimiento> movimientos = todosLosMovimientos
            .Where(m =>
            {
                DateOnly fechaMovimiento = DateOnly.FromDateTime(m.Fecha);
                return fechaMovimiento >= fechaInicio && fechaMovimiento <= fechaFin;
            });

        // Filtramos por categoría si el objetivo la tiene definida
        if (objetivo.CategoriaId.HasValue)
            movimientos = movimientos
                .Where(m => m.CategoriaId == objetivo.CategoriaId);

        decimal totalGastos = movimientos
            .Where(m => m.Tipo == TipoMovimiento.Gasto)
            .Sum(m => m.Cantidad);

        decimal totalIngresos = movimientos
            .Where(m => m.Tipo == TipoMovimiento.Ingreso)
            .Sum(m => m.Cantidad);

        decimal balance = totalIngresos - totalGastos;

        TipoObjetivoEnum tipoId = (TipoObjetivoEnum)objetivo.TipoId;
        decimal cantidadActual;
        double progreso;
        string estado;
        bool estaEnPeligro;
        bool seHaCumplido;
        string textoProgreso;

        switch (tipoId)
        {
            case TipoObjetivoEnum.Ahorro:
                cantidadActual = balance;
                progreso = objetivo.CantidadObjetivo.HasValue && objetivo.CantidadObjetivo > 0
                    ? Math.Min(Math.Max(
                        (double)(balance / objetivo.CantidadObjetivo.Value), 0), 1.0)
                    : 0;
                seHaCumplido = objetivo.CantidadObjetivo.HasValue
                    && balance >= objetivo.CantidadObjetivo.Value;
                estaEnPeligro = balance < 0 || (
                    objetivo.CantidadObjetivo.HasValue &&
                    balance < objetivo.CantidadObjetivo.Value * 0.5m);
                estado = seHaCumplido ? "Meta alcanzada"
                    : balance < 0 ? "Balance negativo" : "En progreso";
                textoProgreso = objetivo.CantidadObjetivo.HasValue
                    ? $"Balance: {balance:C2} de {objetivo.CantidadObjetivo:C2} objetivo"
                    : $"Balance actual: {balance:C2}";
                break;

            case TipoObjetivoEnum.LimiteGasto:
                cantidadActual = totalGastos;
                progreso = objetivo.CantidadObjetivo.HasValue && objetivo.CantidadObjetivo > 0
                    ? Math.Min((double)(totalGastos / objetivo.CantidadObjetivo.Value), 1.0)
                    : 0;
                seHaCumplido = objetivo.CantidadObjetivo.HasValue
                    && totalGastos <= objetivo.CantidadObjetivo.Value;
                estaEnPeligro = objetivo.CantidadObjetivo.HasValue
                    && totalGastos >= objetivo.CantidadObjetivo.Value * 0.8m;
                estado = objetivo.CantidadObjetivo.HasValue
                    && totalGastos > objetivo.CantidadObjetivo.Value
                        ? "Límite superado"
                        : estaEnPeligro ? "Cerca del límite" : "Dentro del límite";
                textoProgreso = objetivo.CantidadObjetivo.HasValue
                    ? $"Gastado {totalGastos:C2} de {objetivo.CantidadObjetivo:C2} límite"
                    : $"Gastado {totalGastos:C2}";
                break;

            case TipoObjetivoEnum.MetaIngreso:
                cantidadActual = totalIngresos;
                progreso = objetivo.CantidadObjetivo.HasValue && objetivo.CantidadObjetivo > 0
                    ? Math.Min((double)(totalIngresos / objetivo.CantidadObjetivo.Value), 1.0)
                    : 0;
                seHaCumplido = objetivo.CantidadObjetivo.HasValue
                    && totalIngresos >= objetivo.CantidadObjetivo.Value;
                estaEnPeligro = objetivo.CantidadObjetivo.HasValue
                    && !seHaCumplido
                    && totalIngresos < objetivo.CantidadObjetivo.Value * 0.5m;
                estado = seHaCumplido ? "Meta alcanzada"
                    : estaEnPeligro ? "Lejos de la meta" : "En progreso";
                textoProgreso = objetivo.CantidadObjetivo.HasValue
                    ? $"Ingresado {totalIngresos:C2} de {objetivo.CantidadObjetivo:C2}"
                    : $"Total ingresos: {totalIngresos:C2}";
                break;

            case TipoObjetivoEnum.ReduccionDeuda:
                cantidadActual = totalGastos;
                progreso = objetivo.CantidadObjetivo.HasValue && objetivo.CantidadObjetivo > 0
                    ? Math.Max(
                        1.0 - (double)(totalGastos / objetivo.CantidadObjetivo.Value), 0)
                    : 0;
                seHaCumplido = objetivo.CantidadObjetivo.HasValue
                    && totalGastos <= objetivo.CantidadObjetivo.Value * 0.5m;
                estaEnPeligro = objetivo.CantidadObjetivo.HasValue
                    && totalGastos >= objetivo.CantidadObjetivo.Value;
                estado = seHaCumplido ? "Deuda reducida"
                    : estaEnPeligro ? "Límite alcanzado" : "Reduciendo";
                textoProgreso = objetivo.CantidadObjetivo.HasValue
                    ? $"Gastado {totalGastos:C2} en categoría — " +
                      $"límite: {objetivo.CantidadObjetivo:C2}"
                    : $"Gastos en categoría: {totalGastos:C2}";
                break;

            default:
                cantidadActual = 0;
                progreso = 0;
                estado = "Sin evaluar";
                estaEnPeligro = false;
                seHaCumplido = false;
                textoProgreso = string.Empty;
                break;
        }

        return new ObjetivoConProgresoDto
        {
            Id = objetivo.Id,
            Descripcion = objetivo.Descripcion ?? string.Empty,
            TipoId = objetivo.TipoId,
            TipoNombre = objetivo.TipoObjetivo?.Nombre ?? string.Empty,
            CategoriaNombre = objetivo.Categoria?.Nombre,
            CategoriaId = objetivo.CategoriaId,
            Periodo = objetivo.Periodo,
            CantidadObjetivo = objetivo.CantidadObjetivo,
            CantidadActual = cantidadActual,
            Progreso = progreso,
            TextoProgreso = textoProgreso,
            Estado = estado,
            EstaVencido = objetivo.EstaVencido,
            EstaEnPeligro = estaEnPeligro,
            SeHaCumplido = seHaCumplido,
            FechaInicio = objetivo.FechaInicio,
            FechaFin = objetivo.FechaFin,
            Activo = objetivo.Activo
        };
    }
}
using FinanzasApp.Core.DTOs.Notificacion;
using FinanzasApp.Core.DTOs.Objetivo;
using FinanzasApp.Core.Enums;
using FinanzasApp.Core.Interfaces;
using FinanzasApp.Core.Models;

namespace FinanzasApp.API.Services;

public class NotificacionService : INotificacionService
{
    private readonly INotificacionRepository _notificacionRepository;
    private readonly IObjetivoService _objetivoService;
    private readonly IObjetivoRepository _objetivoRepository;
    private readonly IMovimientoRepository _movimientoRepository;

    // Semáforo para evitar que dos peticiones simultáneas creen notificaciones duplicadas
    private static readonly SemaphoreSlim _semaforo = new SemaphoreSlim(1, 1);

    public NotificacionService(
        INotificacionRepository notificacionRepository,
        IObjetivoService objetivoService,
        IObjetivoRepository objetivoRepository,
        IMovimientoRepository movimientoRepository)
    {
        _notificacionRepository = notificacionRepository;
        _objetivoService = objetivoService;
        _objetivoRepository = objetivoRepository;
        _movimientoRepository = movimientoRepository;
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<NotificacionDto>> ObtenerNotificaciones(int usuarioId)
    {
        try { await GenerarNotificacionesObjetivos(usuarioId); }
        catch (Exception) { }

        List<NotificacionDto> resultado = new List<NotificacionDto>();
        DateTime ahora = DateTime.UtcNow;

        // Notificaciones persistentes no leídas
        IEnumerable<Notificacion> persistentes = await _notificacionRepository.ObtenerPorUsuarioId(usuarioId);

        // Guardamos los IDs de objetivos que ya tienen notificación persistente
        // para no generar dinámicas duplicadas para los mismos objetivos
        HashSet<int> objetivosConNotificacionPersistente = persistentes
            .Where(n => n.ObjetivoId.HasValue)
            .Select(n => n.ObjetivoId!.Value)
            .ToHashSet();

        resultado.AddRange(persistentes.Select(n => new NotificacionDto
        {
            Id = n.Id,
            Titulo = n.Titulo,
            Mensaje = n.Mensaje,
            Tipo = n.Tipo,
            Icono = n.Icono,
            Leida = n.Leida,
            FechaCreacion = n.FechaCreacion,
            EsDinamica = false
        }));

        // Notificaciones dinámicas en tiempo real
        try
        {
            IEnumerable<ObjetivoConProgresoDto> objetivosEvaluados =
                await _objetivoService.EvaluarTodos(usuarioId);

            foreach (ObjetivoConProgresoDto obj in objetivosEvaluados
                .Where(o => o.EstaEnPeligro && !o.EstaVencido))
            {
                // Si el objetivo ya tiene notificación persistente no añadimos dinámica
                if (objetivosConNotificacionPersistente.Contains(obj.Id)) continue;

                resultado.Add(new NotificacionDto
                {
                    Id = 0,
                    Titulo = $"⚠️ Alerta: {obj.TipoNombre}",
                    Mensaje = obj.TextoProgreso,
                    Tipo = "peligro",
                    Icono = "⚠️",
                    Leida = false,
                    FechaCreacion = ahora,
                    EsDinamica = true
                });
            }

            // Objetivos próximos a vencer
            IEnumerable<Objetivo> proximosAVencer = await _objetivoRepository.ObtenerProximosAVencer(usuarioId, 3);

            foreach (Objetivo obj in proximosAVencer)
            {
                // Si el objetivo ya tiene notificación persistente no añadimos dinámica
                if (objetivosConNotificacionPersistente.Contains(obj.Id)) continue;

                string textoVencimiento;
                if (obj.FechaFin.HasValue)
                {
                    TimeSpan restante = obj.FechaFin.Value - ahora;

                    if (restante.TotalDays >= 2)
                        textoVencimiento = $"en {(int)restante.TotalDays} días";
                    else if (restante.TotalHours >= 1)
                        textoVencimiento = $"en {(int)restante.TotalHours}h {restante.Minutes}min";
                    else if (restante.TotalMinutes >= 1)
                        textoVencimiento = $"en {(int)restante.TotalMinutes} min";
                    else
                        textoVencimiento = $"hoy a las {obj.FechaFin.Value.ToLocalTime():HH:mm}";
                }
                else
                {
                    textoVencimiento = "próximamente";
                }

                resultado.Add(new NotificacionDto
                {
                    Id = 0,
                    Titulo = "Objetivo próximo a vencer",
                    Mensaje = $"'{obj.Descripcion}' vence {textoVencimiento}",
                    Tipo = "advertencia",
                    Icono = "⏰",
                    Leida = false,
                    FechaCreacion = ahora,
                    EsDinamica = true
                });
            }
        }
        catch (Exception) { }

        // Exceso de gasto respecto al mes anterior
        try
        {
            decimal gastosMesActual = await _movimientoRepository
                .ObtenerTotalPorTipoYMes(
                    usuarioId, TipoMovimiento.Gasto, ahora.Year, ahora.Month);

            DateTime mesAnterior = ahora.AddMonths(-1);
            decimal gastosMesAnterior = await _movimientoRepository
                .ObtenerTotalPorTipoYMes(
                    usuarioId, TipoMovimiento.Gasto,
                    mesAnterior.Year, mesAnterior.Month);

            if (gastosMesAnterior > 0 && gastosMesActual > gastosMesAnterior * 1.2m)
            {
                int porcentaje = (int)((gastosMesActual - gastosMesAnterior)
                    / gastosMesAnterior * 100);

                resultado.Add(new NotificacionDto
                {
                    Id = 0,
                    Titulo = "Exceso de gasto mensual",
                    Mensaje = $"Gastas un {porcentaje}% más que el mes anterior " +
                              $"({gastosMesActual:C2} vs {gastosMesAnterior:C2})",
                    Tipo = "peligro",
                    Icono = "📈",
                    Leida = false,
                    FechaCreacion = ahora,
                    EsDinamica = true
                });
            }
        }
        catch (Exception) { }

        return resultado
            .OrderByDescending(n => n.Tipo == "peligro")
            .ThenByDescending(n => n.Tipo == "advertencia")
            .ThenByDescending(n => n.FechaCreacion)
            .ToList();
    }

    /// <inheritdoc/>
    public async Task GenerarNotificacionesObjetivos(int usuarioId)
    {
        // Solo una petición a la vez puede generar notificaciones
        // Las demás esperan hasta que termine
        await _semaforo.WaitAsync();
        try
        {
            IEnumerable<ObjetivoConProgresoDto> objetivosEvaluados =
                await _objetivoService.EvaluarTodos(usuarioId);

            foreach (ObjetivoConProgresoDto obj in objetivosEvaluados)
            {
                TipoObjetivoEnum tipoId = (TipoObjetivoEnum)obj.TipoId;

                bool esCumplimientoInmediato =
                    tipoId == TipoObjetivoEnum.MetaIngreso ||
                    tipoId == TipoObjetivoEnum.Ahorro;

                if (esCumplimientoInmediato && obj.SeHaCumplido)
                {
                    bool yaExiste = await _notificacionRepository
                        .ExisteNotificacionObjetivo(usuarioId, obj.Id, "cumplido");

                    if (!yaExiste)
                        await _notificacionRepository.Crear(new Notificacion
                        {
                            UsuarioId = usuarioId,
                            ObjetivoId = obj.Id,
                            Titulo = "🎉 ¡Objetivo cumplido!",
                            Mensaje = $"Has alcanzado tu objetivo: " +
                                      $"'{obj.Descripcion}'. {obj.TextoProgreso}",
                            Tipo = "cumplido",
                            Icono = "✅"
                        });

                    await _objetivoRepository.MarcarInactivo(obj.Id);
                    continue;
                }

                bool esCumplimientoAlVencer =
                    tipoId == TipoObjetivoEnum.LimiteGasto ||
                    tipoId == TipoObjetivoEnum.ReduccionDeuda;

                if (!esCumplimientoAlVencer || !obj.EstaVencido) continue;

                if (obj.SeHaCumplido)
                {
                    bool yaExiste = await _notificacionRepository.ExisteNotificacionObjetivo(usuarioId, obj.Id, "cumplido");

                    if (!yaExiste)
                        await _notificacionRepository.Crear(new Notificacion
                        {
                            UsuarioId = usuarioId,
                            ObjetivoId = obj.Id,
                            Titulo = "🎉 ¡Objetivo cumplido!",
                            Mensaje = $"Completaste el objetivo " +
                                      $"'{obj.Descripcion}'. {obj.TextoProgreso}",
                            Tipo = "cumplido",
                            Icono = "✅"
                        });
                }
                else
                {
                    bool yaExiste = await _notificacionRepository
                        .ExisteNotificacionObjetivo(usuarioId, obj.Id, "fallido");

                    if (!yaExiste)
                        await _notificacionRepository.Crear(new Notificacion
                        {
                            UsuarioId = usuarioId,
                            ObjetivoId = obj.Id,
                            Titulo = "❌ Objetivo no alcanzado",
                            Mensaje = $"El objetivo '{obj.Descripcion}' ha vencido " +
                                      $"sin cumplirse. {obj.TextoProgreso}",
                            Tipo = "fallido",
                            Icono = "❌"
                        });
                }

                await _objetivoRepository.MarcarInactivo(obj.Id);
            }
        }
        finally
        {
            // Liberamos el semáforo siempre, aunque haya error
            _semaforo.Release();
        }
    }
}
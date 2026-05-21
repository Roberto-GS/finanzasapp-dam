using FinanzasApp.API.Extensions;
using FinanzasApp.Core.DTOs.Grupo;
using FinanzasApp.Core.Interfaces;
using FinanzasApp.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinanzasApp.API.Controllers;

/// <summary>
/// Gestionamos las operaciones relacionadas con los grupos
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class GruposController : ControllerBase
{
    private readonly IGrupoRepository _grupoRepository;
    private readonly ISolicitudGrupoRepository _solicitudRepository;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IMovimientoRepository _movimientoRepository;
    private readonly IRolRepository _rolRepository;

    public GruposController(
        IGrupoRepository grupoRepository,
        ISolicitudGrupoRepository solicitudRepository,
        IUsuarioRepository usuarioRepository,
        IMovimientoRepository movimientoRepository,
        IRolRepository rolRepository)
    {
        _grupoRepository = grupoRepository;
        _solicitudRepository = solicitudRepository;
        _usuarioRepository = usuarioRepository;
        _movimientoRepository = movimientoRepository;
        _rolRepository = rolRepository;
    }

    /// <summary>
    /// Obtenemos todos los grupos a los que pertenece el usuario
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<IActionResult> ObtenerMisGrupos()
    {
        int usuarioId = User.ObtenerUsuarioId();
        IEnumerable<Grupo> grupos = await _grupoRepository.ObtenerGruposPorUsuarioId(usuarioId);

        List<GrupoDto> resultado = new List<GrupoDto>();
        foreach (Grupo grupo in grupos)
        {
            IEnumerable<Usuario> miembros = await _grupoRepository.ObtenerMiembros(grupo.Id);
            resultado.Add(new GrupoDto
            {
                Id = grupo.Id,
                Nombre = grupo.Nombre,
                CreadorId = grupo.CreadorId,
                CreadorNombre = grupo.Creador?.Nombre ?? string.Empty,
                FechaCreacion = grupo.FechaCreacion,
                TotalMiembros = miembros.Count(),
                SoyCreador = grupo.CreadorId == usuarioId
            });
        }

        return Ok(resultado);
    }

    /// <summary>
    /// Creamos un nuevo grupo y automáticamente lo añadimos como miembro con rol de administrador
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="InvalidOperationException"></exception>
    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] CrearGrupoDto dto)
    {
        int usuarioId = User.ObtenerUsuarioId();

        if (string.IsNullOrWhiteSpace(dto.Nombre))
            throw new ArgumentException("El nombre del grupo no puede estar vacío");

        Grupo grupo = new Grupo
        {
            Nombre = dto.Nombre,
            CreadorId = usuarioId
        };

        await _grupoRepository.Crear(grupo);

        // El creador se añade automáticamente como administrador
        Rol rolAdmin = await _rolRepository.ObtenerPorNombre("administrador")
            ?? throw new InvalidOperationException(
                "No se encontró el rol de administrador");

        await _grupoRepository.AgregarMiembro(grupo.Id, usuarioId, rolAdmin.Id);

        IEnumerable<Usuario> miembros = await _grupoRepository.ObtenerMiembros(grupo.Id);

        return CreatedAtAction(nameof(ObtenerResumen), new { id = grupo.Id },
            new GrupoDto
            {
                Id = grupo.Id,
                Nombre = grupo.Nombre,
                CreadorId = grupo.CreadorId,
                FechaCreacion = grupo.FechaCreacion,
                TotalMiembros = miembros.Count(),
                SoyCreador = true
            });
    }

    /// <summary>
    /// Eliminamos un grupo, solo el creador puede eliminarlo y se eliminan todos los datos relacionados
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    /// <exception cref="KeyNotFoundException"></exception>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Eliminar(int id)
    {
        int usuarioId = User.ObtenerUsuarioId();
        Grupo grupo = await _grupoRepository.ObtenerPorID(id)
            ?? throw new KeyNotFoundException("Grupo no encontrado");

        if (grupo.CreadorId != usuarioId)
            return Forbid();

        await _grupoRepository.Eliminar(id);
        return NoContent();
    }

    /// <summary>
    /// Obtenemos un resumen del grupo con el total de gastos e ingresos del mes actual y un ranking de los miembros ordenado por gastos
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    /// <exception cref="KeyNotFoundException"></exception>
    [HttpGet("{id:int}/resumen")]
    public async Task<IActionResult> ObtenerResumen(int id)
    {
        int usuarioId = User.ObtenerUsuarioId();

        bool esMiembro = await _grupoRepository.EsMiembro(id, usuarioId);
        if (!esMiembro) return Forbid();

        Grupo grupo = await _grupoRepository.ObtenerPorID(id)
            ?? throw new KeyNotFoundException("Grupo no encontrado");

        IEnumerable<Usuario> miembros = await _grupoRepository.ObtenerMiembros(id);
        DateTime ahora = DateTime.UtcNow;

        // Calculamos los gastos e ingresos del mes actual para cada miembro
        List<MiembroGrupoDto> ranking = new List<MiembroGrupoDto>();
        foreach (Usuario miembro in miembros)
        {
            decimal gastos = await _movimientoRepository
                .ObtenerTotalPorTipoYMes(miembro.Id,
                    Core.Enums.TipoMovimiento.Gasto, ahora.Year, ahora.Month);
            decimal ingresos = await _movimientoRepository
                .ObtenerTotalPorTipoYMes(miembro.Id,
                    Core.Enums.TipoMovimiento.Ingreso, ahora.Year, ahora.Month);

            ranking.Add(new MiembroGrupoDto
            {
                UsuarioId = miembro.Id,
                Nombre = miembro.Nombre,
                Email = miembro.Email,
                TotalGastos = gastos,
                TotalIngresos = ingresos,
                EsCreador = miembro.Id == grupo.CreadorId
            });
        }

        // Ordenamos por gastos descendente y asignamos la posición aquí en el servidor
        // así el converter de la UI solo recibe un int y no acumula estado
        List<MiembroGrupoDto> rankingOrdenado = ranking
            .OrderByDescending(m => m.TotalGastos)
            .ToList();

        for (int i = 0; i < rankingOrdenado.Count; i++)
            rankingOrdenado[i].Posicion = i + 1;

        ResumenGrupoDto resumen = new ResumenGrupoDto
        {
            GrupoId = grupo.Id,
            GrupoNombre = grupo.Nombre,
            TotalGastosGrupo = rankingOrdenado.Sum(m => m.TotalGastos),
            TotalIngresosGrupo = rankingOrdenado.Sum(m => m.TotalIngresos),
            Ranking = rankingOrdenado
        };

        return Ok(resumen);
    }

    // Solicitudes
    /// <summary>
    /// Enviamos una invitación a un usuario para unirse a un grupo, solo los miembros del grupo pueden invitar a otros usuarios
    /// </summary>
    /// <param name="id"></param>
    /// <param name="dto"></param>
    /// <returns></returns>
    /// <exception cref="KeyNotFoundException"></exception>
    /// <exception cref="ArgumentException"></exception>
    [HttpPost("{id:int}/invitar")]
    public async Task<IActionResult> Invitar(int id, [FromBody] InvitarUsuarioDto dto)
    {
        int usuarioId = User.ObtenerUsuarioId();

        bool esMiembro = await _grupoRepository.EsMiembro(id, usuarioId);
        if (!esMiembro) return Forbid();

        // Buscar por email en minúsculas
        string emailBuscado = dto.EmailInvitado.Trim().ToLowerInvariant();
        Usuario invitado = await _usuarioRepository.ObtenerPorEmail(emailBuscado)
            ?? throw new KeyNotFoundException(
                $"No existe ningún usuario con el email '{dto.EmailInvitado}'");

        if (invitado.Id == usuarioId)
            throw new ArgumentException("No puedes invitarte a ti mismo");

        if (await _grupoRepository.EsMiembro(id, invitado.Id))
            throw new ArgumentException("Este usuario ya es miembro del grupo");

        if (await _solicitudRepository.ExisteSolicitudPendiente(id, invitado.Id))
            throw new ArgumentException("Ya existe una solicitud pendiente para este usuario");

        SolicitudGrupo solicitud = new SolicitudGrupo
        {
            GrupoId = id,
            SolicitanteId = usuarioId,
            InvitadoId = invitado.Id,
            Estado = "pendiente"
        };

        await _solicitudRepository.Crear(solicitud);

        return Ok(new { mensaje = $"Solicitud enviada a {invitado.Nombre}" });
    }

    /// <summary>
    /// Obtenemos todas las solicitudes de grupo pendientes para el usuario
    /// </summary>
    /// <returns></returns>
    [HttpGet("solicitudes")]
    public async Task<IActionResult> ObtenerSolicitudesPendientes()
    {
        int usuarioId = User.ObtenerUsuarioId();
        IEnumerable<SolicitudGrupo> solicitudes = await _solicitudRepository
            .ObtenerSolicitudesPendientesPorUsuario(usuarioId);

        IEnumerable<SolicitudGrupoDto> resultado = solicitudes.Select(s => new SolicitudGrupoDto
        {
            Id = s.Id,
            GrupoId = s.GrupoId,
            GrupoNombre = s.Grupo?.Nombre ?? string.Empty,
            SolicitanteId = s.SolicitanteId,
            SolicitanteNombre = s.Solicitante?.Nombre ?? string.Empty,
            Estado = s.Estado,
            FechaSolicitud = s.FechaSolicitud
        });

        return Ok(resultado);
    }

    /// <summary>
    /// Aceptamos una solicitud de grupo
    /// </summary>
    /// <param name="solicitudId"></param>
    /// <returns></returns>
    /// <exception cref="KeyNotFoundException"></exception>
    /// <exception cref="ArgumentException"></exception>
    [HttpPost("solicitudes/{solicitudId:int}/aceptar")]
    public async Task<IActionResult> AceptarSolicitud(int solicitudId)
    {
        int usuarioId = User.ObtenerUsuarioId();
        SolicitudGrupo solicitud = await _solicitudRepository.ObtenerPorId(solicitudId)
            ?? throw new KeyNotFoundException("Solicitud no encontrada");

        if (solicitud.InvitadoId != usuarioId)
            return Forbid();

        if (solicitud.Estado != "pendiente")
            throw new ArgumentException("Esta solicitud ya ha sido procesada");

        await _solicitudRepository.ActualizarEstado(solicitudId, "aceptada");
        await _grupoRepository.AgregarMiembro(solicitud.GrupoId, usuarioId, 2);

        return Ok(new { mensaje = "Te has unido al grupo" });
    }

    /// <summary>
    /// Rechazamos una solicitud de unión a un grupo
    /// </summary>
    /// <param name="solicitudId"></param>
    /// <returns></returns>
    /// <exception cref="KeyNotFoundException"></exception>
    /// <exception cref="ArgumentException"></exception>
    [HttpPost("solicitudes/{solicitudId:int}/rechazar")]
    public async Task<IActionResult> RechazarSolicitud(int solicitudId)
    {
        int usuarioId = User.ObtenerUsuarioId();
        SolicitudGrupo solicitud = await _solicitudRepository.ObtenerPorId(solicitudId)
            ?? throw new KeyNotFoundException("Solicitud no encontrada");

        if (solicitud.InvitadoId != usuarioId)
            return Forbid();

        if (solicitud.Estado != "pendiente")
            throw new ArgumentException("Esta solicitud ya ha siddo procesada");

        await _solicitudRepository.ActualizarEstado(solicitudId, "rechazada");
        return Ok(new { mensaje = "Solicitud rechazada" });
    }
}
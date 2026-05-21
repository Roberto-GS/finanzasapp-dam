using FinanzasApp.API.Extensions;
using FinanzasApp.API.Services;
using FinanzasApp.Core.Interfaces;
using FinanzasApp.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinanzasApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class NotificacionesController : ControllerBase
{
    private readonly INotificacionService _notificacionService;
    private readonly INotificacionRepository _notificacionRepository;

    public NotificacionesController(
        INotificacionService notificacionService,
        INotificacionRepository notificacionRepository)
    {
        _notificacionService = notificacionService;
        _notificacionRepository = notificacionRepository;
    }

    /// <summary>
    /// Obtenemos todas las notificaciones del usuario: persistentes no leídas y alertas dinámicas generadas en tiempo real.
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<IActionResult> ObtenerNotificaciones()
    {
        int usuarioId = User.ObtenerUsuarioId();
        IEnumerable<object> resultado = await _notificacionService.ObtenerNotificaciones(usuarioId);
        return Ok(resultado);
    }

    /// <summary>
    /// Marca una notificación persistente como leída
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> MarcarLeida(int id)
    {
        int usuarioId = User.ObtenerUsuarioId();
        IEnumerable<Notificacion> notificaciones = await _notificacionRepository
            .ObtenerPorUsuarioId(usuarioId);

        Notificacion? notificacion = notificaciones.FirstOrDefault(n => n.Id == id);
        if (notificacion is null) return NotFound();

        await _notificacionRepository.MarcarLeida(id);
        return NoContent();
    }

    /// <summary>
    /// Marca todas las notificaciones persistentes como leídas
    /// </summary>
    /// <returns></returns>
    [HttpDelete]
    public async Task<IActionResult> MarcarTodasLeidas()
    {
        int usuarioId = User.ObtenerUsuarioId();
        await _notificacionRepository.MarcarTodasLeidas(usuarioId);
        return NoContent();
    }
}
using FinanzasApp.API.Extensions;
using FinanzasApp.Core.DTOs.Usuario;
using FinanzasApp.Core.Interfaces;
using FinanzasApp.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinanzasApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsuariosController : ControllerBase
{
    private readonly IUsuarioRepository _usuarioRepository;

    public UsuariosController(IUsuarioRepository usuarioRepository)
    {
        _usuarioRepository = usuarioRepository;
    }

    [HttpPut("cambiar-password")]
    public async Task<IActionResult> CambiarContrasena([FromBody] CambiarPasswordDto dto)
    {
        int usuarioId = User.ObtenerUsuarioId();

        Usuario usuario = await _usuarioRepository.ObtenerPorId(usuarioId)
            ?? throw new KeyNotFoundException("Usuario no encontrado");

        bool passwordValida = BCrypt.Net.BCrypt.Verify(dto.PasswordActual, usuario.PasswordHash);
        if (!passwordValida)
            throw new UnauthorizedAccessException("La contraseña no es correcta");

        usuario.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.PasswordNueva);
        await _usuarioRepository.Actualizar(usuario);

        return Ok(new { mensaje = "Contraseña actualizada correctamente" });
    }
}
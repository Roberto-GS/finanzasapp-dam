using FinanzasApp.Core.DTOs.Autentificacion;
using FinanzasApp.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace FinanzasApp.API.Controllers;

/// <summary>
/// Se encarga de manejar las solicitudes relacionadas con la autenticación, como iniciar sesión y registrarse.
/// Utiliza el servicio de autenticación para procesar las solicitudes y devolver las respuestas correspondientes
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>
    /// Iniciamos sesión con el email y la contraseña, si son correctos, el servidor nos devuelve un token de autenticación y la información del usuario
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    [HttpPost("login")]
    public async Task<IActionResult> IniciarSesion([FromBody] LoginDto dto)
    {
        AuthResponseDto resultado = await _authService.IniciarSesion(dto);
        return Ok(resultado);
    }

    /// <summary>
    /// Registramos un nuevo usuario, si el registro es exitoso, el servidor nos devuelve un token de autenticación y la información del usuario
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    [HttpPost("registro")]
    public async Task<IActionResult> Registro([FromBody] RegistroDto dto)
    {
        AuthResponseDto resultado = await _authService.Registrar(dto);
        return CreatedAtAction(nameof(IniciarSesion), resultado);
    }
}
using FinanzasApp.Core.DTOs.Autentificacion;

namespace FinanzasApp.API.Services;

public interface IAuthService
{
    /// <summary>
    /// Verifica las credenciales del usuario, y si son correctas, genera un token de autenticación y devuelve la información del usuario junto con el token.
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    Task<AuthResponseDto> IniciarSesion(LoginDto dto);
    /// <summary>
    /// Registramos un nuevo usuario, verificando que no exista una cuenta con el mismo email, y si el registro es exitoso, genera un token de autenticación y devuelve la información del usuario junto con el token.
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    Task<AuthResponseDto> Registrar(RegistroDto dto);
}
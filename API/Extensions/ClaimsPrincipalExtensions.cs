using System.Security.Claims;

namespace FinanzasApp.API.Extensions;

/// <summary>
/// Clase de extensión para ClaimsPrincipal que proporciona métodos para obtener información del usuario autenticado a partir de los claims presentes en el token JWT. En este caso, se incluye un método para obtener el ID del usuario a partir del claim "NameIdentifier" o "sub". Si el claim no se encuentra o no es válido, se lanza una excepción de acceso no autorizado.
/// </summary>
public static class ClaimsPrincipalExtensions
{
    /// <summary>
    /// Obtiene el ID del usuario autenticado a partir de los claims presentes en el token JWT. Busca el claim "NameIdentifier" o "sub" y devuelve su valor como un entero. Si el claim no se encuentra o no es válido, lanza una excepción de acceso no autorizado.
    /// </summary>
    /// <param name="usuario"></param>
    /// <returns></returns>
    /// <exception cref="UnauthorizedAccessException"></exception>
    public static int ObtenerUsuarioId(this ClaimsPrincipal usuario)
    {
        Claim? claim = usuario.FindFirst(ClaimTypes.NameIdentifier)
                 ?? usuario.FindFirst("sub")
                 ?? throw new UnauthorizedAccessException("Token inválido");

        return int.TryParse(claim.Value, out var id)
            ? id
            : throw new UnauthorizedAccessException("Token inválido");
    }
}
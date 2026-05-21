using System.Net;
using System.Text.Json;

namespace FinanzasApp.API.Middleware;

/// <summary>
/// Middleware para manejar errores de forma global en la aplicación. Captura excepciones no controladas, las registra y devuelve una respuesta JSON con el código de estado HTTP y un mensaje de error adecuado.
/// </summary>
public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;

    public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error: {Message}", ex.Message);
            await ManejarExcepcion(context, ex);
        }
    }

    /// <summary>
    /// Transforma una excepción en una respuesta HTTP adecuada. Dependiendo del tipo de excepción, se asigna un código de estado y un mensaje específico para el cliente. Para excepciones no reconocidas, se devuelve un error genérico de servidor.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="ex"></param>
    /// <returns></returns>
    private static async Task ManejarExcepcion(HttpContext context, Exception ex)
    {
        context.Response.ContentType = "application/json";

        (HttpStatusCode codigoEstado, string mensaje) = ex switch
        {
            ArgumentException => (HttpStatusCode.BadRequest, ex.Message),
            UnauthorizedAccessException => (HttpStatusCode.Unauthorized, "No autorizado"),
            KeyNotFoundException => (HttpStatusCode.NotFound, ex.Message),
            _ => (HttpStatusCode.InternalServerError, "Ha ocurrido un error interno. Inténtalo más tarde")
        };

        context.Response.StatusCode = (int)codigoEstado;

        object respuesta = new
        {
            status = (int)codigoEstado,
            error = mensaje
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(respuesta));
    }
}
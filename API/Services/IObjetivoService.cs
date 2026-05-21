using FinanzasApp.Core.DTOs.Objetivo;

namespace FinanzasApp.API.Services;

public interface IObjetivoService
{
    /// <summary>
    /// Evaluamos el progreso de un objetivo específico para un usuario, calculando la cantidad actual, el porcentaje de progreso, el estado del objetivo (cumplido, en peligro, vencido) y otros detalles relevantes, y devolvemos esta información en un DTO que representa el objetivo con su progreso actual. Esta evaluación se realiza teniendo
    /// </summary>
    /// <param name="objetivoId"></param>
    /// <param name="usuarioId"></param>
    /// <returns></returns>
    Task<ObjetivoConProgresoDto> EvaluarObjetivo(int objetivoId, int usuarioId);
    /// <summary>
    /// Evaluamos el progreso de todos los objetivos de un usuario específico, calculando para cada objetivo la cantidad actual, el porcentaje de progreso, el estado del objetivo (cumplido, en peligro, vencido) y otros detalles relevantes, y devolvemos esta información en una lista de DTOs que representan cada objetivo con su progreso actual. Esta evaluación se realiza teniendo
    /// </summary>
    /// <param name="usuarioId"></param>
    /// <returns></returns>
    Task<IEnumerable<ObjetivoConProgresoDto>> EvaluarTodos(int usuarioId);
}
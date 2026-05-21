namespace FinanzasApp.UI.Services;

public interface INavigationService
{
    /// <summary>
    /// Navega a una ruta específica dentro de la aplicación
    /// </summary>
    /// <param name="ruta"></param>
    /// <returns></returns>
    Task Navegar(string ruta);
    /// <summary>
    /// Navega a una ruta pasando parámetros
    /// </summary>
    /// <param name="ruta"></param>
    /// <param name="parametros"></param>
    /// <returns></returns>
    Task Navegar(string ruta, IDictionary<string, object> parametros);
    /// <summary>
    /// Para volver a la página anterior
    /// </summary>
    /// <returns></returns>
    Task NavigarAtras();
    /// <summary>
    /// Para ir a pantalla de login
    /// </summary>
    /// <returns></returns>
    Task NavegarALogin();
    /// <summary>
    /// Navegamos al Shell principal. Se usa tras el inicio de sesión o registro existoso
    /// </summary>
    /// <returns></returns>
    Task NavegarAShell();
}
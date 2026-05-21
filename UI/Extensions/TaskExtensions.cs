namespace FinanzasApp.UI.Extensions;

/// <summary>
/// Extensiones para tareas asíncronas. Permite ejecutar tareas sin esperar a que terminen, pero capturando cualquier excepción que pueda ocurrir para evitar que el programa se bloquee.
/// </summary>
public static class TaskExtensions
{
    /// <summary>
    /// Ejecutamos la tarea sin esperar a que termine, pero capturamos cualquier excepción que pueda ocurrir para evitar que el programa se bloquee.
    /// </summary>
    /// <param name="task"></param>
    public static async void FireAndForget(this Task task)
    {
        try
        {
            await task;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error: {ex.Message}");
        }
    }
}
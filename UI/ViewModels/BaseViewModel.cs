using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace FinanzasApp.UI.ViewModels;

/// <summary>
/// ViewModel base del que heredan todos los ViewModels de la aplicación.
/// Proporciona el estado de carga, mensajes de error y el método
/// de ejecución segura con control de excepciones centralizado.
/// </summary>
public partial class BaseViewModel : ObservableObject
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(NoEstaCargando))]
    private bool _estaCargando;

    [ObservableProperty]
    private string _titulo = string.Empty;

    [ObservableProperty]
    private string _mensajeError = string.Empty;

    [ObservableProperty]
    private bool _hayError;

    /// <summary>
    /// Indica si el ViewModel no está en proceso de carga.
    /// Usado para habilitar o deshabilitar botones en la interfaz.
    /// </summary>
    public bool NoEstaCargando => !EstaCargando;

    /// <summary>
    /// Muestra un mensaje de error en la interfaz.
    /// </summary>
    protected void MostrarError(string mensaje)
    {
        MensajeError = mensaje;
        HayError = true;
    }

    /// <summary>
    /// Limpia el mensaje de error y oculta el panel de error.
    /// </summary>
    protected void LimpiarError()
    {
        MensajeError = string.Empty;
        HayError = false;
    }

    /// <summary>
    /// Ejecuta una acción asíncrona con control de estado de carga
    /// y captura de excepciones. Muestra el error en la interfaz si falla.
    /// </summary>
    /// <param name="accion">Acción asíncrona a ejecutar.</param>
    /// <param name="mensajeError">Prefijo del mensaje de error si falla.</param>
    protected async Task EjecutarSeguro(
        Func<Task> accion,
        string mensajeError = "Ha ocurrido un error")
    {
        try
        {
            LimpiarError();
            EstaCargando = true;
            await accion();
        }
        catch (Exception ex)
        {
            MostrarError($"{mensajeError}: {ex.Message}");
        }
        finally
        {
            EstaCargando = false;
        }
    }
}
using System.Globalization;

namespace FinanzasApp.UI.Converters;

/// <summary>
/// Resaltamos el botón activo en un selector de tipo, comparando el valor del tipo con el parámetro proporcionado. Si el botón coincide con el tipo activo, se devuelve un color blanco para resaltarlo; de lo contrario, se devuelve un color transparente para mantenerlo sin resaltar. Esto es útil para indicar visualmente al usuario cuál es la opción actualmente seleccionada en un conjunto de botones o opciones, mejorando la experiencia de usuario al proporcionar una indicación clara del estado activo en la interfaz de usuario.
/// </summary>
public class TipoSelectorColorConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is string tipo)
        {
            if (parameter is string boton)
            {
                // Resalta el botón activo
                return boton == tipo
                    ? Application.Current!.Resources["White"]
                    : Colors.Transparent;
            }
        }
        return Colors.Transparent;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotImplementedException();
}
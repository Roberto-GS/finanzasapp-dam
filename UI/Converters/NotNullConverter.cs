using System.Globalization;

namespace FinanzasApp.UI.Converters;

/// <summary>
/// Devolvemos un valor booleano que indica si el valor de entrada es nulo o no. Si el valor es nulo, se devuelve falso; si el valor no es nulo, se devuelve verdadero. Esto es útil para mostrar u ocultar elementos en la interfaz de usuario, o para habilitar o deshabilitar controles, dependiendo de si un valor específico está presente o no. Por ejemplo, podríamos usar este convertidor para mostrar un mensaje de error solo cuando un campo de entrada está vacío (nulo), o para habilitar un botón solo cuando se ha seleccionado un elemento (no nulo).
/// </summary>
public class NotNullConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value is not null;

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotImplementedException();
}
using System.Globalization;

namespace FinanzasApp.UI.Converters;

/// <summary>
/// Invertimos un valor booleano. Si el valor es verdadero, se devuelve falso; si es falso, se devuelve verdadero. Esto es útil para situaciones donde queremos mostrar o habilitar algo cuando una condición es falsa, o viceversa, en la interfaz de usuario. Por ejemplo, podríamos usar este convertidor para deshabilitar un botón cuando una condición es verdadera, o para mostrar un elemento solo cuando una condición es falsa.
/// </summary>
public class InvertedBoolConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value is bool b && !b;

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value is bool b && !b;
}
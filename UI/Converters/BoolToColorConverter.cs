using System.Globalization;

namespace FinanzasApp.UI.Converters;

/// <summary>
/// Convertimos un valor booleano a un color específico. Si el valor es verdadero, se devuelve el color definido en TrueColor; si es falso, se devuelve el color definido en FalseColor. Esto es útil para cambiar dinámicamente el color de elementos de la interfaz de usuario en función de condiciones booleanas.
/// </summary>
public class BoolToColorConverter : IValueConverter
{
    public Color TrueColor { get; set; } = Colors.White;
    public Color FalseColor { get; set; } = Colors.Transparent;

    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value is bool b && b ? TrueColor : FalseColor;

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotImplementedException();
}
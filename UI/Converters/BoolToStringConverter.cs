using System.Globalization;

namespace FinanzasApp.UI.Converters;

/// <summary>
/// Convertimos un valor booleano a una cadena de texto específica. Si el valor es verdadero, se devuelve la cadena definida en TrueValue; si es falso, se devuelve la cadena definida en FalseValue. Esto es útil para mostrar mensajes o etiquetas dinámicamente en función de condiciones booleanas en la interfaz de usuario.
/// </summary>
public class BoolToStringConverter : IValueConverter
{
    public string TrueValue { get; set; } = string.Empty;
    public string FalseValue { get; set; } = string.Empty;

    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value is bool b && b ? TrueValue : FalseValue;

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotImplementedException();
}
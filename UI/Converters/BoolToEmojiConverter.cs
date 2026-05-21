using System.Globalization;

namespace FinanzasApp.UI.Converters;

/// <summary>
/// Convertimos un valor booleano a un emoji de tendencia. Si el valor es verdadero, se devuelve el emoji "📈" (indicando crecimiento o positivo); si es falso, se devuelve el emoji "📉" (indicando caída o negativo). Si el valor no es un booleano, se devuelve el emoji "➖" (indicando neutralidad o no aplicable). Esto es útil para representar visualmente estados positivos o negativos en la interfaz de usuario de manera rápida y clara.
/// </summary>
public class BoolToEmojiConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool positivo)
            return positivo ? "📈" : "📉";

        return "➖";
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotImplementedException();
}
using System.Globalization;

namespace FinanzasApp.UI.Converters;

/// <summary>
/// Convertimos un valor de tipo string que representa el tipo de notificación a un color específico. Dependiendo del valor de la cadena, se devuelve un color definido en los recursos de la aplicación o un color específico. Por ejemplo, si el valor es "peligro" o "fallido", se devuelve el color asociado a "GastoLight"; si es "advertencia", se devuelve un color amarillo claro; si es "cumplido" o "exito", se devuelve el color asociado a "IngresoLight". Si el valor no coincide con ninguno de los casos definidos, se devuelve el color asociado a "CardBackground". Esto es útil para resaltar visualmente diferentes tipos de notificaciones en la interfaz de usuario, facilitando la identificación rápida del estado o la gravedad de una notificación específica.
/// </summary>
public class NotificacionColorConverter : IValueConverter
{
    public object Convert(object? value, Type targetType,
        object? parameter, CultureInfo culture)
    {
        if (value is not string tipo)
            return GetColor("CardBackground");

        return tipo switch
        {
            "peligro" => GetColor("GastoLight"),
            "fallido" => GetColor("GastoLight"),
            "advertencia" => Color.FromArgb("#FEF3C7"),
            "cumplido" => GetColor("IngresoLight"),
            "exito" => GetColor("IngresoLight"),
            _ => GetColor("CardBackground")
        };
    }

    public object ConvertBack(object? value, Type targetType,
        object? parameter, CultureInfo culture)
        => throw new NotImplementedException();

    private static Color GetColor(string key)
    {
        if (Application.Current?.Resources.TryGetValue(key, out object? color) == true
            && color is Color c)
            return c;

        return Colors.White;
    }
}
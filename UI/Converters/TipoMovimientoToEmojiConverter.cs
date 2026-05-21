using FinanzasApp.Core.Enums;
using System.Globalization;

namespace FinanzasApp.UI.Converters;

/// <summary>
/// Convertimos un valor de tipo TipoMovimiento a un emoji específico para representar visualmente el tipo de movimiento financiero. Si el valor es TipoMovimiento.Ingreso, se devuelve el emoji "💚" para indicar un ingreso positivo. Si el valor es TipoMovimiento.Gasto, se devuelve el emoji "🔴" para indicar un gasto negativo. Si el valor no coincide con ninguno de los casos definidos, se devuelve el emoji "⚪" como una representación neutral o desconocida. Esto es útil para proporcionar una representación visual rápida y fácil de entender del tipo de movimiento financiero en la interfaz de usuario, permitiendo a los usuarios identificar rápidamente si un movimiento es un ingreso o un gasto, o si no se ha especificado claramente el tipo de movimiento.
/// </summary>
public class TipoMovimientoToEmojiConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is TipoMovimiento tipo)
            return tipo == TipoMovimiento.Ingreso ? "🟢" : "🔴";

        return "⚪";
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotImplementedException();
}
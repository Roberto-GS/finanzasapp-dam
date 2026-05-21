using FinanzasApp.Core.Enums;
using System.Globalization;

namespace FinanzasApp.UI.Converters;

/// <summary>
/// Convertimos un valor de tipo TipoMovimiento a un color específico definido en los recursos de la aplicación para el texto. Dependiendo del valor del enum, se devuelve un color asociado a "Ingreso" para el caso de TipoMovimiento.Ingreso, o un color asociado a "Gasto" para el caso de TipoMovimiento.Gasto. Si el valor no coincide con ninguno de los casos definidos, se devuelve un color asociado a "Balance". Esto es útil para resaltar visualmente los diferentes tipos de movimientos financieros en la interfaz de usuario, facilitando la identificación rápida de si un movimiento es un ingreso o un gasto, o si no se ha especificado claramente el tipo de movimiento, especialmente en el contexto del texto que acompaña a estos movimientos financieros.
/// </summary>
public class TipoMovimientoToTextColorConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is TipoMovimiento tipo)
        {
            return tipo == TipoMovimiento.Ingreso
                ? Application.Current!.Resources["Ingreso"]
                : Application.Current!.Resources["Gasto"];
        }
        return Application.Current!.Resources["Balance"];
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotImplementedException();
}
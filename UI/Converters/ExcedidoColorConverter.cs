using System.Globalization;

namespace FinanzasApp.UI.Converters;

/// <summary>
/// Convertimos un valor booleano que indica si un gasto ha excedido el presupuesto a un color específico.
/// Si el valor es verdadero (excedido), se devuelve el color definido en los recursos de la aplicación 
/// bajo la clave "Gasto"; si es falso (no excedido), se devuelve el color definido bajo la clave "Ingreso".
/// Esto es útil para resaltar visualmente los gastos que han superado el presupuesto en la interfaz de usuario,
/// facilitando la identificación rápida de áreas problemáticas en las finanzas personales.
/// </summary>
public class ExcedidoColorConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool excedido && excedido)
            return Application.Current!.Resources["Gasto"];
        return Application.Current!.Resources["Ingreso"];
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotImplementedException();
}
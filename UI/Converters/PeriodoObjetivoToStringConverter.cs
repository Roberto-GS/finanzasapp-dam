using FinanzasApp.Core.Enums;
using System.Globalization;

namespace FinanzasApp.UI.Converters;

/// <summary>
/// Convertimos un valor de tipo PeriodoObjetivo a una cadena legible para mostrar en la interfaz de usuario. Dependiendo del valor del enum, se devuelve una cadena específica que representa el período objetivo de manera más amigable para el usuario. Por ejemplo, si el valor es PeriodoObjetivo.Diario, se devuelve "Diario"; si es PeriodoObjetivo.Semanal, se devuelve "Semanal"; y así sucesivamente para los demás valores del enum. Si el valor no coincide con ninguno de los casos definidos, se devuelve la representación por defecto del enum como cadena. Esto es útil para mostrar información clara y comprensible sobre el período objetivo en la interfaz de usuario, facilitando la comprensión rápida por parte del usuario final.
/// </summary>
public class PeriodoObjetivoToStringConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is PeriodoObjetivo periodo)
        {
            return periodo switch
            {
                PeriodoObjetivo.Diario => "Diario",
                PeriodoObjetivo.Semanal => "Semanal",
                PeriodoObjetivo.Mensual => "Mensual",
                PeriodoObjetivo.Anual => "Anual",
                _ => periodo.ToString()
            };
        }
        return string.Empty;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotImplementedException();
}
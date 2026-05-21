using System.Globalization;

namespace FinanzasApp.UI.Converters;

/// <summary>
/// Convierte la posición numérica de un miembro en el ranking
/// a un emoji de medalla para las 3 primeras posiciones
/// o al número ordinal para el resto.
/// </summary>
public class PosicionRankingConverter : IValueConverter
{
    public object Convert(object? value, Type targetType,
        object? parameter, CultureInfo culture)
    {
        if (value is not int posicion)
            return string.Empty;

        return posicion switch
        {
            1 => "🥇",
            2 => "🥈",
            3 => "🥉",
            _ => $"{posicion}º"
        };
    }

    public object ConvertBack(object? value, Type targetType,
        object? parameter, CultureInfo culture)
        => throw new NotImplementedException();
}
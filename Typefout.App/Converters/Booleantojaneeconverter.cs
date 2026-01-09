using System.Globalization;

namespace Typefout.App.Converters
{
    public class BooleanToJaNeeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                return boolValue ? "Ja" : "Nee";
            }
            return "Nee";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string stringValue)
            {
                return stringValue.Equals("Ja", StringComparison.OrdinalIgnoreCase);
            }
            return false;
        }
    }
}
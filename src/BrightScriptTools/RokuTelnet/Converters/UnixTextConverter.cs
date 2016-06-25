using System;
using System.Globalization;
using System.Windows.Data;

namespace RokuTelnet.Converters
{
    public class UnixTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && typeof(string) == value.GetType() && value.ToString().StartsWith("\u001b[3"))
            {
                value = value.ToString().Substring(7);
                value = value.ToString().Replace("[0m", "");
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace RokuTelnet.Converters
{
    public class UnixColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && typeof(string) == value.GetType() && value.ToString().StartsWith("\u001b[3"))
            {
                var c = int.Parse(value.ToString().Substring(3, 1));

                switch (c)
                {
                    case 1:
                        return Brushes.Red;
                    case 2:
                        return Brushes.LightGreen;
                    case 3:
                        return Brushes.Yellow;
                    case 4:
                        return Brushes.DeepSkyBlue;
                    case 5:
                        return Brushes.Magenta;
                    case 6:
                        return Brushes.Cyan;
                    case 7:
                        return Brushes.LightCyan;
                }
            }

            return Brushes.WhiteSmoke;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
using System;
using System.Windows.Data;

namespace WereDev.Utils.Wu10Man.Converters
{
    public class BooleanToEnabledDisabledConverter : IValueConverter
    {
        public BooleanToEnabledDisabledConverter() { }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool bValue = (bool)value;
            return bValue ? "Enabled" : "Disabled";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value.ToString().Equals("Enabled", StringComparison.CurrentCultureIgnoreCase);
        }

    }
}

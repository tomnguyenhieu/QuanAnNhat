using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace QuanAnNhat.Converter
{
    class FormPlaceholderConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string placeholder = value.ToString();

            switch (placeholder)
            {
                case "LoginEmail":
                    placeholder = placeholder.Substring(5);
                    break;
                case "RegisterEmail":
                    placeholder = placeholder.Substring(8);
                    break;
            }

            return $"{placeholder.ToLower()}...";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

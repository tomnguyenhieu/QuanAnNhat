using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using SystemConvert = System.Convert;

namespace QuanAnNhat.Converter
{
    class FormDescConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool IsChecked = SystemConvert.ToBoolean(value);
            if (IsChecked)
            {
                return "Already have an account? Log in now!";
            } else
            {
                return "Don’t have an account? Resgister now!";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace QuanAnNhat.Converter
{
    class ThumbnailConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string appPath = AppDomain.CurrentDomain.BaseDirectory;
            appPath = appPath.Substring(0, 29).Replace("\\", "/");

            if (value == null)
            {
                return new BitmapImage(new Uri($"{appPath}/Assets/Images/dish1.jpg"));
            }
            string thumbnailPath = $"{appPath}/Assets/Images/Dishes/{(int)value}.jpg";

            return new BitmapImage(new Uri(thumbnailPath, UriKind.Absolute));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

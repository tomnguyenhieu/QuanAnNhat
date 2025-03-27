using QuanAnNhat.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace QuanAnNhat.Converter
{
    class AvatarConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            User user = (User)value;

            string appPath = AppDomain.CurrentDomain.BaseDirectory;
            appPath = appPath.Substring(0, 29).Replace("\\", "/");
            string filePath = $"{appPath}/Assets/Images/Avatar/{user.Email}/{user.Avatar}";
            if (user.Avatar == null || string.IsNullOrWhiteSpace(user.Avatar))
            {
                return new BitmapImage(new Uri($"{appPath}/Assets/Images/default-avatar.png"));
            }
            else
            {
                if (!File.Exists(filePath))
                {
                    return new BitmapImage(new Uri($"{appPath}/Assets/Images/default-avatar.png"));
                } 
                else
                {
                    Uri uri = new Uri(filePath, UriKind.Absolute);
                    return new BitmapImage(uri);
                }
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

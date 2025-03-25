using QuanAnNhat.Models;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using SystemConvert = System.Convert;

namespace QuanAnNhat.Converter
{
    class WishlistIconConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            FontFamily fontRegular = new FontFamily("Font Awesome 6 Free Regular");
            FontFamily fontSolid = new FontFamily("Font Awesome 6 Free Solid");

            int dishId = SystemConvert.ToInt32(values[0]);
            int userId;
            if (values[1] is int id)
            {
                userId = id;
            }
            else
            {
                return fontRegular;
            }

            foreach (var item in (List<Wishlist>)values[2])
            {
                if (item.DishId == dishId && item.UserId == userId)
                {
                    return fontSolid;
                }
            }

            return fontRegular;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

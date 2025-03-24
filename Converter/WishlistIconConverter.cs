using QuanAnNhat.Models;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace QuanAnNhat.Converter
{
    class WishlistIconConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            FontFamily fontRegular = new FontFamily("Font Awesome 6 Free Regular");
            FontFamily fontSolid = new FontFamily("Font Awesome 6 Free Solid");

            foreach (var item in (List<Wishlist>)values[2])
            {
                if (item.DishId == (int)values[0] && item.UserId == (int)values[1])
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

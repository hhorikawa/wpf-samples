using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data; // IValueConverter

namespace wpf_datagrid
{

[ValueConversion(typeof(Enum), typeof(string))] // sourceType, targetType
public class EnumStringConverter : IValueConverter
{
    // enum を文字列に変換する
    // C# enum 型は名前が取れる。すごいな!!
    public object Convert(object value, Type targetType, object parameter,
                          CultureInfo culture)
    {
        if (value != null && value.GetType().IsEnum)
            return value.ToString();

        return null;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}


[ValueConversion(typeof(string), typeof(Uri))] // sourceType, targetType
public class EmailConverter : IValueConverter
{
    // The source (viewmodel) to the target (WPF element)
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        Uri email = new Uri("mailto:" + (string) value);
        return email;
    }

    // The target (WPF element) to the source (viewmodel)
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}


}

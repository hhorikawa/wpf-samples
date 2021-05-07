using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls; // ValidationRule

namespace wpf_datagrid
{

// XAML 要素の検査器は, System.Windows.Controls 名前空間の ValidationRule から
// 派生させる.
public class ValidatesPresence: ValidationRule
{
    public override ValidationResult Validate(object value, 
                                System.Globalization.CultureInfo cultureInfo)
    {
        // 特定フィールドの場合, value は検査対象の値
        // DependencyProperty.Register() で int にしていても, string が来る.
        string b = value as string;
        if ( String.IsNullOrWhiteSpace(b) )
            return new ValidationResult(false, "値が必須");

        return ValidationResult.ValidResult;
    }
}

}

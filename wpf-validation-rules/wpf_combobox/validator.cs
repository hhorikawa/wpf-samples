
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace wpf_combobox
{

// フォームデータ検証は ValidationRule クラスから派生させる
internal class MyViewValidator: ValidationRule
{
    public override ValidationResult Validate(object value,
                                System.Globalization.CultureInfo cultureInfo)
    {
        // 特定のフィールド値ではなく, BindingGroup が入ってくる
        BindingGroup bg = value as BindingGroup;
        if (bg == null) {
            return new ValidationResult(false, 
                                "internal error: BindingGroup object is invalid.");
        }
        if (bg.Items.Count != 1) {
            return new ValidationResult(false, "internal error: BindingGroup.Items invalid.");
        }
        
        // Get the source object.
        MyViewModel data = bg.Items[0] as MyViewModel;
        if (data == null) {
            return new ValidationResult(false, "internal error: Data is not a MyViewModel.");
        }
        object combo1, combo2;
        // POINT!: Commit される前のデータを得る.
        bool r1 = bg.TryGetValue(data, "Combo1", out combo1);
        bool r2 = bg.TryGetValue(data, "Combo2", out combo2);
        if (!r1 || !r2) {
            return new ValidationResult(false, "internal error: Properties not found");
        }
        
        if ( Convert.ToInt32(combo1) == Convert.ToInt32(combo2) ) { 
            MessageBox.Show("Do not be same");
            return new ValidationResult(false, "Must not be same");
        }

        // OK.
        return ValidationResult.ValidResult;
    }
} // class MyValidator


#if !USE_INPC
// 特定のフィールドの検証.
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


public class ValidatesIntInclusion: ValidationRule
{
    public int Min { get; set; }
    public int Max { get; set; }

    public ValidatesIntInclusion()
    {
        Min = Int32.MinValue;
        Max = Int32.MaxValue;
    }
    
    public override ValidationResult Validate(object value, 
                                System.Globalization.CultureInfo cultureInfo)
    {
        string b = value as string;
        if (b == null)
            return new ValidationResult(false, "internal error: Binding is invalid.");

        int numValue;
        if (!Int32.TryParse(b, out numValue))
            return new ValidationResult(false, "数値を入力してください");
        if (numValue < Min || numValue > Max)
            return new ValidationResult(false, "値の範囲外");

        return ValidationResult.ValidResult;
    }
}
#endif // !USE_INPC

}

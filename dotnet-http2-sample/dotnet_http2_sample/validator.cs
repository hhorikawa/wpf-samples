
using System;
using System.ComponentModel;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace dotnet_http2_sample
{
// 文字列があること [汎用ルーチン]
public class ValidatesPresence : ValidationRule
{
    public override ValidationResult Validate(object value, 
                            System.Globalization.CultureInfo cultureInfo)
    {
        string b = value as string;
        if ( String.IsNullOrWhiteSpace(b) )
            return new ValidationResult(false, "値が必須");

        return ValidationResult.ValidResult;
    }
}


// URI の妥当性を確認する [汎用ルーチン]
public class ValidatesURI : ValidationRule
{
    public override ValidationResult Validate(object value, 
                            System.Globalization.CultureInfo cultureInfo)
    {
        string s = value as string;
        Uri uri;
        if (!s.Contains("://")) 
            s = "http://" + s;
        if (!Uri.TryCreate(s, UriKind.Absolute, out uri)) 
            return new ValidationResult(false, "不正なURI");
        if ( uri.DnsSafeHost == "" )
            return new ValidationResult(false, "URI: ホスト名が必要");
            
        // DNS調べるのはやりすぎだった.  
        // if (Dns.GetHostAddresses(uri.DnsSafeHost).Length > 0)
        return ValidationResult.ValidResult;
    }
} // class ValidatesURI


// WPF ValidationRule の状態を view model に通知 [汎用ルーチン]
// See https://stackoverflow.com/questions/10596452/passing-state-of-wpf-validationrule-to-view-model-in-mvvm
// リフレクションを使っているので、メソッドの名前が重要. 気づかん
// See https://docs.microsoft.com/en-us/dotnet/desktop/wpf/advanced/attached-properties-overview
public static class ValidationBehavior
{
    #region Attached Properties

    public static readonly DependencyProperty HasErrorsProperty = 
            DependencyProperty.RegisterAttached(
                "HasErrors",
                typeof(bool),
                typeof(ValidationBehavior),
                new FrameworkPropertyMetadata(false, 
                        FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, 
                        null, // PropertyChangedCallback を null がポイント
                        CoerceHasError));

    private static readonly DependencyProperty HasErrorsDescriptorProperty = 
            DependencyProperty.RegisterAttached(
                "HasErrorsDescriptor",
                typeof(DependencyPropertyDescriptor),
                typeof(ValidationBehavior));

    #endregion

    private static DependencyPropertyDescriptor GetHasErrorDescriptor(DependencyObject d)
    {
        return (DependencyPropertyDescriptor)d.GetValue(HasErrorsDescriptorProperty);
    }

    private static void SetHasErrorDescriptor(DependencyObject d, DependencyPropertyDescriptor value)
    {
        d.SetValue(HasErrorsDescriptorProperty, value);
    }

    #region Attached Property Getters and setters

    // リフレクションを使っているので、この名前でなければならない. 分からん!
    public static bool GetHasErrors(DependencyObject d)
    {
        return (bool)d.GetValue(HasErrorsProperty);
    }

    public static void SetHasErrors(DependencyObject d, bool value)
    {
        d.SetValue(HasErrorsProperty, value);
    }

    #endregion

    #region CallBacks

    // Validation.HasError と紐付ける
    private static object CoerceHasError(DependencyObject d, object baseValue)
    {
        var result = (bool)baseValue;
        if (BindingOperations.IsDataBound(d, HasErrorsProperty)) {
            if (GetHasErrorDescriptor(d) == null) {
                var desc = DependencyPropertyDescriptor.FromProperty(Validation.HasErrorProperty, d.GetType());
                desc.AddValueChanged(d, OnHasErrorChanged);
                SetHasErrorDescriptor(d, desc);
                result = System.Windows.Controls.Validation.GetHasError(d);
            }
        }
        else {
            if (GetHasErrorDescriptor(d) != null) {
                var desc = GetHasErrorDescriptor(d);
                desc.RemoveValueChanged(d, OnHasErrorChanged);
                SetHasErrorDescriptor(d, null);
            }
        }
        return result;
    }

    private static void OnHasErrorChanged(object sender, EventArgs e)
    {
        var d = sender as DependencyObject;
        if (d != null) {
            d.SetValue(HasErrorsProperty, d.GetValue(Validation.HasErrorProperty));
        }
    }

    #endregion
}


}

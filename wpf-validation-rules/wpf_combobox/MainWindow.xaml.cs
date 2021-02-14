
#if USE_INPC
using System;
using System.ComponentModel; // INotifyPropertyChanged, IDataErrorInfo
using System.Collections; // IEnumerable
#endif
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace wpf_combobox
{

/**
 * Viewmodel は DependencyObject class から派生させるか, INotifyPropertyChanged
 * インタフェイスを実装しなければならない [MUST]。
 * => 公式ドキュメントでは, 原則は DependencyObject 派生, 他のクラスから派生さ
 *    せたい場合は INotifyPropertyChanged 実装.
 * DependencyObject class は INotifyPropertyChanged を実装していないので,
 * 共通のインタフェイスがなく, DataContext プロパティは object 型.
 * See https://docs.microsoft.com/en-us/dotnet/desktop/wpf/advanced/optimizing-performance-data-binding?view=netframeworkdesktop-4.8
 */


#if USE_INPC
/** 
 * INotifyPropertyChanged を実装する場合,
 * 1. いちいち PropertyChanged() を呼び出すのが面倒。そのため、基底クラス 
 *    BindableBase を作ったりするが、でもそうすると, 原則に従って DependencyObject
 *    を使え、となる。
 *   BindableBase の例:
 *     https://github.com/PrismLibrary/Prism/blob/5ddd97b5c0a9169a081401a766d5448c6b29b4a9/src/Prism.Core/Mvvm/BindableBase.cs
 * 
 * 2. エラーチェックは INotifyDataErrorInfo interface を実装 [MAY].
 *    IDataErrorInfo interface は, 古い形式で, 廃れる.
 */
public class MyViewModel : INotifyPropertyChanged, INotifyDataErrorInfo
{
    // 1. View がコールバック関数を登録.
    // 2. Viewmodel の中からこの PropertyChanged() を呼び出すことで, view に変
    //    更を通知する.
    public event PropertyChangedEventHandler PropertyChanged;

    // INotifyDataErrorInfo 
    public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

    // Combobox に表示する辞書は, 静的でもよい. Binding を x:Static で指定.
    public static readonly Dictionary<int, string> 
    s_comboDic1 = new Dictionary<int, string>() {
        {10, "apple"},
        {20, "banana"},
    };
    public static readonly Dictionary<int, string> 
    s_comboDic2 = new Dictionary<int, string>() {
        {20, "banana"},
        {30, "grape"},
    };

    private bool _b;
    public bool ButtonEnabled { 
        get { return _b; }
        set {
            _b = value;
            // View への通知を手書きしなければならない.
            PropertyChanged(this, new PropertyChangedEventArgs("ButtonEnabled"));
        }
    }

    // 選択中の値
    private int _c1;
    public int Combo1 { 
        get { return _c1; }
        set {
            _c1 = value;
            PropertyChanged(this, new PropertyChangedEventArgs("Combo1"));
            ButtonEnabled = _c1 == 20;
        }
    }

    private int _c2;
    public int Combo2 { 
        get { return _c2; }
        set {
            _c2 = value;
            PropertyChanged(this, new PropertyChangedEventArgs("Combo2"));
        }
    }

    private string _numberBox;
    public string NumberBox { // 変換前の型
        get { return _numberBox; }
        set {
            int n;
            if ( String.IsNullOrWhiteSpace(value) ) {
                _errors["NumberBox"] = new []{ "値を入力してください" };
                ErrorsChanged(this, new DataErrorsChangedEventArgs("NumberBox"));
            }
            else if ( !Int32.TryParse(value, out n) ) {
                _errors["NumberBox"] = new []{ "数値を入力してください" };
                ErrorsChanged(this, new DataErrorsChangedEventArgs("NumberBox"));
            }
            else { 
                // TODO: 値の範囲チェック
                _errors.Remove("NumberBox");
            }
            _numberBox = value;
            PropertyChanged(this, new PropertyChangedEventArgs("NumberBox"));
        }
    }

    // INotifyDataErrorInfo 
    private Dictionary<string, string[]> _errors = new Dictionary<string, string[]>();
    public bool HasErrors {
        get {
            return _errors.Count > 0;
        }
    }

    // INotifyDataErrorInfo 
    public IEnumerable GetErrors(string propertyName)
    {
        if (!_errors.ContainsKey(propertyName))
            return null;
        return _errors[propertyName];
    }
}

#else
// [推奨] DependencyObject から派生させる.
public class MyViewModel : DependencyObject 
{
    // Combobox に表示する辞書は, 静的でもよい. Binding を x:Static で指定.
    public static readonly Dictionary<int, string> 
    s_comboDic1 = new Dictionary<int, string>() {
        {10, "apple"},
        {20, "banana"},
    };
    public static readonly Dictionary<int, string> 
    s_comboDic2 = new Dictionary<int, string>() {
        {20, "banana"},
        {30, "grape"},
    };

    // 依存プロパティを使う場合, view への通知は自動.
    public static readonly DependencyProperty ButtonEnabledProperty =
            DependencyProperty.Register(
                "ButtonEnabled",      // name
                typeof(bool),         // propertyType
                typeof(MyViewModel)); // ownerType

    // 値の変更で追加処理したいときは, PropertyChangedCallback() を使う.
    public static readonly DependencyProperty Combo1Property =
            DependencyProperty.Register(
                "Combo1", typeof(int), typeof(MyViewModel),
                new PropertyMetadata(new PropertyChangedCallback(onCombo1Changed)));

    static void onCombo1Changed(DependencyObject d, 
                                DependencyPropertyChangedEventArgs e)
    { 
        d.SetValue(MyViewModel.ButtonEnabledProperty, 
                   (int) d.GetValue(MyViewModel.Combo1Property) == 20);
    }

    public static readonly DependencyProperty Combo2Property =
            DependencyProperty.Register("Combo2", typeof(int), typeof(MyViewModel));

    // 自動型変換 string -> int
    public static readonly DependencyProperty NumberBoxProperty =
            DependencyProperty.Register("NumberBox", typeof(int), typeof(MyViewModel));
} // class MyViewModel
#endif


    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        // FrameworkElement.DataContext Property
        // => XAML 内で紐づけろ.
        // DataContext = new MyViewModel();

        // 初期設定
        MyViewModel vm = (MyViewModel) DataContext;
#if USE_INPC
        vm.Combo1 = 10;
#else
        vm.SetValue(MyViewModel.Combo1Property, 10);
#endif
    }

    private void DoneButton_Click(object sender, RoutedEventArgs e)
    {
        MyViewModel vm = (MyViewModel) DataContext;
        gridView.BindingGroup.CommitEdit();
        if (Validation.GetHasError(gridView)) {
            // [OK] ボタンのときのみ検証するときは, BindingGroup#CancelEdit() +  
            // BindingGroup#BeginEdit() でよい. リアルタイムで更新しているときは,
            // すでに viewmodel の値が変わっているので, 手で戻さないといけない.
        }
#if USE_INPC
        // 選ばれていないときは, 値 = 0
        MessageBox.Show("c1 = " + vm.Combo1 +
                        "; c2 = " + vm.Combo2);
#else
        MessageBox.Show("c1 = " + vm.GetValue(MyViewModel.Combo1Property) +
                        "; c2 = " + vm.GetValue(MyViewModel.Combo2Property));
#endif
    }
} // class MainWindow

}

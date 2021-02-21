using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace dotnet_http2_sample
{

/**
 * MVVM 的に view model でハンドリングし、かつ、ApplicationCommands も使えるコ
 * マンドハンドリングの方法.
 * 世の中の解説では, XAML 側に次のように書き, view にハンドラを実装せざるを得ない
 *   <Window.CommandBindings>
 *     <CommandBinding Command="ApplicationCommands.New" 
 *                     Executed="NewCommand_Executed" CanExecute="NewCommand_CanExecute" />
 *   </Window.CommandBindings>
 * もっと酷いの (ほとんど全部の解説サイト) だと, DelegateCommand で (実装, 実
 * 行可能か) メソッドの組を view model のプロパティに設定し、それで MVVM でご
 * ざい、としている。ApplicationCommands はどうした?
 * 
 * 参考:
 *   #1,081 – Adding CommandBinding to Top-Level CommandBindings
 *   https://wpf.2000things.com/2014/05/28/1081-adding-commandbinding-to-top-level-commandbindings/
 */ 
public class MyViewModel: DependencyObject
{
    public static readonly DependencyProperty UrlBoxProp =
            DependencyProperty.Register("UrlBox", typeof(string), 
                    typeof(MyViewModel));

    public static readonly DependencyProperty ResponseHeaderProp =
            DependencyProperty.Register("ResponseHeader", typeof(string), 
                    typeof(MyViewModel));
    public static readonly DependencyProperty ResponseBodyProp =
            DependencyProperty.Register("ResponseBody", typeof(string), 
                    typeof(MyViewModel));

    //////////////////////////////////////////
    // コンストラクタ 

    public MyViewModel()
    {
        CommandBindings = new List<CommandBinding>();

        // このウィンドウにおける, コマンドと実行する関数とを紐付ける.
        // "コマンド" として標準コマンドも使える。
        CommandBindings.Add( 
                new CommandBinding(ApplicationCommands.Close, FileExit));
        CommandBindings.Add(
                new CommandBinding(MyCommands.FetchCommand, Fetch, CanFetch));
    }


    //////////////////////////////////////////
    // Public Properties

    // これを view 側から参照する.
    // ただ、どんくさい. 
    // TODO: 方法の改善? グローバルに一つだけ持つか.
    //       System.Windows.Input.CommandManager class か?
    public List<CommandBinding> CommandBindings { get; protected set; }

    // Behaviour で書き込む.
    public bool HasErrors {
        get; set;
    }


    void FileExit( object sender, ExecutedRoutedEventArgs e )
    {
        throw new NotImplementedException();
    }

    async void Fetch( object sender, ExecutedRoutedEventArgs e )
    {
        var http = new WinHttpHandler();
        using (var client = new HttpClient(http)) {
            var request = new HttpRequestMessage(HttpMethod.Get,
                                    (string) GetValue(UrlBoxProp));
            request.Version = new Version("2.0");
            var response = await client.SendAsync(request);

            SetValue(ResponseHeaderProp, 
                     response.Version + "\n" + response.Headers.ToString());
            SetValue(ResponseBodyProp, 
                     await response.Content.ReadAsStringAsync());
        }
    }

    void CanFetch( object sender, CanExecuteRoutedEventArgs e )
    {
        e.CanExecute = !HasErrors ;
    }
} // class MyViewModel


    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        MyViewModel vm = (MyViewModel) DataContext;
        foreach (var binding in vm.CommandBindings) 
            CommandBindings.Add(binding);
    }

    private void urlBox_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Return) {
            // TODO:
        }
    }
} // class MainWindow

}

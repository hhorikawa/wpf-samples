using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Windows;
using System.Windows.Input;

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
        MyCommands.CommandBindings.Add(
                new CommandBinding(MyCommands.FetchCommand, Fetch, CanFetch));
    }

    //////////////////////////////////////////
    // Public Properties

    // Behaviour で書き込む.
    public bool HasErrors {
        get; set;
    }

    //////////////////////////////////////////
    // Methods

    // System.Net.Http.HttpClient は, ソケットを使い回すため, static にしなけれ
    // ばならない.
    // DNSの変更が云々というのは当たらないようだ
    // See https://www.makcraft.com/blog/meditation/2020/03/31/httpclient-%E3%81%AE%E6%8C%AF%E3%82%8B%E8%88%9E%E3%81%84%E3%81%AE%E8%AA%BF%E6%9F%BB/
    static readonly HttpClient http_client =
            new HttpClient(new WinHttpHandler() {
                               AutomaticRedirection = false });

    async void Fetch( object sender, ExecutedRoutedEventArgs e )
    {
        // Call asynchronous network methods in a try/catch block to handle
        // exceptions.
        try {
            var request = new HttpRequestMessage(HttpMethod.Get,
                                    (string) GetValue(UrlBoxProp));
            request.Version = new Version("2.0");
            var response = await http_client.SendAsync(request);

            SetValue(ResponseHeaderProp,
                     response.Version + "\n" + response.Headers.ToString());
            SetValue(ResponseBodyProp,
                     await response.Content.ReadAsStringAsync());
        }
        catch (HttpRequestException ex) {
            Console.WriteLine("\nException Caught!");
            Console.WriteLine("Message :{0} ", ex.Message);
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

        // CommandBindings は readonly のため、単に代入は不可
        foreach (var binding in MyCommands.CommandBindings)
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

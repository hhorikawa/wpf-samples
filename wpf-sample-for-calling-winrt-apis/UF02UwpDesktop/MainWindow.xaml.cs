using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

/*
  Calling Windows 10 APIs From a Desktop Application
  https://blogs.windows.com/windowsdeveloper/2017/01/25/calling-windows-10-apis-desktop-application/
    この記事は2017年のもので、やや古い。
    現代は, Microsoft.Windows.SDK.Contracts nuget パッケージを使う。

  Windows.Storage 名前空間や, Windows.Media 名前空間を使うには, package identity が必要。
  See Windows Runtime APIs available to desktop apps
      https://docs.microsoft.com/en-us/windows/apps/desktop/modernize/desktop-to-uwp-supported-api
 */

namespace UF02UwpDesktop
{
  /// <summary>
  /// MainWindow.xaml の相互作用ロジック
  /// </summary>
public partial class MainWindow : Window
{
    private static readonly Version _osVersion  
      = (new Func<Version>(() => {
        using (var mc = new System.Management.ManagementClass("Win32_OperatingSystem"))
        using (var moc = mc.GetInstances())
          foreach (System.Management.ManagementObject mo in moc)
          {
            var v = mo["Version"] as string;
            if (!string.IsNullOrWhiteSpace(v))
              return new Version(v);
          }

        return new Version("0.0.0.0");
      }))();


    public MainWindow()
    {
        if (_osVersion.Major < 10) { 
            MessageBox.Show("このアプリは、Windows 10 でなければ正常に動作しません。",
          "動作不可", MessageBoxButton.OK, MessageBoxImage.Error);
          }
        else if (_osVersion.Build < 14393) { 
            MessageBox.Show("このアプリは、Windows 10 build 14393 未満では一部の機能が動作しません。",
          "動作不完全", MessageBoxButton.OK, MessageBoxImage.Exclamation);
      // ※注：↑簡易的にここに書いたけれど、本来はAppクラスにMainメソッドを追加して、
      //         Appクラスをインスタンス化する前に行うべきです
      //         http://www.atmarkit.co.jp/ait/articles/1511/04/news027.html
        }
        InitializeComponent();

      //// お試し：
      //// My Picture フォルダーのファイル一覧を取得してみる
      //Task.Run(async () =>
      //{
      //  var files = await Windows.Storage.KnownFolders.PicturesLibrary.GetFilesAsync();
      //  var myPictures = string.Join("\n", files.Select(f => f.Name));
      //  MessageBox.Show(myPictures);
      //});
    }


    void Window_Loaded(object sender, RoutedEventArgs e)
    {
        ClearRubyText();

#if DEBUG
        InputText.Text = "日本語の漢字・カナ交じりの文字列です。山河と山川、須弥山登山計画";
#endif

#if DEBUG
        // 以下、説明用のコード (このアプリの動作には関係がない)

      // JapanesePhoneticAnalyzer は Windows 8.1 から使える
      // ただし、半角英数字を全角に変換してくれちゃうのが玉に瑕
      //    https://www.atmarkit.co.jp/ait/articles/1511/25/news028.html
        IReadOnlyList<Windows.Globalization.JapanesePhoneme> list
        = Windows.Globalization.JapanesePhoneticAnalyzer.GetWords("日本語の文字列abc");
        foreach (var phoneme in list) {
        // 分割した文字列（形態素）
        string displayText = phoneme.DisplayText;

        // 分割した文字列の読み仮名
        string yomiText = phoneme.YomiText;

        // この形態素は句の先頭か？
        bool isPhraseStart = phoneme.IsPhraseStart;

        // 形態素ごとに何か処理をする
        }

        // DualApiPartitionAttribute 属性が付いていなくても使えたりする
        //     1     10.0.10240.0
        //     3     10.0.14393.0
        //     5     Windows 10 v1709 / SDK 16299用のSDK
        bool isContract5Present
            = Windows.Foundation.Metadata.ApiInformation
                .IsApiContractPresent("Windows.Foundation.UniversalApiContract", 5);
        MessageBox.Show(">= Windows 10 v1709 = " + isContract5Present.ToString()); 

        // 使えない (API が package identity を必要とするため)
        try {
            Windows.Storage.ApplicationDataContainer localSettings = 
                        Windows.Storage.ApplicationData.Current.LocalSettings;
            MessageBox.Show(localSettings.ToString());
        }
        catch (InvalidOperationException ex) {
            var exMsg = ex.Message;
            Console.WriteLine(exMsg); // "プロセスにパッケージ ID がありません。 (HRESULT からの例外:0x80073D54)"
        }

        // 使える
        var gamepad = Windows.Gaming.Input.Gamepad.Gamepads.FirstOrDefault();

        // 使えない (UwpDesktop が 15063 に未対応なため)
        var flightStick = Windows.Gaming.Input.FlightStick.FlightSticks.FirstOrDefault();
#endif
    }


    // Event handler
    async void InputText_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (_osVersion.Major < 10)
            return;

        string inputText = (sender as TextBox).Text;

        // 文字列全体の読み仮名を取得する（Win10全バージョン）
        string yomi = await GetReadingAsync(inputText);
        YomiText.Text = yomi;

        if (_osVersion.Build >= 14393) {
            // 形態素分解して読み仮名を個別に取得する（14393以降）
            await GetRubyAndDisplayAsync(inputText);
        }

      // 読み仮名から漢字に変換（Win10全バージョン）
      ListBox1.ItemsSource = await ReconvertAsync(yomi);
    }

    private async Task<string> GetReadingAsync(string inputText)
    {
      // 読み仮名を取得するためのクラス
      var trcg = new Windows.Data.Text.TextReverseConversionGenerator("ja");
      
      // 文字列全体の読み仮名を取得する（Win10全バージョン）
      var yomi = await trcg.ConvertBackAsync(inputText);

      return yomi;
    }

    // 14393以前では、このメソッドを最初に呼び出そうとした時にTypeLoadException例外が出る
    // (14393以前ではTextPhoneme型がない)
    private async Task GetRubyAndDisplayAsync(string inputText)
    {
        // 読み仮名を取得するためのクラス
        var trcg = new Windows.Data.Text.TextReverseConversionGenerator("ja");

      // 形態素分解して読み仮名を個別に取得する（14393以降）
      IReadOnlyList<Windows.Data.Text.TextPhoneme> textPhonemeList
        = await trcg.GetPhonemesAsync(inputText);

      // ルビ付きの文字列として表示
      ClearRubyText();
      foreach (var phoneme in textPhonemeList)
      {
        string text = phoneme.DisplayText;
        string ruby = phoneme.ReadingText;
        AppendTextAndRuby(text, ruby);
      }
    }

    private async Task<IReadOnlyList<string>> ReconvertAsync(string yomi)
    {
        // 読み仮名から漢字に変換（Win10全バージョン）
        // Input of Japanese and Chinese Pinyin is supported.
        // ★IMEを使っているわけではなさそう。
        var tcg = new Windows.Data.Text.TextConversionGenerator("ja");
        IReadOnlyList<string> candidatesList = await tcg.GetCandidatesAsync(yomi);

        return candidatesList;
    }


    private void ClearRubyText()
    {
      P1.Inlines.Clear();
    }

    private void AppendTextAndRuby(string text, string ruby)
    {
      if (ruby == text)
        ruby = string.Empty;

      P1.Inlines.Add(
        new InlineUIContainer
        {
          Child = new TextWithRubyControl
          {
            Body = text,
            Ruby = ruby,
          },
        }
      );
    }


    async void geolocatorButton_Click(object sender, RoutedEventArgs e)
    {
        // 使える API
        var locator = new Windows.Devices.Geolocation.Geolocator();
        var position = await locator.GetGeopositionAsync();
        var point = position.Coordinate.Point.Position;
        geolocatorText.Text = string.Format("Latitude(北緯+):{0}, Longitude(東経+):{1}", 
                                            point.Latitude, point.Longitude);
    }
} // class MainWindow

}

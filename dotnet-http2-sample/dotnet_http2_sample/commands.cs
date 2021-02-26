
using System.Collections.Generic;
using System.Windows.Input;

namespace dotnet_http2_sample
{
// アプリケーションで利用する "コマンド" を並べる. アプリケーショレ内で使い回
// す. 
// ここには実行する関数は出てこない。
public static class MyCommands
{
    // CommandBindingCollection クラスは非ジェネリックな IList から派生.
    public static readonly List<CommandBinding> CommandBindings =
                                             new List<CommandBinding>();

    // Static にするのがキモ!
    static readonly RoutedUICommand _fetchCommand = 
            new RoutedUICommand("取得!", "Fetch", typeof(MyCommands));
    public static RoutedUICommand FetchCommand { 
        get { return _fetchCommand; }
    }
} // MyCommands

}

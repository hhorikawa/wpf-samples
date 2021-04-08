# wpf-samples

.NET Core 3.0 [2019年9月] から再び WPF が使えるようになり, .NET 5.0 でも普通に動いているので, WPF はアプリケーション開発の選択肢として選べる。

<i>Silverlight</i>, UWP などに移行したほうがいいのか, 先行きに不安があった。Silverlight は単に終了し、UWP もデスクトップアプリケーションとしては主流にならない。

WPF は多様な書き方が可能だが、かえってどのように書くべきなのかがさっぱり分からない。整理しておく。


## 01_wpf-validation-rules

 - View と view model との分離. View model は `INotifyPropertyChanged` インタフェイスを実装するの *ではなく*, `DependencyObject` class から派生させるべき。

 - View model の binding は, ソースコードではなく, XAML の `<Window.DataContext>` で指定する。ここでの指定は, 原則として "型". インスタンスが自動的に生成される。
 
 - データ検証. `IDataErrorInfo` インタフェイスは古い. `INotifyDataErrorInfo` インタフェイスを実装する。もっと良いのは, `ValidationRule` クラスから派生させ, XAML 側から検証器を指定する。入力コントロールごとの検証と, submit 時の検証とがある。
 
 - エラーの表示. フィールドの近くに表示するのが UI 観点でも good. XAML の `<Window.Resources>` 内に `<ControlTemplate>` 要素を記述する。


 
参考: <a href="https://blog.magnusmontin.net/2013/08/26/data-validation-in-wpf/">Data validation in WPF | Magnus Montin</a>





## 02_ListView-DataVirtualization

表形式で大量のデータを表示する場合、データ全部をメモリに載せると大変。Data virtualization で, 表示するのに必要なデータだけ view model から提供する。

`ListView` のサンプル. このサンプルでは, 比較のため, `DataContext` として `List<>` (全部をメモリに載せる), `VirtualizingCollection<>` および `AsyncVirtualizingCollection<>` という3つのクラスを用意している。

過去との互換性のため, WPF に渡すコンテナとしては, ジェネリック版ではなく非ジェネリックな `IList` から派生させたクラスで `this[]` と `GetEnumerator()` を実装する。


Forked from <a href="https://www.codeproject.com/Articles/34405/WPF-Data-Virtualization">WPF: Data Virtualization - CodeProject</a> Original license: public domain







## 03_dotnet-http2-sample

サーバに HTTP/2 でリクエストを投げるサンプル。

 - `MenuItem.Command` や `Button.Command` の使い方。ググると `DelegateCommand` を使う例ばかりだが、妥当ではない。`CommandBindings` を使え。

 - "コマンド" として `ApplicationCommands.Close` などの標準コマンドも使える。これらのコマンドとハンドラとを `CommandBinding` クラスで結びつける。
 
 - テキストフィールドの値によって, `Command` プロパティを持つボタンの有効・無効を切り替える。

  1. `<TextBox> に `<Binding.ValidationRules>` でヴァリデータを設定する。
  2. ヴァリデータから view model に状態を通知し, `ICommand.CanExecute` に反映させる。そのためには, Behaviour を使うのが妥当だが、これもググると `System.Windows.Interactivity` を使う例ばかり出てくる。そんなの不要。
  3. WPF の `<Button>` は `ICommand.CanExecute` の状態変化に伴って、有効・無効が自動的に切り替わる。


.NET Framework 4.7 で HTTP/2 を使うには `WinHttpHandler` クラスを使わなければならない。.NET Core 3.0 以降は素の `HttpClient` でも HTTP/2 がサポートされる。






## 04_wpf-sample-for-calling-winrt-apis

UWP アプリ用の API "Windows Runtime APIs" (WinRT APIs) を呼び出す。現代は, Microsoft.Windows.SDK.Contracts パッケージを使えばよい。

`Windows.Globalization.JapanesePhoneticAnalyzer` class は、日本語の形態素解析が手軽に可能。




## 05_DataGrid-DataVirtualization

再び data virtualization について。今度は `DataGrid` に適用する。

`DataGrid` は列ヘッダのクリックでソートできる。Data virtualization とどのように組み合わせるか。

Collection クラスではなく, collection view クラスを作り、これを `DataGrid.ItemsSource` に bind する。手を抜くため, `ListCollectionView` クラスから派生して最小限だけ override するのがよい。



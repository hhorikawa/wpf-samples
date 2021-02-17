# wpf-samples

.NET Core 3.0 [2019年9月] から再び WPF が使えるようになり, .NET 5.0 でも普通に動いているので, WPF はアプリケーション開発の選択肢として選べる。Silverlight, UWP などに移行したほうがいいのか, 先行きに不安があった。

WPF は多様な書き方が可能だが、かえってどのように書くべきなのかがさっぱり分からない。整理しておく。


## wpf-validation-rules

 - View と view model との分離. View model は `INotifyPropertyChanged` インタフェイスを実装するの *ではなく*, `DependencyObject` class から派生させるべき。

 - データ検証. `IDataErrorInfo` インタフェイスは古い. `INotifyDataErrorInfo` インタフェイスを実装する。もっと良いのは, `ValidationRule` クラスから派生させ, XAML 側から検証器を指定する。
 
 - エラーの表示. フィールドの近くに表示するのが UI 観点でも good.
 
 
<a href="https://blog.magnusmontin.net/2013/08/26/data-validation-in-wpf/">Data validation in WPF | Magnus Montin</a>



## ListView-DataVirtualization

表形式で大量のデータを表示する場合、データ全部をメモリに載せると大変。Data virtualization で, 表示するのに必要なデータだけ view model から提供する。

`ListView` のサンプル. `IList` から派生させたクラスで `this[]` と `GetEnumerator()` を実装する。


Forked from <a href="https://www.codeproject.com/Articles/34405/WPF-Data-Virtualization">WPF: Data Virtualization - CodeProject</a>


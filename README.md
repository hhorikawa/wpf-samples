# wpf-samples

WPF は多様な書き方が可能だが、かえってどのように書くべきなのかがさっぱり分からない。
.NET 5.0 でも WPF が使えるようになったので, 整理しておく。


## wpf-validation-rules

 - View と view model との分離. View model は `INotifyPropertyChanged` インタフェイスを実装するの *ではなく*, `DependencyObject` class から派生させるべき。

 - データ検証. `IDataErrorInfo` インタフェイスは古い. `INotifyDataErrorInfo` インタフェイスを実装する。もっと良いのは, `ValidationRule` クラスから派生させ, XAML 側から検証器を指定する。
 
 - エラーの表示. フィールドの近くに表示するのが UI 観点でも good.
 
 

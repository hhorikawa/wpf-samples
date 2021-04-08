
# wpf-validation-rules

WPF は, XAML で view を作る。C# のコードを宣言的に書くもの。全部 C# で書くこともできるが、XAML で書けるものは、できるだけ XAML で書くようにする。


## View - view model

View model との binding が必須。View に表示している内容を保持する。`FrameworkElement.DataContext` プロパティで紐付ける。原則として view (ウィンドウ、各コントロールなど) と view model は 1:1 の関係になる。

`DataContext` は, C# コードのコンストラクタで設定してもよいが, XAML で設定するのが推奨。

`<Window xmlns:local="clr-namespace:wpf_combobox"` のようにして XML名前空間と C#コードの名前空間を紐付ける。

```xaml
<Window xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
        xmlns:local="clr-namespace:wpf_combobox"
        x:Class="wpf_combobox.MainWindow" 
        Title="MainWindow" Height="400" Width="500">
  <Window.DataContext>
    <local:MyViewModel />
  </Window.DataContext>
  <Window.Resources>
```

View model は, 多くの解説では, `INotifyPropertyChanged` インタフェイスを実装するようになっているが、そうではなくて, `DependencyObject` class から派生させるべき。



## データ検証

フィールド単位の検証と、複数のフィールドを組み合わせた検証とがある。

フィールド単位の検証は、これも多くの解説では view model で行うようになっているが、そうではなく, `ValidationRule` クラスを使うべき。検証器を再利用できる。

```xaml
    <TextBox Name="numberBox" HorizontalAlignment="Left" Height="23" Margin="85,141,0,0" 
             Validation.ErrorTemplate="{StaticResource errorTemplate}" VerticalAlignment="Top" Width="120">
      <TextBox.Text>
        <!-- XAML 属性値の {...} も, 子要素として書いてよい。というかそれの簡略記法.
          TextBox の UpdateSourceTrigger の default 値は LostFocus. リアルタイ
          ム検証したいときは変更すること。-->
        <Binding Path="NumberBox" UpdateSourceTrigger="PropertyChanged">
          <!-- 検証器は複数書ける. DependencyProperty.Register() では一つしか書けない. -->
          <Binding.ValidationRules>
            <local:ValidatesPresence />
            <local:ValidatesIntInclusion Min="1" ValidationStep="RawProposedValue" />
          </Binding.ValidationRules>
        </Binding>
      </TextBox.Text>
    </TextBox>
```



-*- coding:utf-8 -*-

# WPF Desktop アプリから UWP アプリ用の API "Windows Runtime APIs" (WinRT APIs) を呼び出す

Forked from https://github.com/biac/codezine/tree/master/UwpForefront/UF02/

解説記事: [超簡単! WPFなどの.NETのアプリからUWPのAPIを使う ～日本語の読み仮名を取得するAPIを題材に](https://codezine.jp/article/detail/10654)




## NuGet パッケージ

現代は, Microsoft.Windows.SDK.Contracts パッケージを使う。


## Package identity

多くの WinRT API は package identity が必要。 -- 例えば, `Windows.Storage` 名前空間など。
デスクトップアプリでこれらの API を使うには, MSIX package にパッケージするか, *sparse* MSIX package を作る。

例えば, [Set up your desktop application for MSIX packaging in Visual Studio](https://docs.microsoft.com/en-us/windows/msix/desktop/desktop-to-uwp-packaging-dot-net)




## WinRT APIs

このサンプルで使っているもの

 - `Windows.Globalization.JapanesePhoneticAnalyzer` class (introduced in 10.0.10240.0)

 - `Windows.Data.Text.TextReverseConversionGenerator` class (introduced in 10.0.10240.0 - for Xbox, see UWP features that aren't yet supported on Xbox)

 - `Windows.Data.Text.TextConversionGenerator` class (introduced in 10.0.10240.0 - for Xbox, see UWP features that aren't yet supported on Xbox)

 - `Windows.Devices.Geolocation.Geolocator` class (introduced in 10.0.10240.0)


このサンプルでは使っていないが、押さえておきたい

 - `Windows.UI.Xaml.Controls.InkCanvas` class

 - 適応性のあるインタラクティブなトースト通知
 


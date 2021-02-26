-*- coding:utf-8 -*-

# WPF Desktop アプリから UWP アプリ用の API "Windows Runtime APIs" (WinRT APIs) を呼び出す



Forked from https://github.com/biac/codezine/tree/master/UwpForefront/UF02/

解説記事: [超簡単! WPFなどの.NETのアプリからUWPのAPIを使う ～日本語の読み仮名を取得するAPIを題材に](https://codezine.jp/article/detail/10654)


多くの API は package identity が必要。 -- 例えば, `Windows.Storage` 名前空間など。
デスクトップアプリでこれらの API を使うには, MSIX package にパッケージするか, *sparse* MSIX package を作る。

例えば, [Set up your desktop application for MSIX packaging in Visual Studio](https://docs.microsoft.com/en-us/windows/msix/desktop/desktop-to-uwp-packaging-dot-net)

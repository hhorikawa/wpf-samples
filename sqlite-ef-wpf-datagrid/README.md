
# SQLite - Entity Framework - WPF DataGrid



## SQLite のインストール

結局、ADO.NET Entity Data Model からの "データベースから EF Designer / Code First" ができない。

たまたま試したヴァージョンの問題なのか、どうやっても ADO.NET データプロバイダを認識させることができなかった。

インストールの解説は多くあるが、断片的で、上手くいかない。Visual Studio (開発環境) の話と、実行環境の話が混ざっていることが多い。

例えば, <a href="https://knkomko.hatenablog.com/entry/2020/11/14/014119">Setup DB First EntityFramework SQLite</a> は, すんなりできるように見えるが。


<p><a href="https://www.nslabs.jp/ramble-201707.rhtml#05">Visual Studio 2017 と Oracle (ODAC) を組み合わせる</a> でも似たようなことになっていた。







### Visual Studio 2019 拡張機能

Visual Studio 2019 の [Server Explorer] で SQLite に接続するには、次の二つともが必要

開いたソリューション・プロジェクトとは関係ない. ソリューションを閉じていても使える画面。


○1. SQLite セットアップ版

<a href="https://system.data.sqlite.org/index.html/doc/trunk/www/downloads.wiki">System.Data.SQLite: Downloads Page</a>

<i>Setups for 32-bit Windows (.NET Framework 4.6)</i>, bundle版をダウンロード.
 64bit 環境であっても、32bit版 *のみ* でよい。<code>sqlite-netFx46-setup-bundle-x86-2015-1.0.113.0.exe</code>

インストールには管理者権限が必要. "Install the designer components for Visual Studio 2015" にチェックを入れる。

おそらく、本当はこれだけでイケるようになるはずだが、きちんとインストールされていない?

グローバルな <code>machine.config</code> ファイルは次のようになる; <code>C:\Windows\Microsoft.NET\Framework\v4.0.30319\Config</code> ディレクトリにある.

<pre>
  &lt;system.data>
    &lt;DbProviderFactories>
      &lt;add name="SQLite Data Provider" invariant="System.Data.SQLite.EF6"
           description=".NET Framework Data Provider for SQLite"
           type="System.Data.SQLite.EF6.SQLiteProviderFactory, System.Data.SQLite.EF6, Version=1.0.113.0, Culture=neutral, PublicKeyToken=db937bc2d44ff139" />
    &lt;/DbProviderFactories>
  &lt;/system.data>
</pre>


○2. SQLite &amp; SQL Server Compact Toolbox

<a href="https://github.com/ErikEJ/SqlCeToolbox/">ErikEJ/SqlCeToolbox: SQLite &amp; SQL Server Compact Toolbox extension for Visual Studio, SSMS (and stand alone)</a>

拡張機能 > 拡張機能の管理, から <i>SQLite/SQL Server Compact Toolbox</i>

VSIX Installer -- インストールに管理者権限が必要

確認する:

ツール > SQLite/SQL Server Compact Toolbox, から はてなマークの [About] をクリック。
  -> SQLite EF6 DbProvider in GAC を認識 (Yes になっている)


これで, [Server Explorer] 画面で "データ接続" を右クリック > [接続の追加...] から SQLite DB に接続できる。

注意: 先にファイルの用意が必要。(なので、別にSQLite DBファイルを作れるツールが必要。)







### 各プロジェクト

これは、上記の開発環境とは独立した話。これを混ぜるから混乱する。[Server Explorer] から SQLite DB に接続しないのであれば、以下だけでよい。

[NuGetパッケージの管理...]をクリック

SQLite パッケージは、似たものが大量にある。Microsoft.Data.Sqlite と System.Data.SQLite とがある。後者を使う。v1.0.113.7

System.Data.SQLite パッケージは、次に依存:
 - System.Data.SQLite.Core    -- ADO.NET data provider
 - System.Data.SQLite.Linq
 - System.Data.SQLite.EF6

なので、これ一つだけでよい。

○プロジェクトの <code>App.config</code> ファイルに接続情報を追加。

ここまでできていても、ADO.NET Entity Data Model は失敗。

追加 > [新しい項目...] ADO.NET Entity Data Model を選択, "モデルに含めるコンテンツ" ペインの
 - データベースからEF Designer
 - データベースから Code First

[データ接続の選択] で, [新しい接続...] ボタンをクリック
SQL Server しか表示されない。うーむ。







### dotConnect for SQLite

<i>dotConnect ADO.NET Data Provider for SQLite Express</i> という拡張機能もある。これもインストールしてみた。SQLite セットアップ版は不要。

[Server Explorer] のほうはすんなり使えるようになったが、やはり, "データベースから EF Designer / Code First" はできなかった。

ギブアップ。



## Entity Framework






# SqliteMigrationSample

設定など何らかの構造化されたデータをローカルに保存しておきたいことがある。LINQ が使えると便利なので, SQLite が役立つ。

.NET Framework 4.7 - デスクトップアプリで作ってみた。

Entity Framework 6 (EF6) での SQLite は System.Data.SQLite パッケージを使うが、ヴァージョンの問題か `App.config` の設定か、とにかくちゃんと動かすのが非常に困難。というか、何度やっても結局まともに動かすことができなかった。

Entity Framework Core (EF Core) 3.1 までは .NET Framework 4.7 と組み合わせることができるので、こちらを使う。これはすんなり行く。System.Data.SQLite での苦労が嘘のよう。

パッケージは次の3つを入れる. いずれもヴァージョンは 3.1.x を指定する。
 - Microsoft.EntityFrameworkCore
 - Microsoft.EntityFrameworkCore.Sqlite
 - Microsoft.EntityFrameworkCore.Tools

Code-First モデルでテーブルをデザインし, 自動的に migration を生成させ、データベースを更新できる。




## System.Data.SQLite パッケージ

以下は失敗談。

結局, ADO.NET Entity Data Model からの "データベースから EF Designer", "データベースから Code First" ができない。試したヴァージョンの問題なのか、どうやっても ADO.NET データプロバイダとして認識させることができなかった。

インストールの解説は多くあるが、断片的で、上手くいかない。Visual Studio の機能 (統合) の話, ソリューション・プロジェクトでのパッケージの利用と、さらに実行環境の話が混ざっていることが多い。

例えば, <a href="https://knkomko.hatenablog.com/entry/2020/11/14/014119">Setup DB First EntityFramework SQLite</a> は, すんなりできるように見えるが。



### プロジェクトの NuGet パッケージ

Visual Studio の拡張機能とは独立した話。[Server Explorer] から SQLite DB に接続<em>しない</em>のであれば、本来は、以下だけでよい。

[NuGetパッケージの管理...]をクリック

SQLite パッケージは似たものが大量にある。Microsoft.Data.Sqlite と System.Data.SQLite とがある。EF6 では後者を使う。v1.0.113.7

System.Data.SQLite パッケージは、次に依存している。これ一つだけでよい。
 - System.Data.SQLite.Core    -- ADO.NET data provider
 - System.Data.SQLite.Linq
 - System.Data.SQLite.EF6


○プロジェクトの <code>App.config</code> ファイルに接続情報を追加。試行錯誤で, 全然自信が持てない。

ここまでできていても、ADO.NET Entity Data Model は失敗。

追加 > [新しい項目...] ADO.NET Entity Data Model を選択, "モデルに含めるコンテンツ" ペインの
 - データベースからEF Designer
 - データベースから Code First

[データ接続の選択] で, [新しい接続...] ボタンをクリック
SQL Server しか表示されない。うーむ。



### Visual Studio 2019 拡張機能

グローバルにインストールしてはどうか。

Visual Studio 2019 の [Server Explorer] で SQLite に接続するには、次の二つともが必要. 


○1. SQLite セットアップ版

<a href="https://system.data.sqlite.org/index.html/doc/trunk/www/downloads.wiki">System.Data.SQLite: Downloads Page</a>

<i>Setups for 32-bit Windows (.NET Framework 4.6)</i>, bundle版をダウンロード.
 64bit 環境であっても 32bit版 *のみ* でよい。<code>sqlite-netFx46-setup-bundle-x86-2015-1.0.113.0.exe</code>

インストールには管理者権限が必要. "Install the designer components for Visual Studio 2015" にチェックを入れる。

データプロバイダとしてインストールされる、はず。おそらく、本当はこれだけでイケるようになるはずだが、きちんとインストールされていない?

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

注意: 先にSQLite DBファイルの用意が必要。なので、別途, SQLite DBファイルを作れるツールが必要。





## dotConnect for SQLite

<i>dotConnect ADO.NET Data Provider for SQLite Express</i> という拡張機能もある。これもインストールしてみた。
Visual Studio との統合と、データプロバイダが同梱されている。SQLite セットアップ版は不要。

[Server Explorer] のほうはすんなり使えるようになったが、やはり, "データベースから EF Designer / Code First" はできなかった。

Standard エディション (無償) でも a fully-featured ADO.NET data provider with design time support となっているのに・・・






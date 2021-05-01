
// パッケージマネージャーコンソールで, 次のコマンドで、Configuration.cs が生成される。
//     PM> Enable-Migrations
// "Migrations/Configuration.cs" というファイル名でなければならない。

namespace wpf_datagrid.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

internal sealed class Configuration: DbMigrationsConfiguration<wpf_datagrid.Models.Model1>
{
    public Configuration()
    {
        AutomaticMigrationsEnabled = false;
    }

    protected override void Seed(wpf_datagrid.Models.Model1 context)
    {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method
            //  to avoid creating duplicate seed data.
    }
} // class Configuration

}

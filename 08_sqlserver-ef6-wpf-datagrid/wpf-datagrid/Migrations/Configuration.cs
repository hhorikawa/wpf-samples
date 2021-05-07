
// パッケージマネージャーコンソールで, 次のコマンドで、Configuration.cs が生成される。
//     PM> Enable-Migrations
// "Migrations/Configuration.cs" というファイル名でなければならない。

namespace wpf_datagrid.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using wpf_datagrid.Models;

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
        var cat = new ProdCategory() {
                            Name = "金属製品" };
        context.ProdCategories.AddOrUpdate(cat);
        context.SaveChanges();

        context.Products.AddOrUpdate(new Product() {
                            Name = "ABC 123",
                            NameKana = "エイビーシー123",
                            Description = "",
                            CategoryId = cat.Id });
        context.Products.AddOrUpdate(new Product() {
                            Name = "XYZ 567",
                            NameKana = "エックスワイゼット567",
                            Description = "",
                            CategoryId = cat.Id });
        context.SaveChanges();
    }
} // class Configuration

}

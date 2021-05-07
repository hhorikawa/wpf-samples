using System;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace wpf_datagrid.Models
{

public class Model1 : DbContext
{
    // コンテキストは、アプリケーション構成ファイル (App.config または
    // Web.config) から 'Model1' 接続文字列を使用して生成される。
    public Model1()
            : base("name=Model1")
    {
    }


    // created_at, updated_at の自動更新
    public override int SaveChanges()
    {
        add_timestamps();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(
                        CancellationToken cancellationToken = default)
    {
        add_timestamps();
        return base.SaveChangesAsync(cancellationToken);
    }

    // `created_at`, `updated_at` を更新。
    void add_timestamps()
    {
        var entities = ChangeTracker.Entries()
            .Where(x => x.Entity is RecordBase && 
                   (x.State == EntityState.Added || x.State == EntityState.Modified));

        var now = DateTime.UtcNow; // current datetime
        foreach (var entity in entities) {
            if (entity.State == EntityState.Added)
                ((RecordBase) entity.Entity).CreatedAt = now;
            ((RecordBase) entity.Entity).UpdatedAt = now;
        }
    }

    // モデルに含めるエンティティ型ごとに DbSet を追加します。Code First モデ
    // ルの構成および使用の詳細については,
    // http://go.microsoft.com/fwlink/?LinkId=390109 を参照してください。

    public virtual DbSet<Customer> Customers { get; set; }
    
    // Product
    public virtual DbSet<Product> Products { get; set; }
    public virtual DbSet<ProdCategory> ProdCategories { get; set; }

    // SalesOrder
    public virtual DbSet<SalesOrder> SalesOrders { get; set; }
    public virtual DbSet<SalesOrderDetail> SalesOrderDetails { get; set; }

} // class Model1

}

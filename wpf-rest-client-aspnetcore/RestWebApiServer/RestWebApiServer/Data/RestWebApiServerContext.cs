using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RestWebApiServer.Models;

namespace RestWebApiServer.Data
{

// データベースに接続するクラス.
// IdentityDbContext<ApplicationUser> から派生すると, ASP.NET Identity のテー
// ブルがいくつか生成される。どうしてこういう構造になった??
//   -> ASP.NET Core 5.0 でも IdentityDbContext<TUser> クラスは健在. むむ
public class RestWebApiServerContext : DbContext
{
    public RestWebApiServerContext(
                        DbContextOptions<RestWebApiServerContext> options) :
        base(options)
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

    // テーブル一覧
    public DbSet<RestWebApiServer.Models.Book> Book { get; set; }
    public DbSet<RestWebApiServer.Models.Creator> Creator { get; set; }
    public DbSet<AuthorBook> AuthorBook { get; set; }

/*
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // EF Core 5.0: 複合主キーは, OnModelCreating() 内で定義するしかない.
        // => この方法では scaffolding できない.
        //modelBuilder.Entity<AuthorBook>()
        //    .HasKey(au_bo => new { au_bo.AuthorId, au_bo.BookId });

        // 多対多: 自動的に生成される中間テーブルをカスタマイズする。
        // => しかし, Author, Book クラスそれぞれから, 中間テーブルと他方の
        //    テーブルと両方を参照するようにはできない。
        modelBuilder.Entity<Author>()
            .HasMany(au => au.Books).WithMany(bo => bo.Authors)  // ここまでが条件式
            // 次のように定義する
            .UsingEntity<AuthorBook>( // 中間テーブルの型. Dictionary<> でもよい.
                // right
                j => j.HasOne<Book>().WithMany(bo => bo.AuthorsBooks)
                      .HasForeignKey(au_bo => au_bo.BookId),
                // left
                j => j.HasOne<Author>().WithMany(au => au.AuthorsBooks)
                      .HasForeignKey(au_bo => au_bo.AuthorId) );
    }
*/

}

}

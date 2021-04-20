
using System;
using System.Collections.Generic;

// 古いアノテイション - .NET 4.8 まで。[非推奨]
//using System.Data.Linq.Mapping;
// Data annotations
using System.ComponentModel.DataAnnotations; // .NET 4.0以降
using System.ComponentModel.DataAnnotations.Schema; // .NET 4.5以降

using System.Data.Entity;
using System.Linq;

namespace wpf_datagrid
{
public class Model1 : DbContext
{
    // コンテキストは、アプリケーションの構成ファイル (App.config または
    // Web.config) から 'Model1' 接続文字列を使用するように構成されています。
    public Model1() : base("name=Model1")
    {
    }

    // モデルに含めるエンティティ型ごとに DbSet を追加します。Code First モデ
    // ルの構成および使用の詳細については,
    // http://go.microsoft.com/fwlink/?LinkId=390109 を参照してください。
    public virtual DbSet<Product> Products { get; set; }
    public virtual DbSet<ProductCategory> ProductCategories { get; set; }
    public virtual DbSet<Customer> Customers { get; set; }
    public virtual DbSet<SalesOrder> SalesOrders { get; set; }
}

public class RecordBase
{
    [Column("created_at"), Required]
    public DateTime CreatedAt { get; set; }

    [Column("updated_at"), Required]
    public DateTime UpdatedAt { get; set; }

    [Column("lock_version"), Required]
    public int LockVersion { get; set; }
}

// 無駄に見えるが、カラム全部をプロパティにしなければ LINQ が動かない.
[Table("product_categories")]
public class ProductCategory: RecordBase
{
    [Key, Column("id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    //[Column(IsPrimaryKey =true, IsDbGenerated = true)]
    public int Id { get; protected set; } // 自動生成でも set が必要.

    [Required]  //[Column(CanBeNull =false)]
    public String Name { get; set; }

    /// ////////////////
    // Navigation property

    [ForeignKey("CategoryId")]
    public virtual ICollection<Product> Products { get; set; }
}

// [Table("products")]  自動で複数形
public class Product: RecordBase
{
    [Key, Column("id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    //[Column(IsPrimaryKey =true, IsDbGenerated = true)]
    public int Id { get; protected set; }

    [Required]  //[Column(CanBeNull =false)]
    public String Name { get; set; }

    [Column("name_kana"), Required]
    public String NameKana { get; set; }

    [Required(AllowEmptyStrings = true)] // 単なる `Required` は "" もエラー.
    public String Description { get; set; }

    // Foreign key
    [Column("product_category_id"), Required]
    public int CategoryId { get; set; }

    // Navigation properties

    [ForeignKey("CategoryId")]
    public virtual ProductCategory Category { get; set; }
}

// 顧客。本当はもっとたくさんフィールドが必要。
public class Customer: RecordBase
{
    public enum MembershipGrade {
        Silver = 1,
        Gold = 2,
        Platinum = 3,
    };

    [Key, Column("id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; protected set; }

    // 苗字がない地域もあることに注意
    public string Surname { get; set; }

    [Column("given_name"), Required]
    public string GivenName { get; set; }

    [Column("ship_to"), Required]
    public string ShipTo { get; set; }

    [Required]
    public string Email { get; set; }

    [Required]
    public MembershipGrade Grade { get; set; }
}

// 受注
// 普通は一つの注文に複数の商品がある。手抜き。
[Table("sales_orders")]
public class SalesOrder: RecordBase
{
    public enum OrderStatus {
        New = 1,
        Processing = 2,
        Shipped = 3,
        //Paid = 3,   軸が違う。
    };

    public static readonly Dictionary<OrderStatus, string> StatusList =
        new Dictionary<OrderStatus, string>() {
            {OrderStatus.New,        "New"},
            {OrderStatus.Processing, "Processing" },
            {OrderStatus.Shipped, "Shipped"},
            //{OrderStatus.Paid, "Paid"},
    };

    [Key, Column("id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; protected set; }

    // Foreign key
    [Column("customer_id"), Required]
    public int CustomerId { get; set; }

    [Column("product_id"), Required]
    public int ProductId { get; set; }

    [Required]
    public OrderStatus Status { get; set; }

    // 顧客情報。
    // 変化するものはコピーするのが定石. ここでは, 試しに ship_to だけ.

    [Column("customer_ship_to"), Required]
    public string CustomerShipTo { get; set; }

    /// //
    // Navigation properties

    [ForeignKey("CustomerId")]
    public virtual Customer Customer { get; set; }
}

}

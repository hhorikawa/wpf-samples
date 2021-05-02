using System;
using System.Collections.Generic;

// 古いアノテイション - .NET 4.8 まで。[非推奨]
//using System.Data.Linq.Mapping;

// 現行の data annotations
using System.ComponentModel.DataAnnotations; // .NET 4.0以降
using System.ComponentModel.DataAnnotations.Schema; // .NET 4.5以降

using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wpf_datagrid.Models
{

public enum SalesOrderStatus {
    New = 1,
    Processing = 2,
    Shipped = 3,
    //Paid = 3,   軸が違う。
};

// 受注
// 一つの注文に複数の商品がある。=> SalesOrderDetail を複数持つ。
[Table("sales_orders")]
public class SalesOrder: RecordBase
{
    public static readonly Dictionary<SalesOrderStatus, string> StatusList =
        new Dictionary<SalesOrderStatus, string>() {
            {SalesOrderStatus.New,        "New"},
            {SalesOrderStatus.Processing, "Processing" },
            {SalesOrderStatus.Shipped, "Shipped"},
            //{OrderStatus.Paid, "Paid"},
    };

    [Key, Column("id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; protected set; }

    // Foreign key
    [Column("customer_id"), Required]
    public int CustomerId { get; set; }

    // 顧客情報。
    // 変化するものはコピーするのが定石. ここでは, 試しに ship_to だけ.
    [Column("customer_ship_to"), Required]
    public string CustomerShipTo { get; set; }

    /// //
    // Navigation properties

    [ForeignKey("CustomerId")]
    public virtual Customer Customer { get; set; }

    [ForeignKey("SalesOrderId")]
    public virtual ICollection<SalesOrderDetail> Details { get; }
}


[Table("sales_order_details")]
public class SalesOrderDetail
{
    [Key, Column("id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; protected set; }

    // 親
    [Column("sales_order_id"), Required]
    public int SalesOrderId { get; set; }

    [Column("product_id"), Required]
    public int ProductId { get; set; }

    [Required]
    public SalesOrderStatus Status { get; set; }

    /// //
    // Navigation properties

    public virtual SalesOrder SalesOrder { get; set; }

    public virtual Product Product { get; set; }
}

}
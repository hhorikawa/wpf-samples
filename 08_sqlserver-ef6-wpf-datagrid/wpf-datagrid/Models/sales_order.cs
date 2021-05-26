using System;
using System.Collections.Generic;
using System.ComponentModel;

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
// 保存時検証は, IValidatableObject を実装する.
[Table("sales_orders")]
public class SalesOrder: RecordBase, INotifyPropertyChanged, IValidatableObject
{
    public event PropertyChangedEventHandler PropertyChanged;

    protected void RaisePropertyChanged(string propertyName)
    {
        PropertyChanged ?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public static readonly Dictionary<SalesOrderStatus, string> StatusList =
        new Dictionary<SalesOrderStatus, string>() {
            {SalesOrderStatus.New,        "New"},
            {SalesOrderStatus.Processing, "Processing" },
            {SalesOrderStatus.Shipped, "Shipped"},
            //{OrderStatus.Paid, "Paid"},
    };

    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; protected set; }

    // Foreign key
    [Column("customer_id"), Required]
    public int CustomerId { get; set; }

    string _cst;
    // 顧客情報。
    // 変化するものはコピーするのが定石. ここでは, 試しに ship_to だけ.
    [Column("customer_ship_to"), Required]
    public string CustomerShipTo {
        get { return _cst; }
        set { _cst = value;
            RaisePropertyChanged(nameof(CustomerShipTo));
        }
    }


    // @override IValidatableObject
    public IEnumerable<ValidationResult> Validate(
                                        ValidationContext validationContext)
    {
        if ( CustomerShipTo.Contains("福岡県" )) {
            // IEnumerable<> なので yield で返す.
            yield return new ValidationResult(
                    "Blog Title cannot match Blogger Name",
                    new[] {nameof(CustomerShipTo)} ); // 対象メンバリスト. 複数可.
        }

        if (Details.Count == 0) {
            yield return new ValidationResult(
                    "一つは明細が必要", new[] {nameof(Details)} );
        }
    }


    // データベースに保存しない
    [NotMapped]
    public SalesOrderStatus Status {
        get {
            if (Details.All(x => x.Status == SalesOrderStatus.Shipped))
                return SalesOrderStatus.Shipped;
            return SalesOrderStatus.New;
        }
    }

    /// //
    // Navigation properties

    [ForeignKey("CustomerId")]
    public virtual Customer Customer { get; set; }

    [ForeignKey("SalesOrderId")]
    public virtual ICollection<SalesOrderDetail> Details { get; set; } =
                new List<SalesOrderDetail>();
}


// 明細行
[Table("sales_order_details")]
public class SalesOrderDetail: IValidatableObject
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; protected set; }

    // 親
    [Column("sales_order_id"), Required]
    public int SalesOrderId { get; set; }

    [Column("product_id"), Required]
    public int ProductId { get; set; }

    [Required]
    public SalesOrderStatus Status { get; set; }

    [Required(AllowEmptyStrings = true) ]
    public string Comment { get; set; }

    // @override IValidatableObject
    public IEnumerable<ValidationResult> Validate(
                                        ValidationContext validationContext)
    {
        if ( Comment.Equals("NG") ) {
            // IEnumerable<> なので yield で返す.
            yield return new ValidationResult(
                "トランザクションのテスト",
                new[] {nameof(Comment)} ); // 対象メンバリスト. 複数可.
        }
    }


    /// //
    // Navigation properties

    public virtual SalesOrder SalesOrder { get; set; }

    public virtual Product Product { get; set; }
}

}

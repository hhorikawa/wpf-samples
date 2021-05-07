using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace wpf_datagrid.Models
{

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
[Table("prod_categories")]
public class ProdCategory: RecordBase
{
    [Key, Column("id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    //[Column(IsPrimaryKey =true, IsDbGenerated = true)]
    public int Id { get; protected set; } // 自動生成でも set が必要.

    [Required]  //[Column(CanBeNull =false)]
    public String Name { get; set; }

    /// ////////////////
    // Navigation property

/* set が必要!! 次のコードで, cat.Products に product list が 暗黙に set され
 * る.
 *         context.Products.AddOrUpdate(new Product() {
                            Name = "XYZ 567",
                            NameKana = "エックスワイゼット567",
                            Description = "",
                            CategoryId = cat.Id });
 */
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
    [Column("prod_category_id"), Required]
    public int CategoryId { get; set; }

    // Navigation properties

    [ForeignKey("CategoryId")]
    public virtual ProdCategory Category { get; set; }
}

}

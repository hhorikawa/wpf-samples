using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestWebApiServer.Models
{ 

/// <summary>
/// Summary description for Class1
/// </summary>
[Table("books")]
public class Book : RecordBase
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; protected set; }

    [Required]
    public string Title { get; set; }

    [Required]
    public int Year { get; set; }

    [Required(AllowEmptyStrings = true)]
    public string Description { get; set; }

    // EF Core 5.0 は, 明示的にマッピングしなくても, 多対多を生成してくれる
    // -> 本当に単純な中間テーブルの場合以外は, 自分で作ったほうがよい.
    // 中間テーブルと他方のテーブルの両方を参照するようにはできない。
    //public ICollection<Author> Authors { get; set; }

    // 中間テーブル
    [InverseProperty("Book")]
    public ICollection<AuthorBook> authors_books { get; set; }
}

}


using Microsoft.EntityFrameworkCore; // IndexAttribute
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema; // ForeignKeyAttribute

namespace RestWebApiServer.Models
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


// 中間テーブル
// EFCore 5.0 では, 複合キーは Fluent API のみ対応。
// しかし, scaffolding では [Key] アノテイションが必須。
// => Id を key にして, 真のキーの方にはユニーク制約を付けるしかない.
[Table("authors_books")]
[Index(nameof(CreatorId), nameof(BookId), IsUnique = true)] // Composite Unique Constraint 
public class AuthorBook   // EFでは中間テーブルは単数
{
    public enum Type {
        Author = 1, // 著者
        Editor = 2, // 編者
        Translator = 3, // 翻訳者
        Illustrator = 4, // 挿絵
    }

    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; protected set; }

    //[Key] EF Core 5.0: Error: Composite primary keys can only be set using 'HasKey' in 'OnModelCreating'.
    [Column("creator_id"), Required]
    // `<navigation property name>Id` という名前なら自動認識.
    // => 単数形.
    public int CreatorId { get; set; }

    //[Key] EF Core 5.0: Error: Composite primary keys can only be set using 'HasKey' in 'OnModelCreating'.
    [Column("book_id"), Required]
    public int BookId { get; set; }

    [Required]
    public Type type { get; set; }

    [Required]
    public int sort { get; set; }

    ///////////////////////////////////////////////
    // Navigation properties
    [ForeignKey("CreatorId")] // 1対多は, "1" の側に [ForeignKey()] を付ける
    public Creator Creator { get; set; }

    [ForeignKey("BookId")]
    public Book Book { get; set; }
}


/// <summary>
/// Summary description for Class1
/// </summary>
[Table("creators")]
public class Creator : RecordBase
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; protected set; }

    [Required]
    public string Name { get; set; }

    // EF Core 5.0 は, 明示的にマッピングしなくても, 多対多を生成してくれる
    // 1. 自動的に生成される中間テーブルは AuthorBook (単数形),
    //    外部キーは BooksId, AuthorsId (複数形 + Id).
    // 2. 中間テーブルに何も追加情報がなければ "お任せ" でよいが、大抵はそうで
    //    はない。
    //      -> 例 physicians <-> appointments <-> patients. 予約日など
    // => 結局, 本当に単純な場合以外は, 中間テーブルも自分で作ったほうがよい.
    //public ICollection<Book> Books { get; set; }

    // 中間テーブル
    [InverseProperty("Creator")]
    public ICollection<AuthorBook> authors_books { get; set; }
}

}

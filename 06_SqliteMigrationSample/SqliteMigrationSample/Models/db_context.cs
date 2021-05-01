using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqliteMigrationSample.Models
{

class MyDbContext: DbContext
{
    // ここでファイル名を指定.
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite(@"Data Source=blogging.sqlite");
    }

    public DbSet<Blog> Blogs { get; set; }
    public DbSet<Post> Posts { get; set; }
}


[Table("blogs")]
public class Blog
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; protected set; }

    [Required]
    public string Url { get; set; }

    // Get のみ
    public ICollection<Post> Posts { get; }
}

[Table("posts")]
public class Post
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; protected set; }

    [Required]
    public string Title { get; set; }

    [Required(AllowEmptyStrings = true)]
    public string Content { get; set; }

    [Column("blog_id"), Required]
    public int BlogId { get; set; }

    public Blog Blog { get; set; }
}

}

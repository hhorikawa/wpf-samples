using Microsoft.EntityFrameworkCore;
using SqliteMigrationSample.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqliteMigrationSample
{

class Program
{
    static void Main(string[] args)
    {
        using (var dbContext = new Models.MyDbContext()) {
            // Create
            Console.WriteLine("Inserting a new blog");
            dbContext.Add(new Blog { Url = "http://blogs.msdn.com/adonet" });
            dbContext.SaveChanges();

/*
  - Eager Loading "一括読み込み" 
        Related entities are loaded as part of the initial query.
        Include() メソッドを使う
  - Explicit Loading "明示的読み込み" 
        Related entities are loaded explicitly, not as part of the initial 
        query, but at a later point of time.
        DbContext#Entry() メソッドで, 後からコレクションを読み込み, 紐付ける.
  - Lazy Loading "遅延読み込み"
        Related entities are loaded when the navigation property is accessed.
        `Microsoft.EntityFrameworkCore.Proxies` パッケージを使う。
 */
            // Read
            Console.WriteLine("Querying for a blog");
            var blog = dbContext.Blogs
                    .Include(b => b.Posts) // Eager Loading
                    .OrderBy(b => b.Id)
                    .First();

            // Update
            Console.WriteLine("Updating the blog and adding a post");
            blog.Url = "https://devblogs.microsoft.com/dotnet";
            blog.Posts.Add(
                    new Post { Title = "Hello World", 
                            Content = "I wrote an app using EF Core!" });
            dbContext.SaveChanges();

            // Delete
            Console.WriteLine("Delete the blog");
            dbContext.Remove(blog); // All posts are cascade deleted.
            dbContext.SaveChanges();
        }
    }
} // class Program

}

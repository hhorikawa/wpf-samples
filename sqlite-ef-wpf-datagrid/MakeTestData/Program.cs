using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO; // Path
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wpf_datagrid;

namespace MakeTestData
{

class Program
{
    static SQLiteConnection _conn;

    static void create_table(string sql)
    {
        using (var cmd = _conn.CreateCommand()) {
            cmd.CommandText = sql;
            if (cmd.ExecuteNonQuery() == -1)
                throw new Exception("SQL Failed");
        };
    }

    // @return エラー時 -1
    static int create_tables()
    {
        SQLiteTransaction tran = null;
        try { 
            tran = _conn.BeginTransaction();
        
            create_table(
                "CREATE TABLE product_categories (" +
                "  id         INTEGER PRIMARY KEY AUTOINCREMENT," +
                "  name       VARCHAR(100) NOT NULL," +
                "  created_at TIMESTAMP NOT NULL," +
                "  updated_at TIMESTAMP NOT NULL," +
                "  lock_version int NOT NULL" +
                ")" );

            create_table(
                "CREATE TABLE products (" +
                "  id           INTEGER PRIMARY KEY AUTOINCREMENT," +
                "  name         VARCHAR(100) NOT NULL," +
                "  name_kana    VARCHAR(100) NOT NULL," +
                "  description  TEXT         NOT NULL," +
                "  product_category_id INT NOT NULL REFERENCES product_categories(id)," +
                "  created_at   TIMESTAMP NOT NULL," +
                "  updated_at   TIMESTAMP NOT NULL," +
                "  lock_version int NOT NULL" +
                ")" );

            create_table(
                "CREATE TABLE customers (" +
                "  id         INTEGER PRIMARY KEY AUTOINCREMENT, " +
                "  surname    VARCHAR(100) NOT NULL, " +
                "  given_name VARCHAR(100) NOT NULL, " +
                "  ship_to    VARCHAR(200) NOT NULL, " +
                "  email      VARCHAR(100) NOT NULL, " +
                "  grade      INT          NOT NULL, " +
                "  created_at   TIMESTAMP NOT NULL," +
                "  updated_at   TIMESTAMP NOT NULL," +
                "  lock_version int NOT NULL" +
                ")" );

            create_table(
                "CREATE TABLE sales_orders (" +
                "  id     INTEGER PRIMARY KEY AUTOINCREMENT, " +
                "  customer_id INTEGER NOT NULL REFERENCES customers(id), " +
                "  product_id  INTEGER NOT NULL REFERENCES products(id), " +
                "  status INTEGER NOT NULL, " +
                "  customer_ship_to VARCHAR(200) NOT NULL, " +
                "  created_at   TIMESTAMP NOT NULL," +
                "  updated_at   TIMESTAMP NOT NULL," +
                "  lock_version int NOT NULL" +
                ")" );
        }
        catch (Exception e) {
            tran.Rollback();
            return -1;
        }
        tran.Commit();
        return 0;
    }

    const int PRODUCT_SIZE = 100;
    const int ORDER_SIZE = 1000;

    static Model1 new_db_context()
    {
        var model = new Model1();
        model.Configuration.AutoDetectChangesEnabled = false;
        model.Configuration.ValidateOnSaveEnabled = false;
        return model;
    }

    static void create_mass_data()
    {
        Model1 model = new_db_context();
/*
        // LINQ はクエリ式かメソッド構文かで記述する。
        // 1. クエリ式は from句で始めなければならない。from, where, select な
        //    どは文脈依存キーワード.
        // 2. メソッド構文でないと書けない内容もある。
        var products1 = from product in model.Products
                       orderby product.Id
                       select product;  // クエリ式は group句または select句で終わる

        var products2 = model.Products
                        .OrderBy(a => a.Id)
                        .Select(a => a);
 */
        var now = DateTime.Now;

        ProductCategory category = null;
        for (int i = 0; i < PRODUCT_SIZE; i++) {
            if ((i % 30) == 0) {
                category = new ProductCategory() {
                    Name = Path.GetRandomFileName(),
                    CreatedAt = now, UpdatedAt = now, LockVersion = 1
                };
                model.ProductCategories.Add(category);
                model.SaveChanges();
                System.Diagnostics.Debug.Assert(category.Id != 0);
            }

            var product = new Product() {
                Name = Path.GetRandomFileName(),
                NameKana = "あいうえお",
                Description = Path.GetRandomFileName(),
                CategoryId = category.Id,
                CreatedAt = now, UpdatedAt = now, LockVersion = 1
            };
            model.Products.Add(product);

            if ( (i % 100_000) == 0) { 
                Console.Write(" " + i);
                model.SaveChanges();
                model.Dispose();  // Out of memory exception 対策
                model = new_db_context();
            }
        }
        model.SaveChanges();

        var customer = new Customer() {
            Surname = "田中", GivenName = "太郎",
            ShipTo = "東京都アラスカ", Email = "foo@example.com",
            Grade = Customer.MembershipGrade.Silver,
            CreatedAt = now, UpdatedAt = now, LockVersion = 1
        };
        model.Customers.Add(customer);
        model.SaveChanges();

        Random rnd = new Random();
        for (var i = 0; i < ORDER_SIZE; i++) {
            var order = new SalesOrder() {
                CustomerId = customer.Id,
                ProductId = rnd.Next(1, PRODUCT_SIZE + 1),
                Status = SalesOrder.OrderStatus.New,
                CustomerShipTo = "Copy - " + customer.ShipTo,
                CreatedAt = now, UpdatedAt = now, LockVersion = 1
            };
            model.SalesOrders.Add(order);
        }
        model.SaveChanges();
    }

    static void Main(string[] args)
    {
        File.Delete("SampleDb.sqlite");
        _conn = new SQLiteConnection("Data Source=SampleDb.sqlite");
        _conn.Open();
        create_tables();
        create_mass_data();
        _conn.Close();
    }

} // class Program

}

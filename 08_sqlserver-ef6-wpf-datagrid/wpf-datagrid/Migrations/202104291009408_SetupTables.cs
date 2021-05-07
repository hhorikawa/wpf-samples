namespace wpf_datagrid.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SetupTables : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Customers",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        Surname = c.String(),
                        given_name = c.String(nullable: false),
                        ship_to = c.String(nullable: false),
                        Email = c.String(nullable: false),
                        Grade = c.Int(nullable: false),
                        created_at = c.DateTime(nullable: false),
                        updated_at = c.DateTime(nullable: false),
                        lock_version = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.id);
            
            CreateTable(
                "dbo.prod_categories",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        created_at = c.DateTime(nullable: false),
                        updated_at = c.DateTime(nullable: false),
                        lock_version = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.id);
            
            CreateTable(
                "dbo.Products",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        name_kana = c.String(nullable: false),
                        Description = c.String(nullable: false),
                        prod_category_id = c.Int(nullable: false),
                        created_at = c.DateTime(nullable: false),
                        updated_at = c.DateTime(nullable: false),
                        lock_version = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.prod_categories", t => t.prod_category_id, cascadeDelete: true)
                .Index(t => t.prod_category_id);
            
            CreateTable(
                "dbo.sales_order_details",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        sales_order_id = c.Int(nullable: false),
                        product_id = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.Products", t => t.product_id, cascadeDelete: true)
                .ForeignKey("dbo.sales_orders", t => t.sales_order_id, cascadeDelete: true)
                .Index(t => t.sales_order_id)
                .Index(t => t.product_id);
            
            CreateTable(
                "dbo.sales_orders",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        customer_id = c.Int(nullable: false),
                        customer_ship_to = c.String(nullable: false),
                        created_at = c.DateTime(nullable: false),
                        updated_at = c.DateTime(nullable: false),
                        lock_version = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.Customers", t => t.customer_id, cascadeDelete: true)
                .Index(t => t.customer_id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.sales_order_details", "sales_order_id", "dbo.sales_orders");
            DropForeignKey("dbo.sales_orders", "customer_id", "dbo.Customers");
            DropForeignKey("dbo.sales_order_details", "product_id", "dbo.Products");
            DropForeignKey("dbo.Products", "prod_category_id", "dbo.prod_categories");
            DropIndex("dbo.sales_orders", new[] { "customer_id" });
            DropIndex("dbo.sales_order_details", new[] { "product_id" });
            DropIndex("dbo.sales_order_details", new[] { "sales_order_id" });
            DropIndex("dbo.Products", new[] { "prod_category_id" });
            DropTable("dbo.sales_orders");
            DropTable("dbo.sales_order_details");
            DropTable("dbo.Products");
            DropTable("dbo.prod_categories");
            DropTable("dbo.Customers");
        }
    }
}

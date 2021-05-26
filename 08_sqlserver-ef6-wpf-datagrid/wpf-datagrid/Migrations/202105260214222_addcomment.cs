namespace wpf_datagrid.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addcomment : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.sales_order_details", "Comment", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.sales_order_details", "Comment");
        }
    }
}

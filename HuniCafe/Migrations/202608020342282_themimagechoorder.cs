namespace HuniCafe.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class themimagechoorder : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OrderDetails", "Image", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.OrderDetails", "Image");
        }
    }
}

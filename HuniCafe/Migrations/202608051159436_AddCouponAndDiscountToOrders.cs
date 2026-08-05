namespace HuniCafe.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddCouponAndDiscountToOrders : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Coupons",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Code = c.String(nullable: false, maxLength: 20),
                        Description = c.String(nullable: false, maxLength: 200),
                        DiscountType = c.String(nullable: false, maxLength: 20),
                        DiscountValue = c.Decimal(nullable: false, precision: 18, scale: 2),
                        MinimumOrderValue = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Quantity = c.Int(nullable: false),
                        UsedQuantity = c.Int(nullable: false),
                        StartDate = c.DateTime(nullable: false),
                        EndDate = c.DateTime(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        CreatedDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            AddColumn("dbo.Orders", "CouponID", c => c.Int());
            AddColumn("dbo.Orders", "DiscountAmount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            CreateIndex("dbo.Orders", "CouponID");
            AddForeignKey("dbo.Orders", "CouponID", "dbo.Coupons", "ID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Orders", "CouponID", "dbo.Coupons");
            DropIndex("dbo.Orders", new[] { "CouponID" });
            DropColumn("dbo.Orders", "DiscountAmount");
            DropColumn("dbo.Orders", "CouponID");
            DropTable("dbo.Coupons");
        }
    }
}

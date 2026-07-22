namespace HuniCafe.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddAdminAccountTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AdminAccounts",
                c => new
                {
                    AdminID = c.Int(nullable: false, identity: true),
                    Username = c.String(nullable: false, maxLength: 50),
                    Password = c.String(nullable: false, maxLength: 100),
                    FullName = c.String(nullable: false, maxLength: 100),
                    Email = c.String(maxLength: 100),
                    IsActive = c.Boolean(nullable: false),
                })
                .PrimaryKey(t => t.AdminID);
        }

        public override void Down()
        {
            DropTable("dbo.AdminAccounts");
        }
    }
}

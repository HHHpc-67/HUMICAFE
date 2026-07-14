namespace HuniCafe.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<HuniCafe.Models.HuniCafeDB>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true; //True để dễ dàng cập nhật cơ sở dữ liệu khi mô hình thay đổi (Model)
        }

        protected override void Seed(HuniCafe.Models.HuniCafeDB context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method
            //  to avoid creating duplicate seed data.
        }
    }
}

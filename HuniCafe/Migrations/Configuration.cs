namespace HuniCafe.Migrations
{
    using HuniCafe.Models;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<HuniCafe.Models.HuniCafeDB>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;//True để dễ dàng cập nhật cơ sở dữ liệu khi mô hình thay đổi (Model)
        }

        protected override void Seed(HuniCafe.Models.HuniCafeDB context)
        {

        }
    }
}

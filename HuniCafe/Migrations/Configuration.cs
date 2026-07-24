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
            AutomaticMigrationsEnabled = true; //True để dễ dàng cập nhật cơ sở dữ liệu khi mô hình thay đổi (Model)
        }

        protected override void Seed(HuniCafe.Models.HuniCafeDB context)
        {
            //  This method will be called after migrating to the latest version.
            context.Categories.AddOrUpdate(
        c => c.CategoryName, 

        new Category { CategoryName = "Coffee", CategoryDescription = "Pha đậm sống chất" },
        new Category { CategoryName = "Tea", CategoryDescription = "Trà thơm ngon" },
        new Category { CategoryName = "Cake", CategoryDescription = "Bánh ngọt hấp dẫn" },
        new Category { CategoryName = "Juice", CategoryDescription = "Nước ép tươi mát" }
    );
            //  You can use the DbSet<T>.AddOrUpdate() helper extension method
            //  to avoid creating duplicate seed data.

            context.Products.AddOrUpdate(
                p => p.ProductName,
                new Product { ProductName = "Cà phê sữa", Description = "Cà phê pha với sữa đặc", Price = 30000, CategoryID = 1, Image = "https://res.cloudinary.com/dgzvq4bfv/image/upload/v1784874650/Screenshot_2026-07-24_133017_gqtuij.png" },
                new Product { ProductName = "Cà phê đen", Description = "cà phê nguyên chất không sữa", Price = 25000, CategoryID = 1, Image = "https://res.cloudinary.com/dgzvq4bfv/image/upload/v1784874650/Screenshot_2026-07-24_133017_gqtuij.png" },
                new Product { ProductName = "Trà sữa", Description = "Trà pha với sữa và trân châu", Price = 35000, CategoryID = 2, Image= "https://res.cloudinary.com/dgzvq4bfv/image/upload/v1784874650/Screenshot_2026-07-24_133017_gqtuij.png" },
                new Product { ProductName = "Bánh ngọt", Description = "Bánh ngọt hấp dẫn", Price = 40000, CategoryID = 3, Image = "https://res.cloudinary.com/dgzvq4bfv/image/upload/v1784874650/Screenshot_2026-07-24_133017_gqtuij.png" },
                new Product { ProductName = "Nước ép trái cây", Description  = "Nước ép tươi mát", Price = 30000, CategoryID = 4 , Image = "https://res.cloudinary.com/dgzvq4bfv/image/upload/v1784874650/Screenshot_2026-07-24_133017_gqtuij.png" }
            );

          
        }
    }
}

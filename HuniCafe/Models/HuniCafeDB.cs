using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace HuniCafe.Models
{
    public class HuniCafeDB : DbContext
    {
        public HuniCafeDB() : base("name=HuniCafeConnection")
        {
        }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<Users> Users { get; set; }
        public DbSet<Coupon> Coupons { get; set; }
    }
}
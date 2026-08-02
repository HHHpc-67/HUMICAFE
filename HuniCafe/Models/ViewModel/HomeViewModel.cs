using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace HuniCafe.Models.ViewModel
{
    public class HomeViewModel
    {
        public List<Product> FeaturedProducts { get; set; }
        public List<Category> Categories { get; set; }
     public List<Product> Products { get; set; }
    }
}
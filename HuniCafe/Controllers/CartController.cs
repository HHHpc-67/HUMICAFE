using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HuniCafe.Models;
namespace HuniCafe.Controllers
{
    public class CartController : Controller
    {
        private readonly HuniCafeDB db = new HuniCafeDB();
        // GET: Cart
        public ActionResult Index()
        {
            var cart = Session["Cart"] as List<Cart>;

            if (cart == null)
            {
                cart = new List<Cart>();
            }

            return View(cart);
        }


        public ActionResult AddToCart(int id)
        {
            var product = db.Products.Find(id);

            if (product == null)
            {
                return HttpNotFound();
            }

            var cart = Session["Cart"] as List<Cart>;

            if (cart == null)
            {
                cart = new List<Cart>();
            }

            var item = cart.FirstOrDefault(x => x.ProductID == id);

            if (item != null)
            {
                item.Quantity++;
            }
            else
            {
                cart.Add(new Cart
                {
                    ProductID = product.ProductID,
                    ProductName = product.ProductName,
                    Image = product.Image,
                    Price = product.Price,
                    Quantity = 1
                });
            }

            Session["Cart"] = cart;

            return RedirectToAction("Index");
        }


        //nút tăng số lượng 
        public ActionResult Increase(int id)
        {
            var cart = Session["Cart"] as List<Cart>;

            if (cart != null)
            {
                var item = cart.FirstOrDefault(x => x.ProductID == id);

                if (item != null)
                {
                    item.Quantity++;
                }

                Session["Cart"] = cart;
            }

            return RedirectToAction("Index");
        }



        //giảm
        public ActionResult Decrease(int id)
        {
            var cart = Session["Cart"] as List<Cart>;

            if (cart != null)
            {
                var item = cart.FirstOrDefault(x => x.ProductID == id);

                if (item != null)
                {
                    item.Quantity--;

                    if (item.Quantity <= 0)
                    {
                        cart.Remove(item);
                    }
                }

                Session["Cart"] = cart;
            }

            return RedirectToAction("Index");
        }



        //nút xáo
        
        public ActionResult Remove(int id)
        {
            var cart = Session["Cart"] as List<Cart>;

            if (cart != null)
            {
                var item = cart.FirstOrDefault(x => x.ProductID == id);

                if (item != null)
                {
                    cart.Remove(item);
                }

                Session["Cart"] = cart;
            }

            return RedirectToAction("Index");
        }

    }
}
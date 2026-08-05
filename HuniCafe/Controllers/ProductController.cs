using HuniCafe.Models;
using HuniCafe.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Razor.Tokenizer.Symbols;

namespace HuniCafe.Controllers
{
    public class ProductController : Controller
    {
        private readonly HuniCafeDB db = new HuniCafeDB();

        //MainPage - Product List
        public ActionResult Index(int? categoryId, string keyword)
        {
            var products = db.Products.Where(p => p.Category.IsActive == true).AsQueryable();

            if (categoryId.HasValue)
            {
                products = products.Where(p => p.CategoryID == categoryId.Value);
            }


            //tìm kiếm theo tên 

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                products = products.Where(p => p.ProductName.Contains(keyword));
            }

            var model = new ProductViewModel
            {
                Products = products.ToList(),
                //Chỉ lấy các Danh mục đang Active để hiện trên thanh Menu/Tab lọc
                Categories = db.Categories.Where(c => c.IsActive == true).ToList(),
                SelectedCategory = categoryId
            };

          

            ViewBag.Keyword = keyword;
            ViewBag.CategoryId = categoryId;

            return View(model);
        }

        // ADMIN - Product List Page
        public ActionResult Product_Admin()
        {
            var products = db.Products.AsNoTracking().Include(p => p.Category).ToList();
            return View(products.ToList());
        }

        // CREATE
        [HttpGet]
        public ActionResult Create()
        {
            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Product dept, HttpPostedFileBase ImageFile)
        {
            if (ModelState.IsValid)
            {
                db.Products.Add(dept);
                db.SaveChanges();
                return RedirectToAction("Product_Admin");
            }

            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName", dept.CategoryID);
            return View(dept);
        }

        // DETAILS
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }

            return View(product);
        }

        // EDIT
        [HttpGet]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }

            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName", product.CategoryID);
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Product product, HttpPostedFileBase ImageFile)
        {
            if (ModelState.IsValid)
            {
                db.Entry(product).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Product_Admin");
            }

            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName", product.CategoryID);
            return View(product);
        }

        // DELETE
        [HttpGet]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }

            return View(product);
        }
        // POST: Product/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Product product = db.Products.Find(id);
            if (product != null)
            {
                db.Products.Remove(product);
                db.SaveChanges();
            }

            return RedirectToAction("Product_Admin");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
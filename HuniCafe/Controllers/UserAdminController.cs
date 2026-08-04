using HuniCafe.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HuniCafe.Controllers
{
    public class UserAdminController : Controller
    {
        private readonly HuniCafeDB db = new HuniCafeDB();
        public ActionResult Index()
        {
            var users = db.Users.ToList();
            return View(users);
        }
    }
}
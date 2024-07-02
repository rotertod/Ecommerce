using Ecommerce.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Ecommerce.Areas.Admin.Controllers
{
    public class HomeAdminController : Controller
    {
        private QLToyShopEntities db = new QLToyShopEntities();

        // GET: Admin/HomeAdmin
        public ActionResult Index()
        {
            if (Session["user"] == null)
            {
                return RedirectToAction("Login");
            }
            else
            {
                return View();
            }
        }

        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(string user, string password)
        {
            // Check db
            var account = db.Accounts.FirstOrDefault(m => m.Email.ToLower() == user.ToLower() && m.Password == password);
            // Check code
            if (account != null)
            {
                Session["user"] = account;
                ViewBag.user = user;
                // Thêm thông báo thành công vào TempData
                TempData["SuccessMessage"] = "Đăng nhập thành công";
                return RedirectToAction("Index");
            }
            else
            {
                // Thêm thông báo thất bại vào TempData
                TempData["ErrorMessage"] = "Tài khoản đăng nhập không đúng";
                return View();
            }
        }

        public ActionResult Logout()
        {
            Session.Remove("user");
            FormsAuthentication.SignOut();
            return RedirectToAction("Login");
        }
    }
}
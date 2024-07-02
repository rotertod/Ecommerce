using Ecommerce.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Ecommerce.Controllers
{
    public class AccountController : Controller
    {
        private QLToyShopEntities db = new QLToyShopEntities();

        // GET: Account
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(string user, string password)
        {
            // Check db
            var customer = db.Customers.FirstOrDefault(m => m.Email.ToLower() == user.ToLower() && m.Password == password);
            // Check code
            if (customer != null)
            {
                Session["user"] = customer;
                ViewBag.user = user;
                // Thêm thông báo thành công vào TempData
                TempData["SuccessMessage"] = "Đăng nhập thành công";
                return RedirectToAction("Index", "Home");
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
            return RedirectToAction("Index", "Home");
        }

        public ActionResult Register()
        {

            ViewBag.LocationID = new SelectList(db.Locations, "LocationID", "NameWithType");
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register([Bind(Include = "CustomerID,FullName,Birthday,Avatar,Address,Email,Phone,LocationID,District,Ward,CreateDate,Password,Salt,LastLogin,Active")] Customer customer, string confirmPassword)
        {

            if (ModelState.IsValid)
            {

                if (customer.Password != (confirmPassword))
                {
                    ViewBag.LocationID = new SelectList(db.Locations, "LocationID", "NameWithType", customer.LocationID);
                    // Thêm thông báo thất bại vào TempData
                    TempData["ErrorMessage"] = "Vui lòng nhập mật khẩu giống nhau";
                    return View(customer);
                }
                if (db.Customers.Any(c => c.Email == customer.Email))
                {
                    // Thêm thông báo thất bại vào TempData
                    TempData["ErrorMessage"] = "Email bạn vừa nhập đã tồn tại";
                    ViewBag.LocationID = new SelectList(db.Locations, "LocationID", "NameWithType", customer.LocationID);
                    return View(customer);
                }

                db.Customers.Add(customer);
                customer.CreateDate = DateTime.Now;
                customer.Birthday = DateTime.Now;
                customer.Avatar = "default-avatar.jpg";
                if (customer.LocationID == null)
                {
                    customer.LocationID = 1;
                }
                db.SaveChanges();
                // Thêm thông báo thành công vào TempData
                TempData["SuccessMessage"] = "Đăng ký thành công";
                return RedirectToAction("Login");
            }

            ViewBag.LocationID = new SelectList(db.Locations, "LocationID", "NameWithType", customer.LocationID);

            return View(customer);
        }


        public ActionResult MyProfile(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer customer = db.Customers.Find(id);
            if (customer == null)
            {
                return HttpNotFound();
            }
            return View(customer);
        }

        public ActionResult EditProfile(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer customer = db.Customers.Find(id);
            if (customer == null)
            {
                return HttpNotFound();
            }
            ViewBag.LocationID = new SelectList(db.Locations, "LocationID", "NameWithType");

            return View(customer);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditProfile([Bind(Include = "CustomerID,FullName,Birthday,Avatar,Address,Email,Phone,LocationID,District,Ward,CreateDate,Password,Salt,LastLogin,Active")] Customer customer)
        {
            if (ModelState.IsValid)
            {
                db.Entry(customer).State = EntityState.Modified;
                customer.CreateDate = DateTime.Now;
                db.SaveChanges();
                // Thêm thông báo thành công vào TempData
                TempData["SuccessMessage"] = "Chỉnh sửa thành công";
                return RedirectToAction("Index", "Home");
            }
            // Thêm thông báo thất bại vào TempData
            TempData["ErrorMessage"] = "Chỉnh sửa thất bại";

            ViewBag.LocationID = new SelectList(db.Locations, "LocationID", "NameWithType", customer.LocationID);
            return View(customer);
        }
        public static string GetMD5(string str)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] fromData = Encoding.UTF8.GetBytes(str);
            byte[] targetData = md5.ComputeHash(fromData);
            string byte2String = null;

            for (int i = 0; i < targetData.Length; i++)
            {
                byte2String += targetData[i].ToString("x2");

            }
            return byte2String;
        }
    }
}

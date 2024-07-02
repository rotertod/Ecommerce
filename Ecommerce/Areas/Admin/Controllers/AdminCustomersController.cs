using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Ecommerce.Models;
using PagedList;

namespace Ecommerce.Areas.Admin.Controllers
{
    public class AdminCustomersController : Controller
    {
        private QLToyShopEntities db = new QLToyShopEntities();

        // GET: Admin/AdminCustomers
        public ActionResult Index(int? page)
        {
            int pageSize = 10; // Số lượng khách hàng trên mỗi trang
            ViewBag.PageSize = pageSize;

            int pageNumber = (page ?? 1);

            var customers = db.Customers.Include(a => a.Location);

            // Sắp xếp theo một trường (ví dụ: CustomerID)
            customers = customers.OrderBy(c => c.CustomerID);

            return View(customers.ToPagedList(pageNumber, pageSize));
        }

        // GET: Admin/AdminCustomers/Details/5
        public ActionResult Details(int? id)
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

        public bool CheckFunctions(int functionID)
        {
            // kiểm tra quyền
            // 1. Count phân quyền trong database
            Account accountSession = (Account)Session["user"];
            var count = db.Authorizations.Count(m => m.RoleID == accountSession.RoleID && m.FunctionID == functionID);
            if (count == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        // GET: Admin/AdminCustomers/Create
        public ActionResult Create()
        {
            if (CheckFunctions(1) == false)
            {
                // Thêm thông báo thất bại vào TempData
                TempData["ErrorMessage"] = "Bạn không có quyền thêm";
                return RedirectToAction("Index");
            }

            ViewBag.LocationID = new SelectList(db.Locations, "LocationID", "NameWithType");
            return View();
        }

        // POST: Admin/AdminCustomers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CustomerID,FullName,Birthday,Avatar,Address,Email,Phone,LocationID,District,Ward,CreateDate,Password,Salt,LastLogin,Active")] Customer customer)
        {
            if (ModelState.IsValid)
            {
                db.Customers.Add(customer);
                customer.CreateDate = DateTime.Now;
                db.SaveChanges();
                // Thêm thông báo thành công vào TempData
                TempData["SuccessMessage"] = "Thêm mới thành công";
                return RedirectToAction("Index");
            }
            // Thêm thông báo thất bại vào TempData
            TempData["ErrorMessage"] = "Thêm mới thất bại";

            ViewBag.LocationID = new SelectList(db.Locations, "LocationID", "NameWithType", customer.LocationID);

            return View(customer);
        }

        // GET: Admin/AdminCustomers/Edit/5
        public ActionResult Edit(int? id)
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
            if (CheckFunctions(2) == false)
            {
                // Thêm thông báo thất bại vào TempData
                TempData["ErrorMessage"] = "Bạn không có quyền sửa";
                return RedirectToAction("Index");
            }
            ViewBag.LocationID = new SelectList(db.Locations, "LocationID", "NameWithType");

            return View(customer);
        }

        // POST: Admin/AdminCustomers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CustomerID,FullName,Birthday,Avatar,Address,Email,Phone,LocationID,District,Ward,CreateDate,Password,Salt,LastLogin,Active")] Customer customer)
        {
            if (ModelState.IsValid)
            {
                db.Entry(customer).State = EntityState.Modified;
                customer.CreateDate = DateTime.Now;
                db.SaveChanges();
                // Thêm thông báo thành công vào TempData
                TempData["SuccessMessage"] = "Chỉnh sửa thành công";
                return RedirectToAction("Index");
            }
            // Thêm thông báo thất bại vào TempData
            TempData["ErrorMessage"] = "Chỉnh sửa thất bại";

            ViewBag.LocationID = new SelectList(db.Locations, "LocationID", "NameWithType", customer.LocationID);
            return View(customer);
        }

        // GET: Admin/AdminCustomers/Delete/5
        public ActionResult Delete(int? id)
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
            if (CheckFunctions(3) == false)
            {
                // Thêm thông báo thất bại vào TempData
                TempData["ErrorMessage"] = "Bạn không có quyền xóa";
                return RedirectToAction("Index");
            }

            return View(customer);
        }

        // POST: Admin/AdminCustomers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Customer customer = db.Customers.Find(id);
            db.Customers.Remove(customer);
            db.SaveChanges();
            // Thêm thông báo thành công vào TempData
            TempData["SuccessMessage"] = "Xóa thành công";
            return RedirectToAction("Index");
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

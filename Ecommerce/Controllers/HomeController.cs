using Ecommerce.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Ecommerce.Controllers
{
    public class HomeController : Controller
    {
        QLToyShopEntities db = new QLToyShopEntities();

        // GET: Home
        public ActionResult Index()
        {
            // Lấy danh sách sản phẩm có rating = 5
            var products = db.Products.Where(p => p.Ratting == 5).Take(5).ToList();

            // Truyền danh sách sản phẩm đến View
            ViewData["SanPham5Sao"] = products;

            // Lấy 2 sản phẩm có rating cao nhất từ danh mục "Sách Mới Nhất"
            var topCatToy = db.Products
                .Where(p => p.Category.CatName == "Marvel")
                .GroupBy(p => p.Category.CatName)
                .SelectMany(group => group.OrderByDescending(p => p.Ratting).Take(4))
                .ToList();

            // Truyền danh sách sản phẩm đến View
            ViewData["Marvel"] = topCatToy;

            // Lấy 2 sản phẩm có rating cao nhất từ danh mục "Sách Trẻ Em"
            var topCatChibi = db.Products
                .Where(p => p.Category.CatName == "Gundam")
                .GroupBy(p => p.Category.CatName)
                .SelectMany(group => group.OrderByDescending(p => p.Ratting).Take(4))
                .ToList();

            // Truyền danh sách sản phẩm đến View
            ViewData["Gundam"] = topCatChibi;

            // Lấy 2 sản phẩm có rating cao nhất từ danh mục "Sách Tri Thức"
            var topCatBottle = db.Products
                .Where(p => p.Category.CatName == "One Piece")
                .GroupBy(p => p.Category.CatName)
                .SelectMany(group => group.OrderByDescending(p => p.Ratting).Take(4))
                .ToList();

            // Truyền danh sách sản phẩm đến View
            ViewData["One Piece"] = topCatBottle;

            // Lấy 2 sản phẩm có rating cao nhất từ danh mục "Sách IT"
            var topCatWatch = db.Products
                .Where(p => p.Category.CatName == "Dragon Balls")
                .GroupBy(p => p.Category.CatName)
                .SelectMany(group => group.OrderByDescending(p => p.Ratting).Take(4))
                .ToList();

            // Truyền danh sách sản phẩm đến View
            ViewData["Dragon Balls"] = topCatWatch;

            ViewData["DanhMuc"] = new SelectList(db.Categories, "CatID", "CatName");

            return View();
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult Contact()
        {
            return View();
        }
    }
}
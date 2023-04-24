using DoAnCs.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DoAnCs.Controllers
{
    public class LoginSVController : Controller
    {
        // GET: LoginSV
        TracNghiemEntities1 db = new TracNghiemEntities1();
        [HttpGet]
        public ActionResult Login()
        {
            if (Session["TaiKhoanSV"] != null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }
        [HttpPost]
        public ActionResult Login(FormCollection f)
        {
            string semail = f["txtEmail"].ToString();
            string smatkhau = f["txtMatKhau"].ToString();
            Student sv = db.Students.SingleOrDefault(n => n.Email == semail && n.Password == smatkhau);
            if (sv != null)
            {
                ViewBag.thongbao = "Bạn đã đăng nhập thành công";
                Session["TaiKhoanSV"] = sv;
                ViewBag.thbao = ((Student)Session["TaiKhoanSV"]).Email;
                return RedirectToAction("Index", "Home");

            }
            ViewBag.thongbao = "Tên Email hoặc mật khẩu không đúng !";
            return View();
        }
        public ActionResult DangXuat()
        {

            Session.Clear(); // Xóa tất cả các session
            Session.Abandon(); // Hủy tất cả các session
            return RedirectToAction("Login", "LoginAd"); // Chuyển hướng đến login
        }
    }
}
using DoAnCs.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DoAnCs.Areas.Admin.Controllers
{
    public class LoginAdController : Controller
    {
        // GET: Admin/LoginAd
        TracNghiemEntities1 db = new TracNghiemEntities1();
        [HttpGet]
        public ActionResult Login()
        {
            if (Session["TaiKhoanGV"] != null)
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
            Lecturer gv = db.Lecturers.SingleOrDefault(n => n.Email == semail && n.Password == smatkhau);
            if (gv != null)
            {
                Session["FullName"] = gv.Name;
                Session["Addres"] = gv.Address;
                Session["Email"] = gv.Email;
                Session["Phone"] = gv.Phone;


                ViewBag.thongbao = "Bạn đã đăng nhập thành công";
                Session["TaiKhoanGV"] = gv;
                ViewBag.thbao = ((Lecturer)Session["TaiKhoanGV"]).Email;
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
        public ActionResult EmailPartial()
        {
            if (Session["TaiKhoanGV"] != null)
            {
                ViewBag.thbao = ((Lecturer)Session["TaiKhoanGV"]).Name;
            }
            else
            {
                ViewBag.thbao = "Admin";
            }

            return PartialView();
        }
    }
}
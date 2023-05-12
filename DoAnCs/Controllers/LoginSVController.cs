using DoAnCs.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using PagedList.Mvc;
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
                Session["Fullname"] = sv.Name;
                Session["Email"] = sv.Email;
                Session["Phone"] = sv.Phone;
                Session["Addrress"] = sv.Address;
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
            return RedirectToAction("Login", "LoginSV"); // Chuyển hướng đến login
        }
        public ActionResult LichSuThi(int? page)
        {
            if (Session["TaiKhoanSV"] == null)
            {
                return RedirectToAction("Login", "Account"); // Chuyển hướng đến trang đăng nhập nếu chưa đăng nhập
            }

            var student = (Student)Session["TaiKhoanSV"];
            var lichsudethi = db.Exam_Results.Where(e => e.IdStudent == student.IdStudent).ToList();

            int pageSize = 10;
            int pageNumber = (page ?? 1);
            var pagedLichSuDeThi = lichsudethi.ToPagedList(pageNumber, pageSize);

            ViewBag.PageSize = pageSize;
            ViewBag.Page = page;

            return View(pagedLichSuDeThi);
        }
    }
}
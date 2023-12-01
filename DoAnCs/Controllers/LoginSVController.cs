using DoAnCs.Models;
using PagedList;
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
            Student sv = db.Students.SingleOrDefault(n => n.Email == semail);
            if (sv != null)
            {
                sv.OnlineStd = true;
                db.SaveChanges();

                ViewBag.thongbao = "Bạn đã đăng nhập thành công";
                Session["TaiKhoanSV"] = sv;
                Session["Fullname"] = sv.Name;
                Session["OnlineS"] = sv.OnlineStd;
                Session["Online"] = true;

                ViewBag.thbao = ((Student)Session["TaiKhoanSV"]).Email;
                return RedirectToAction("Index", "Home");
            }
            ViewBag.thongbao = "Tên Email hoặc mật khẩu không đúng !";
            return View();
        }
        public ActionResult DangXuat()
        {
            if (Session["TaiKhoanSV"] != null)
            {
                Student sv = (Student)Session["TaiKhoanSV"];
                try
                {
                    sv.OnlineStd = false;
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Processing failed: {ex.Message}");
                    ViewBag.thongbao = "Bạn khong luu dc";
                }
                Session.Clear(); 
                Session.Abandon(); 
            }
            return RedirectToAction("Login", "LoginSV"); 
        }
        public ActionResult LichSuThi(int? page)
        {
            if (Session["TaiKhoanSV"] == null)
            {
                return RedirectToAction("Login", "Account"); 
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
        [HttpGet]
        public ActionResult Dangky()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Dangky(Student st)
        {
            if (st != null)
            {
                
                if (string.IsNullOrEmpty(st.Name) || string.IsNullOrEmpty(st.Email) || string.IsNullOrEmpty(st.Password) || string.IsNullOrEmpty(st.Address) || st.Phone == 0)
                {
                    ModelState.AddModelError("", "Vui lòng nhập đầy đủ thông tin đã cung cấp ở bên dưới.");
                    return View(st); 
                }

                if (ModelState.IsValid)
                {

                    var check = db.Students.FirstOrDefault(p => p.Name == st.Name);
                    if (check == null)
                    {

                        //st.Password = PasswordHasher.HashPassword(st.Password);
                        db.Students.Add(st);
                        db.SaveChanges();
                        return RedirectToAction("Login", "LoginSV");

                    }
                    else
                    {
                        string message = "Tên Người Dùng Đã Tồn Tại!";
                        string script = "alert('" + message + "');";
                        return Content("<script type='text/javascript'>" + script + "window.location.href='/LoginSV/Dangky';</script>");

                    }
                }
            }
            ModelState.AddModelError("", "Dữ liệu không hợp lệ.");
            return View(st); 
        }


    }
}
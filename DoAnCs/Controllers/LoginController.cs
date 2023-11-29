using DoAnCs.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DoAnCs.Controllers
{
    public class LoginController : Controller
    {
        private TracNghiemEntities1 db = new TracNghiemEntities1();
        public ActionResult Login()
        {
            var model = new LoginViewModel1
            {
                Student = new Student(),
                Lecturer = new Lecturer()
            };
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]

        public ActionResult Login(LoginViewModel1 model)
        {

            var student = db.Students.SingleOrDefault(s => s.Name == model.Student.Name && s.Password == model.Student.Password);
            if (student != null)
            {
                // Đăng nhập thành công, lưu thông tin vào session
                Session["Name"] = student.Name;
                Session["Email"] = student.Email;
                Session["Phone"] = student.Phone;
                Session["Address"] = student.Address;
                return RedirectToAction("Index", "Home");
            }

            // Kiểm tra đăng nhập của Lecturer
            var lecturer = db.Lecturers.SingleOrDefault(l => l.Name == model.Lecturer.Name && l.Password == model.Lecturer.Password);
            if (lecturer != null)
            {
                // Đăng nhập thành công, lưu thông tin vào session
                Session["Name"] = lecturer.Name;
                Session["Email"] = lecturer.Email;
                Session["Phone"] = lecturer.Phone;
                Session["Address"] = lecturer.Address;
                return RedirectToAction("Index", "Home", new { area = "Admin" });
            }

            // Đăng nhập không thành công, hiển thị thông báo lỗi
            ModelState.AddModelError("", "Đăng nhập không thành công");
            return View(model);

        }
        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(Student usr)
        {
            if (ModelState.IsValid)
            {
                var check = db.Students.FirstOrDefault(p => p.Name == usr.Name);
                if (check == null)
                {
                    db.Students.Add(usr);
                    db.SaveChanges();
                    return RedirectToAction("Login", "Login");
                }
                else
                {
                    string message = "Tên Người Dùng Đã Tồn Tại!";
                    string script = "alert('" + message + "');";
                    return Content("<script type='text/javascript'>" + script + "window.location.href='/login/Register';</script>");
                  
                }
            }
            return View();
        }



        public ActionResult Logout()
        {
            Session.Remove("Name");
            Session.Remove("Email");
            Session.Remove("Phone");
            Session.Remove("Address");
       
            return RedirectToAction("Login", "Login");
        }
        [HttpGet]
        public ActionResult ChitietDeThi(int? id)
        {
            var exem = db.Exams.FirstOrDefault(x => x.IdExam == id);
            return View(exem);
        }
    }

}
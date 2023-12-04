using DoAnCs.Models;
using DoAnCs.Models.Viewmodel;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DoAnCs.Areas.Admin.Controllers
{
    public class ThongKeController : Controller
    {
        private TracNghiemEntities1 db = new TracNghiemEntities1();

        public ActionResult Index()
        {
            // Lấy ra tháng hiện tại
            DateTime today = DateTime.Today;

            // Đếm số lượng kỳ thi trong tháng hiện tại
            int examCountInCurrentMonth = db.Exam_Results
                .Count(x => x.ExamDay != null && x.ExamDay.Value.Month == today.Month && x.ExamDay.Value.Year == today.Year);

            ViewBag.ExamCountInCurrentMonth = examCountInCurrentMonth;

         

            // Đếm số lượng kỳ thi trong tháng hiện tại
            int DethiCountInCurrentMonth = db.Exams
                .Count(x => x.Exam_date != null && x.Exam_date.Value.Month == today.Month && x.Exam_date.Value.Year == today.Year);

            ViewBag.DethiCountInCurrentMonth = DethiCountInCurrentMonth;





            int sosinhvienthangnay = db.AspNetUsers
                .Count(x => x.CreateDate != null && x.CreateDate.Value.Month == today.Month && x.CreateDate.Value.Year == today.Year);

            ViewBag.Sosinhvienthangnay = sosinhvienthangnay; // Gửi số lượng sinh viên đến View



            return View();
        }
    }
}
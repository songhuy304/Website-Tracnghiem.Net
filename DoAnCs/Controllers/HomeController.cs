using DoAnCs.Controllers.AuthenClient;
using DoAnCs.Models;
using DoAnCs.Models.Viewmodel;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DoAnCs.Controllers
{
    public class HomeController : Controller
    {
        public TracNghiemEntities1 db = new TracNghiemEntities1();
        [AllowAnonymous]
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }
        public ActionResult Refresh()
        {
            var item = new ThongKeModel();

            ViewBag.Visitors_online = HttpContext.Application["visitors_online"];
            var hn = HttpContext.Application["HomNay"];
            item.HomNay = HttpContext.Application["HomNay"].ToString();
            item.HomQua = HttpContext.Application["HomQua"].ToString();
            item.TuanNay = HttpContext.Application["TuanNay"].ToString();
            item.TuanTruoc = HttpContext.Application["TuanTruoc"].ToString();
            item.ThangNay = HttpContext.Application["ThangNay"].ToString();
            item.ThangTruoc = HttpContext.Application["ThangTruoc"].ToString();
            item.TatCa = HttpContext.Application["TatCa"].ToString();
            return PartialView(item);
        }
        [HttpGet]

        [ClientAuthen("User", "Admin")]

        public ActionResult detail(int id)
        {

            var exam = db.Exams
                 .Where(e => e.IdExam == id)
                 .Select(e => new { e.IdExam, e.NameExam, e.CreateBy, e.Exam_date, e.image, e.NumberQ, e.Time })
                 .FirstOrDefault();
            if (exam == null)
            {
                return Json(new { error = "Exam not found" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { exam = exam }, JsonRequestBehavior.AllowGet);
        }
        [ClientAuthen("User", "Admin")]

        public ActionResult ReviewExam(int id)
        {

            var examReview = db.Reviews.Where(x => x.ExamId == id).ToList().Take(5);
            if (examReview == null)
            {
                return Json(new { error = "Exam not found" }, JsonRequestBehavior.AllowGet);
            }

            // Chuyển đổi DateTime sang chuỗi định dạng dd/MM/yyyy HH:mm:ss (hoặc định dạng mong muốn)
            var formattedReviews = examReview.Select(review => new
            {
                CreateDate = review.CreateDate.HasValue
                ? review.CreateDate.Value.ToString("dd/MM/yyyy") // Chuyển đổi ngày thành chuỗi định dạng dd/MM/yyyy nếu giá trị không null
                : "N/A",
                userName = review.userName,
                Description = review.Description,
                Rate = review.Rate,
                Id = review.id

            });

            return Json(new { examReview = formattedReviews }, JsonRequestBehavior.AllowGet);
        }
        [ClientAuthen("User", "Admin")]
        public ActionResult partialModalReview()
        {
            return PartialView();
        }
        [ClientAuthen("User", "Admin")]
        public ActionResult AddReview(Review rq)
        {
            {
                if (ModelState.IsValid)
                {
                    var userName = User.Identity.GetUserName();

                    // Check if ExamId or userName is null or empty
                    if (string.IsNullOrEmpty(userName) || rq.ExamId <= 0)
                    {
                        return Json(new { msg = "Lỗi: ExamId hoặc userName không hợp lệ.", success = false }, JsonRequestBehavior.AllowGet);
                    }

                    var review = new Review()
                    {
                        userName = userName,
                        CreateDate = DateTime.Now,
                        Email = rq.Email,
                        ExamId = rq.ExamId,
                        Description = rq.Description,
                        Rate = rq.Rate
                    };
                    db.Reviews.Add(review);
                    db.SaveChanges();
                    return Json(new { msg = "Đánh Giá Thành Công", success = true }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { msg = "Lỗi", success = false }, JsonRequestBehavior.AllowGet);

            }
        }
    }
}
using DoAnCs.Controllers.AuthenClient;
using DoAnCs.Models;
using DoAnCs.Models.Viewmodel;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;
using System.Windows.Input;

namespace DoAnCs.Controllers
{

    public class DeThiController : Controller
    {
        // GET: DeThi
        private TracNghiemEntities1 db = new TracNghiemEntities1();
        [AllowAnonymous]
        public ActionResult Index(string currentFilter, int? page, string searchString, int? idSubject)
        {
            var item = new List<Exam>();
            if (searchString != null) /*nếu ô tìm kiếm bằng khác null  thì bắt đầu từ  page  1*/
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            if (!string.IsNullOrEmpty(searchString))
            {
                //lấy ds sản phẩm theo từ khóa
                item = db.Exams.Where(x => x.NameExam.Contains(searchString) || searchString == null).ToList();
            }
            else
            {

                item = db.Exams.ToList();
            }
            if (idSubject != null)
            {
                item = db.Exams.Where(x => x.IdSubject == idSubject).ToList();
            }
            ViewBag.CurrentFilter = searchString;
            int pageIndex = (page ?? 1);
            int pagesize = 10; /*số lượng item của trang = 5*/
            item = item.Where(n => n.isactive).ToList();
            ViewBag.PageSize = pagesize;
            ViewBag.Page = page;
            return View(item.ToPagedList(pageIndex, pagesize));
        }
        [ClientAuthen("User", "Admin")]

        public JsonResult Chitiet(int? id)
        {
            if (id == null)
            {
                return Json(new { error = "Bad Request" }, JsonRequestBehavior.AllowGet);
            }

            var exam = db.Exams.Find(id);
            if (exam == null)
            {
                return Json(new { error = "Exam not found" }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { exam = exam }, JsonRequestBehavior.AllowGet);
        }
        [ClientAuthen("User", "Admin")]

        public ActionResult PartiaviewDeThiPhoBien()
        {
            var item = db.Exams.OrderByDescending(x=>x.Viewcount).Take(5).ToList();
            return View(item);
        }
        [HttpGet]
        [ClientAuthen("User", "Admin")]

        public ActionResult DeThi(int? id, FormCollection form)
        {
            List<Question> questions;
            if (Session["cauhoi"] == null || Session["bathiId"] == null || (int)Session["bathiId"] != id)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Exam exam = db.Exams.Find(id);
                if (exam == null)
                {
                    return HttpNotFound();
                }
                if (exam != null)
                {
                    exam.Viewcount = exam.Viewcount + 1;
                    db.SaveChanges();
                }

                var questionIds = db.Question_Exam.Where(mapping => mapping.IdExam == exam.IdExam).Select(mapping => mapping.IdQuestion).ToList();
                // Sử dụng một seed ngẫu nhiên để tránh việc lặp lại kết quả ngẫu nhiên
                var seed = Guid.NewGuid().GetHashCode();
                var random = new Random(seed);
                var randomQuestionIds = questionIds.OrderBy(q => random.Next()).Take((int)exam.NumberQ).ToList();
                questions = db.Questions.Where(q => randomQuestionIds.Contains(q.IdQuestion)).OrderBy(q => Guid.NewGuid()).ToList();
                foreach (var question in questions)
                {
                    question.AnswerList = JsonConvert.DeserializeObject<List<SubjectItem>>(question.answer);
                }




                Session["cauhoi"] = questions;
                Session["bathiId"] = id;
            }
            else
            {
                questions = (List<Question>)Session["cauhoi"];
            }

            Session["bathi"] = db.Exams.Find(id);
            ViewBag.Exam = Session["bathi"];
            ViewBag.sophut = ((Exam)Session["bathi"]).Time;
            ViewBag.tenbaithi = ((Exam)Session["bathi"]).NameExam;
            ViewBag.socauhoi = ((Exam)Session["bathi"]).NumberQ;
            return View(questions);
        }
        [ClientAuthen("User", "Admin")]

        [HttpPost]
        public ActionResult DeThi(List<QuestionAnswer> examResults, FormCollection form, int? socaudung, string thoigianthi)
        {
            try
            {
                if (socaudung == null || examResults == null || Session["bathi"] == null)
                {
                    // Trả lỗi nếu thiếu thông tin cần thiết.
                    return Json(new { error = "Lỗi: Thiếu thông tin cần thiết." });
                }

                var exam = (Exam)Session["bathi"];
                //var student = (Student)Session["TaiKhoanSV"];

                // Lấy thời gian thi
                TimeSpan thoigianthiStr = TimeSpan.Parse(thoigianthi);
                TimeSpan ThoiGian = TimeSpan.FromMinutes(exam.Time);
                TimeSpan ketQua = ThoiGian - thoigianthiStr;

                //Tính thời gian đã thi
                string ketQuaString = ketQua.ToString(@"hh\:mm\:ss");
                // Tính Điểm 
                double TongDiem = (double)socaudung * (100.0 / exam.NumberQ);
                TongDiem = Math.Floor(TongDiem * 2) / 2;
                decimal diemDecimal = Convert.ToDecimal(TongDiem);
                if (TongDiem < 0 || TongDiem > 100)
                {
                    // Trả lỗi nếu điểm số không hợp lệ.
                    return Json(new { error = "Lỗi: Điểm số không hợp lệ." });
                }

                string json = JsonConvert.SerializeObject(examResults);
                // Lấy ra id Người dùng
                var userId = User.Identity.GetUserId();

                var examResult = new Exam_Results
                {
                    IdExam = exam.IdExam,
                    //IdStudent = student.IdStudent,
                    IdUser = userId,
                    KetQuaThi = json,
                    Score = diemDecimal,
                    Time = ketQuaString,
                    ExamDay = DateTime.Now
                };

                db.Exam_Results.Add(examResult);
                db.SaveChanges();

                int idResult = examResult.IdResult;
                Session.Remove("bathi");
                Session.Remove("cauhoi");
                Session.Remove("bathiId");
                Session.Remove("ThongTinKyThiCuaSinhVien");
                return Json(new { redirectTo = Url.Action("ketquathi", new { id = idResult, showReviewModal = true }) });
            }
            catch (Exception ex)
            {
                // Trả lỗi nếu xảy ra lỗi không mong muốn.
                TempData["ErrorMessage"] = "Lỗi: " + ex.Message;
                return View();
            }
        }
        [ClientAuthen("User", "Admin")]

        public ActionResult ketquathi(int? id)
        {
            var idn = db.Exam_Results.FirstOrDefault(x => x.IdResult == id);
            if (idn != null)
            {
                var questionData = JsonConvert.DeserializeObject<List<QuestionAnswer>>(idn.KetQuaThi);
                ViewBag.questionData = questionData;
                ViewBag.TenBaiThi = idn.Exam.NameExam;
                ViewBag.Score = idn.Score;
                ViewBag.idBaiThi =idn.IdExam;
                ViewBag.Time = idn.Time;
                ViewBag.SocauHoiKetQua = idn.Exam.NumberQ;
                ViewBag.ThoiGian = idn.Exam.Time;
                return View(questionData);
            }

            return View();
        }

        [AllowAnonymous]

        [HttpGet]
        public ActionResult modalconfirmThi(int id)
        {
            var exam = db.Exams.FirstOrDefault(e => e.IdExam == id);
            if (exam != null)
            {
                return Json(new { exam = exam });
            }
            return Json(new { error = "Không tìm thấy bài kiểm tra" });
        }

        [AllowAnonymous]
        public ActionResult PartialView_sidebar()
        {
            var items = db.Subjects.ToList();
            return PartialView( items);
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
    }
}
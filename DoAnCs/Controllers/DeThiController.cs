using DoAnCs.Models;
using DoAnCs.Models.Viewmodel;
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
    public class DeThiController : KtraLoginSVController
    {
        // GET: DeThi
        private TracNghiemEntities1 db = new TracNghiemEntities1();

        public ActionResult Index(string currentFilter, int? page, string searchString ,int? idSubject)
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
            if(idSubject != null)
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
        [HttpGet]

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

                var student = (Student)Session["TaiKhoanSV"];
                if (student != null)
                {
                    Session["ThongTinKyThiCuaSinhVien"] = exam.NameExam;
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

        [HttpPost]
        public ActionResult DeThi(List<QuestionAnswer> examResults, FormCollection form, int? socaudung , string thoigianthi)
        {
            try
            {
                if (socaudung == null || examResults == null || Session["bathi"] == null || Session["TaiKhoanSV"] == null)
                {
                    // Trả lỗi nếu thiếu thông tin cần thiết.
                    return Json(new { error = "Lỗi: Thiếu thông tin cần thiết." });
                }

                var exam = (Exam)Session["bathi"];
                var student = (Student)Session["TaiKhoanSV"];


                TimeSpan thoigianthiStr = TimeSpan.Parse(thoigianthi);
                TimeSpan ThoiGian = TimeSpan.FromMinutes(exam.Time);
                TimeSpan ketQua = ThoiGian - thoigianthiStr;


                string ketQuaString = ketQua.ToString(@"hh\:mm\:ss");

                double TongDiem = (double)socaudung * (100.0 / exam.NumberQ);
                TongDiem = Math.Floor(TongDiem * 2) / 2;
                decimal diemDecimal = Convert.ToDecimal(TongDiem);
                if (TongDiem < 0 || TongDiem > 100)
                {
                    // Trả lỗi nếu điểm số không hợp lệ.
                    return Json(new { error = "Lỗi: Điểm số không hợp lệ." });
                }

                string json = JsonConvert.SerializeObject(examResults);

                var examResult = new Exam_Results
                {
                    IdExam = exam.IdExam,
                    IdStudent = student.IdStudent,
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
                return Json(new { redirectTo = Url.Action("ketquathi", new { id = idResult }) });
            }
            catch (Exception ex)
            {
                // Trả lỗi nếu xảy ra lỗi không mong muốn.
                TempData["ErrorMessage"] = "Lỗi: " + ex.Message;
                return View();
            }
        }


        public ActionResult LichSuThi()
        {
            if (Session["TaiKhoanSV"] == null)
            {
                return RedirectToAction("Login", "Account"); 
            }
            var student = (Student)Session["TaiKhoanSV"];
            
            var lichsudethi  = db.Exam_Results.Where(e=> e.IdStudent == student.IdStudent).ToList();
            return View(lichsudethi);
        }
        public ActionResult ketquathi(int? id)
            { 
                var idn = db.Exam_Results.FirstOrDefault(x => x.IdResult == id);
                
            
                
                if (idn != null)
                {
                    var questionData = JsonConvert.DeserializeObject<List<QuestionAnswer>>(idn.KetQuaThi);
                    ViewBag.questionData = questionData;
                    ViewBag.TenBaiThi = idn.Exam.NameExam;
                    ViewBag.Score = idn.Score;
                    ViewBag.Time = idn.Time;
                    ViewBag.SocauHoiKetQua = idn.Exam.NumberQ;
                    ViewBag.ThoiGian = idn.Exam.Time;
                    return View(questionData);
                }

                return View();
            }
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
      
    }
   
}
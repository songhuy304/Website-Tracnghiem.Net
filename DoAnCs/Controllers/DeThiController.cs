using DoAnCs.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

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
            item = item.OrderByDescending(n => n.IdSubject).ToList();
            ViewBag.PageSize = pagesize;
            ViewBag.Page = page;
            return View(item.ToPagedList(pageIndex, pagesize));
        }
        [HttpGet]

        public ActionResult DeThi(int? id, FormCollection form)
        {

            

            if (Session["cauhoi"] != null && ((List<Question>)Session["cauhoi"]).Count > 0 && id != ((List<Question>)Session["cauhoi"])[0].IdExam)
            {
                Session.Remove("cauhoi");
            }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Exam exam = db.Exams.Find(id);
            if (exam == null)
            {
                return HttpNotFound();
            }
            List<Question> questions;
            if (Session["cauhoi"] == null)
            {
                questions = db.Questions.Where(q => q.IdExam == exam.IdExam).OrderBy(q => Guid.NewGuid()).Take((int)exam.NumberQ).ToList();
                // Lưu danh sách câu hỏi vào session
                Session["cauhoi"] = questions;
            }
            else
            {
                questions = (List<Question>)Session["cauhoi"];
            }
            // Sử dụng session để lưu thông tin bài thi
            Session["bathi"] = exam;
            ViewBag.Exam = exam;
            ViewBag.sophut = exam.Time;
            ViewBag.tenbaithi = exam.NameExam;
            ViewBag.socauhoi = exam.NumberQ;

            return View(questions);
        }
        [HttpPost]
        public ActionResult DeThi(FormCollection form)
        {

            var exam = (Exam)Session["bathi"];
            var student = (Student)Session["TaiKhoanSV"];
            //var questionss = (Session["cauhoi"] as IQueryable<Question>).ToList();
            var questionss = (Session["cauhoi"] as List<Question>) ?? new List<Question>();
            int numQuestions = questionss.Count;
            int numCorrectAnswers = 0;

            foreach (var question in questionss)
            {

                var answer = HttpUtility.HtmlDecode(form["question-" + question.IdQuestion]);

                if (answer == question.DapAn)
                {
                    numCorrectAnswers++;
                }
            }
            double score = Math.Round((double)numCorrectAnswers / numQuestions * 10, 2);
            var examResult = new Exam_Results
            {
                IdExam = exam.IdExam,
                IdStudent = student.IdStudent,
                Score = (decimal?)score,
                ExamDay = DateTime.Now
            };
            db.Exam_Results.Add(examResult);
            db.SaveChanges();
            return RedirectToAction("index", "DeThi");
        }


        public ActionResult LichSuThi()
        {
            if (Session["TaiKhoanSV"] == null)
            {
                return RedirectToAction("Login", "Account"); // Chuyển hướng đến trang đăng nhập nếu chưa đăng nhập
            }
            var student = (Student)Session["TaiKhoanSV"];
            
            var lichsudethi  = db.Exam_Results.Where(e=> e.IdStudent == student.IdStudent).ToList();
            return View(lichsudethi);
        }
    }
   
}
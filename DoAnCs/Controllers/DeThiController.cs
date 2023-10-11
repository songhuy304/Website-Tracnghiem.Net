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

                questions = db.Questions.Where(q => questionIds.Contains(q.IdQuestion)).OrderBy(q => Guid.NewGuid()).Take((int)exam.NumberQ).ToList();

             
                Session["cauhoi"] = questions;

            
                Session["bathiId"] = id;
            }
            else
            {
               
                questions = (List<Question>)Session["cauhoi"];
            }

            Session["bathi"] = db.Exams.Find(id);

            foreach (var question in questions)
            {
                List<SubjectItem> subjectItems = JsonConvert.DeserializeObject<List<SubjectItem>>(question.answer);
                ViewBag.QuestionItems = subjectItems;
            }


            ViewBag.Exam = Session["bathi"];
            ViewBag.sophut = ((Exam)Session["bathi"]).Time;
            ViewBag.tenbaithi = ((Exam)Session["bathi"]).NameExam;
            ViewBag.socauhoi = ((Exam)Session["bathi"]).NumberQ;

            return View(questions);
        }



        [HttpPost]
        public ActionResult DeThi(FormCollection form )
        {

            var exam = (Exam)Session["bathi"];
            var student = (Student)Session["TaiKhoanSV"];
         
          

         
           
            var examResult = new Exam_Results
            {
                IdExam = exam.IdExam,
                IdStudent = student.IdStudent,
                //Score = (decimal?)score,
                ExamDay = DateTime.Now
            };
            db.Exam_Results.Add(examResult);
            db.SaveChanges();
            return View();
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
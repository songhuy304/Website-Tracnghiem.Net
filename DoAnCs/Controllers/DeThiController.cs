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

        public ActionResult Index(string currentFilter, int? page, string searchString)
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
            ViewBag.CurrentFilter = searchString;
            int pageIndex = (page ?? 1);
            int pagesize = 10; /*số lượng item của trang = 5*/
            item = item.OrderByDescending(n => n.IdSubject).ToList();
            ViewBag.PageSize = pagesize;
            ViewBag.Page = page;
            return View(item.ToPagedList(pageIndex, pagesize));
        }


        public ActionResult DeThi(int? id, FormCollection form)
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

            var questions = db.Questions.Where(q => q.IdSubject == exam.IdSubject).OrderBy(q => Guid.NewGuid()).Take((int)exam.NumberQ);
            Session["bathi"] = exam;
            Session["cauhoi"] = questions;
            ViewBag.Exam = exam;
            ViewBag.sophut = exam.Time;
            ViewBag.tenbaithi = exam.NameExam;
            ViewBag.socauhoi = exam.NumberQ;
            if (exam == null)
            {
                return HttpNotFound();
            }
            return View(questions);
        }
        [HttpPost]
        public ActionResult DeThi(FormCollection form)
        {

            var exam = (Exam)Session["bathi"];
            var student = (Student)Session["TaiKhoanSV"];
            var questionss = (Session["cauhoi"] as IQueryable<Question>).ToList();

            foreach (var question in questionss)
            {
                var answer = form["question-" + question.IdQuestion];



                // Kiểm tra xem bản ghi đã tồn tại hay chưa
                var result = db.Exam_Results.SingleOrDefault(r => r.IdStudent == student.IdStudent
                                                                      && r.IdExam == exam.IdExam
                                                                      && r.IdQuestion == question.IdQuestion);

                if (result == null)
                {
                    // Chưa có bản ghi nào, thêm mới
                    result = new Exam_Results
                    {
                        IdExam = exam.IdExam,
                        IdStudent = student.IdStudent,
                        IdQuestion = question.IdQuestion,
                        Answer_student = answer
                    };
                    db.Exam_Results.Add(result);
                }
                else
                {
                    // Đã có bản ghi, cập nhật lại giá trị đáp án của sinh viên
                    result.Answer_student = answer;
                }
            }

            db.SaveChanges();

            return View();
        }


    }
}
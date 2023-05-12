using DoAnCs.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DoAnCs.Areas.Admin.Controllers
{
    public class DeThiController : KtraLoginAdController
    {
        TracNghiemEntities1 db = new TracNghiemEntities1();
        // GET: Admin/BaiThi
        public ActionResult Index()
        {
            var item = db.Exams.ToList();
            return View(item);
        }
        [HttpGet]
        public ActionResult Add()
        {
            ViewBag.IdSubject = new SelectList(db.Subjects, "IdSubject", "NameSubject");
            return View();


        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(Exam exam)
        {
            if (ModelState.IsValid)
            {
                exam.Exam_date = DateTime.Now;
                db.Exams.Add(exam);
                db.SaveChanges();



                return RedirectToAction("Index");
            }
            ViewBag.IdSubject = new SelectList(db.Subjects, "IdSubject", "NameSubject");

            return View(exam);
        }
        public ActionResult edit(int id)
        {
            ViewBag.IdSubject = new SelectList(db.Subjects, "IdSubject", "NameSubject");

            var exem = db.Exams.Find(id);
            return View(exem);
        }
        [HttpPost]
        public ActionResult edit(Exam exem)

        {


            if (ModelState.IsValid)
            {
                db.Exams.Attach(exem);
                db.Entry(exem).State = System.Data.Entity.EntityState.Modified;

                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.IdSubject = new SelectList(db.Subjects, "IdSubject", "NameSubject");

            return View(exem);
        }
        //Xóa Không load trang khác  

        public ActionResult Delete(int? id)
        {
            var exem = db.Exams.Find(id);


            db.Exams.Remove(exem);
            db.SaveChanges();
            return RedirectToAction("Index", "DeThi");

        }

        //thêm cau hỏi trực tiếp zo bài thi
        public ActionResult Addquestion()
        {
            return View();
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Addquestion(int id, Question question)
        {
            var exam = db.Exams.Find(id);
            if (exam != null)
            {
                //mã hóa nội dung
                question.Contentt = HttpUtility.HtmlEncode(question.Contentt);
                // Gán IdExam cho question
                question.IdExam = id;

                // Thêm question vào bài thi
                exam.Questions.Add(question);             
                db.SaveChanges();
            }

            return RedirectToAction("Index", "DeThi");
        }
       
       


    }
}
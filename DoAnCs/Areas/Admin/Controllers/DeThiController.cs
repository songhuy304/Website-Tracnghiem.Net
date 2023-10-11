using DoAnCs.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json; // Cần thư viện Newtonsoft.Json
using System.Net;
using PagedList;
using DoAnCs.Models.Viewmodel;

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

                var lectured = (Lecturer)Session["TaiKhoanGV"];
                exem.ModifierDate = DateTime.Now;
                exem.MordifierBy = lectured.Name;
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


        [HttpGet]

        public ActionResult Partial_CauHoi123()

        {

            var item = db.Questions.ToList();


            return PartialView(item);
        }

        [HttpGet]

        public ActionResult Partial_CauHoi(int? idExam , int? page)

        {

            var item = db.Questions.ToList();


            int pageIndex = (page ?? 1);
            int pagesize = 5;

            var pagedItems = item.ToPagedList(pageIndex, pagesize);

            ViewBag.PageSize = pagesize;
            ViewBag.Page = page;
            ViewBag.TotalPages = pagedItems.PageCount; // Đưa thông tin số trang vào ViewBag

            return PartialView(pagedItems);
        }
        [HttpGet]
        public ActionResult Chitiet(int? id)
        {

                List<Question> questions;

            

                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                 Exam exam = db.Exams.Find(id);
                if (exam == null)
                {
                    return HttpNotFound();
                }
                else
                {
                     var questionIds = db.Question_Exam.Where(mapping => mapping.IdExam == exam.IdExam).Select(mapping => mapping.IdQuestion).ToList();
                     questions = db.Questions.Where(q => questionIds.Contains(q.IdQuestion)).ToList();
               
                }
                 Session["cauhoi"] = questions;
                 Session["bathiId"] = id;
                 Session["bathi"] = db.Exams.Find(id);
            return View(questions);
        }

        public ActionResult XoaCauHoi1(int? examId, int? questionId)
        {
            if (examId == null || questionId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Exam exam = db.Exams.Find(examId);
            if (exam == null)
            {
                return HttpNotFound();
            }

            // Xác định câu hỏi cần xóa từ danh sách Session
            List<Question> questions = Session["cauhoi"] as List<Question>;

            // Tìm mapping trong bảng Question_Exam với "IdExam" và "IdQuestion" tương ứng
            var mappingToRemove = db.Question_Exam.FirstOrDefault(mapping => mapping.IdExam == examId && mapping.IdQuestion == questionId);

            if (mappingToRemove != null)
            {
                // Loại bỏ câu hỏi khỏi danh sách
                questions.RemoveAll(q => q.IdQuestion == questionId);
                // Cập nhật lại danh sách câu hỏi trong Session
                Session["cauhoi"] = questions;

                
                db.Question_Exam.Remove(mappingToRemove);
                db.SaveChanges();

               
                exam.NumberQ -= 1;
                db.SaveChanges();
            }

            // Redirect trở lại trang chi tiết đợt thi
            return RedirectToAction("Chitiet", new { id = examId });
        }




        public ActionResult setcauhoi(int? id, int[] questionIds) // Thêm tham số questionId
        {
            if (id.HasValue && questionIds != null && questionIds.Length > 0)
            {
                foreach (int questionId in questionIds)
                {
                    var exam = new Question_Exam()
                    {
                        IdExam = id.Value,
                        IdQuestion = questionId
                    };
                    db.Question_Exam.Add(exam);
                }
                db.SaveChanges();
            }

            return RedirectToAction("Index");
        }



        [HttpGet]
        public ActionResult Addtest()
        {
            ViewBag.datetimnow = DateTime.Now.ToString("yyyy-MM-dd");
            ViewBag.IdSubject = new SelectList(db.Subjects, "IdSubject", "NameSubject");
            return View();

        }



        [HttpPost]       
        public ActionResult AddExamWithQuestions(Exam exam, int[] questionIds)
        {
            if (ModelState.IsValid/* && questionIds != null && questionIds.Length > 0*/)
            
                exam.Exam_date = DateTime.Now;
                exam.ModifierDate = DateTime.Now;
                var lectur = (Lecturer)Session["TaiKhoanGV"];
                exam.CreateBy = lectur.Name;
                db.Exams.Add(exam);
                db.SaveChanges();

                int examId = exam.IdExam;
                if (questionIds != null && questionIds.Length > 0)
                    {
                     foreach (int questionId in questionIds)
                        {
                          var questionExam = new Question_Exam()
                          {
                             IdExam = examId,
                             IdQuestion = questionId
                          };
                          db.Question_Exam.Add(questionExam);
                     }
                         db.SaveChanges();
                   return RedirectToAction("Index");
            }




            ViewBag.datetimnow = DateTime.Now.ToString("yyyy-MM-dd");


            ViewBag.IdSubject = new SelectList(db.Subjects, "IdSubject", "NameSubject");
            return View(exam);
        }
        [HttpPost]
        public ActionResult Isactive (int id)
        {
            var item = db.Exams.Find(id);
            if(item != null)
            {
                item.isactive = !item.isactive;
                db.Entry(item).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return Json(new { success = true, Isactive = item.isactive });
            }
            return Json(new { success = false });
        }
        [HttpPost]
        public ActionResult CapNhatNumberQ(int id)
        {
            var item = db.Exams.Find(id);
            if (item != null)
            {
                item.isactive = !item.isactive;
                db.Entry(item).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return Json(new { success = true, Isactive = item.isactive });
            }
            return Json(new { success = false });
        }


    }
}
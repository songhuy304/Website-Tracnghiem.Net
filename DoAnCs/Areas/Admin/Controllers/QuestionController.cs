using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using DoAnCs.Models;
using PagedList;

namespace DoAnCs.Areas.Admin.Controllers
{
    public class QuestionController : Controller
    {
        private TracNghiemEntities1 db = new TracNghiemEntities1();

        // GET: Admin/Question
        public ActionResult Index(string currentFilter, int? page, string searchString)
        {
            var item = new List<Question>();


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
                item = db.Questions.Where(x => x.Contentt.Contains(searchString) || searchString == null).ToList();

            }
            else
            {

                item = db.Questions.ToList();
            }
            ViewBag.CurrentFilter = searchString;
            int pageIndex = (page ?? 1);
            int pagesize = 10; /*số lượng item của trang = 5*/
            item = item.OrderByDescending(n => n.IdExam).ToList();
            ViewBag.PageSize = pagesize;
            ViewBag.Page = page;
            return View(item.ToPagedList(pageIndex, pagesize));



        }
        public ActionResult Create()
        {
            ViewBag.IdDifficulty = new SelectList(db.Difficulties, "IdDifficulty", "NameDifficulty");

            ViewBag.Ans = new SelectList(db.Answers.ToList(), "IdAnswer", "DapAn");

            ViewBag.IdExam = new SelectList(db.Exams, "IdExam", "NameExam");
            return View();
        }


        [HttpPost]
        [ValidateInput(false)]
        //[ValidateAntiForgeryToken]
        public ActionResult Create(Question question)
        {
            if (ModelState.IsValid)
            {
                question.Contentt = HttpUtility.HtmlEncode(question.Contentt);
                db.Questions.Add(question);
                db.SaveChanges();
                return RedirectToAction("index");
            }
            ViewBag.IdDifficulty = new SelectList(db.Difficulties, "IdDifficulty", "NameDifficulty");

            
            ViewBag.IdExam = new SelectList(db.Exams, "IdExam", "NameExam");
            return View(question);
        }

        // GET: Admin/Question/Edit/5

        [HttpGet]
       
        public ActionResult edit(int id)
        {
            var cauhoi = db.Questions.SingleOrDefault(n => n.IdQuestion == id);
            if (cauhoi == null)
            {
                Response.StatusCode = 404;
                return null;
            }

            ViewBag.IdDifficulty = new SelectList(db.Difficulties, "IdDifficulty", "NameDifficulty");

            ViewBag.IdExam = new SelectList(db.Exams, "IdExam", "NameExam");

            return View(cauhoi);
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult edit(Question question)

        {

            
            if (ModelState.IsValid)
            {
                question.Contentt = HttpUtility.HtmlEncode(question.Contentt);

                db.Entry(question).State = System.Data.Entity.EntityState.Modified;
                //cauhoi.Contentt = question.Contentt;
                db.SaveChanges();
                
            }
            return RedirectToAction("Index");

            
        }
        //Xóa Không load trang khác  

        public ActionResult Delete(int? id)
        {

            var question = db.Questions.Find(id);

            db.Questions.Remove(question);
            db.SaveChanges();
            return RedirectToAction("Index", "Question");

        }
        public ActionResult detail(int? id)
        {
            var question = db.Questions.SingleOrDefault(m => m.IdQuestion == id);
            ViewBag.Id = question.IdQuestion;

            return View(question);
        }
    }


}

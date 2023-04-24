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
            item = item.OrderByDescending(n => n.IdSubject).ToList();
            ViewBag.PageSize = pagesize;
            ViewBag.Page = page;
            return View(item.ToPagedList(pageIndex, pagesize));



        }
        public ActionResult Create()
        {
            ViewBag.IdDifficulty = new SelectList(db.Difficulties, "IdDifficulty", "NameDifficulty");

            ViewBag.Ans = new SelectList(db.Answers.ToList(), "IdAnswer", "DapAn");

            ViewBag.IdSubject = new SelectList(db.Subjects, "IdSubject", "NameSubject");
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Question question)
        {
            if (ModelState.IsValid)
            {
                db.Questions.Add(question);
                db.SaveChanges();
                return RedirectToAction("index");
            }
            ViewBag.IdDifficulty = new SelectList(db.Difficulties, "IdDifficulty", "NameDifficulty");

            //ViewBag.Ans = new SelectList(db.Answers.ToList(), "IdAnswer", "DapAn");

            ViewBag.IdSubject = new SelectList(db.Subjects, "IdSubject", "NameSubject");
            return View(question);
        }

        // GET: Admin/Question/Edit/5

        [HttpGet]
        public ActionResult edit(int id)
        {
            var question = db.Questions.Find(id);
            ViewBag.IdDifficulty = new SelectList(db.Difficulties, "IdDifficulty", "NameDifficulty");

            //ViewBag.Ans = new SelectList(db.Answers.ToList(), "IdAnswer", "DapAn");

            ViewBag.IdSubject = new SelectList(db.Subjects, "IdSubject", "NameSubject");

            return View(question);
        }
        [HttpPost]
        public ActionResult edit(Question question)

        {


            if (ModelState.IsValid)
            {
                db.Questions.Attach(question);
                db.Entry(question).State = System.Data.Entity.EntityState.Modified;

                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(question);
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

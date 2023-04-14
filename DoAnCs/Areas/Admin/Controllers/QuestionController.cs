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

namespace DoAnCs.Areas.Admin.Controllers
{
    public class QuestionController : Controller
    {
        private TracNghiemEntities1 db = new TracNghiemEntities1();

        // GET: Admin/Question
        public ActionResult Index()
        {
    
            var questions = db.Questions.OrderByDescending(x=>x.IdSubject);
            return View(questions.ToList());
        }

        // GET: Admin/Question/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Question question = db.Questions.Find(id);
            if (question == null)
            {
                return HttpNotFound();
            }
            return View(question);
        }

        // GET: Admin/Question/Create
        public ActionResult Create()
        {
            ViewBag.IdDifficulty = new SelectList(db.Difficulties, "IdDifficulty", "NameDifficulty");
            
            ViewBag.Ans = new SelectList(db.Answers.ToList(), "IdAnswer", "DapAn");

            ViewBag.IdSubject = new SelectList(db.Subjects, "IdSubject", "NameSubject");
            return View();
        }

       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create( Question question)
        {
            if (ModelState.IsValid)
            {
                db.Questions.Add(question);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.IdDifficulty = new SelectList(db.Difficulties, "IdDifficulty", "NameDifficulty");

            ViewBag.Ans = new SelectList(db.Answers.ToList(), "IdAnswer", "DapAn");

            ViewBag.IdSubject = new SelectList(db.Subjects, "IdSubject", "NameSubject");
            return View(question);
        }

        // GET: Admin/Question/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Question question = db.Questions.Find(id);
            if (question == null)
            {
                return HttpNotFound();
            }
            ViewBag.IdDifficulty = new SelectList(db.Difficulties, "IdDifficulty", "NameDifficulty", question.IdDifficulty);
            ViewBag.IdSubject = new SelectList(db.Subjects, "IdSubject", "NameSubject", question.IdSubject);
            return View(question);
        }

        // POST: Admin/Question/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IdQuestion,Contentt,optionA,optionB,optionC,optionD,IdSubject,IdDifficulty")] Question question)
        {
            if (ModelState.IsValid)
            {
                db.Entry(question).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.IdDifficulty = new SelectList(db.Difficulties, "IdDifficulty", "NameDifficulty", question.IdDifficulty);
            ViewBag.Ans = new SelectList(db.Answers, "IdAnswer", "DapAn");

            ViewBag.IdSubject = new SelectList(db.Subjects, "IdSubject", "NameSubject", question.IdSubject);
            return View(question);
        }

        // GET: Admin/Question/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Question question = db.Questions.Find(id);
            if (question == null)
            {
                return HttpNotFound();
            }
            return View(question);
        }

        // POST: Admin/Question/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Question question = db.Questions.Find(id);
            db.Questions.Remove(question);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}

using DoAnCs.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DoAnCs.Areas.Admin.Controllers
{
    public class DeThiController : Controller
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

            return View(exam);
        }
    }
}
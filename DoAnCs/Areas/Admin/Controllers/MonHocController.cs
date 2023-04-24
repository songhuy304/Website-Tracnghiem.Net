using DoAnCs.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DoAnCs.Areas.Admin.Controllers
{
    public class MonHocController : KtraLoginAdController
    {
        TracNghiemEntities1 db = new TracNghiemEntities1();

        public ActionResult Index()
        {
            var item = db.Subjects.ToList();
            return View(item);

        }
        [HttpGet]
        public ActionResult Add()
        {
            
            return View();
            

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(Subject Subjects)
        {
            if (ModelState.IsValid)
            {

                db.Subjects.Add(Subjects);
                db.SaveChanges();



                return RedirectToAction("Index");
            }
            
            return View(Subjects);
        }
    }
}
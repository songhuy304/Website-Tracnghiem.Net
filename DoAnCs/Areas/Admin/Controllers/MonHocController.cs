using DoAnCs.Models;
using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DoAnCs.Models.Viewmodel;


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
        public ActionResult delete(int? id)
        {

            try
            {
                if (id != null)
                {
                    var sb = db.Subjects.Find(id);
                    db.Subjects.Remove(sb);
                    db.SaveChanges();
                    return RedirectToAction("Index", "MonHoc");
                }
            }
            catch(Exception e)
            {
                
            }

            return View();
        }
        
        public ActionResult Deleteall(string[] ids)
        {
            if (ids != null && ids.Length > 0)
            {
                foreach (var id in ids)
                {
                    var obj = db.Subjects.Find(Convert.ToInt32 (id));
                    if (obj != null)
                    {
                        db.Subjects.Remove(obj);
                    }
                }
                db.SaveChanges();
                return Json(new { success = true });
            }

            return Json(new { success = false });
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
        [HttpGet]
        public ActionResult addsubject()
        {
            // This is only for show by default one row for insert data to the database
            List<SubjectItem> ci = new List<SubjectItem> { new SubjectItem { } };
            return View(ci);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult addsubjectitem(List<SubjectItem> ci , string Namesubjectss)
        {
            if (ModelState.IsValid)
            {

                string json = JsonConvert.SerializeObject(ci);
                
                var subject = new Subject
                {
                    NameSubject = Namesubjectss,
                    textJson = json
                };

                db.Subjects.Add(subject);
                db.SaveChanges();
                ViewBag.Message = "Data successfully saved!";
                ModelState.Clear();
                ci = new List<SubjectItem> { new SubjectItem { } };
                return RedirectToAction ("Index");

            }
            return View(ci);
        }
    }
}
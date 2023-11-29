using DoAnCs.Models;
using NPOI.POIFS.Properties;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace DoAnCs.Areas.Admin.Controllers
{
    public class ParentTopicController : KtraLoginAdController
    {
        TracNghiemEntities1 db = new TracNghiemEntities1();
        // GET: Admin/BaiThi
        public ActionResult Index()
        {
            ViewBag.IdParent = new SelectList(db.ParentTopics, "IdParTopic", "NameParTopic");
            var parentTopics = db.ParentTopics.ToList();

            return View(parentTopics);
        }
        public ActionResult Themchudecha()
        {

            return PartialView();
        }
        public ActionResult Themchudecon()
        {

            return PartialView();
        }
        [HttpGet]
     
        public ActionResult Add()
        {
           
          
            return PartialView();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(string parentTopic)
        {
            if (ModelState.IsValid)
            {
                var subject = new ParentTopic
                {
                    NameParTopic = parentTopic
                };
                db.ParentTopics.Add(subject);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(parentTopic);
        }
    }
}
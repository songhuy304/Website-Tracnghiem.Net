using DoAnCs.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DoAnCs.Areas.Admin.Controllers
{
    public class topicController : Controller
    {
        TracNghiemEntities1 db = new TracNghiemEntities1();
        // GET: Admin/topic
        public ActionResult Index()
        {
            return View();
        }
      
        [HttpPost]
        public ActionResult themchude(ParentTopic parentTopic)
        {
            if (ModelState.IsValid)
            {
                db.ParentTopics.Add(parentTopic);
                db.SaveChanges();
                return Json(new { success= true }) ;
            }
            return Json(new { success = false });
        }
        public ActionResult themchudecon(Topic topic , int idchudecha)
        {
            if (ModelState.IsValid)
            {
                topic.idPartopic = idchudecha;
                db.Topics.Add(topic);
                db.SaveChanges();
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }
    }
}
using DoAnCs.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DoAnCs.Controllers
{
    public class MenuController : Controller
    {
        private TracNghiemEntities1 db = new TracNghiemEntities1(); 
        // GET: Menu
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Partial_Mon()
        {

           
                var items = db.Subjects.ToList();
                return PartialView("Partial_Mon", items);
            
        }
    }
}
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using DoAnCs.Models;
using PagedList;

namespace DoAnCs.Areas.Admin.Controllers
{
   

    public class UserController : Controller
    {
        // GET: Admin/User
        private TracNghiemEntities1 db = new TracNghiemEntities1();
        [HttpGet]
        public ActionResult Index()
        {
            var students = db.Students.ToList();
            var onlines = Session["Online"];
            var sortedStudents = students.OrderByDescending(s => s.OnlineStd).ToList();
           

            return View(sortedStudents);
        }
        
        

    }
}
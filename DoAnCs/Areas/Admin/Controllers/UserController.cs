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
        public ActionResult Index()

        {
            var item = db.Students.ToList();
            return View(item);
        }
    }
}
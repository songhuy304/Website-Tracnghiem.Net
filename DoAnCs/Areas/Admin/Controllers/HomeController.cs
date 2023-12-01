using DoAnCs.Areas.Admin.Controllers.customAuthen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace DoAnCs.Areas.Admin.Controllers
{
    [CustomAuthorize(Roles = "Admin")]

    public class HomeController : Controller
    {
        // GET: Admin/Home
        public ActionResult Index()
        {
            return View();
        }
    }
}
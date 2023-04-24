using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DoAnCs.Areas.Admin.Controllers
{
    public class KtraLoginAdController : Controller
    {
        // GET: Admin/KtraLoginAd
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (Session["TaiKhoanGV"] == null)
            {
                filterContext.Result = new RedirectToRouteResult(
                     new System.Web.Routing.RouteValueDictionary(new { Controller = "LoginAd", Action = "Login" })
                    );
            }
            base.OnActionExecuting(filterContext);
        }
    }
}
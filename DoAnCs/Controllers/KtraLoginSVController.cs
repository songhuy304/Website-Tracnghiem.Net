using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DoAnCs.Controllers
{
    public class KtraLoginSVController : Controller
    {
        // GET: KtraLoginSV
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (Session["TaiKhoanSV"] == null)
            {
                filterContext.Result = new RedirectToRouteResult(
                     new System.Web.Routing.RouteValueDictionary(new { Controller = "LoginSV", Action = "Login" })
                    );
            }
            base.OnActionExecuting(filterContext);
        }
    }
}
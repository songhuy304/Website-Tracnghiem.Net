using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace DoAnCs
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional },
             namespaces: new[] { "DoAnCs.Controllers" }

            );
            routes.MapPageRoute("DeThi/LichSuThi", "De-Thi/Lich-Su-Thi", "~/DeThi/LichSuThi.aspx");
        }
    }
}

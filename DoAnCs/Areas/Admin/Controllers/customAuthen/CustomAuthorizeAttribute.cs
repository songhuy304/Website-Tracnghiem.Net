using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace DoAnCs.Areas.Admin.Controllers.customAuthen
{
    public class CustomAuthorizeAttribute : AuthorizeAttribute
    {
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            if (!filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                // Nếu người dùng chưa đăng nhập, chuyển hướng đến trang đăng nhập
                filterContext.Result = new RedirectResult("~/admin/Account/Login");
            }
            else
            {
                // Đăng xuất người dùng
                filterContext.HttpContext.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);

                // Chuyển hướng đến trang đăng nhập sau khi đăng xuất
                filterContext.Result = new RedirectResult("~/admin/Account/Login");
            }
        }
    }
}
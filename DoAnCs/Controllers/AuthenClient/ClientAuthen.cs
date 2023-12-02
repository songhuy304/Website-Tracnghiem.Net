using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DoAnCs.Controllers.AuthenClient
{
    public class ClientAuthen : AuthorizeAttribute
    {
        public ClientAuthen(params string[] roles)
        {
            Roles = string.Join(",", roles);
        }
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            if (!filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                // Nếu người dùng chưa đăng nhập, chuyển hướng đến trang đăng nhập
                filterContext.Result = new RedirectResult("~/Account/Login");
            }
            else
            {
                filterContext.Result = new RedirectResult("~/Account/Logout");
            }
        }
    }

}
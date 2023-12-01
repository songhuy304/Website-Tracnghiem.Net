using DoAnCs.Areas.Admin.Controllers.customAuthen;
using DoAnCs.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.UI;

namespace DoAnCs.Areas.Admin.Controllers
{
    [CustomAuthorize(Roles = "Admin")]

    public class BaiThiController : Controller
    {
        // GET: Admin/BaiThi
        private TracNghiemEntities1 db = new TracNghiemEntities1(); 
        public ActionResult Index(int? page)
        {
         
             var item = db.Exam_Results.ToList();
            
          
            int pageIndex = (page ?? 1);
            int pagesize = 10; /*số lượng item của trang = 5*/
            item = item.OrderByDescending(n => n.IdStudent).ToList();
            ViewBag.PageSize = pagesize;
            ViewBag.Page = page;
            return View(item.ToPagedList(pageIndex, pagesize));
        }
    }
}
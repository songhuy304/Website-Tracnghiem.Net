using DoAnCs.Controllers.AuthenClient;
using DoAnCs.Models;
using Microsoft.AspNet.Identity;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DoAnCs.Controllers
{
    [ClientAuthen("User", "Admin")]

    public class InforController : Controller
    {
        // GET: Infor
        TracNghiemEntities1 db = new TracNghiemEntities1();

        public ActionResult Index()
        {
            return View();
        }
        public ActionResult LichSuThi(int? page)
        {


            var userId = User.Identity.GetUserId();
            var lichsudethi = db.Exam_Results.Where(e => e.IdUser == userId).ToList();

            int pageSize = 10;
            int pageNumber = (page ?? 1);
            var pagedLichSuDeThi = lichsudethi.ToPagedList(pageNumber, pageSize);

            ViewBag.PageSize = pageSize;
            ViewBag.Page = page;

            return View(pagedLichSuDeThi);
        }
    }
}
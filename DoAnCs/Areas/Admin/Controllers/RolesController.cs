using System;
using System.Collections.Generic;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DoAnCs.Models;
using DoAnCs.Areas.Admin.Controllers.customAuthen;
using System.Web.Security;

namespace DoAnCs.Areas.Admin.Controllers
{
    [CustomAuthorize(Roles = "Admin")]

    public class RolesController : Controller
    {
        private ApplicationDbContext db1 = new ApplicationDbContext();
     

        private TracNghiemEntities1 db = new TracNghiemEntities1();

        // GET: Admin/Roles
        public ActionResult Index()
        {
            return View(db1.Roles.ToList());
        }

       
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IdentityRole model)
        {
            if (ModelState.IsValid)
            {
                var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(db1));
                roleManager.Create(model);
                return RedirectToAction("Index");
            }
            return View(model);
        }

        // GET: Admin/Roles/Edit/5
        public ActionResult Edit(int id)
        {
            var item = db1.Roles.Find(id);
            return View(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(IdentityRole model)
        {
            if (ModelState.IsValid)
            {
                var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(db));
                roleManager.Update(model);
                return RedirectToAction("Index");
            }
            return View(model);
        }
       
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db1.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}

using DoAnCs.Areas.Admin.Controllers.customAuthen;
using DoAnCs.Models;
using NPOI.POIFS.Properties;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.UI.WebControls;

namespace DoAnCs.Areas.Admin.Controllers
{
    [CustomAuthorize(Roles = "Admin")]

    public class ParentTopicController : Controller
    {
        TracNghiemEntities1 db = new TracNghiemEntities1();
        // GET: Admin/BaiThi
        public ActionResult Index()
        {
            ViewBag.IdParent = new SelectList(db.ParentTopics, "IdParTopic", "NameParTopic");
            var parentTopics = db.ParentTopics.ToList();

            return View(parentTopics);
        }
        public ActionResult Themchudecha()
        {

            return PartialView();
        }
        public ActionResult Themchudecon()
        {

            return PartialView();
        }
        [HttpGet]
     
        public ActionResult Add()
        {
           
          
            return PartialView();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(string parentTopic)
        {
            if (ModelState.IsValid)
            {
                var subject = new ParentTopic
                {
                    NameParTopic = parentTopic
                };
                db.ParentTopics.Add(subject);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(parentTopic);
        }
        public ActionResult Delete(int id)
        {
            try
            {
                if (id != 0)
                {
                    var examToDelete = db.Topics.Find(id);

                    if (examToDelete != null) // Kiểm tra xem bản ghi có tồn tại không
                    {
                        db.Topics.Remove(examToDelete);
                        db.SaveChanges();
                        TempData["Succes"] = "Xóa Thành Công";
                        return Json(new { success = true });
                    }
                    else
                    {
                        TempData["warning"] = "Lỗi Không Tìm Thấy Bbản Ghi";
                        return Json(new { success = false, message = "Không tìm thấy bản ghi để xóa." });
                    }
                }
                else
                {
                    TempData["warning"] = "Lỗi Không Tìm Thấy Bbản Ghi";
                    return Json(new { success = false, message = "ID không hợp lệ." });
                }
            }
            catch (Exception e)
            {

                TempData["warning"] = "Có lỗi xảy ra khi xóa bản ghi.";
                Console.WriteLine($"Processing failed: {e.Message}");
                return Json(new { success = false, message = "Có lỗi xảy ra khi xóa bản ghi." });

            }
        }
    }
}
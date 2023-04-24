using DoAnCs.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace DoAnCs.Controllers
{
    public class DeThiController : Controller
    {
        // GET: DeThi
        private TracNghiemEntities1 db = new TracNghiemEntities1();
       
        public ActionResult Index(string currentFilter, int? page, string searchString)
        {
            var item = new List<Exam>();


            if (searchString != null) /*nếu ô tìm kiếm bằng khác null  thì bắt đầu từ  page  1*/
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            if (!string.IsNullOrEmpty(searchString))
            {
                //lấy ds sản phẩm theo từ khóa
                item = db.Exams.Where(x => x.NameExam.Contains(searchString) || searchString == null).ToList();

            }
            else
            {

                item = db.Exams.ToList();
            }
            ViewBag.CurrentFilter = searchString;
            int pageIndex = (page ?? 1);
            int pagesize = 10; /*số lượng item của trang = 5*/
            item = item.OrderByDescending(n => n.IdSubject).ToList();
            ViewBag.PageSize = pagesize;
            ViewBag.Page = page;
            return View(item.ToPagedList(pageIndex, pagesize));
        }
        

        public ActionResult DeThi(int? id, FormCollection form)
        {

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Exam exam = db.Exams.Find(id);
            var questions = db.Questions.Where(q => q.IdSubject == exam.IdSubject).OrderBy(q => Guid.NewGuid()).Take((int)exam.NumberQ);
            ViewBag.sophut = exam.Time;
        
            return View(questions);
        }
      
    }
}
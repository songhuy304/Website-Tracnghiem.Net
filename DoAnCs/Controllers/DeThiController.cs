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
            item = item.OrderByDescending(n => n.Exam_date).ToList();
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
        
            return View(questions);
        }
        [HttpPost]
        public ActionResult DeThi(FormCollection form, IEnumerable<Question> questions)
        {
            int mark = 0;
            int countdung = 0;
            int countcauhoi = questions.Count(); // Lấy số lượng câu hỏi từ danh sách
            for (int i = 1; i <= countcauhoi; i++)
            {
                string id = form[string.Format("question1_{0}", i)];
                string idDapAn = form[string.Format("question1_{0}_idDapAn", i)];
                string choice = "";

                switch (id)
                {
                    case "A":
                        choice = "A";
                        break;
                    case "B":
                        choice = "B";
                        break;
                    case "C":
                        choice = "C";
                        break;
                    case "D":
                        choice = "D";
                        break;
                }

                // Lấy câu hỏi tương ứng với vòng lặp
                var question = questions.ElementAt(i - 1);

                if (choice == question.DapAn) // Kiểm tra câu trả lời của người dùng
                {
                    mark += 2;
                    countdung++;
                }
            }

            ViewBag.Mark = mark;
            ViewBag.CountDung = countdung;
            ViewBag.CountCauHoi = countcauhoi;

            return View();
        }
    }
}
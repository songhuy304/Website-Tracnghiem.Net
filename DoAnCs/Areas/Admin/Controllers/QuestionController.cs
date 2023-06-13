using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using DoAnCs.Models;
using PagedList;
using GemBox.Document;
using System.IO;

namespace DoAnCs.Areas.Admin.Controllers
{
    public class QuestionController : KtraLoginAdController
    {
        private TracNghiemEntities1 db = new TracNghiemEntities1();

        // GET: Admin/Question
        public ActionResult Index(string currentFilter, int? page, string searchString)
        {
            var item = new List<Question>();


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
                item = db.Questions.Where(x => x.Contentt.Contains(searchString) || searchString == null).ToList();

            }
            else
            {

                item = db.Questions.ToList();
            }
            ViewBag.CurrentFilter = searchString;
            int pageIndex = (page ?? 1);
            int pagesize = 10; /*số lượng item của trang = 5*/
            item = item.OrderByDescending(n => n.IdExam).ToList();
            ViewBag.PageSize = pagesize;
            ViewBag.Page = page;
            return View(item.ToPagedList(pageIndex, pagesize));



        }
        public ActionResult Create()
        {

            ViewBag.Ans = new SelectList(db.Answers.ToList(), "IdAnswer", "DapAn");
            ViewBag.iddd = new SelectList(db.Exams.ToList(), "IdExam", "NameExam");

            ViewBag.IdExam = new SelectList(db.Exams, "IdExam", "NameExam");
            return View();
        }


        [HttpPost]
        [ValidateInput(false)]
        //[ValidateAntiForgeryToken]
        public ActionResult Create(Question question)
        {
            if (ModelState.IsValid)
            {
                question.Contentt = HttpUtility.HtmlEncode(question.Contentt);
                db.Questions.Add(question);
                db.SaveChanges();
                return RedirectToAction("index");
            }
            ViewBag.iddd = new SelectList(db.Exams.ToList(), "IdExam", "NameExam");


            ViewBag.IdExam = new SelectList(db.Exams, "IdExam", "NameExam");
            return View(question);
        }

        // GET: Admin/Question/Edit/5

        [HttpGet]
       
        public ActionResult edit(int id)
        {
            var cauhoi = db.Questions.SingleOrDefault(n => n.IdQuestion == id);
            if (cauhoi == null)
            {
                Response.StatusCode = 404;
                return null;
            }

            ViewBag.IdDifficulty = new SelectList(db.Difficulties, "IdDifficulty", "NameDifficulty");

            ViewBag.IdExam = new SelectList(db.Exams, "IdExam", "NameExam");

            return View(cauhoi);
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult edit(Question question)

        {

            
            if (ModelState.IsValid)
            {
                question.Contentt = HttpUtility.HtmlEncode(question.Contentt);

                db.Entry(question).State = System.Data.Entity.EntityState.Modified;
                //cauhoi.Contentt = question.Contentt;
                db.SaveChanges();
                
            }
            return RedirectToAction("Index");

            
        }
        //Xóa Không load trang khác  

        public ActionResult Delete(int? id)
        {

            var question = db.Questions.Find(id);

            db.Questions.Remove(question);
            db.SaveChanges();
            return RedirectToAction("Index", "Question");

        }
        public ActionResult detail(int? id)
        {
            var question = db.Questions.SingleOrDefault(m => m.IdQuestion == id);
            ViewBag.Id = question.IdQuestion;

            return View(question);
        }
        [HttpGet]
        public ActionResult taocauhoi()
        {
            return View();
        }
        [HttpPost]
        public ActionResult taocauhoi(HttpPostedFileBase questionFile)
        {
            if (questionFile != null && questionFile.ContentLength > 0)
            {
                // Lưu tệp Word vào một đường dẫn tạm thời
                string filePath = Server.MapPath("~/Temp/" + Path.GetFileName(questionFile.FileName));
                questionFile.SaveAs(filePath);


                // Mở file Word
                ComponentInfo.SetLicense("FREE-LIMITED-KEY");
                DocumentModel document = DocumentModel.Load(filePath);

                // Lặp qua các đoạn văn trong tài liệu
                foreach (Paragraph paragraph in document.GetChildElements(true, ElementType.Paragraph))
                {
                    // Đọc nội dung câu hỏi và các lựa chọn từ đoạn văn
                    string content = paragraph.Content.ToString();
                    string optionA = GetOption(paragraph, "A");
                    string optionB = GetOption(paragraph, "B");
                    string optionC = GetOption(paragraph, "C");
                    string optionD = GetOption(paragraph, "D");
                    string answer = GetAnswer(paragraph);

                    // Tạo một đối tượng Question và thêm các thuộc tính
                    Question question = new Question
                    {
                        Contentt = content,
                        optionA = optionA,
                        optionB = optionB,
                        optionC = optionC,
                        optionD = optionD,
                        DapAn = answer
                    };

                    // Thêm câu hỏi vào cơ sở dữ liệu
                    if (ModelState.IsValid)
                    {
                        question.Contentt = HttpUtility.HtmlEncode(question.Contentt);
                        db.Questions.Add(question);
                        db.SaveChanges();
                    }
                }

                // Xóa tệp Word tạm thời sau khi đã xử lý
                //System.IO.File.Delete(filePath);
            }

            return RedirectToAction("Index");
        }
        private string GetOption(Paragraph paragraph, string optionIdentifier)
        {
            string paragraphText = paragraph.Content.ToString();

            // Tìm văn bản của lựa chọn trong đoạn văn
            string optionText = string.Empty;
            int optionStartIndex = paragraphText.IndexOf(optionIdentifier + ".");
            if (optionStartIndex >= 0)
            {
                int optionEndIndex = paragraphText.IndexOf('\n', optionStartIndex);
                if (optionEndIndex >= 0)
                {
                    // Xác định vị trí bắt đầu và kết thúc của lựa chọn
                    int optionContentStartIndex = optionStartIndex + optionIdentifier.Length + 2;
                    int optionContentEndIndex = optionEndIndex;

                    // Kiểm tra nếu có lựa chọn tiếp theo cùng dòng
                    int nextOptionStartIndex = paragraphText.IndexOf((char)(optionIdentifier[0] + 1) + ".", optionEndIndex);
                    if (nextOptionStartIndex >= 0)
                    {
                        optionContentEndIndex = nextOptionStartIndex;
                    }

                    optionText = paragraphText.Substring(optionContentStartIndex, optionContentEndIndex - optionContentStartIndex).Trim();
                }
            }

            return optionText;
        }



        private string GetAnswer(Paragraph paragraph)
        {
            string paragraphText = paragraph.Content.ToString();

            // Tìm văn bản của đáp án trong đoạn văn
            string answerText = string.Empty;
            int answerStartIndex = paragraphText.IndexOf("Answer:");
            if (answerStartIndex >= 0)
            {
                answerText = paragraphText.Substring(answerStartIndex + 7).Trim();
            }

            return answerText;
        }






    }


}

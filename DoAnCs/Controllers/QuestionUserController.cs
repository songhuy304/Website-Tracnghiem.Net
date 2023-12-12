using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DoAnCs.Models;
using PagedList;
using System.IO;
using DoAnCs.Models.Viewmodel;
using Newtonsoft.Json;
using System.Linq.Dynamic;
using NPOI.XWPF.UserModel;
using System.Text.RegularExpressions;
using DoAnCs.Areas.Admin.Controllers.customAuthen;
using System.Web.Security;
using System.Web.UI.WebControls;
using Microsoft.AspNet.Identity;
using DoAnCs.Controllers.AuthenClient;

namespace DoAnCs.Controllers
{
    [ClientAuthen("User", "Admin")]
    public class QuestionUserController : Controller
    {
        public TracNghiemEntities1 db = new TracNghiemEntities1();
        // GET: QuestionUser
        public ActionResult Index()
        {
            return View();
        }
      
        [HttpPost]
        public ActionResult UploadWord(HttpPostedFileBase wordFile)
        {
            if (ModelState.IsValid)
            {
                if (wordFile != null && wordFile.ContentLength > 0)
                {
                    if (Path.GetExtension(wordFile.FileName).Equals(".docx"))
                    {
                        using (var stream = wordFile.InputStream)
                        {
                            XWPFDocument doc = new XWPFDocument(stream);
                            IList<XWPFParagraph> danhSachDoanVan = doc.Paragraphs;
                            List<XWPFParagraph> paragraphs = danhSachDoanVan.ToList();

                            string topic = ""; // Initialize the 'topic' variable
                            string thongbao = "";
                            int questionCount = 1;
                            List<QuestionForUser> questions = new List<QuestionForUser>();

                            foreach (XWPFParagraph paragraph in paragraphs)
                            {
                                string text = paragraph.ParagraphText;

                                if (!string.IsNullOrWhiteSpace(text))
                                {
                                    if (text.StartsWith("Đề "))
                                    {
                                        // Assuming 'Đề' is the indicator for the 'topic' field
                                        topic = text.Substring(3).Trim(); // Extract the topic
                                    }
                                    else if (text.StartsWith($"Câu {questionCount}:"))
                                    {
                                        string[] lines = text.Split('\n');
                                        // Assuming the format is fixed with correct indexes, modify as needed.
                                        string questionContent = lines[0];
                                        List<string> options = new List<string>();
                                        bool coDauSao = false;
                                        for (int i = 1; i < lines.Length; i++)
                                        {
                                            if (lines[i].StartsWith(""))
                                            {
                                                options.Add(lines[i].Trim());
                                                if (lines[i].Trim().Contains("*"))
                                                {
                                                    coDauSao = true; // Đặt cờ thành true nếu thấy ký tự '*'
                                                }
                                            }
                                        }
                                        if (options.Count == 0)
                                        {
                                            thongbao += $"\n*Câu {questionCount}: Không có Đáp Án, Đáp án phải được xuống dòng bằng (SHIFT + ENTER).";
                                        }
                                        else
                                        {
                                            // Kiểm tra các điều kiện khác
                                            if (!coDauSao)
                                            {
                                                thongbao += $"\n*Câu {questionCount}: Không Có Đáp Án Đúng.";
                                            }
                                            else if (options.Count(o => o.Contains("*")) > 1)
                                            {
                                                thongbao += $"\n*Câu {questionCount}: Có Nhiều Đáp án đúng.";
                                            }
                                        }
                                        QuestionForUser question = CreateQuestion(questionContent, options);
                                        questions.Add(question);
                                        questionCount++;
                                    }
                                }
                            }

                            ViewBag.Questions = questions;
                            TempData["Questions"] = questions;
                            TempData["ThongBao"] = thongbao;
                            return RedirectToAction("DisplayQuestion");
                        }
                    }
                    else
                    {
                        TempData["Loi"] = "Vui lòng chọn tệp Word hợp lệ!";
                    }
                }
                else
                {
                    ModelState.AddModelError("wordFile", "Vui lòng chọn một tệp Word.");
                }
            }

            return RedirectToAction("index");
        }

        //Lưu Vào Database từ fileword
        private QuestionForUser CreateQuestion(string questionContent, List<string> options)
        {
            List<SubjectItem> questionOptions = new List<SubjectItem>();

            for (int i = 0; i < options.Count; i++)
            {
                // Check if the option starts with '*'
                bool isTrue = options[i].StartsWith("*");

                // Remove the '*' character if present
                string content = options[i].TrimStart('*');

                // Create a QuestionOption object
                SubjectItem option = new SubjectItem
                {
                    id = i + 1, // Assigning an ID to the option
                    Content = content,
                    Trueis = isTrue
                };

                questionOptions.Add(option);
            }

            // Serialize the list of options into JSON
            string jsonOptions = JsonConvert.SerializeObject(questionOptions);


            Regex regex = new Regex(@"Câu \d+:");
            string cleanContent = regex.Replace(questionContent, "");
            cleanContent = cleanContent.Trim();
            // So sách topic từ bảng topic và word 
            //Topic topicDB = db.Topics.FirstOrDefault(x => x.NameTopic == topic);
            QuestionForUser question = new QuestionForUser();
         
                question.Contentt = cleanContent;
                question.answer = jsonOptions;
             


            return question;
        }
        [HttpGet]
        public ActionResult DisplayQuestion()
        {
            List<QuestionForUser> questions1 = TempData["Questions"] as List<QuestionForUser>;
            TempData["Questions123"] = questions1;
            string thongBao = TempData["ThongBao"] as string;
            ViewBag.ThongBao = thongBao;
            return View(questions1);

        }
        [HttpPost]
        public ActionResult SaveDisplayQuestion(string Tendethi , int Thoigian)
        {
            List<QuestionForUser> questions1 = TempData["Questions123"] as List<QuestionForUser>;
            var userId = User.Identity.GetUserId();

            if (questions1 == null || questions1.Count == 0)
            {
                ModelState.AddModelError("Lỗi", "Không có câu hỏi nào để lưu.");
                return RedirectToAction("Index", "ExamUser");
            }

            try
            {
                var ExamUser = new ExamForUser
                {
                    NameExam = Tendethi,
                    NumberQ = questions1.Count,
                    Exam_date = DateTime.Now,
                    Time = Thoigian,
                    idUser = userId
                };

                using (var transaction = db.Database.BeginTransaction())
                {
                    db.ExamForUsers.Add(ExamUser);
                    db.QuestionForUsers.AddRange(questions1);
                    db.SaveChanges();

                    int examId = ExamUser.IdExam;

                    foreach (var questionId in questions1)
                    {
                        var questionExam = new Question_ExamForUser()
                        {
                            IdExam = examId,
                            IdQuestion = questionId.IdQuestion,
                        };
                        db.Question_ExamForUser.Add(questionExam);
                    }

                    db.SaveChanges();
                    transaction.Commit();
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Lỗi", "Đã xảy ra lỗi khi lưu câu hỏi.");
                // Log the exception for debugging purposes
                // logger.LogError(ex, "Error occurred while saving questions");
                return RedirectToAction("Index", "ExamUser");
            }

            return RedirectToAction("Index", "ExamUser");
        }
        //View Đọc File
        public ActionResult updatepartiav()
        {
            return PartialView();
        }

        public ActionResult DownloadFile()
        {
            string filePath = Server.MapPath("~/Content/Mau.docx"); // Điền tên file DOCX thực tế ở đây

            if (System.IO.File.Exists(filePath))
            {
                byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, "FileMau.docx"); // Điền tên file muốn hiển thị khi tải về
            }
            else
            {
                return HttpNotFound("File không tồn tại.");
            }
        }


    }
}
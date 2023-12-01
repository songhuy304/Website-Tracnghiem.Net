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

namespace DoAnCs.Areas.Admin.Controllers
{
    public class QuestionController : Controller
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
            item = item.OrderByDescending(n => n.idtopic).ToList();
            ViewBag.PageSize = pagesize;
            ViewBag.Page = page;
            return View(item.ToPagedList(pageIndex, pagesize));

        }
        public ActionResult Create()
        {
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

            return View(question);
        }
        [HttpGet]
        public ActionResult edit(int id)
        {
            var cauhoi = db.Questions.SingleOrDefault(n => n.IdQuestion == id);
            if (cauhoi == null)
            {
                Response.StatusCode = 404;
                return null;
            }
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
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }
        //Xóa Không load trang khác  

        [HttpPost]
        public ActionResult Delete(int id)
        {
            try
            {
                var question = db.Questions.Find(id);

                if (question != null)
                {
                    db.Questions.Remove(question);
                    db.SaveChanges();
                    return Json(new { success = true });
                }
                else
                {
                    return Json(new { success = false, message = "Câu hỏi không tồn tại." });
                }
            }
            catch (Exception ex)

            {
                Console.WriteLine($"Processing failed: {ex.Message}");
                return Json(new { success = false, message = "Lỗi khi xóa câu hỏi." });
            }
        }
        public ActionResult Chitiet(int? id)
        {
            List<SubjectItem> subjectItems;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Question question = db.Questions.Find(id);
            if (question == null)
            {
                return HttpNotFound();
            }
            else
            {
                subjectItems = JsonConvert.DeserializeObject<List<SubjectItem>>(question.answer);
            }
            ViewBag.Id = id;
            return View(subjectItems);
        }
        [HttpGet]

        public ActionResult Partial_SanPham()
        {
            var item = db.Exams.ToList();
            return PartialView(item);
        }

        [HttpGet]
        public JsonResult LoadDsDethi()
        {
            try
            {
                var dslop = db.Questions.ToList();
                return Json(new { code = 200, dslop = dslop, msg = "Lấy Thành Công" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { code = 500, msg = "Lỗi: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }



        [HttpGet]
        public ActionResult saveJson()
        {
            ViewBag.IdTopic = new SelectList(db.Topics, "idTopic", "NameTopic");

            List<SubjectItem> ci = new List<SubjectItem> { new SubjectItem { Content = "" } };
            return View(ci);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult saveJson(List<SubjectItem> ci, string contenntt, int IdTopic , string difficult)
        {
            int i = 1;
            foreach (var item in ci)
            {
                item.id = i++;
            }
            string json = JsonConvert.SerializeObject(ci);
            var subject = new Question
            {
                Contentt = contenntt,
                answer = json,
                idtopic = IdTopic,
                Difficult = difficult
            };
            db.Questions.Add(subject);
            db.SaveChanges();
            ViewBag.Message = "Data successfully saved!";
            ViewBag.IdTopic = new SelectList(db.Topics, "idTopic", "NameTopic");

            ci = new List<SubjectItem> { new SubjectItem { Content = "", Trueis = false } };
            return View(ci);

        }
        [HttpGet]
        [ValidateInput(false)]
        //Sửa đáp án câu hỏi
        public ActionResult EditQuestion(int id)
        {
            var question = db.Questions.Find(id);
            if (question == null)
            {
                return HttpNotFound();
            }

            var subjectItems = JsonConvert.DeserializeObject<List<SubjectItem>>(question.answer);

            var viewModel = new QuestionViewModel
            {
                Question = question,
                SubjectItems = subjectItems
            };

            ViewBag.IdTopic = new SelectList(db.Topics, "idTopic", "NameTopic");

            return View(viewModel);
        }

        [HttpPost]
        [ValidateInput(false)]

        public ActionResult EditQuestion(QuestionViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var updatedQuestion = db.Questions.Find(viewModel.Question.IdQuestion);
                if (updatedQuestion != null)
                {
                    updatedQuestion.Contentt = viewModel.Question.Contentt;
                    updatedQuestion.answer = JsonConvert.SerializeObject(viewModel.SubjectItems);
                    db.SaveChanges();
                    return RedirectToAction("Index", "Question");
                }
            }

            // Nếu có lỗi, quay trở lại trang chỉnh sửa với dữ liệu hiện tại và thông báo lỗi
            ViewBag.IdTopic = new SelectList(db.Topics, "idTopic", "NameTopic");
            return View(viewModel);
        }
        public ActionResult DeleteRecord(int questionId, int recordId)
        {
            // Truy vấn đối tượng Question từ cơ sở dữ liệu theo questionId.
            var question = db.Questions.FirstOrDefault(q => q.IdQuestion == questionId);

            if (question != null)
            {
                // Deserialize chuỗi JSON từ trường "answer" thành danh sách đối tượng.
                List<SubjectItem> subjectList = JsonConvert.DeserializeObject<List<SubjectItem>>(question.answer);

                // Tìm và xóa bản ghi cần xóa dựa trên recordId.
                var recordToDelete = subjectList.SingleOrDefault(item => item.id == recordId);
                if (recordToDelete != null)
                {
                    subjectList.Remove(recordToDelete);

                    // Cập nhật lại giá trị id cho các phần tử còn lại trong danh sách.
                    int newId = 1;
                    foreach (var subject in subjectList)
                    {
                        subject.id = newId;
                        newId++;
                    }

                    // Serialize danh sách đã sửa thành chuỗi JSON.
                    string updatedJson = JsonConvert.SerializeObject(subjectList);

                    // Cập nhật trường "answer" của đối tượng Question với chuỗi JSON đã sửa.
                    question.answer = updatedJson;

                    // Lưu thay đổi vào cơ sở dữ liệu.
                    db.SaveChanges();
                }

            }
            return RedirectToAction("Index", "Question");
        }

        // Them Dap An Cua Cau Hoi 
        [ValidateInput(false)]
        public ActionResult themdapan(SubjectItem newsub, int questionId)
        {
            if (ModelState.IsValid && questionId != 0)
            {
                var question = db.Questions.FirstOrDefault(q => q.IdQuestion == questionId);
                if (question != null)
                {
                    List<SubjectItem> subjectItems = JsonConvert.DeserializeObject<List<SubjectItem>>(question.answer);
                    int maxid = subjectItems.Max(a => a.id);
                    newsub.id = maxid + 1;
                    subjectItems.Add(newsub);
                    question.answer = JsonConvert.SerializeObject(subjectItems);
                    db.SaveChanges();

                }
            }

            return PartialView();
        }
        //đọc fileword
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
                            List<Question> questions = new List<Question>();

                            foreach (XWPFParagraph paragraph in paragraphs)
                            {
                                string text = paragraph.ParagraphText;

                                if (!string.IsNullOrWhiteSpace(text))
                                {
                                    if (text.StartsWith("\t\t\t\t\tĐề "))
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


                                        Question question = CreateQuestion(questionContent, options, topic);
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
        private Question CreateQuestion(string questionContent, List<string> options, string topic)
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
            Topic topicDB = db.Topics.FirstOrDefault(x => x.NameTopic == topic);
            Question question = new Question();
            if (topicDB != null)
            {

                question.Contentt = cleanContent;
                question.answer = jsonOptions;
                question.idtopic = topicDB.idTopic;

            }
            else
            {
                Topic newTopic = new Topic
                {
                    NameTopic = topic
                };
                db.Topics.Add(newTopic);
                db.SaveChanges();
                question.Contentt = cleanContent;
                question.answer = jsonOptions;
                question.idtopic = newTopic.idTopic;

            }


            return question;
        }
        [HttpGet]
        public ActionResult DisplayQuestion()
        {
            List<Question> questions1 = TempData["Questions"] as List<Question>;
            TempData["Questions123"] = questions1;
            string thongBao = TempData["ThongBao"] as string;
            ViewBag.ThongBao = thongBao;
            return View(questions1);

        }
        [HttpPost]
        public ActionResult SaveDisplayQuestion()
        {
            List<Question> questions1 = TempData["Questions123"] as List<Question>;
            if (questions1 == null)
            {
                ModelState.AddModelError("Lỗi", "Vui lòng Up lại File một tệp Word.");

            }
            db.Questions.AddRange(questions1);
            db.SaveChanges();
            return RedirectToAction("index");

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





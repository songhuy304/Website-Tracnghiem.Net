
using System.IO;

using System.Linq.Dynamic;
using DoAnCs.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json; // Cần thư viện Newtonsoft.Json
using System.Net;
using PagedList;
using DoAnCs.Models.Viewmodel;
using System.Data.Entity.Infrastructure;
using System.Windows.Interop;
using Org.BouncyCastle.Asn1.Crmf;

namespace DoAnCs.Areas.Admin.Controllers
{
    public class DeThiController : KtraLoginAdController
    {
        TracNghiemEntities1 db = new TracNghiemEntities1();
        // GET: Admin/BaiThi
        public ActionResult Index()
        {
            var item = db.Exams.ToList();
            return View(item);
        }

        [HttpGet]
        public ActionResult Add()
        {
            ViewBag.IdSubject = new SelectList(db.Subjects, "IdSubject", "NameSubject");
            return View();


        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(Exam exam)
        {
            if (ModelState.IsValid)
            {
                exam.Exam_date = DateTime.Now;
                db.Exams.Add(exam);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.IdSubject = new SelectList(db.Subjects, "IdSubject", "NameSubject");

            return View(exam);
        }
        public ActionResult edit(int id)
        {
            ViewBag.IdSubject = new SelectList(db.Subjects, "IdSubject", "NameSubject");

            var exem = db.Exams.Find(id);
            return View(exem);
        }
        [HttpPost]
        public ActionResult edit(Exam exem)
        {
            if (ModelState.IsValid)
            {
                db.Exams.Attach(exem);
                db.Entry(exem).State = System.Data.Entity.EntityState.Modified;
                var lectured = (Lecturer)Session["TaiKhoanGV"];
                exem.ModifierDate = DateTime.Now;
                exem.MordifierBy = lectured.Name;
                db.SaveChanges();
                return Json(new { success = true });
            }
            ViewBag.IdSubject = new SelectList(db.Subjects, "IdSubject", "NameSubject");

            return Json(new { msg = "Không thể xóa câu hỏi khỏi đợt thi." });
        }
        //Xóa Không load trang khác  
        [HttpPost]
        public ActionResult Delete1(int id)
        {
            try
            {
                if (id != 0)
                {
                    var examToDelete = db.Exams.Find(id);

                    if (examToDelete != null) // Kiểm tra xem bản ghi có tồn tại không
                    {
                        db.Exams.Remove(examToDelete);
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

                return Json(new { success = false, message = "Có lỗi xảy ra khi xóa bản ghi." });

            }
        }



        public ActionResult GetList()
        {
            int start = Convert.ToInt32(Request["start"]);
            int length = Convert.ToInt32(Request["length"]);
            string searchValue = Request["search[value]"];
            string sortColumnName = Request["columns[" + Request["order[0][column]"] + "][name]"];
            string sortDirection = Request["order[0][dir]"];

            List<Question> empList = new List<Question>();

            empList = db.Questions.ToList<Question>();
            int totalrows = empList.Count;
            if (!string.IsNullOrEmpty(searchValue)) //filter
            {
                empList = empList
                    .Where(x => x.Contentt.ToLower().Contains(searchValue.ToLower()) || x.Topic.NameTopic.ToLower().Contains(searchValue.ToLower()))
                    .ToList<Question>();
            }
            int totalrowsafterfiltering = empList.Count;

            // Selecting only specific columns (ID, Content, and Topic)
            var selectedData = empList
                .Select(q => new
                {
                    q.IdQuestion,
                    q.Contentt,
                    TopicName = q.Topic.NameTopic
                })
                .ToList();

            // Sorting
            if (!string.IsNullOrEmpty(sortColumnName))
            {
                if (sortDirection == "asc")
                {
                    selectedData = selectedData.OrderBy(sortColumnName).ToList();
                }
                else
                {
                    selectedData = selectedData.OrderBy(sortColumnName).ToList();
                }
            }

            // Paging
            selectedData = selectedData.Skip(start).Take(length).ToList();

            return Json(new
            {
                data = selectedData,
                draw = Request["draw"],
                recordsTotal = totalrows,
                recordsFiltered = totalrowsafterfiltering
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult Partial_CauHoi123()
        {
            var empList = db.Questions.ToList<Question>();
            return PartialView("Partial_CauHoi123", empList);
        }


        [HttpGet]

        public ActionResult Partial_CauHoi(int? idExam, int? page)
        {
            var item = db.Questions.ToList();

            int pageIndex = (page ?? 1);
            int pagesize = 5;

            var pagedItems = item.ToPagedList(pageIndex, pagesize);

            ViewBag.PageSize = pagesize;
            ViewBag.Page = page;
            ViewBag.TotalPages = pagedItems.PageCount;

            return PartialView(pagedItems);
        }
        [HttpGet]
        public ActionResult Chitiet(int? id)
        {

            ViewBag.IdSubject = new SelectList(db.Subjects, "IdSubject", "NameSubject");

            var exem = db.Exams.Find(id);
            return View(exem);
        }
        [HttpGet]
        public ActionResult CauHoiInBaiThi(int? id)
        {
            List<object> selectedQuestions = new List<object>();

            Exam exam = db.Exams.Find(id);
            if (exam == null)
            {
                return HttpNotFound();
            }
            else
            {
                var questionIds = db.Question_Exam
                    .Where(mapping => mapping.IdExam == exam.IdExam)
                    .Select(mapping => mapping.IdQuestion)
                    .ToList();

                var questions = db.Questions
                    .Where(q => questionIds.Contains(q.IdQuestion))
                    .Select(q => new
                    {
                        IdQuestion = q.IdQuestion,
                        Contentt = q.Contentt,
                        TopicName = q.Topic.NameTopic
                    })
                    .ToList();

                return Json(questions, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult CauHoiChuaCoTrongBaiThi(int? id)
        {
            List<object> unselectedQuestionList = new List<object>();

            Exam exam = db.Exams.Find(id);
            if (exam == null)
            {
                return HttpNotFound();
            }
            else
            {
                var questionIds = db.Question_Exam
                    .Where(mapping => mapping.IdExam == exam.IdExam)
                    .Select(mapping => mapping.IdQuestion)
                    .ToList();

                var unselectedQuestionIds = db.Questions
                    .Where(q => !questionIds.Contains(q.IdQuestion))
                    .Select(q => q.IdQuestion)
                    .ToList();

                var unselectedQuestions = db.Questions
                    .Where(q => unselectedQuestionIds.Contains(q.IdQuestion))
                    .Select(q => new
                    {
                        IdQuestion = q.IdQuestion,
                        Contentt = q.Contentt,
                        TopicName = q.Topic.NameTopic
                    })
                    .ToList();

                return Json(unselectedQuestions, JsonRequestBehavior.AllowGet);
            }
        }


        public ActionResult CauHoiKhongthuocBaiThi()
        {
            return PartialView();
        }
        public ActionResult XoaCauHoi1(int? examId, int? questionId)
        {
            if (examId == null || questionId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Exam exam = db.Exams.Find(examId);
            if (exam == null)
            {
                return HttpNotFound();
            }

            var mappingToRemove = db.Question_Exam.FirstOrDefault(mapping => mapping.IdExam == examId && mapping.IdQuestion == questionId);

            if (mappingToRemove != null)
            {
                db.Question_Exam.Remove(mappingToRemove);


                exam.NumberQ -= 1;


                if (exam.NumberQ >= 0)
                {
                    exam.isactive = true;
                }

                db.SaveChanges(); // Lưu thay đổi

                return Json(new { success = true });
            }

            return Json(new { msg = "Không thể xóa câu hỏi khỏi đợt thi." });
        }

        public ActionResult setcauhoi(int? id, int questionIds) // Thêm tham số questionId
        {
            if (id != 0 && questionIds != 0)
            {
                var exam = new Question_Exam()
                {
                    IdExam = id.Value,
                    IdQuestion = questionIds
                };
                db.Question_Exam.Add(exam);
                var exem = db.Exams.Find(id);
                if (exem != null)
                {
                    exem.NumberQ += 1;
                }
                db.SaveChanges();
                return Json(new { success = true, msg = "Thêm Thành Công" });
            }
            return Json(new { success = false, msg = "Không Tìm Thấy id" });

        }



        [HttpGet]
        public ActionResult Addtest()
        {
            ViewBag.datetimnow = DateTime.Now.ToString("yyyy-MM-dd");
            ViewBag.IdSubject = new SelectList(db.Subjects, "IdSubject", "NameSubject");
            var parentTopics = db.ParentTopics.Select(pt => new
            {
                IdParTopic = pt.IdParTopic,
                NameParTopic = pt.NameParTopic,
                ChildTopicCount = pt.Topics.Count()
            }).ToList();

            var parentTopicsWithCount = parentTopics.Select(pt => new
            {
                IdParTopic = pt.IdParTopic,
                NameParTopic = $"{pt.NameParTopic} ({pt.ChildTopicCount})"
            });

            ViewBag.IdParent = new SelectList(parentTopicsWithCount, "IdParTopic", "NameParTopic");


            return View();

        }
        [HttpPost]
        public ActionResult AddExamWithQuestions(Exam exam, int?[] questionIds = null)
        {
            if (ModelState.IsValid)
            {
                exam.Exam_date = DateTime.Now;
                exam.ModifierDate = DateTime.Now;
                var lectur = (Lecturer)Session["TaiKhoanGV"];
                exam.CreateBy = lectur.Name;
                db.Exams.Add(exam);
                db.SaveChanges();
                int examId = exam.IdExam;
                if (questionIds != null && questionIds.Length > 0)
                {
                    foreach (int questionId in questionIds)
                    {
                        var questionExam = new Question_Exam()
                        {
                            IdExam = examId,
                            IdQuestion = questionId
                        };
                        db.Question_Exam.Add(questionExam);
                    }
                    db.SaveChanges();
                }

                return Json(new { success = true, message = "Thêm thành công" });
            }
            ViewBag.IdParent = new SelectList(db.ParentTopics, "IdParTopic", "NameParTopic");

            ViewBag.datetimnow = DateTime.Now.ToString("yyyy-MM-dd");
            ViewBag.IdSubject = new SelectList(db.Subjects, "IdSubject", "NameSubject");
            return Json(new { success = false, message = "Dữ liệu không hợp lệ." });
        }


        [HttpPost]
        public ActionResult Isactive(int id)
        {
            var item = db.Exams.Find(id);
            if (item != null)
            {
                item.isactive = !item.isactive;
                db.Entry(item).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return Json(new { success = true, Isactive = item.isactive });
            }
            return Json(new { success = false });
        }

        [HttpPost]
        [ValidateInput(false)]

        public ActionResult setAnh(int id, string Image)
        {
            Exam exam = db.Exams.Find(id);

            if (exam != null)
            {
                exam.image = Image;
                db.SaveChanges();

                return Json(new { msg = "Thành công" });
            }

            return Json(new { msg = "Thất bại" });
        }

        public ActionResult Getquestionbytopic(int id)
        {
            if (ModelState.IsValid)
            {
                var questionIds = db.Questions
                    .Join(db.Topics, q => q.idtopic, t => t.idTopic, (q, t) => new { Question = q, Topic = t })
                    .Where(x => x.Topic.idPartopic == id)
                    .Select(x => x.Question.IdQuestion)
                    .ToList();

                return Json(questionIds, JsonRequestBehavior.AllowGet);
            }

            return Json(new { success = false, msg = "Không Tìm Thấy id" });
        }
        [HttpPost]
        public ActionResult Getquestionsbytopics(List<QuestionTopic> ids, string nameExam, int monHoc, int thoiGian)
        {
            if (ids == null || ids.Count == 0 || string.IsNullOrWhiteSpace(nameExam) || monHoc <= 0 || thoiGian <= 0)
            {
                return Json(new { success = false, msg = "Invalid input data" });
            }

           

            var questionIds = new List<int>();
            foreach (var id in ids)
            {
                if (id.Socauhoi <= 0)
                {
                    continue;
                }

                var questionsForId = db.Questions
                    .Where(q => q.idtopic == id.idTopic)
                    .Select(q => q.IdQuestion)
                    .Take(id.Socauhoi)
                    .ToList();

                questionIds.AddRange(questionsForId);
            }

            var exam = new Exam()
            {
                Exam_date = DateTime.Now,
                ModifierDate = DateTime.Now,
                NameExam = nameExam,
                IdSubject = monHoc,
                Time = thoiGian,
                NumberQ = questionIds.Count()
            };

            db.Exams.Add(exam);
            db.SaveChanges();
            int examId = exam.IdExam;

            foreach (var itemId in questionIds)
            {
                var exam_question = new Question_Exam()
                {
                    IdExam = examId,
                    IdQuestion = itemId
                };
                db.Question_Exam.Add(exam_question);
            }

            db.SaveChanges();

            return Json(new { success = true, message = "Thêm Thành Công" });

           
        }

        [HttpPost]
        public ActionResult Themcauhoitumatran(Exam exam, int[] questionIds)
        {
            if (ModelState.IsValid)
            {
                exam.Exam_date = DateTime.Now;
                exam.ModifierDate = DateTime.Now;
                var lectur = (Lecturer)Session["TaiKhoanGV"];
                exam.CreateBy = lectur.Name;
                db.Exams.Add(exam);
                db.SaveChanges();
                int examId = exam.IdExam;
                if (questionIds != null && questionIds.Length > 0)
                {
                    // Lấy số lượng câu hỏi ngẫu nhiên
                    Random random = new Random();
                    int socauhoi = exam.NumberQ;
                    int[] randomQuestionIds = questionIds.OrderBy(x => random.Next()).Take(socauhoi).ToArray();

                    foreach (int questionId in randomQuestionIds)
                    {
                        var questionExam = new Question_Exam()
                        {
                            IdExam = examId,
                            IdQuestion = questionId
                        };
                        db.Question_Exam.Add(questionExam);
                    }
                    db.SaveChanges();
                }

                return Json(new { success = true, message = "Thêm thành công" });
            }

            return Json(new { success = false, message = "Dữ liệu không hợp lệ." });
        }
        public JsonResult GetStateList(int TopicPrId)
        {
            db.Configuration.ProxyCreationEnabled = false;
            List<Topic> topics = db.Topics.Where(x => x.idPartopic == TopicPrId).ToList();

            var topicsWithQuestionCount = topics.Select(topic => new
            {
                idTopic = topic.idTopic,
                NameTopic = topic.NameTopic
            }).ToList();

            return Json(topicsWithQuestionCount, JsonRequestBehavior.AllowGet);
        }
        // Xây Dựng Api
        [HttpGet]
        public ActionResult AddquestionMatran()
        {
            ViewBag.datetimnow = DateTime.Now.ToString("yyyy-MM-dd");
            ViewBag.IdSubject = new SelectList(db.Subjects, "IdSubject", "NameSubject");
            var parentTopics = db.ParentTopics.Select(pt => new
            {
                IdParTopic = pt.IdParTopic,
                NameParTopic = pt.NameParTopic,
                ChildTopicCount = pt.Topics.Count()
            }).ToList();

            var parentTopicsWithCount = parentTopics.Select(pt => new
            {
                IdParTopic = pt.IdParTopic,
                NameParTopic = $"{pt.NameParTopic} ({pt.ChildTopicCount})"
            });

            ViewBag.IdParent = new SelectList(parentTopicsWithCount, "IdParTopic", "NameParTopic");
            return View();
        }


    }
}
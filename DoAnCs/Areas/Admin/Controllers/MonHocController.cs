using DoAnCs.Models;
using Newtonsoft.Json;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DoAnCs.Models.Viewmodel;


namespace DoAnCs.Areas.Admin.Controllers
{
    public class MonHocController : KtraLoginAdController
    {
        TracNghiemEntities1 db = new TracNghiemEntities1();

        public ActionResult Index()
        {
            var item = db.Subjects.ToList();
            return View(item);

        }
        [HttpGet]
        public ActionResult Add()
        {
            
            return View();
            

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(Subject Subjects)
        {
            if (ModelState.IsValid)
            {

                db.Subjects.Add(Subjects);
                db.SaveChanges();



                return RedirectToAction("Index");
            }

            return View(Subjects);
        }
        [HttpGet]
        public ActionResult DisplayQuestion(int? subjectId)
        {
            var subject = db.Subjects.FirstOrDefault(s => s.IdSubject == subjectId);
            if (subject != null)
            {
                var questionData = JsonConvert.DeserializeObject<List<QuestionData>>(subject.textJson);

              
                ViewBag.QuestionData = questionData; 

                return View(questionData);
            }

            return RedirectToAction("Index"); // Hoặc xử lý lỗi nếu không tìm thấy dữ liệu cho môn học.
        }
        public ActionResult BulkData()
        {
            // This is only for show by default one row for insert data to the database
            List<Subject> ci = new List<Subject> { new Subject { IdSubject = 0, NameSubject = "", textJson = "" } };
            return View(ci);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult BulkData(List<Subject> ci)
        {
            if (ModelState.IsValid)
            {
               
                    foreach (var i in ci)
                    {
                    string json = JsonConvert.SerializeObject(i);
                    i.textJson = json;
                    db.Subjects.Add(i);
                }
                    db.SaveChanges();
                    ViewBag.Message = "Data successfully saved!";
                    ModelState.Clear();
                    ci = new List<Subject> { new Subject { IdSubject = 0, NameSubject = "", textJson = "" } };
                
            }
            return View(ci);
        }
        [HttpGet]
        public ActionResult save1( )
        {

            List<SubjectItem> ci = new List<SubjectItem> { new SubjectItem {} };
            return View(ci);
        }

        [HttpPost]
     
        public ActionResult SaveQuestion(SubjectItem inputs)
        {
            try
            {
                // Tạo danh sách subjects từ dữ liệu đầu vào
                List<SubjectItem> subjects = new List<SubjectItem>
                 {
                        
                      new SubjectItem
                      {
                        Content = inputs.Content,
                       
                     
                       }
                 };

                
                    string json = JsonConvert.SerializeObject(subjects);
                    var subject = new Subject
                    {
                      
                        textJson = json
                    };

                    db.Subjects.Add(subject);
                    db.SaveChanges();
               

                
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                string message = ex.Message;
                return View("Error"); // Thay "Error" bằng trang lỗi của bạn hoặc xử lý lỗi theo cách khác
            }
        }



        [HttpGet]
        public ActionResult save1123()
        {

            List<SubjectItem> ci = new List<SubjectItem> { new SubjectItem { } };
            return View(ci);
        }

        [HttpPost]

        public ActionResult SaveQuestion1(List<SubjectItem> inputs)
        {
            
                try
                {
                    // Tạo danh sách subjects từ dữ liệu đầu vào
                    List<SubjectItem> subjects = new List<SubjectItem>();

                    foreach (var input in inputs)
                    {
                        subjects.Add(new SubjectItem
                        {
                            Content = input.Content,
                         
                        });
                    }
                  
                    string json = JsonConvert.SerializeObject(subjects);
                    var subject = new Subject
                    {
                       
                        textJson = json
                    };

                    db.Subjects.Add(subject);
                    db.SaveChanges();

                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                string msg = ex.Message;    
                    return View("Error"); // Thay "Error" bằng trang lỗi của bạn hoặc xử lý lỗi theo cách khác
                }
            }



        public ActionResult addsubject()
        {
            // This is only for show by default one row for insert data to the database
            List<SubjectItem> ci = new List<SubjectItem> { new SubjectItem { } };
            return View(ci);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult addsubjectitem(List<SubjectItem> ci , string Namesubjectss)
        {
            if (ModelState.IsValid)
            {

                string json = JsonConvert.SerializeObject(ci);
                
                var subject = new Subject
                {
                    NameSubject = Namesubjectss,
                    textJson = json
                };

                db.Subjects.Add(subject);
                db.SaveChanges();
                ViewBag.Message = "Data successfully saved!";
                ModelState.Clear();
                ci = new List<SubjectItem> { new SubjectItem { } };
                return RedirectToAction ("Index");

            }
            return View(ci);
        }
    }
}
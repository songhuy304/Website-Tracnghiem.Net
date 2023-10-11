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
using OfficeOpenXml;
using DoAnCs.Models.Viewmodel;
using Newtonsoft.Json;

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
            item = item.OrderByDescending(n => n.Contentt).ToList();
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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UploadExcel( HttpPostedFileBase excelFile)
        {

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial; // Hoặc LicenseContext.Commercial tùy vào loại giấy phép bạn sử dụng

            if (ModelState.IsValid)
            {
                if (excelFile != null && excelFile.ContentLength > 0)
                {
                    if (Path.GetExtension(excelFile.FileName).Equals(".xls") || Path.GetExtension(excelFile.FileName).Equals(".xlsx"))
                    {
                        using (var package = new ExcelPackage(excelFile.InputStream))
                        {
                            ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                            int rowCount = worksheet.Dimension.Rows;

                            for (int row = 2; row <= rowCount+1 ; row++)
                            {
                                string Contentts = worksheet.Cells[row, 2].Value.ToString();
                                string optionAs = worksheet.Cells[row, 3].Value.ToString();
                                string optionBs = worksheet.Cells[row, 4].Value.ToString();
                                string optionCs = worksheet.Cells[row, 5].Value.ToString();
                                string optionDs = worksheet.Cells[row, 6].Value.ToString();
                                string DapAns = worksheet.Cells[row, 7].Value.ToString();
                           

                                Question question = new Question
                                {
                                  
                                };

                                db.Questions.Add(question);
                            }

                            db.SaveChanges();
                            TempData["SuccessMessage"] = "Dữ liệu đã được thêm thành công.";
                            return RedirectToAction("Index");
                        }
                    }
                    else
                    {
                        TempData["Loi"] = "Vui Lòng Chọn Tệp Hợp Lệ!";

                    }
                }
                else
                {
                    ModelState.AddModelError("excelFile", "Vui lòng chọn một tệp Excel.");
                }
            }

            return View("index");
        }

        [HttpGet]
        public ActionResult saveJson()
        {

            List<SubjectItem> ci = new List<SubjectItem> { new SubjectItem { Content=""  } };
            return View(ci);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult saveJson(List<SubjectItem> ci, string contenntt)
        {
            string mahoa = HttpUtility.HtmlEncode(contenntt);
            string json = JsonConvert.SerializeObject(ci);
            var subject = new Question
                    {
                        Contentt = mahoa,
                        answer = json
                    };

                    db.Questions.Add(subject);
                    db.SaveChanges();
                    ViewBag.Message = "Data successfully saved!";
            
                    ci = new List<SubjectItem> { new SubjectItem { Content ="" , Trueis=false } };
                    return View(ci);
        
        }
    }


}

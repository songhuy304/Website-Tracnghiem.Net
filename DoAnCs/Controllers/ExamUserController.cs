using DoAnCs.Controllers.AuthenClient;
using DoAnCs.Models;
using DoAnCs.Models.Viewmodel;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace DoAnCs.Controllers
{
    public class ExamUserController : Controller
    {
        public TracNghiemEntities1 db = new TracNghiemEntities1();
        // GET: ExamUser
        [AllowAnonymous]
        public ActionResult Index()
        {
            var userId = User.Identity.GetUserId();
            var Dethi = db.ExamForUsers.Where(e => e.idUser == userId).ToList();
            return View(Dethi);
        }

        [ClientAuthen("User", "Admin")]

        public ActionResult UpFile()
        {
            return PartialView();
        }
        [ClientAuthen("User", "Admin")]

        public ActionResult UpThuCong()
        {
            return PartialView();
        }
        [ClientAuthen("User", "Admin")]

        public ActionResult ThamGia()
        {
            return PartialView();
        }

        public ActionResult DeThiCuaMinh()
        {

            return PartialView();
        }
        [ClientAuthen("User", "Admin")]

        public ActionResult VaoThi(int? id, FormCollection form)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ExamForUser exam = db.ExamForUsers.Find(id);
            if (exam == null)
            {
                return View("~/Views/ExamUser/Er404.cshtml");
            }

            var questionIds = db.Question_ExamForUser
                                .Where(mapping => mapping.IdExam == exam.IdExam)
                                .Select(mapping => mapping.IdQuestion)
                                .ToList();

            if (questionIds.Count < exam.NumberQ)
            {
                return Json(new { success = false, msg = "Số lượng câu hỏi trong đề không đủ" });
            }

            var seed = Guid.NewGuid().GetHashCode();
            var random = new Random(seed);

            var randomQuestionIds = questionIds.OrderBy(q => random.Next()).Take((int)exam.NumberQ).ToList();
            var questions = db.QuestionForUsers
                                .Where(q => randomQuestionIds.Contains(q.IdQuestion))
                                .OrderBy(q => Guid.NewGuid())
                                .ToList();

            foreach (var question in questions)
            {
                question.AnswerList = JsonConvert.DeserializeObject<List<SubjectItem>>(question.answer);
            }

            Session["cauhoi123"] = questions;
            Session["bathiId123"] = id;
            Session["bathi123"] = exam;

            ViewBag.Exam = exam;
            ViewBag.sophut = exam.Time;
            ViewBag.tenbaithi = exam.NameExam;
            ViewBag.socauhoi = exam.NumberQ;

            return View(questions);
        }
        [ClientAuthen("User", "Admin")]

        [HttpPost]
        public ActionResult VaoThi(List<QuestionAnswer> examResults, FormCollection form, int? socaudung, string thoigianthi)
        {
            try
            {
                if (socaudung == null || examResults == null || Session["bathi123"] == null)
                {
                    // Trả lỗi nếu thiếu thông tin cần thiết.
                    return Json(new { error = "Lỗi: Thiếu thông tin cần thiết." });
                }

                var exam = (ExamForUser)Session["bathi123"];
         

             
                TimeSpan thoigianthiStr = TimeSpan.Parse(thoigianthi);
                TimeSpan ThoiGian = TimeSpan.FromMinutes(exam.Time);
                TimeSpan ketQua = ThoiGian - thoigianthiStr;

                //Tính thời gian đã thi
                string ketQuaString = ketQua.ToString(@"hh\:mm\:ss");
                // Tính Điểm 
                double TongDiem = (double)socaudung * (100.0 / exam.NumberQ);
                TongDiem = Math.Floor(TongDiem * 2) / 2;
                decimal diemDecimal = Convert.ToDecimal(TongDiem);
                if (TongDiem < 0 || TongDiem > 100)
                {
                    // Trả lỗi nếu điểm số không hợp lệ.
                    return Json(new { error = "Lỗi: Điểm số không hợp lệ." });
                }

                string json = JsonConvert.SerializeObject(examResults);
                // Lấy ra id Người dùng
                var userId = User.Identity.GetUserId();

                var examResult = new Exam_ResultsForUser
                {
                    IdExam = exam.IdExam,
                    //IdStudent = student.IdStudent,
                    IdUser = userId,
                    KetQuaThi = json,
                    Score = diemDecimal,
                    Time = ketQuaString,
                    ExamDay = DateTime.Now
                };

                db.Exam_ResultsForUser.Add(examResult);
                db.SaveChanges();

                int idResult = examResult.IdResult;
                Session.Remove("bathi123");
                Session.Remove("cauhoi123");
                Session.Remove("bathiId123");
           
                return Json(new { redirectTo = Url.Action("KetQua", new { id = idResult, showReviewModal = true }) });
            }
            catch (Exception ex)
            {
                // Trả lỗi nếu xảy ra lỗi không mong muốn.
                TempData["ErrorMessage"] = "Lỗi: " + ex.Message;
                return View();
            }
        }
        [ClientAuthen("User", "Admin")]

        public ActionResult KetQua(int? id)
        {
            var idn = db.Exam_ResultsForUser.FirstOrDefault(x => x.IdResult == id);



            if (idn != null)
            {
                var questionData = JsonConvert.DeserializeObject<List<QuestionAnswer>>(idn.KetQuaThi);
                ViewBag.questionData = questionData;
                ViewBag.TenBaiThi = idn.ExamForUser.NameExam;
                ViewBag.Score = idn.Score;
                ViewBag.idBaiThi = idn.IdExam;
                ViewBag.Time = idn.Time;
                ViewBag.SocauHoiKetQua = idn.ExamForUser.NumberQ;
                ViewBag.ThoiGian = idn.ExamForUser.Time;
                return View(questionData);
            }

            return View();
        }


    }
}
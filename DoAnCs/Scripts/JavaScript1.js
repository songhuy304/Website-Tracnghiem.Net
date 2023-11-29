var countdownInterval;
var thoigianlambai = 0;
var sogiaylambai = 0;
$(document).ready(function () {
  


    startCountdown();
    function collectExamResults() {
        var questionAnswers = [];
        CheckResult();
        $(".formthi .row").each(function () {
            var questionAnswer = {};

            var text = $(this).find(".card-title").text().trim();
            var match = text.match(/\d+/);
            var number = match ? match[0] : null;
            questionAnswer.QuestionNumber = number;

            var questionContent = $(this).find(".card-body h4").html().trim();
            questionAnswer.QuestionContent = questionContent.replace(/^Câu \d+: /, '').trim();
            questionAnswer.Choices = [];
            var correctAnswerDataId = 1;

            // Tạo biến để kiểm tra xem người dùng đã chọn đáp án chưa
            var userAnswerSelected = false;

            $(this).find("input[type=radio]").each(function () {
                var dataId = $(this).attr('data-id');
                var choiceText = $(this).closest(".input-group").find(".form-control[data-id='" + dataId + "']").html().trim();
                questionAnswer.Choices.push(choiceText);

                if (dataId == correctAnswerDataId) {
                    questionAnswer.CorrectAnswerIndex = questionAnswer.Choices.length - 1;
                }

                if ($(this).is(":checked")) {
                    questionAnswer.UserAnswerIndex = questionAnswer.Choices.length - 1;
                    userAnswerSelected = true;
                }
            });

            // Kiểm tra xem người dùng đã chọn đáp án hay chưa
            if (!userAnswerSelected) {
                questionAnswer.UserAnswerIndex = -1; // Hoặc giá trị khác để chỉ ra rằng không có đáp án nào được chọn
            }

            questionAnswer.Score = questionAnswer.CorrectAnswerIndex === questionAnswer.UserAnswerIndex ? 1 : 0;

            questionAnswers.push(questionAnswer);
        });

        //var element = document.getElementById("re");
        //var value = element.textContent; // hoặc element.innerText
        value = CheckResult();
        var countdownElement = document.getElementById("countdow");
        var textContent = countdownElement.textContent;

        var intValue = parseInt(value, 10); // 10 là hệ cơ số (decimal)
        var jsonData = {
            examResults: questionAnswers,
            socaudung: intValue,
            thoigianthi: textContent
        };

        return jsonData;
    }
    checknav();
    $('#btnnop').click(function () {
        if (confirm('Bạn Muốn Nộp Bài ')) {
            clearInterval(countdownInterval); 
            var jsonData = collectExamResults();
            $.ajax({
                url: "/DeThi/Dethi", 
                type: "POST",
                contentType: "application/json",
                data: JSON.stringify(jsonData),
                success: function (result) {
                    window.location.href = result.redirectTo;
                },
                error: function (error) {
                    alert(error);
                    console.log(error);
                }
            });
            localStorage.clear();
            sessionStorage.clear();
            clearsscountdown();
        }
    });
 
    $('.butoon1').on('click', function () {
        var questionNumber = $(this).data('question-number');
        var targetQuestion = $("#question_" + questionNumber);
        if (targetQuestion.length) {
            var windowHeight = $(window).height();
            var targetOffset = targetQuestion.offset().top;
            var scrollTo = targetOffset - (windowHeight / 3);
            $('html, body').animate({
                scrollTop: scrollTo
            }, 1000);
        }
    });
    doimaubutton();

});


$(window).on("load", function () {
    // Các hàm được đặt ở đây sẽ chạy khi toàn bộ trang và tài nguyên bên ngoài đã được tải hoàn toàn
  
});
function doimaubutton() {
    // Hàm để kiểm tra và thiết lập trạng thái đã chọn từ Local Storage
  

    // Xử lý sự kiện khi radio button thay đổi
    $('input[type="radio"]').change(function () {
        var dataIdex = $(this).attr('data-idex');
      
        if ($(this).is(':checked')) {
           
            localStorage.setItem('radio_' + dataIdex, 'true');
            var questionName = $(this).prop('name');
            var questionNumber = questionName.split('_')[1];
            $('#button_' + questionNumber).addClass('answered');
        } else {
            localStorage.setItem('radio_' + dataIdex, 'false');
            // Tìm nút button và xóa class 'answered'
            var questionName = $(this).prop('name');
            var questionNumber = questionName.split('_')[1];
            $('#button_' + questionNumber).removeClass('answered');
        }
    });
    function checkAndUpdateState() {
        $('input[type="radio"]').each(function () {
            var dataIdex = $(this).attr('data-idex');
            var isChecked = localStorage.getItem('radio_' + dataIdex);

            if (isChecked === 'true') {
                $(this).prop('checked', true);
                // Tìm nút button và thêm class 'answered'
                var questionName = $(this).prop('name');
                var questionNumber = questionName.split('_')[1];

                $('#button_' + questionNumber).addClass('answered');
            }
        });
    }

    // Gọi hàm để kiểm tra và thiết lập trạng thái khi trang được tải
    checkAndUpdateState();
}




function startCountdown() {
    var countdownElement = document.getElementById("countdow");
    var sophut = countdownElement.getAttribute("data-id");
    sophut = parseInt(sophut);

    var timeLeft = sessionStorage.getItem("timeLeft");
    var endTime = sessionStorage.getItem("endTime");

    if (timeLeft && endTime) {
        var remainingTime = endTime - Date.now() - timeLeft;
    } else {
        var remainingTime = sophut * 60 * 1000; // tính theo milliseconds
        endTime = Date.now() + remainingTime;
    }

    countdownInterval = setInterval(function () {
        var remainingTime = Math.max(0, endTime - Date.now());
        var timeLeft = sophut * 60 * 1000 - remainingTime;
        sessionStorage.setItem("timeLeft", timeLeft);
        sessionStorage.setItem("endTime", endTime);

        var remainingHours = Math.floor(remainingTime / 1000 / 3600);
        var remainingMinutes = Math.floor((remainingTime / 1000 / 60) % 60);
        var remainingSeconds = Math.floor((remainingTime / 1000) % 60);

        var remainingTimeString = (remainingHours < 10 ? "0" : "") + remainingHours + ":" +
            (remainingMinutes < 10 ? "0" : "") + remainingMinutes + ":" +
            (remainingSeconds < 10 ? "0" : "") + remainingSeconds;

        countdownElement.textContent = remainingTimeString;

        if (remainingTime <= 0) {
            clearInterval(countdownInterval);
            countdownElement.textContent = "Kết Thúc!";
            alert("Kết Thúc Bài Thi")

            sessionStorage.removeItem("timeLeft");
            sessionStorage.removeItem("endTime");
            CheckResult();
            var jsonData = collectExamResults();
            $.ajax({
                url: "/DeThi/Dethi",
                type: "POST",
                contentType: "application/json",
                data: JSON.stringify(jsonData),
                success: function (result) {
                    window.location.href = result.redirectTo;
                },
                error: function (error) {
                    alert(error);
                    console.log(error);
                }
            });

            $('#btnnop').hide();
            $('#btnquaylai').show();
            disradio();
            localStorage.clear();
            sessionStorage.clear();
            clearsscountdown();
        }
    }, 1000);
}

// Xoas sstore tisnh gio
function clearsscountdown() {
    sessionStorage.removeItem("timeLeft");
    sessionStorage.removeItem("endTime");
}
// check Khi lam xong bai
//function checknav() {
//    const links = $('a');
//    links.each(function () {
//        $(this).click(function (e) {
//            if (confirm('Bạn Muốn Thoát Bài Thi?')) {
//                // Cho phép chuyển đến trang được liên kết khi click vào liên kết
//                localStorage.clear();
//                sessionStorage.clear();
//                clearsscountdown();
//            } else {
//                // Ngăn chặn hành động mặc định của thẻ a khi click
//                e.preventDefault();
//            }
//        });
//    });

//}

function checknav() {
    const links = $('a');
    links.each(function () {
        $(this).click(function (e) {
            if (confirm('Bạn Muốn Thoát Bài Thi?')) {
                // Cho phép chuyển đến trang được liên kết khi click vào liên kết
                localStorage.clear();
                sessionStorage.clear();
                clearsscountdown();
            } else {
                // Ngăn chặn hành động mặc định của thẻ a khi click
                e.preventDefault();
            }
        });
    });

}

function CheckResult() {
 
    var radioButtons = document.querySelectorAll("input[type='radio']");
    var score = 0; // Biến để tính điểm
    radioButtons.forEach(function (radioButton) {
        var dataId = radioButton.getAttribute("data-id");
        var questionId = radioButton.id.replace("question_", "");
        var selectedAnswer = document.querySelector("input[name='question_" + questionId + "']:checked");
        if (selectedAnswer !== null) {
            var selectedDataId = selectedAnswer.getAttribute("data-id");
            if (selectedDataId === "1" && dataId === "1") {
                score++;
            } else {

            }
        }
        
    });
  
    //var s = document.getElementById("re");
    //s.textContent = score;
    return score;
}

var countdownInterval;
var thoigianlambai = 0;
var sogiaylambai = 0;
$(document).ready(function () {
    alert('Bắt Đầu Bài Thi');
    startCountdown();
    $('#btnquaylai').hide();



    $('#btnnop').click(function () {
        if (confirm('Bạn Muốn Nộp Bài ')) {
            clearInterval(countdownInterval); 
            CheckResult();
          

            $.ajax({
                url: "/DeThi/DeThi",
                type: "POST",
                data: $('#examForm').serialize(),

                success: function (response) {
                    var reElement = document.getElementById("re");
                    if (reElement) {
                        reElement.scrollIntoView({ behavior: 'smooth' });
                    }
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    alert("Nộp bài thất bại: " + textStatus);
                }
            });
           
            $('#btnnop').hide();
            $('#btnquaylai').show();
           
            localStorage.clear();
            sessionStorage.clear();
            clearsscountdown();

          

        }
    });
    checknav();
   




   



});



//function luuthongtininput() {
//    //Lưu Giá Trị ô input radio vào trong local
//    $('input[type="radio"]').click(function () {
//        let name = $(this).attr('name');
//        let value = $(this).attr('value');
//        sessionStorage.setItem(name, value); // Lưu giá trị vào sessionStorage
//    });

//    // Khi mình vào lại thì hiện giá trị đó lên từ sessionStorage
//    $('input[type="radio"]').each(function () {
//        let name = $(this).attr('name');
//        let value = sessionStorage.getItem(name);
//        if (value && $(this).attr('value') === value) {
//            $(this).prop('checked', true);
//        }
//    });

//}
//Tính Giờ 
function startCountdown() {

    var countdownElement = document.getElementById("countdow");

    // Lấy giá trị của thuộc tính "data-id"
    var sophut = countdownElement.getAttribute("data-id");

    // Chuyển đổi giá trị sang số nguyên
    sophut = parseInt(sophut);

    // Kiểm tra xem có lưu thời gian đã trôi qua và thời gian kết thúc trong sessionStorage không
    var timeLeft = sessionStorage.getItem("timeLeft");
    var endTime = sessionStorage.getItem("endTime");

    if (timeLeft && endTime) {
        // Nếu có, tính toán lại thời gian còn lại từ thời gian đã trôi qua và thời gian kết thúc
        var remainingTime = endTime - Date.now() - timeLeft;
    } else {
        // Nếu không, tính toán thời gian kết thúc của đếm ngược
        var remainingTime = sophut * 60 * 1000; // tính theo milliseconds
        endTime = Date.now() + remainingTime;
    }

    // Cập nhật đếm ngược mỗi giây
    countdownInterval = setInterval(function () { // Lưu interval vào biến countdownInterval
        // Tính thời gian còn lại
        var remainingTime = Math.max(0, endTime - Date.now());

        // Lưu thời gian đã trôi qua vào sessionStorage
        var timeLeft = sophut * 60 * 1000 - remainingTime;
        sessionStorage.setItem("timeLeft", timeLeft);
        sessionStorage.setItem("endTime", endTime);

        // Chuyển đổi thời gian còn lại sang phút
        var remainingMinutes = Math.floor(remainingTime / 1000 / 60);

        // Chuyển đổi phần thập phân sang giây
        var remainingSeconds = Math.floor((remainingTime / 1000) % 60);

        // Tạo chuỗi định dạng cho thời gian còn lại
        var remainingTimeString = remainingMinutes + ":" + (remainingSeconds < 10 ? "0" : "") + remainingSeconds;

        // Cập nhật phần tử "countdow" với chuỗi thời gian còn lại
        countdownElement.textContent = remainingTimeString;

        if (remainingTime <= 0) {
          
            clearInterval(countdownInterval);
            countdownElement.textContent = "Kết Thúc!";
            alert("Kết Thúc Bài Thi")


            sessionStorage.removeItem("timeLeft");
            sessionStorage.removeItem("endTime");
            CheckResult();
            $.ajax({
                url: "/DeThi/DeThi",
                type: "POST",
                data: $('#examForm').serialize(),

                success: function (response) {

                },
                error: function (jqXHR, textStatus, errorThrown) {
                    alert("Nộp bài thất bại: " + textStatus);
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
var calculatedScore;
function CheckResult() {
    // Đổi màu nền cho các phần tử có data-id="1" thành màu xanh
    var elements = document.querySelectorAll("[data-id='1']");
    elements.forEach(function (element) {
        element.style.backgroundColor = "lightgreen";
    });

    // Đổi màu nền cho các phần tử có data-id="0" thành màu đỏ
    var elementss = document.querySelectorAll("[data-id='0']");
    elementss.forEach(function (element) {
        element.style.backgroundColor = "lightcoral";
    });
   
    

    // In số lượng câu hỏi ra màn hình
    

    var maxScore = 100; // Điểm tối đa
   

    var radioButtons = document.querySelectorAll("input[type='radio']");
    var score = 0; // Biến để tính điểm

    radioButtons.forEach(function (radioButton) {
        var dataId = radioButton.getAttribute("data-id");
        var questionId = radioButton.id.replace("question_", "");
        var selectedAnswer = document.querySelector("input[name='question_" + questionId + "']:checked");
       
        //var correctElementId = "correct_" + questionId;
        //var correctElement = document.querySelector("#" + correctElementId);

        if (selectedAnswer !== null) {
            var selectedDataId = selectedAnswer.getAttribute("data-id");

            if (selectedDataId === "1" && dataId === "1") {
                // Nếu người dùng chọn đúng đáp án có data-id = 1
                score++;

                var correctElementId = "correct_" + questionId;
                var correctElement = document.querySelector("#" + correctElementId);

                correctElement.style.backgroundColor = 'lightgreen';

                // Thêm dòng văn bản "Đúng" vào phần tử correct tương ứng
                var correctText = document.createElement("p");
                var c = correctText.classList.add("text-center")
                correctText.innerHTML = `<i class="fa-solid fa-check fa-xl" style="color: #2fda90;"></i> Correct`;
                correctElement.appendChild(correctText);


              
            } else {
            
            }
        }

        // Đếm số câu hỏi dựa trên số lần lặp qua các nút radio
       
    });
    
    // Số câu hỏi có trong form thi đã được lưu trong biến numberOfQuestions
    var socauhoiValue = document.getElementById("socauhoi").getAttribute("data-viewbag-value");
    let diem1cau = 100 / socauhoiValue;


    calculatedScore = diem1cau * score;

    // Hiển thị kết quả
    var resultElement = document.getElementById("re");
    var resultElementsss = document.querySelector(".matbuon");

    var congtron = document.querySelector(".circle");
    if (calculatedScore >= 80) {
       
        congtron.style.border = "9px outset #32ff00";
        resultElement.style.color = "#32ff00";
        resultElement.innerHTML = calculatedScore + "\\100";
        resultElementsss.innerHTML = `<i class="fa-regular fa-face-surprise fa-xl" style="color: #32ff00; font-size:6.5rem; line-height:0px"></i>
                                         <h4 class="m-6" style="
                                         margin-top: 34px;
                                         margin-left: 10px;
                                        ">Your Are Huy</h4>
                                        `;

    } else if (calculatedScore >= 50 && calculatedScore < 80)

    {
        resultElementsss.innerHTML = `<i class="fa-regular fa-face-smile fa-xl" style="color: yellow; font-size:6.5rem; line-height:0px"></i>
                                        <h4 class="m-6" style="
                                         margin-top: 34px;
                                         margin-left: 10px;
                                        ">Not Bad</h4>`;
       
        congtron.style.border = "8px outset yellow";
        resultElement.innerHTML = calculatedScore + "\\100";
    }
    else {
        resultElementsss.innerHTML = `<p><i class="fa-regular fa-face-frown fa-xl" style="color:red; font-size:6.5rem; line-height:0px"></i></p>
                                        <h4 class="m-6" style="
                                         margin-top: 34px;
                                         margin-left: 10px;
                                        ">So Bad</h4>
                                        `;
       
         congtron.style.border = "9px outset red";
         resultElement.innerHTML = calculatedScore + "\\100";   
    }
    return calculatedScore;
    
}

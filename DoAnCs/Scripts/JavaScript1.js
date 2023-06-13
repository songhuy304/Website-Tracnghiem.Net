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
                   
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    alert("Nộp bài thất bại: " + textStatus);
                }
            });

            $('#btnnop').hide();
            $('#btnquaylai').show();
            disradio();
            localStorage.clear();
            clearsscountdown();
        }
    });
    checknav();
    //Lưu Giá Trị ô input radio vào trong local
    $('input[type="radio"]').click(function () {
        localStorage.setItem($(this).attr('name'), $(this).attr('value'));
    });

    //Khi mình vào lại thì hiện giá trị đó lên 
    $('input[type="radio"]').each(function () {
        let name = $(this).attr('name');
        let value = localStorage.getItem(name);
        if (value && $(this).attr('value') === value) {
            $(this).prop('checked', true);
        }
    });



});

//Lấy Danh sách câu hỏi menu left

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
            clearsscountdown();
        }
        
        
    }, 1000);

}
function clearsscountdown() {
    sessionStorage.removeItem("timeLeft");
    sessionStorage.removeItem("endTime");
}
function checknav() {
    const links = $('a');
    links.each(function () {
        $(this).click(function (e) {
            if (confirm('Bạn Muốn Thoát Bài Thi?')) {
                // Cho phép chuyển đến trang được liên kết khi click vào liên kết
                localStorage.clear();
                clearsscountdown();
            } else {
                // Ngăn chặn hành động mặc định của thẻ a khi click
                e.preventDefault();
            }
        });
    });

}
function disradio() {
    $('#question div h5').each(function (k, v) {

        $(v).parent().find('input[type="radio"]').each(function () {
            $(this).prop('disabled', true);
        });

    });
}


function CheckResult() {

    let countdung = 0;
    let soCauHoi = $('#question div h5').length;
    console.log('Số câu hỏi có trong View: ' + soCauHoi);
    $('#question div h5').each(function (k, v) {
        let id = $(v).attr('id');
        let idDapAn = $(v).data('idDapAn');
        let da = $(v).parent().find('input[type="radio"]:checked').val();
        if (da === idDapAn) {
            countdung++;
        }
        else {
        }

    });


    // Dừng đếm ngược
    clearInterval(countdownInterval);
    let diem1cau = 100 / soCauHoi;
    let TongDiem = diem1cau * countdung;
    TongDiem = Math.round(TongDiem * 2) / 2;
    alert('Điểm Của Bạn Là : ' + TongDiem + 'số Câu Đúng' + countdung + '/' + soCauHoi);
    var HienThiDiem = document.getElementById("sodiem");
    var HienThisocaudung = document.getElementById("socaudung");
    var HienThisocausai = document.getElementById("socausai");

    HienThiDiem.textContent = TongDiem;
    HienThisocaudung.textContent = countdung;
    HienThisocausai.textContent = soCauHoi - countdung;


    $('.dapan').each(function (k, v) {
        let dapAnDung = $(v).text();
        $('.dapan').css("display", "block");
        $('.dapan').css("color", "red");
        $('.dapan').css("font-weight", "bold");

        let dapAn = $(v).find('#dapan').text().trim();

        let questionContainer = $(v).closest('.question-container');
        let optionA = questionContainer.find('.rdoptionA');
        let optionB = questionContainer.find('.rdoptionB');
        let optionC = questionContainer.find('.rdoptionC');
        let optionD = questionContainer.find('.rdoptionD');
        let labelA = questionContainer.find('.A');
        let labelB = questionContainer.find('.B');
        let labelC = questionContainer.find('.C');
        let labelD = questionContainer.find('.D');

        if (dapAn === 'A') {
            labelA.css("background-color", "red");
        } else if (dapAn === 'B') {
            labelB.css("background-color", "red");
        } else if (dapAn === 'C') {
            labelC.css("background-color", "red");
        } else if (dapAn === 'D') {
            labelD.css("background-color", "red");
        }


        $('input[type="radio"]').css("background", "blue");



    });
    $('#question div input[type="radio"]:checked').each(function (k, v) {
        let da = $(v).parent().find('label').css("color", "blue");

    });


}
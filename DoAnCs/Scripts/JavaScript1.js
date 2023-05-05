var countdownInterval;

$(document).ready(function () {
    alert('Bắt Đầu Bài Thi');
    startCountdown();
    $("#test").click(function (event) {
        layDsCauHoi();

    });


    $('#btnluubai').hide();
    $("#btnluubai").click(function (event) {
        // Ngăn chặn hành vi mặc định của trình duyệt
        event.preventDefault();

        // Thực hiện lưu bài bằng Ajax request
        $.ajax({
            url: "/DeThi/DeThi",
            type: "POST",
            data: { bai: "noidungbaiviet" },
            success: function (response) {
                // Cập nhật trạng thái của trang web nếu cần thiết
                alert("Lưu bài thành công!");
                window.location.href = 'https://localhost:44337/Home/index';
            },
            error: function (jqXHR, textStatus, errorThrown) {
                alert("Lưu bài thất bại: " + textStatus);
            }
        });
    });
    $('#btnquaylai').hide();
    $('#btnnop').click(function () {
        if (confirm('Bạn Muốn Nộp Bài ')) {
            clearInterval(countdownInterval); // Xóa bỏ interval
            CheckResult();
            disradio();
            //$('#btnquaylai').show();
            $('#btnnop').hide();
            $('#btnluubai').show();
            localStorage.clear();
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
function layDsCauHoi() {
    let soCauHoi = $('#question div h5').length;
    let cauHoiHTML = '';
    let idcauhoiArr = []; // Tạo mảng để lưu id của các câu hỏi

    $('#question h5').each(function (k, v) {
        $(v).parent().find('input[type="radio"]').each(function () {
            if ($(this).is(':checked')) {
                var checkedH5Id = $(v).attr('id');
                console.log('The h5 element with id ' + checkedH5Id + ' is checked.');
            }
        });



        let cauHoi = $(v).find('label').text();
        let idcauhoi = $(v).find('label').data('id-question-number');

        cauHoiHTML += '<div id="phantucauhoi" class="' + idcauhoi + '" >' + cauHoi + '</div>';
        idcauhoiArr.push(idcauhoi); // Thêm id của câu hỏi vào mảng
    });

    $('#cauhoi').html(cauHoiHTML);

    $('#main1 #cauhoi #phantucauhoi').each(function (k, v) {
        let ids = $(v).attr('class').split(/\s+/); // Chuyển ids thành mảng các id

        // Duyệt qua từng phần tử của mảng idcauhoiArr
        for (let i = 0; i < idcauhoiArr.length; i++) {
            // Kiểm tra xem phần tử i có trùng khớp với bất kỳ phần tử nào trong mảng ids hay không
            if (ids.indexOf(idcauhoiArr[i].toString()) > -1) {
                console.log(`Phần tử ${idcauhoiArr[i]} trùng khớp với phần tử của mảng ids`);
            }
        }
    });
}
//Tính Giờ 
function startCountdown() {

    var countdownElement = document.getElementById("countdow");

    // Lấy giá trị của thuộc tính "data-id"
    var sophut = countdownElement.getAttribute("data-id");

    // Chuyển đổi giá trị sang số nguyên
    sophut = parseInt(sophut);

    // Tính thời gian kết thúc của đếm ngược
    var endTime = Date.now() + sophut * 60 * 1000; // endTime tính theo milliseconds

    // Cập nhật đếm ngược mỗi giây
    countdownInterval = setInterval(function () { // Lưu interval vào biến countdownInterval
        // Tính thời gian còn lại
        var remainingTime = Math.max(0, endTime - Date.now());

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
            countdownElement.textContent = "Kết Thúc Bài Thi!";
            alert("Kết Thúc Bài Thi")
        }


        var thoigianlambaiElement = document.getElementById("thoigianlambai");

        var thoiGianLamBai = (sophut * 60 * 1000) - remainingTime;


        var sophutthi = Math.floor(thoiGianLamBai / 1000 / 60);


        var sophutthiGiay = Math.floor((thoiGianLamBai / 1000) % 60);

        var sophutthiString = sophutthi + ":" + (sophutthiGiay < 10 ? "0" : "") + sophutthiGiay;


        thoigianlambaiElement.textContent = sophutthiString;
    }, 1000);
}
function checknav() {
    const links = $('a');
    links.each(function () {
        $(this).click(function (e) {
            if (confirm('Bạn Muốn Thoát Bài Thi?')) {
                // Cho phép chuyển đến trang được liên kết khi click vào liên kết
                localStorage.clear();

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
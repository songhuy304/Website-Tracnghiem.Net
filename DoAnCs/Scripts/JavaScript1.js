var countdownInterval;

$(document).ready(function () {
    alert('Bắt Đầu Bài Thi');
    startCountdown();
    $('#btnquaylai').hide();
    $('#btnnop').click(function () {
        if (confirm('Bạn Muốn Nộp Bài ')) {
            clearInterval(countdownInterval); // Xóa bỏ interval
            CheckResult();
            $('#btnquaylai').show();
            $('#btnnop').hide();
            //var thoigianlambaiElement = document.getElementById("thoigianlambai");
            //var thoiGianLamBai = (sophut * 60 * 1000);
            //var sophutthi = Math.floor(thoiGianLamBai / 1000 / 60);
            //var sophutthiGiay = Math.floor((thoiGianLamBai / 1000) % 60);
            //var sophutthiString = sophutthi + ":" + (sophutthiGiay < 10 ? "0" : "") + sophutthiGiay;
            //thoigianlambaiElement.textContent = sophutthiString;
        }
    });
    checknav();




});
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
            } else {
                // Ngăn chặn hành động mặc định của thẻ a khi click
                e.preventDefault();
            }
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
        $('input[type="radio"]').css("background", "blue");



    });
    $('#question div input[type="radio"]:checked').each(function (k, v) {
        let da = $(v).parent().find('label').css("color", "blue");

    });


}
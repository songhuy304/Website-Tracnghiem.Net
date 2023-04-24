
$(document).ready(function () {
    alert('Bắt Đầu Bài Thi');
    startCountdown();
    $('#btnquaylai').hide();
    $('#btnnop').click(function () {
        var endTime = Date.now();
        
        CheckResult();
        $('#btnquaylai').show();
        $('#btnnop').hide();
       

    }); 
    
    
   
   

});
function startCountdown() {
    var countdownElement = document.getElementById("countdow");
    var startTime = Date.now();
    // Lấy giá trị của thuộc tính "data-id"
    var sophut = countdownElement.getAttribute("data-id");

    // Chuyển đổi giá trị sang số nguyên
    sophut = parseInt(sophut);

    // Tính thời gian kết thúc của đếm ngược
    var endTime = Date.now() + (sophut * 60 * 1000);

    // Cập nhật đếm ngược mỗi giây
    var countdownInterval = setInterval(function () {
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

        // Kiểm tra xem đếm ngược đã kết thúc chưa
        if (remainingTime <= 0) {
            clearInterval(countdownInterval);
            countdownElement.textContent = "Kết Thúc Bài Thi!";
            alert("Kết Thúc Bài Thi")
        }
    }, 1000);
}

function CheckResult() {
   
    let countdung = 0;
    let soCauHoi = $('#question div h5').length;
    console.log('Số câu hỏi có trong View: ' + soCauHoi);
    $('#question div h5').each(function (k, v) {
        
        let id = $(v).attr('id');
        let idDapAn = $(v).data('idDapAn');
        console.log(id + ' : Câu Trả Lời Đúng ' + idDapAn);
      
        let da = $(v).parent().find('input[type="radio"]:checked').val();
     

        if (da === idDapAn) {
            
         
         

            countdung++;
        }
        else {
           

        }
        
    });
    clearInterval(countdownInterval);
    let diem1cau = 100 / soCauHoi; 
    let TongDiem = diem1cau * countdung;
    alert(TongDiem + 'số Câu Đúng' + countdung);
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
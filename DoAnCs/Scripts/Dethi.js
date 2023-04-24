$(document).ready(function () {

    $('.btnvaothi').click(function (e) {

        if (confirm("Xác Nhận Vào Thi")) {


        }
        else {
            e.preventDefault(); // Ngăn chặn hành động mặc định của thẻ a khi click
        }
    });

});

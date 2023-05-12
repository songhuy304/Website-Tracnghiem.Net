$(document).ready(function () {
    $('input[type="radio"]').click(function () {
        var selectedOption = $('input[type="radio"]:checked').val();
        $('#DapAn').val(selectedOption);
    });
    var optionA = $('#optionA');
    var formgroupB = $('#groupB');
    var optionB = $('#optionB');
    var formgroupC = $('#groupC');
    var optionC = $('#optionC');
    var formgroupD = $('#groupD');
    var optionD = $('#optionD');
    var formgroupE = $('#groupE');
    var optionE = $('#optionE');
    var formgroupF = $('#groupF');
    var optionF = $('#optionF');
    var formgroupF = $('#groupG');
    
   
    // Ẩn formgroup phía dưới ban đầu
    
    // Xử lý sự kiện onchange của Editor optionA
    optionA.on('keyup', function () {
        // Nếu Editor optionA không rỗng thì hiển thị formgroup phía dưới, ngược lại ẩn formgroup phía dưới
        if (optionA.val().trim() !== '') {
            formgroupB.show();
        }
    });
    optionB.on('keyup', function () {
        // Nếu Editor optionA không rỗng thì hiển thị formgroup phía dưới, ngược lại ẩn formgroup phía dưới
        if (optionB.val().trim() !== '') {
            formgroupC.show();
        }
    });
    optionC.on('keyup', function () {
        // Nếu Editor optionA không rỗng thì hiển thị formgroup phía dưới, ngược lại ẩn formgroup phía dưới
        if (optionC.val().trim() !== '') {
            formgroupD.show();
        }
    });
    optionD.on('keyup', function () {
        // Nếu Editor optionA không rỗng thì hiển thị formgroup phía dưới, ngược lại ẩn formgroup phía dưới
        if (optionD.val().trim() !== '') {
            formgroupE.show();
        }
    }); optionE.on('keyup', function () {
        // Nếu Editor optionA không rỗng thì hiển thị formgroup phía dưới, ngược lại ẩn formgroup phía dưới
        if (optionE.val().trim() !== '') {
            formgroupF.show();
        }

    });
    optionF.on('keyup', function () {
        // Nếu Editor optionA không rỗng thì hiển thị formgroup phía dưới, ngược lại ẩn formgroup phía dưới
        if (optionF.val().trim() !== '') {
            formgroupG.show();
        }
    });

    //$('#btnthemcauhoi').click(function () {
    //    /*formgroupB.css("display", "block");*/
    //    formgroupB.show();

    //});

   
});

$('#btnnop').click(function () {
   

    
    

  
    CheckResult();
    
});
function CheckResult() {
    
    let countdung = 0;
    let soCauHoi = $('#question div h5').length;
    console.log('Số câu hỏi có trong View: ' + soCauHoi);
    $('#question div h5').each(function (k, v) {
        
        let id = $(v).attr('id');
        let idDapAn = $(v).data('idDapAn');
        console.log(id + ': ' + idDapAn);
        let socauhoi = $(v).parent().find('.socauhoi').val();
        
        let da = $(v).parent().find('input[type="radio"]:checked').val();
        let choice = '';

        switch (da) {
            case 'A':
                choice ='A';
                break;
            case 'B':
                choice ='B';
                break;
            case 'C':
                choice ='C';
                break;
            case 'D':
                choice ='D';
                break;
        }

        if (choice === idDapAn) {
            
            console.log('Câu Có id đúng :' + id);
            countdung++;
        }
        else {
            console.log('Câu Có id Sai :'+ id);

        }
        console.log('Đáp Án Ngừoi Dùng chọn' + da);
        
    });
    let diem1cau = 100 / soCauHoi; //Tính 1 Câu Cso mấy điểm tùy thuốc Vào Sso câu
    let TongDiem = diem1cau * countdung;
    alert(TongDiem + 'số Câu Đúng' + countdung);
        
}
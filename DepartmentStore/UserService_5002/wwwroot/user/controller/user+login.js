//let ladda;

//$(document).ready(function () {
//    $('#loginForm').on('submit', function (e) {
//        e.preventDefault(); 
//        let formData = new FormData(this); // Tạo FormData từ form
//        //ladda = Ladda.create(document.querySelector('#btn_submit_form')); // Khởi tạo Ladda cho nút submit
//        //ladda.start(); 

//        $.ajax({
//            url: '/User/Login',
//            type: 'POST',
//            data: formData,
//            contentType: false,
//            processData: false,
//            success: function (response) {
//                //ladda.stop(); 
//                if (response.success == true) {
//                    //ShowToastNoti('success', '', response.message, 4000, 'topRight');
//                    location.href = 'https://localhost:7076/Home/Index';
//                } else {
//                    ShowToastNoti('error', '', response.message, 4000, 'topRight');
//                }
//            },
//            error: function (err) {
//                //ladda.stop(); 
//                ShowToastNoti('error', '', 'Có lỗi xảy ra', 4000, 'topRight');
//            }
//        });
//    });
//});

//$(document).ready(function () {
//    $('#btn_logout').on('click', function (e) {
//        e.preventDefault(); 
//        //var ladda = Ladda.create(document.querySelector('#btn_logout'));
//        //ladda.start();

//        $.ajax({
//            url: '/User/Logout',
//            type: 'POST',
//            success: function () {
//                //ladda.stop();
//                ShowToastNoti('success', '', "Đăng xuất thành công", 4000, 'topRight');
//            },
//            error: function (err) {
//                //ladda.stop();
//                ShowToastNoti('error', '', 'Có lỗi xảy ra', 4000, 'topRight');
//            }
//        });
//    });
//});

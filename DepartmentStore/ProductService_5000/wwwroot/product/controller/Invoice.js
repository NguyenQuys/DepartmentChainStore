function CheckPromotion() {
    let promotionCode = $('#input_promotion_code').val();
    let totalCart = parseFloat($('#total_cart').val()) || 0;

    $.ajax({
        url: '/Promotion/GetByPromotionCode',
        type: 'GET',
        data: { promotionCode: promotionCode },
        success: function (response) {
            let percentage = response.data?.percentage || 0;
            let discount = totalCart * percentage / 100;
            if (response.result === 1) {
                $('#div_check_promotion_available').html('<p class="text-success">Chúc mừng. Bạn đã áp dụng voucher thành công</p>');
            } else {
                $('#div_check_promotion_available').html(`<p class="text-danger">${response.message}</p>`);
            }
            $('#discount_invoice').text('- ' + discount.toLocaleString('vi-VN') + ' VND').addClass('text-success');

        },
        error: function () {
            $('#div_check_promotion_available').html('<p class="text-danger">Đã xảy ra lỗi khi áp dụng mã khuyến mãi.</p>');
        }
    });
}

function ToggleDelivery() {
    const pickUpAtStore = document.getElementById('pick_up_at_store');
    const btnChooseLocation = document.getElementById('btn_choose_location');
    if (pickUpAtStore.checked) {
        btnChooseLocation.classList.add('d-none'); // Ẩn nếu chọn 'Nhận tại cửa hàng'
    } else {
        btnChooseLocation.classList.remove('d-none'); // Hiển thị nếu chọn 'Ship đến tận nhà'
    }
}

// Thiết lập trạng thái mặc định khi trang được tải
//window.onload = toggleLocation;

//function OpenModalBranchLocation(idBranch) {
//    $.ajax({
//        url: '/Branch/GetById',
//        type: 'GET',
//        data: { id: idBranch },
//        success: function (response) {
//            lat = response.latitude;
//            lng = response.longtitude;
//            $('#modal_location').modal('show');
//        }
//    });
//}



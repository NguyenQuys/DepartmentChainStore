function CheckPromotion() {
    let listIdProducts = [];
    let listQuantities = [];

    // Capture the selected product IDs
    $('.idProductChosen').each(function () {
        listIdProducts.push($(this).val());
    });

    // Capture the quantities
    $('.quantityChosen').each(function () {
        listQuantities.push($(this).val());
    });

    let promotionCode = $('#input_promotion_code').val();

    // Combine product IDs and quantities into an object
    let listIdProductsAndQuantities = {};
    for (let i = 0; i < listIdProducts.length; i++) {
        listIdProductsAndQuantities[listIdProducts[i]] = parseInt(listQuantities[i]) || 0;
    }

    $.ajax({
        url: '/Promotion/GetByPromotionCode',
        type: 'GET',
        data: {
            promotionCode: promotionCode,
            listIdProductsAndQuantity: JSON.stringify(listIdProductsAndQuantities)
        },
        contentType: 'application/json',
        success: function (response) {
            let discount = response.data;
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


function SubmitFormShipping() {
    let distance = $('#distance').val();

    $.ajax({
        url: `/Shipping/ShippingFee?distance=${distance}`,
        type: 'POST',
        success: function (response) {
            $('#noti_available_delivery')
                .html(`<p>${response.data}</p>`)
                .removeClass('text-danger')
                .addClass('text-success');

            $('#btn_submit_shipping').on('click', function () {
                let formattedData = parseFloat(response.data).toLocaleString('vi-VN');
                $('#delivery').text('+ ' + formattedData + ' VND').addClass('text-danger');
                $('#modal_location').modal('hide');
            });
        },
        error: function (error) {
            let errorMessage = error.responseJSON?.message || "An error occurred.";
            $('#noti_available_delivery')
                .html(`<p>${errorMessage}</p>`)
                .removeClass('text-success')
                .addClass('text-danger');
        }
    });
}

function UpdateTotal() {
    const subtotal = parseFloat(document.getElementById('total_cart').value) || 0;
    const shipping = parseFloat(document.getElementById('delivery').innerText.replace(/[^\d]/g, '')) || 0;
    const discount = parseFloat(document.getElementById('discount_invoice').innerText.replace(/[^\d]/g, '')) || 0;
    const total = subtotal + shipping - discount;
    document.getElementById('total_price').innerText = total.toLocaleString('vi-VN') + ' VND';
}

// Gọi hàm để cập nhật tổng và thêm sự kiện nếu cần
UpdateTotal();

const observer = new MutationObserver(UpdateTotal);
observer.observe(document.getElementById('delivery'), { childList: true, subtree: true });
observer.observe(document.getElementById('discount_invoice'), { childList: true, subtree: true });

function OpenModalConfirmPayment() {

    $('#modal_confirmPayment').modal('show');
}
let global_idPaymentMethod;
let checkChooseDeliveryMethod = false;

function CheckPromotion(event) {
    event.preventDefault();
    let promotionCode = $('#input_promotion_code').val();

    $.ajax({
        url: '/Promotion/GetByPromotionCode',
        type: 'GET',
        data: {
            promotionCode: promotionCode,
            listIdProductsAndQuantity: JSON.stringify(ListIdProductsAndQuantities())
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
        btnChooseLocation.classList.add('d-none'); 
        $('#delivery').text('0 VND');
        global_idPaymentMethod = 1;
    } else {
        btnChooseLocation.classList.remove('d-none'); 
        global_idPaymentMethod = 2;
    }
    checkChooseDeliveryMethod = true;
}

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

function OpenModalConfirmInformation(idUser) {
    if (!checkChooseDeliveryMethod) {
        ShowToastNoti('error', '', 'Vui lòng chọn phương thức vận chuyển', 4000);
    } else {
        if (idUser !== 0) {
            $.ajax({
                url: '/User/GetCustomerById',
                type: 'GET',
                data: { id: idUser },
                success: function (response) {
                    $('#input_customerName').val(response.fullName);
                    $('#input_phoneNumber').val(response.phoneNumber);
                    $('#input_email').val(response.email);
                    $('#modal_information').modal('show');
                },
                error: function (xhr, status, error) {
                    console.error('Error fetching customer information:', error);
                }
            });
        }
        $('#modal_information').modal('show');
    }
}

function AddInvoice() {
    let listSinglePrice = [];
    $('.single-price-product').each(function () {
        listSinglePrice.push($(this).val());
    });
    console.log(listSinglePrice);

    var formData = new FormData();
    formData.append('Promotion', $('#input_promotion_code').val() ?? 0);
    formData.append('Email', $('#input_email').val());
    formData.append('listIdProductsAndQuantities', JSON.stringify(ListIdProductsAndQuantities()));
    listSinglePrice.forEach(price => formData.append('SinglePrice', price));
    formData.append('SumPrice', parseFloat(document.getElementById('total_price').innerText.replace(/[^\d]/g, '')));
    formData.append('IdPaymentMethod', global_idPaymentMethod);
    formData.append('IdBranch', global_idBranch);
    formData.append('CustomerPhoneNumber', $('#input_phoneNumber').val());
    formData.append('CustomerName', $('#input_customerName').val());

    $.ajax({
        url: '/Invoice/AddAtStoreOnline',
        type: 'POST',
        data: formData,
        contentType: false,
        processData: false,
        success: function (response) {
            if (response.result == 1) {
                $('#modal_body_information').html(`<h2 class="text-success text-center">${response.message}</h2>`);
                const indexButton = document.createElement('button');
                indexButton.textContent = "Quay về trang chủ";
                indexButton.classList.add('text-center', 'btn', 'btn-success');

                indexButton.addEventListener('click', function () {
                    window.location.href = '/Product/Index'; 
                });

                $('#modal_body_information').append(indexButton);
            } else {
                ShowToastNoti('error', '', response.message, 4000);
                $('#modal_information').modal('hide');
            }
        },
        error: function (xhr, status, error) {
            console.error('Error adding invoice:', error);
        }
    });
}


function ListIdProductsAndQuantities() {
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

    // Combine product IDs and quantities into an object
    let listIdProductsAndQuantities = {};
    for (let i = 0; i < listIdProducts.length; i++) {
        listIdProductsAndQuantities[listIdProducts[i]] = parseInt(listQuantities[i]) || 0;
    }

    return listIdProductsAndQuantities;
}
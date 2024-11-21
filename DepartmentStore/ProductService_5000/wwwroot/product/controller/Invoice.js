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
                    $('#input_address').val(response.address);
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

    let listIdProductsAndQuantities = {};
    for (let i = 0; i < listIdProducts.length; i++) {
        listIdProductsAndQuantities[listIdProducts[i]] = parseInt(listQuantities[i]) || 0;
    }

    return listIdProductsAndQuantities;
}

function OpenModalHistoryPurchase(idInvoice) {
    $.ajax({
        url: '/Invoice/GetDetailsInvoice',
        type: 'GET',
        data: { id: idInvoice },
        success: function (response) {
            if (response) {
                let productRows = '';
                let initialTotal = response.total + response.discount; 
                let index = 0;

                if (response.productNameAndQuantity && typeof response.productNameAndQuantity === 'object') {
                    for (let productName in response.productNameAndQuantity) {
                        if (response.productNameAndQuantity.hasOwnProperty(productName)) {
                            let quantity = response.productNameAndQuantity[productName];
                            let singlePrice = response.singlePrice[index] || 0; 
                            let totalPrice = quantity * singlePrice;
                            productRows += `
                                <tr>
                                    <td style='border: 1px solid #ddd; padding: 8px;'>${productName}</td>
                                    <td style='border: 1px solid #ddd; padding: 8px;'>${quantity}</td>
                                    <td style='border: 1px solid #ddd; padding: 8px;'>${singlePrice.toLocaleString('vi-VN')} VND</td>
                                    <td style='border: 1px solid #ddd; padding: 8px;'>${totalPrice.toLocaleString('vi-VN')} VND</td>
                                </tr>
                            `;
                        }
                        index++;
                    }
                } else {
                    productRows = '<tr><td colspan="4" style="text-align: center; padding: 8px;">No products found</td></tr>';
                }

                let bodyDetail = `
                    <div style='width: 100%; font-family: Arial, sans-serif;'>
                        <h1>Hóa đơn mua hàng #${response.invoiceNumber || 'N/A'}</h1>
                        <h2>Thời gian: ${response.time ? new Date(response.time).toLocaleString() : 'N/A'}</h2>
                        <h2>Ghi chú từ khách hàng: ${response.customerNote ?? ''}</h2>
                        <h2>Ghi chú từ cửa hàng: ${response.storeNote ?? ''}</h2>
                        <table style='width: 100%; border-collapse: collapse;'>
                            <thead>
                                <tr class='bg-primary text-white''>
                                    <th style='border: 1px solid #ddd; padding: 8px;'>Sản phẩm</th>
                                    <th style='border: 1px solid #ddd; padding: 8px;'>Số lượng</th>
                                    <th style='border: 1px solid #ddd; padding: 8px;'>Đơn giá</th>
                                    <th style='border: 1px solid #ddd; padding: 8px;'>Thành tiền</th>
                                </tr>
                            </thead>
                            <tbody>
                                ${productRows}
                                <tr>
                                    <td colspan='3' style='border: 1px solid #ddd; padding: 8px; text-align: right; font-weight: bold;'>Tổng ban đầu:</td>
                                    <td style='border: 1px solid #ddd; padding: 8px;'>${initialTotal.toLocaleString('en-US')} VND</td>
                                </tr>
                                <tr>
                                    <td colspan='3' style='border: 1px solid #ddd; padding: 8px; text-align: right; font-weight: bold;'>Giảm giá:</td>
                                    <td style='border: 1px solid #ddd; padding: 8px;'>-${response.discount.toLocaleString('en-US')} VND</td>
                                </tr>
                                <tr>
                                    <td colspan='3' style='border: 1px solid #ddd; padding: 8px; text-align: right; font-weight: bold;'>Tổng cộng:</td>
                                    <td style='border: 1px solid #ddd; padding: 8px;'>${response.total.toLocaleString('en-US')} VND</td>
                                </tr>
                            </tbody>
                        </table>
                        <h3>Phương thức thanh toán: ${response.paymentMethod || 'N/A'}</h3>
                        <h3>Trạng thái: ${response.status || 'N/A'}</h3>
                    </div>
                `;

                $('#modal_purchaseHistory .modal-body').html(bodyDetail);
                $('#modal_purchaseHistory').modal('show');
            } else {
                alert('Không tìm thấy dữ liệu chi tiết hóa đơn.');
            }
        },
        error: function () {
            alert('Đã xảy ra lỗi khi lấy dữ liệu chi tiết hóa đơn.');
        }
    });
}



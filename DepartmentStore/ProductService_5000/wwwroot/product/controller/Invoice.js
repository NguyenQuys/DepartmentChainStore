let global_idPaymentMethod;
let checkChooseDeliveryMethod = false;
let listIdProducts = [];
let listQuantities = [];
let listProduct = [];

async function CheckPromotion(event) {
    event.preventDefault();
    let promotionCode = $('#input_promotion_code').val();

    $('#div_check_promotion_available').html('<div class="spinner-border text-primary" role="status"><span class="sr-only">Loading...</span></div>');

    try {
        const response = await fetch('/Promotion/GetByPromotionCode?' + new URLSearchParams({
            promotionCode: promotionCode,
            listIdProductsAndQuantity: JSON.stringify(ListIdProductsAndQuantities())
        }), {
            method: 'GET'
        });

        if (!response.ok) {
            throw new Error('HTTP error ' + response.status);
        }

        const data = await response.json();

        if (data.result === 1) {
            $('#div_check_promotion_available').html('<p class="text-success">Chúc mừng. Bạn đã áp dụng voucher thành công</p>');
        } else {
            $('#div_check_promotion_available').html(`<p class="text-danger">${data.message}</p>`);
        }

        let discount = data.data;
        $('#discount_invoice').text('- ' + discount.toLocaleString('vi-VN') + ' VND').addClass('text-success');
        UpdateTotalStore();

    } catch (error) {
        // Xử lý lỗi
        console.error('Fetch error:', error);
        $('#div_check_promotion_available').html('<p class="text-danger">Đã xảy ra lỗi khi áp dụng mã khuyến mãi.</p>');
    } finally {
        // Ẩn spinner khi hoàn tất
        $('.spinner-border').remove();
    }
}

function ToggleDelivery() {
    const pickUpAtStore = document.getElementById('pick_up_at_store');
    const btnChooseLocation = document.getElementById('btn_choose_location');
    if (pickUpAtStore.checked) {
        if (btnChooseLocation) {
            btnChooseLocation.classList.add('d-none');
        }
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
    if (global_idPaymentMethod === 2) {
        $('#div_address_invoice').removeClass('d-none');

    } else {
        $('#div_address_invoice').addClass('d-none');
    }

    if (!checkChooseDeliveryMethod) {
        ShowToastNoti('error', '', 'Vui lòng chọn phương thức vận chuyển', 4000);
    } else {
        if (idUser !== 0) {
            $.ajax({
                url: '/User/GetCustomerById',
                type: 'GET',
                data: { id: idUser },
                success: function (response) {
                    $('#input_address').val(response.address);
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
        else {
            $('#div_address_invoice').addClass('d-none');
        }
        $('#modal_information').modal('show');
    }
}

function AddInvoice() {
    let listSinglePrice = [];
    $('.single-price-product').each(function () {
        listSinglePrice.push($(this).val());
    });

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
    formData.append('Address', $('#input_address').val());

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
                indexButton.classList.add('btn', 'btn-success');
                $('#modal_body_information').addClass('text-center');
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

async function AddInvoiceOffline() {
    const formData = new FormData();
    formData.append('Promotion', $('#input_promotion_code').val() ?? 0);
    formData.append('ListIdProductsAndQuantities', JSON.stringify(ListIdProductsAndQuantities()));
    formData.append('SumPrice', parseFloat(document.getElementById('final_price').innerText.replace(/[^\d]/g, '')));
    formData.append('IdPaymentMethod', 3);
    formData.append('IdBranch', global_idBranch);

    try {
        const response = await fetch('/Invoice/AddAtStoreOffline', {
            method: 'POST',
            body: formData, 
        });

        if (!response.ok) {
            throw new Error('Network response was not ok');
        }

        const data = await response.json();

        if (data.result === 1) {
            ShowToastNoti('success', '', data.message, 4000);
            $('#div_selectedProduct').html('');
        } else {
            ShowToastNoti('error', '', data.message, 4000);
        }
    } catch (error) {
        console.error('Error adding invoice:', error);
        ShowToastNoti('error', '', 'Có lỗi xảy ra khi tạo hóa đơn.', 4000);
    }
}

function ListIdProductsAndQuantities() {
    //let listIdProducts = [];
    //let listQuantities = [];

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
    // With multiple params
    //const param = new URLSearchParams({
    //    id: idInvoice,
    //    //others
    //});

    fetch(`/Invoice/GetDetailsInvoice?id=${idInvoice}`, {
        method: 'GET',
    })
        .then(response => {
            if (!response.ok) {
                throw new Error('Network response was not ok');
            }
            return response.json();
        })
        .then(response => {
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
                        <h2>Địa chỉ: ${response.address ?? ''}</h2>
                        <h2>Ghi chú từ khách hàng: ${response.customerNote ?? ''}</h2>
                        <h2>Ghi chú từ cửa hàng: ${response.storeNote ?? ''}</h2>
                        <table style='width: 100%; border-collapse: collapse;'>
                            <thead>
                                <tr class='bg-primary text-white'>
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

                document.querySelector('#modal_purchaseHistory .modal-body').innerHTML = bodyDetail;
                new bootstrap.Modal(document.getElementById('modal_purchaseHistory')).show();
            } else {
                alert('Không tìm thấy dữ liệu chi tiết hóa đơn.');
            }
        })
        .catch(error => {
            console.error('Error fetching invoice details:', error);
            alert('Đã xảy ra lỗi khi lấy dữ liệu chi tiết hóa đơn.');
        });
}

function GetDetailsInvoice(idInvoice) {
    $.ajax({
        url: '/Invoice/GetDetailsInvoice',
        type: 'GET',
        data: { id: idInvoice },
        success: function (response) {
            if (response.idStatus === 1) {
                $('#div_action').removeClass('d-none');
            }

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
                        <h2>Địa chỉ: ${response.address ?? ''}</h2>
                        <h2>Ghi chú từ khách hàng: ${response.customerNote ?? ''}</h2>
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
                        <h3 class='my-3'>Phương thức thanh toán: ${response.paymentMethod || 'N/A'}</h3>
                        <h3 class='my-3'>Trạng thái: <span id='invoice_status'>${response.status || 'N/A'}</span></h3>
                        <div>
                            <div class='d-flex my-3'>
                                <h3>Nhân viên giao hàng</h3>
                                <input class='ms-3' type='text' id='employeeShip_invoice'}>
                            </div>
                            <div class='d-flex ${response.status !== "Đang chờ xử lý" ? 'd-none' : ''}'>
                                <h3>Ghi chú</h3>
                                <input class='ms-3 store-note-invoice' type='text'>
                            </div>
                        </div>
                        <div class='mt-3 my-3 d-flex justify-content-evenly ${response.status !== "Đang chờ xử lý" ? 'd-none' : ''}' id='div_action'>
                            <button type='button' class='btn btn-success' onclick='ChangeStatusInvoice(${response.idInvoice},2)'>Hoàn tất đóng gói</button>
                            <button type='button' class='btn btn-danger' onclick='ChangeStatusInvoice(${response.idInvoice},5)'>Huý đơn hàng</button>
                        </div>
                    </div>
                `;

                $('#div_data_detail_branch').html(bodyDetail);
            } else {
                alert('Không tìm thấy dữ liệu chi tiết hóa đơn.');
            }
        },
        error: function () {
            alert('Đã xảy ra lỗi khi lấy dữ liệu chi tiết hóa đơn.');
        }
    });
}

// Store
function AddProductToQueue(idProduct, productName, price) {
    let product = listProduct.find(p => p.id === idProduct);

    if (product) {
        product.quantity += 1;
    } else {
        listProduct.push({
            id: idProduct,
            name: productName,
            price: price,
            quantity: 1
        });
    }
    UpdateIdProductsAndQuantities();
    RenderSelectedProducts();
}

function UpdateIdProductsAndQuantities() {
    // Reset lại danh sách trước khi cập nhật
    listIdProducts = [];
    listQuantities = [];

    listProduct.forEach(product => {
        listIdProducts.push(product.id);
        listQuantities.push(product.quantity);
    });
}

function RenderSelectedProducts() {
    // Làm trống div trước khi hiển thị lại
    $('#div_selectedProduct').html('');
    let quantityProduct = 0;
    let sumAllProductPrice = 0;

    // Tạo bảng hiển thị sản phẩm
    let table = `
    <table border="1" style="width: 100%; border-collapse: collapse;">
        <thead>
            <tr>
                <th style="text-align: left; padding: 8px;">STT</th>
                <th style="text-align: left; padding: 8px;">Tên sản phẩm</th>
                <th style="text-align: left; padding: 8px;">Số lượng</th>
                <th style="text-align: left; padding: 8px;">Đơn giá</th>
                <th style="text-align: left; padding: 8px;">Tổng</th>
            </tr>
        </thead>
        <tbody>
    `;

    // Lặp qua các sản phẩm để hiển thị
    listProduct.forEach((product, index) => {
        let total = product.price * product.quantity;
        sumAllProductPrice += total;
        quantityProduct += product.quantity;

        table += `
        <tr>
            <td style="padding: 8px;">${index + 1}</td>
            <td style="padding: 8px;">${product.name}</td>
            <td style="padding: 8px;">${product.quantity}</td>
            <td style="padding: 8px;">${product.price.toLocaleString()}đ</td>
            <td style="padding: 8px;">${total.toLocaleString()}đ</td>
        </tr>
        `;
    });

    // Lấy giá trị giảm giá từ discount_invoice
    let discountA = parseFloat($('#discount_invoice').text().replace(/[^\d\-]/g, '')) || 0;
    let finalPrice = sumAllProductPrice + discountA; // Cộng discount (giá trị âm)
    // Tính giá cuối cùng

    // Ghi vào bảng
    table += `
        <tr class="fw-bold text-danger">
            <td style="padding: 8px;" colspan="2">Tổng</td>
            <td style="padding: 8px;">${quantityProduct}</td>
            <td style="padding: 8px;"></td>
            <td style="padding: 8px;">${sumAllProductPrice.toLocaleString('vi-VN')}đ</td>
        </tr>
        <tr>
            <td style="padding: 8px;" colspan="4">Giảm giá</td>
            <td style="padding: 8px;" id='discount_invoice'></td>
        </tr>
        <tr>
            <td style="padding: 8px;" colspan="4">Giá cuối cùng</td>
            <td style="padding: 8px;" id='final_price'>${finalPrice.toLocaleString('vi-VN')}đ</td>
        </tr>
        </tbody>
    </table>
            <div class="m-3" id="div_check_promotion_available"></div>

    <div class='d-flex justify-content-end'>
        <input type="text" class="form-control text-left px-3" placeholder="Nhập mã giảm giá..." id="input_promotion_code">
        <p><button type="submit" onclick="CheckPromotion(event)" class="btn btn-primary m-3">Áp dụng</button></p>
        <button type='button' class='btn btn-success m-3' onclick='AddInvoiceOffline()'>Hoàn tất</button>
    </div>
    `;

    $('#div_selectedProduct').html(table);

}

function UpdateTotalStore() {
    // Lấy giá trị tổng tiền sản phẩm
    let sumAllProductPrice = listProduct.reduce((sum, product) => sum + product.price * product.quantity, 0);

    // Lấy giá trị giảm giá từ discount_invoice
    let discountA = parseFloat($('#discount_invoice').text().replace(/[^\d\-]/g, '')) || 0;
    console.log('Discount:', discountA);

    // Tính giá cuối cùng
    let finalPrice = sumAllProductPrice + discountA;
    console.log('Final Price:', finalPrice);

    // Cập nhật giá cuối cùng vào DOM
    $('#final_price').text(finalPrice.toLocaleString('vi-VN') + 'đ');
}

const discountElement = document.querySelector('#discount_invoice'); // Lấy phần tử đầu tiên có class 'discount_invoice'

if (discountElement) {
    const observerA = new MutationObserver(UpdateTotalStore);

    observerA.observe(discountElement, { childList: true, subtree: true });
} else {
    console.error('Element with class "discount_invoice" not found!');
}

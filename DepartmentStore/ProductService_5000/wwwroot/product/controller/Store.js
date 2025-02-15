﻿ //Gọi hàm khi DOM đã sẵn sàng
document.addEventListener('DOMContentLoaded', function () {
    RenderProduct_BranchTab();
});

async function RenderProduct_BranchTab() {
    try {
        const response = await fetch('/Product/Product_BranchTab', {
            method: 'GET', 
        });

        if (!response.ok) {
            throw new Error('HTTP error ' + response.status); 
        }

        const data = await response.json(); 

        const productContainer = document.querySelector('#div_content_store');
        productContainer.classList.add('row'); 
        productContainer.innerHTML = '';  

        let productHTML = `
        <div class="col-lg-7 ftco-animate fadeInUp ftco-animated">
            <div class="row">`;

        data.forEach(product => {
            productHTML += `
                <div class="col-lg-4">
                    <div class="product">
                        <a onclick="AddProductToQueue(${product.id}, '${product.productName}', ${product.price})" class="img-prod">
                            <img class="img-fluid" src="${product.mainImage}" style="height:200px" alt="Product Image">
                            <span class="status">30%</span>
                            <div class="overlay"></div>
                        </a>
                        <div class="text py-3 pb-4 px-3 text-center">
                            <h3>${product.productName}</h3>
                            <div class="d-flex">
                                <div class="pricing">
                                    <p class="price">
                                        <span class="mr-2">
                                            ${product.price.toLocaleString('vi-VN')} VND
                                        </span>
                                    </p>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>`;
        });

        productHTML += `
            </div>
        </div>`; 

        const selectedProductHTML = `
        <div class="col-lg-5 bg-secondary">
            <div class="container">
                <div><p class="text-center">Danh sách chọn</p></div>
                <hr>
                <div class="bg-secondary" id="div_selectedProduct"></div>
            </div>
        </div>`;

        productContainer.innerHTML = productHTML + selectedProductHTML;

    } catch (error) {
        console.error('Error fetching products:', error); 
        const productContainer = document.querySelector('#div_content_store');
        productContainer.innerHTML = '<p class="text-danger">Không thể tải sản phẩm. Vui lòng thử lại sau.</p>'; // Hiển thị thông báo lỗi
    }
}

async function RenderTableInvoice(idStatus = null) {
    let tableBody = '';
    $('#div_content_store').html(`
        <div class="d-flex align-items-center justify-content-center">
            <div class="spinner-border text-primary" role="status">
                <span class="sr-only">Loading...</span>
            </div>
        </div>
    `);

    try {
        const response = await fetch(`/Invoice/GetListInvoiceBranch?` + new URLSearchParams({
            idBranch: global_idBranch,
            idStatus: idStatus
        }));

        if (!response.ok) {
            throw new Error('Network response was not ok');
        }

        const data = await response.json();

        if (!data || data.length === 0) {
            tableBody = '<tr><td colspan="6" class="text-center">Không có dữ liệu để hiển thị</td></tr>';
        } else {
            data.forEach((invoice, index) => {
                let rowClass = '';

                if (invoice.idPaymentMethod === 1) {
                    rowClass = 'bg-info text-white';
                }

                if (invoice.idStatus === 8) {
                    rowClass = 'bg-warning text-white'; 
                }

                tableBody += `
                    <tr class="${rowClass}">
                        <td>${index + 1}</td>
                        <td>${invoice.invoiceNumber || 'N/A'}</td>
                        <td>${invoice.createdDate ? new Date(invoice.createdDate).toLocaleString('vi-VN') : 'N/A'}</td>
                        <td>${invoice.status?.type || 'N/A'}</td>
                        <td>
                            <a href="javascript:void(0)" class="btn btn-danger" onclick="OpenModalInvoiceStore(${invoice.id}, ${idStatus})">Chi tiết</a>
                        </td>
                    </tr>
                `;
            });
        }

        $('#div_content_store').html(`
            <table class="table table-striped">
                <thead>
                    <tr class="text-white bg-primary">
                        <th>STT</th>
                        <th>Mã đơn hàng</th>
                        <th>Thời gian</th>
                        <th>Trạng thái</th>
                        <th>Hành động</th>
                    </tr>
                </thead>
                <tbody>${tableBody}</tbody>
            </table>
        `);

        $('#div_content_store').removeClass('row');

    } catch (error) {
        console.error('Error fetching invoices:', error);
        $('#div_content_store').html('<p class="text-danger">Không thể tải danh sách hóa đơn. Vui lòng thử lại sau.</p>');
    }
}

async function OpenModalInvoiceStore(idInvoice, idStatus) {
    try {
        const response = await fetch(`/Invoice/GetDetailsInvoice?id=${idInvoice}`, {
            method: 'GET',
        });

        if (!response.ok) {
            throw new Error('Failed to fetch invoice details.');
        }

        const data = await response.json();

        if (data) {
            let productRows = '';
            let initialTotal = data.total + data.discount;
            let index = 0;

            if (data.productNameAndQuantity && typeof data.productNameAndQuantity === 'object') {
                for (let productName in data.productNameAndQuantity) {
                    if (data.productNameAndQuantity.hasOwnProperty(productName)) {
                        let quantity = data.productNameAndQuantity[productName];
                        let singlePrice = data.singlePrice[index] || 0;
                        let totalPrice = quantity * singlePrice;
                        productRows += `
                            <tr>
                                <td style='border: 1px solid #ddd; padding: 8px;'>${productName}</td>
                                <td style='border: 1px solid #ddd; padding: 8px;'>${quantity}</td>
                                <td style='border: 1px solid #ddd; padding: 8px;'>${singlePrice.toLocaleString('vi-VN')} VND</td>
                                <td style='border: 1px solid #ddd; padding: 8px;'>${totalPrice.toLocaleString('vi-VN')} VND</td>
                            </tr>
                        `;
                        index++;
                    }
                }
            } else {
                productRows = '<tr><td colspan="4" style="text-align: center; padding: 8px;">Không có sản phẩm</td></tr>';
            }

            let buttonsHTML = '';
            if (data.paymentMethod === "Nhận tại cửa hàng") {
                buttonsHTML += `<button class="btn btn-success" onclick="ChangeStatusInvoiceStore(${data.idInvoice}, 4)">Hoàn tất đơn hàng</button>`;
            } else {
                buttonsHTML += `
                <button class="btn btn-success" onclick="ChangeStatusInvoiceStore(${data.idInvoice}, 2)">Hoàn tất đóng gói</button>
            `;
            }

            let bodyDetail = `
                <div class="container p-4">
                    <h2 class="text-center text-primary">Chi tiết hóa đơn #${data.invoiceNumber || 'N/A'}</h2>
                    <div class="mb-4">
                        <p><strong>Thời gian:</strong> ${data.time ? new Date(data.time).toLocaleString('vi-VN') : 'N/A'}</p>
                        <p><strong>Địa chỉ giao hàng:</strong> ${data.address || 'N/A'}</p>
                        <p><strong>Ghi chú từ khách hàng:</strong> ${data.customerNote || 'Không có'}</p>
                        <p><strong>Số điện thoại:</strong> ${data.phoneNumber || 'N/A'}</p>
                    </div>
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
                                <td style='border: 1px solid #ddd; padding: 8px;'>${initialTotal.toLocaleString('vi-VN')} VND</td>
                            </tr>
                            <tr>
                                <td colspan='3' style='border: 1px solid #ddd; padding: 8px; text-align: right; font-weight: bold;'>Giảm giá:</td>
                                <td style='border: 1px solid #ddd; padding: 8px;'>-${data.discount.toLocaleString('vi-VN')} VND</td>
                            </tr>
                            <tr>
                                <td colspan='3' style='border: 1px solid #ddd; padding: 8px; text-align: right; font-weight: bold;'>Tổng cộng:</td>
                                <td style='border: 1px solid #ddd; padding: 8px;'>${data.total.toLocaleString('vi-VN')} VND</td>
                            </tr>
                        </tbody>
                    </table>
                    <p><strong>Phương thức thanh toán:</strong> ${data.paymentMethod || 'N/A'}</p>
                    <p><strong>Trạng thái:</strong> <span id="invoice_status" class="badge badge-info">${data.status || 'N/A'}</span></p>
                    <div class="mt-4 ${data.status !== 'Đang chờ xử lý' ? 'd-none' : ''}">
                        ${data.paymentMethod !== 'Nhận tại cửa hàng' ? `
                            <h5>Nhân viên giao hàng</h5>
                            <select id="employee_select" class="form-control mb-3">
                                <option selected disabled value="">Chọn nhân viên giao hàng</option>
                                <option value="22">emvvn1</option>
                                <option value="23">emvvn2</option>
                            </select>
                        ` : ''}
                        <h5>Ghi chú</h5>
                        <input id="note_store_invoice" class="form-control mb-3" type="text" placeholder="Nhập ghi chú">
                        <div class="d-flex justify-content-around">
                            ${buttonsHTML}
                            <button class="btn btn-danger" onclick="ChangeStatusInvoiceStore(${data.idInvoice}, 5)">Huỷ đơn hàng</button>
                        </div>
                    </div>
                </div>
            `;

            document.getElementById('body_invoiceStore').innerHTML = bodyDetail;

            $('.modal-store').modal('show');
        } else {
            alert('Không tìm thấy dữ liệu chi tiết hóa đơn.');
        }
    } catch (error) {
        alert('Đã xảy ra lỗi khi lấy dữ liệu chi tiết hóa đơn.');
        console.error(error);
    }
}


//async function RenderEmployee() {
//    try {
//        const response = await fetch(`/User/GetEmployeesByIdBranch?idBranch=${global_idBranch}`, {
//            method: 'GET'
//        });

//        if (response.ok) {
//            const employees = await response.json(); 
//            const selectElement = document.getElementById('employee_select'); 

//            selectElement.innerHTML = '';

//            const defaultOption = document.createElement('option');
//            defaultOption.value = '';
//            defaultOption.textContent = 'Chọn nhân viên giao hàng';
//            selectElement.appendChild(defaultOption);

//            // Populate the select element with employee data
//            employees.forEach(employee => {
//                const option = document.createElement('option');
//                option.value = employee.id; 
//                option.textContent = employee.name; 
//                selectElement.appendChild(option);
//            });
//        } else {
//            console.error('Failed to fetch employees:', response.statusText);
//            alert('Không thể tải danh sách nhân viên. Vui lòng thử lại sau.');
//        }
//    } catch (error) {
//        console.error('Error:', error);
//        alert('Đã xảy ra lỗi khi tải danh sách nhân viên.');
//    }
//}

async function ChangeStatusInvoiceStore(idInvoice, idStatus) {
    let confirmationMessage = '';
    if (idStatus === 2) {
        confirmationMessage = 'Hoàn tất đóng gói?';
    } else if (idStatus === 3)
    {
        confirmationMessage = 'Giao hàng thành công?';
    }
    else if (idStatus === 4) {
        confirmationMessage = 'Bạn đã nhận được hàng?';
    }
    else if (idStatus === 5) {
        confirmationMessage = 'Cửa hàng hủy đơn hàng?';
    }
    else if (idStatus === 5) {
        confirmationMessage = 'Khách hàng hủy đơn hàng?';
    } else if (idStatus === 7) {
        confirmationMessage = 'Shipper hủy đơn hàng?';
    }

    if (confirmationMessage && confirm(confirmationMessage)) {
        const data = {
            IdInvoice: idInvoice,
            IdStatus: idStatus,
            EmployeeShip: document.getElementById('employee_select')?.value || null,
            StoreNote: document.getElementById('note_store_invoice')?.value || null
        };

        try {
            const response = await fetch('/Invoice/ChangeStatusInvoice', {
                method: 'PUT',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(data)
            });

            if (response.ok) {
                ShowToastNoti('success', '', 'Trạng thái đã thay đổi thành công!', 4000);
                document.getElementById('div_action')?.classList.add('d-none');

                if (idStatus === 2) {
                    const invoiceStatus = document.getElementById('invoice_status');
                    if (invoiceStatus) {
                        invoiceStatus.textContent = 'Đóng gói đơn hàng thành công';
                        invoiceStatus.classList.add('text-success');
                    }
                    document.getElementById('note_store_invoice').readOnly = true;
                } else if (idStatus === 5) {
                    const invoiceStatus = document.getElementById('invoice_status');
                    if (invoiceStatus) {
                        invoiceStatus.textContent = 'Đơn hàng đã hủy';
                        invoiceStatus.classList.add('text-danger');
                    }
                }
                $('.modal-store').modal('hide');
                RenderTableInvoice(1);
                
            } else {
                console.error('Failed to update status:', response.statusText);
                ShowToastNoti('error', 'Error', 'Có lỗi xảy ra, vui lòng thử lại!', 4000);
            }
        } catch (error) {
            console.error('Error:', error);
            ShowToastNoti('error', 'Error', 'Có lỗi xảy ra, vui lòng thử lại!', 4000);
        }
    } else {
        console.log('User canceled the action.');
    }
}

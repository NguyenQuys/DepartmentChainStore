document.addEventListener('DOMContentLoaded', function () {
    RenderOrderList();
});

async function RenderOrderList() {
    try {
        const response = await fetch('/Invoice/GetListInvoiceByIdShipper', {
            method: 'GET'
        });

        if (!response.ok) {
            throw new Error('Failed to fetch invoice details.');
        }

        const data = await response.json();
        let tableBody = '';

        if (data.length === 0) {
            tableBody = '<tr><td colspan="3" class="text-center">Không có đơn đi ship</td></tr>';
        } else {
            data.forEach((item, index) => {
                tableBody += `
                    <tr class="text-center">
                        <td class="col-1">${index + 1}</td>
                        <td class="col-7">#${item.invoiceNumber}</td>
                        <td class="col-4">
                            <button type="button" class="btn btn-primary btn-sm p-2" onclick="OpenModalInvoiceShipper(${item.idInvoice})">Chi tiết</button>
                        </td>
                    </tr>`;
            });
        }

        document.getElementById('data_order_shipper').innerHTML = tableBody;
    } catch (error) {
        console.error('Error fetching order list:', error.message);
    }
}

async function OpenModalInvoiceShipper(idInvoice) {
    try {
        const response = await fetch(`/Invoice/GetDetailsInvoice?id=${idInvoice}`, {
            method: 'GET',
        });

        if (!response.ok) {
            throw new Error('Failed to fetch invoice details.');
        }

        const data = await response.json();

        if (data) {
            const productRows = data.productNameAndQuantity
                ? Object.keys(data.productNameAndQuantity).map((productName, index) => {
                    const quantity = data.productNameAndQuantity[productName];
                    const singlePrice = data.singlePrice[index] || 0;
                    const totalPrice = quantity * singlePrice;

                    return `
                          <tr>
                              <td style='border: 1px solid #ddd; padding: 8px;'>${productName}</td>
                              <td style='border: 1px solid #ddd; padding: 8px;'>${quantity}</td>
                              <td style='border: 1px solid #ddd; padding: 8px;'>${singlePrice.toLocaleString('vi-VN')} VND</td>
                              <td style='border: 1px solid #ddd; padding: 8px;'>${totalPrice.toLocaleString('vi-VN')} VND</td>
                          </tr>`;
                }).join('')
                : `<tr><td colspan="4" style="text-align: center; padding: 8px;">Không có sản phẩm</td></tr>`;

            const bodyDetail = `
                <div style='width: 100%; font-family: Arial, sans-serif;'>
                    <h1>Hóa đơn mua hàng #${data.invoiceNumber || 'N/A'}</h1>
                    <h2>Thời gian: ${data.time ? new Date(data.time).toLocaleString('vi-VN') : 'N/A'}</h2>
                    <h2>Địa chỉ: ${data.address ?? ''}</h2>
                    <h2>Ghi chú từ khách hàng: ${data.customerNote ?? ''}</h2>
                    <h2>Số điện thoại: ${data.phoneNumber}</h2>
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
                                <td style='border: 1px solid #ddd; padding: 8px;'>${(data.total + data.discount).toLocaleString('vi-VN')} VND</td>
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
                    <h3 class='my-3'>Trạng thái: <span id='invoice_status'>${data.status || 'N/A'}</span></h3>
                    <div id='div_action' class='d-flex mt-3 my-3 justify-content-around'>
                        <button type='button' class='btn btn-success' onclick='ChangeStatusInvoiceShipper(${data.idInvoice}, 3)'>Hoàn tất giao hàng</button>
                        <button type='button' class='btn btn-danger' onclick='ChangeStatusInvoiceShipper(${data.idInvoice}, 7)'>Huỷ đơn hàng</button>
                    </div>
                </div>`;

            document.getElementById('body_shipper').innerHTML = bodyDetail;
            $('.modal-shipper').modal('show');
        } else {
            alert('Không tìm thấy dữ liệu chi tiết hóa đơn.');
        }
    } catch (error) {
        alert('Đã xảy ra lỗi khi lấy dữ liệu chi tiết hóa đơn.');
        console.error('Error fetching invoice details:', error);
    }
}

async function ChangeStatusInvoiceShipper(idInvoice, idStatus) {
    const confirmationMessage = idStatus === 3 ? 'Giao hàng thành công?' : 'Shipper hủy đơn hàng?';

    if (confirm(confirmationMessage)) {
        try {
            const response = await fetch('/Invoice/ChangeStatusInvoice', {
                method: 'PUT',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({ IdInvoice: idInvoice, IdStatus: idStatus })
            });

            if (response.ok) {
                ShowToastNoti('success', '', 'Trạng thái đã thay đổi thành công!', 4000);

                const invoiceStatus = document.getElementById('invoice_status');
                if (invoiceStatus) {
                    invoiceStatus.textContent = idStatus === 3 ? 'Đóng gói đơn hàng thành công' : 'Đơn hàng đã hủy';
                    invoiceStatus.className = idStatus === 3 ? 'text-success' : 'text-danger';
                }

                document.getElementById('div_action')?.classList.add('d-none');
                $('.modal-shipper').modal('hide');
                RenderOrderList(); // Re-render the list
            } else {
                ShowToastNoti('error', 'Error', 'Có lỗi xảy ra, vui lòng thử lại!', 4000);
            }
        } catch (error) {
            console.error('Error updating status:', error);
            ShowToastNoti('error', 'Error', 'Có lỗi xảy ra, vui lòng thử lại!', 4000);
        }
    }
}

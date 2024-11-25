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

            let bodyDetail = `
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
                    <h3 class='my-3'>Trạng thái: <span id='invoice_status'>${data.status || 'N/A'}</span></h3>
                    <div>
                        <div class='${data.status !== "Đang chờ xử lý" ? 'd-none' : 'd-flex'}'>
                            <h3>Ghi chú</h3>
                            <input class='ms-3' id='note_store_invoice' type='text'>
                        </div>
                    </div>
                    <div class='mt-3 my-3 justify-content-around d-flex' id='div_action'>
                        <button type='button' class='btn btn-success' onclick='ChangeStatusInvoiceStore(${data.idInvoice},3)'>Hoàn tất giao hàng</button>
                        <button type='button' class='btn btn-danger' onclick='ChangeStatusInvoiceStore(${data.idInvoice},7)'>Huỷ đơn hàng</button>
                    </div>
                </div>
            `;

            document.getElementById('body_shipper').innerHTML = bodyDetail;

            $('.modal-shipper').modal('show');
        } else {
            alert('Không tìm thấy dữ liệu chi tiết hóa đơn.');
        }
    } catch (error) {
        alert('Đã xảy ra lỗi khi lấy dữ liệu chi tiết hóa đơn.');
        console.error(error);
    }
}
function RenderTableImportProduct(idBranch) {
    let tableBody = '';

    $.ajax({
        url: '/Product_Branch/ViewHistoryExportByIdBranch',
        type: 'GET',
        data: { idBranch: idBranch },
        success: function (response) {
            if (response.length === 0) {
                tableBody = '<tr><td colspan="6" class="text-center">Không có dữ liệu để hiển thị</td></tr>';
            } else {
                response.forEach(function (ex, index) {
                    const date = new Date(ex.dateImport);
                    const formattedDate = date.toLocaleString("vi-VN", {
                        day: "2-digit",
                        month: "2-digit",
                        year: "numeric",
                        hour: "2-digit",
                        minute: "2-digit",
                        hour12: false  // Định dạng 24 giờ
                    });

                    tableBody += `
                                <tr>
                                    <td>${index + 1}</td>
                                    <td>${ex.productName}</td>
                                    <td>${ex.batchNumber}</td>
                                    <td>${ex.quantity}</td>
                                    <td>${formattedDate}</td>
                                    <td>${ex.consignee}</td>
                                </tr>`;
                });
            }

            $('#div_data_detail_branch').html(`
                <div>
                    <table class="table table-striped">
                        <thead>
                            <tr class='bg-primary text-white'>
                                <th>STT</th>
                                <th>Tên sản phẩm</th>
                                <th>Số lô hàng</th>
                                <th>Số lượng</th>
                                <th>Ngày nhập</th>
                                <th>Người nhận</th>
                            </tr>
                        </thead>
                        <tbody>${tableBody}</tbody>
                    </table>
                </div>
            `);
        },
        error: function (error) {
            alert('Có lỗi xảy ra khi tải danh sách chi nhánh.');
        }
    });
}
function RenderCustomerTable() {
    $('#div_table_product').hide();
    $('#div_table_batch').hide();
    $('#div_table_branch').hide();
    $('#div_table_customer').show();

    let tableBody = ''; 

    $.ajax({
        url: '/User/GetCustomerList',
        type: 'GET',
        success: function (response) {
            if (response.length === 0) {
                tableBody = '<tr><td colspan="5" class="text-center">Không có dữ liệu để hiển thị</td></tr>';
            } else {
                response.forEach(function (cus, index) {
                    tableBody += `
                    <tr>
                        <td>${index + 1}</td> 
                        <td>${cus.fullName}</td>
                        <td>${cus.phoneNumber}</td>
                        <td style="text-align: center; vertical-align: middle;">
                            <div class="form-check form-switch" style="display: inline-block;">
                                <input class="form-check-input" type="checkbox">
                            </div>
                        </td>
                        <td>
                            <a href="javascript:void(0)" class="btn btn-info" onclick="OpenModalDetailCustomer(${cus.idUser})">Chi tiết</a>
                        </td>
                    </tr>
                    `;
                });
            }
            $('#div_table_customer').html(`
            <div>
                <table class="table table-striped">
                    <thead>
                        <tr class='bg-primary text-white'>
                            <th>STT</th>
                            <th>Họ Tên</th>
                            <th>Số điện thoại</th>
                            <th>Trạng thái</th>
                            <th>Hành động</th>
                        </tr>
                    </thead>
                    <tbody>${tableBody}</tbody>
                </table>
            </div>

            `);
        }
    });
}

function OpenModalDetailCustomer(idCustomer) {
    $.ajax({
        url: '/User/GetCustomerById',
        type: 'GET',
        data: { id: idCustomer },
        success: function (response) {
            $('#fullName').val(response.fullName);
            $('#phoneNumber').val(response.phoneNumber);
            $('#email').val(response.email);
            $('#dateOfBirth').val(response.dateOfBirth);
            $('#gender').val(response.gender === 1 ? 'Nam' : (response.gender === 0 ? 'Nữ' : 'Khác')); 
            $('#updatedAt').val(response.updatedAt ?? 'Không có thông tin');
            $('#loginTime').val(response.loginTime ?? 'Không có thông tin');
            $('#logoutTime').val(response.logoutTime ?? 'Không có thông tin');

            $('#modal_customer').modal('show');
        },
        error: function () {
            alert('Có lỗi xảy ra khi lấy thông tin khách hàng');
        }
    });
}

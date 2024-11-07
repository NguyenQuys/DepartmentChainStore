function RenderCustomerTable() {
    $('#div_table_product,#div_table_batch,#div_table_branch,#div_table_promotion').hide();
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
                            <div class="form-check form-switch d-inline-block">
                                <input class="form-check-input toggle-status-customer" type="checkbox" data-customer-id="${cus.idUser}" ${!cus.isActive ? 'checked' : ''}>
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

$(document).ready(function () {
    $(document).on('click', '.toggle-status-customer', function () {
        var customerId = $(this).data('customer-id');
        ChangeStatusCustomer(customerId);
    });
});

function ChangeStatusCustomer(customerId) {
    $.ajax({
        url: '/User/ChangeStatusCustomer',
        type: 'PUT',
        data: { id: customerId },
        success: function (response) {
            ShowToastNoti('success', '', response, 4000, 'topRight');
        },
        error: function (error) {
            ShowToastNoti('error', '', error, 4000, 'topRight');
        }
    });
}

function RenderCustomerTable() {
    $('#div_table_product,#div_table_batch,#div_table_branch,#div_table_promotion,#div_table_export').hide();
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
    // Gỡ bỏ sự kiện trước khi gắn lại để tránh bị gọi nhiều lần
    $(document).off('click', '.toggle-status-customer').on('click', '.toggle-status-customer', function () {
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


function RenderSignUpBody(action) {
    let body = '';
    if (action === 'signUp') {
        $('#modal_signup .modal-title').text('Tạo tài khoản mới');
        body = `
        <form id="signupForm">
            <div class="mb-3">
                <label for="fullName" class="form-label">Họ và tên</label>
                <input type="text" class="form-control" id="fullName" name="fullName" maxlength="60" required>
            </div>
            <div class="mb-3">
                <label for="email" class="form-label">Email</label>
                <input type="email" class="form-control" id="email" name="email" maxlength="30" required>
            </div>
            <div class="mb-3">
                <label for="phoneNumber" class="form-label">Số điện thoại</label>
                <input type="text" class="form-control" id="phoneNumber" name="phoneNumber" maxlength="10" required>
            </div>
            <div class="mb-3">
                <label for="address" class="form-label">Địa chỉ</label>
                <input type="text" class="form-control" id="address" name="address" maxlength="30" required>
            </div>
            <div class="mb-3">
                <label for="password" class="form-label">Mật khẩu</label>
                <input type="password" class="form-control" id="password" name="password" maxlength="61" required>
            </div>
            <div class="mb-3">
                <label for="confirmPassword" class="form-label">Xác nhận mật khẩu</label>
                <input type="password" class="form-control" id="confirmPassword" name="confirmPassword" maxlength="61" required>
            </div>
            <div class="mb-3">
                <label for="dateOfBirth" class="form-label">Ngày sinh</label>
                <input type="date" class="form-control" id="dateOfBirth" name="dateOfBirth" required>
            </div>
            <div class="mb-3">
                <label for="gender" class="form-label">Giới tính</label>
                <select class="form-select" id="gender" name="gender" required>
                    <option value="" selected>Chọn giới tính</option>
                    <option value="0">Nam</option>
                    <option value="1">Nữ</option>
                    <option value="2">Khác</option>
                </select>
            </div>
        </form>
        <div class="modal-footer">
            <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Đóng</button>
            <button type="submit" class="btn btn-primary" onclick="SignUp()">Đăng ký</button>
        </div>
    `;
    } else if (action === 'reIdentify') {
        $('#modal_signup .modal-title').text('Xác thực lại tài khoản');
        body = ` <form>
                    <div class="mb-3">
                        <label class="form-label">Email</label>
                        <input type="email" class="form-control" id="resendEmail" maxlength="60" placeholder='Nhập tài khoản Email...' required>
                    </div>
                 </form>
                 <div class="modal-footer">
                    <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Đóng</button>
                    <button type="submit" class="btn btn-primary" onclick="RequestResendOTP()">Gửi lại mã OTP</button>
                </div>
                `;
    }

    $('#div_content_signup').html(body);
    $('#modal_signup').modal('show');
    
}

async function SignUp() {
    const data = {
        PhoneNumber: document.getElementById('phoneNumber').value,
        Password: document.getElementById('password').value,
        FullName: document.getElementById('fullName').value,
        Email: document.getElementById('email').value,
        DateOfBirth: document.getElementById('dateOfBirth').value,
        Gender: document.getElementById('gender').value,
        Address: document.getElementById('address').value
    };

    try {
        const response = await fetch('/User/SignUp', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify(data),
        });

        if (!response.ok) {
            throw new Error(`HTTP error! status: ${response.status}`);
        }

        const result = await response.json();

        if (result.result === 1) {
            ShowToastNoti('success', '', 'Tạo tài khoản thành công, mã OTP đã được gửi vào email của bạn', 4000);
            $('#div_content_signup').html(`
                <label>Nhập mã OTP đã được gửi vào email của bạn</label>
                <input type="number" class="form-control" placeholder="Nhập mã OTP..." id="otpCode">
                <a style="cursor:pointer" class='mt-3 d-flex justify-content-center' onclick="RequestResendOTP('${result.email}')">Gửi lại mã OTP</a>
                <button class="btn btn-primary m-3 float-end" onclick="VerifyOTP()">Xác nhận OTP</button>
            `);
        } else if (result.result === -1) {
            ShowToastNoti('error', '', result.message, 4000);
            RenderSignUpBody(); 
        }
    } catch (err) {
        console.error('Error occurred:', err);
        alert('An error occurred while signing up. Please try again.');

        RenderSignUpBody();
    }
}

async function VerifyOTP() {
    try {
        const otpCode = document.getElementById('otpCode').value;

        if (!otpCode) {
            alert('Vui lòng nhập mã OTP.');
            return;
        }

        const response = await fetch('/User/ValidateOTP', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(otpCode) 
        });

        const result = await response.json();

        if (result.result === 1) {
            ShowToastNoti('success', result.message, 'Bạn đã có thể đăng nhập', 4000);
            $('#modal_signup').modal('hide');
        } else {
            ShowToastNoti('error', '', result.message, 4000);
        }
    } catch (error) {
        console.error('Error occurred:', error);
        alert('Đã xảy ra lỗi khi xác thực OTP. Vui lòng thử lại.');
    }
}

async function RequestResendOTP(emailSignIn) {
    let email = null;
    if (emailSignIn != null) {
        email = emailSignIn;
    }
    else {
        email = $('#resendEmail').val();
    }

    const response = await fetch(`/User/ResendOTP?email=${email}`, {
        method: 'POST',
        headers: {
            'Content-Type': 'applaication/json'
        }
    });
    
    if (!response.ok) {
        throw new Error(`HTTP error! status: ${response.status}`);
    }

    const result = await response.json();
    if (result.result === 1) {
        ShowToastNoti('success', '', result.message, 4000);
        let body = `
            <label>Nhập mã OTP đã được gửi vào email của bạn</label>
            <input type="number" class="form-control" placeholder="Nhập mã OTP..." id="otpCode">
            <button class="btn btn-primary m-3 float-end" onclick="VerifyOTP()">Xác nhận OTP</button>
        `;
        $('#div_content_signup').html(body);
    } else {
        ShowToastNoti('error', '', result.message, 4000);
        $('#modal_signup').modal('hide');
    }
}

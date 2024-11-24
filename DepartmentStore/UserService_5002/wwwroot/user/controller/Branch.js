let currentType = '';
let idBranch_global;
let idUser_global;

// Render Branch Table
function RenderBranchTable() {
    $('#div_table_product, #div_table_batch, #div_table_customer, #div_table_export,#div_table_promotion').hide();
    $('#div_table_branch').show();
    let tableBody = '';

    $.ajax({
        url: '/Branch/GetAllBranches',
        type: 'GET',
        success: function (response) {
            if (response.length === 0) {
                tableBody = '<tr><td colspan="6" class="text-center">Không có chi nhánh để hiển thị</td></tr>';
            } else {
                response.forEach(function (branch, index) {
                    tableBody += `
                                <tr>
                                    <td>${index + 1}</td> 
                                    <td>${branch.location}</td>
                                    <td></td>
                                    <td>
                                        <a href="javascript:void(0)" onclick="StatisticRevenue(${branch.id})" class="btn btn-dark">Thống kê</a>
                                        <a href="javascript:void(0)" onclick="OpenModalDetailBranch(${branch.id}, '${branch.location}')" class="btn btn-info">Chi tiết</a>
                                        <a href="javascript:void(0)" onclick="OpenModalBranch('updateBranch', ${branch.id})" class="btn btn-primary">Sửa</a>
                                        <a href="javascript:void(0)" onclick="RemoveBranch(${branch.id})" class="btn btn-danger">Xóa</a>
                                    </td>
                                </tr>
                                `;
                });
            }

            $('#div_table_branch').html(`
                            <div>
                                <button type="button" class="btn btn-primary m-4" id="btn_add_branch" data-bs-toggle="modal" onclick="OpenModalBranch('addBranch')">
                                    Thêm chi nhánh
                                </button>
                            </div>
                            <div>
                                <table class="table table-striped">
                                    <thead>
                                        <tr class='bg-primary text-white'>
                                            <th>STT</th>
                                            <th>Chi nhánh</th>
                                            <th>Quản lý</th>
                                            <th>Hành động</th>
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

// Open Modal branch
function OpenModalBranch(type, branchId = null) {
    let modalBranchTitle = $('#modal_title_branch');
    const btnBranch = $('#btn_branch');

    // Reset form
    $('#add_form_branch')[0].reset();
    btnBranch.off('click');  // Clear previous click events

    if (type === 'addBranch') {
        modalBranchTitle.text('Thêm chi nhánh mới');
        btnBranch.text('Thêm').on('click', addBranch);  // Bind add branch handler

    } else if (type === 'updateBranch') {
        modalBranchTitle.text('Cập nhật chi nhánh');
        btnBranch.text('Cập nhật').on('click', function () {
            UpdateBranch(branchId);
        });

        // Fetch branch data if branchId is provided
        if (branchId) {
            $.ajax({
                url: '/Branch/GetById',
                type: 'GET',
                data: { id: branchId },
                success: function (response) {
                    $('#location').val(response.location);
                    $('#account').val(response.account);
                    $('#password').val(response.password);
                },
                error: function (error) {
                    ShowToastNoti('error', '', 'Unable to fetch branch details', 4000, 'topRight');
                }
            });
        }
    }

    new bootstrap.Modal(document.getElementById('modal_branch')).show();
}

function OpenModalDetailBranch(idBranch, location) {
    let modalTitle = $('#modal_title_detail_branch').html(`Chi tiết chi nhánh <span class="text-primary">${location}</span>`);
    idBranch_global = idBranch;
    OnChangeTypeBranchDetail($('.nav-detail-branch').eq(0), 'product_branch');

    new bootstrap.Modal(document.getElementById('modal_detail_branch')).show();
}

// Add Branch
function addBranch() {
    const branchLocation = $('#location').val();
    const account = $('#account').val();
    const password = $('#password').val();

    const formData = new FormData();
    formData.append('Location', branchLocation);
    formData.append('Account', account);
    formData.append('Password', password);

    $.ajax({
        url: '/Branch/Create',
        type: 'POST',
        data: formData,
        contentType: false,
        processData: false,
        success: function (response) {
            if (response.result === 1) {
                ShowToastNoti('success', '', response.message, 4000, 'topRight');
                $('#modal_branch').modal('hide');  
                $('#add_form_branch')[0].reset(); 
                RenderBranchTable();
            } else {
                ShowToastNoti('error', '', response.message, 4000, 'topRight');
            }
        },
        error: function (error) {
            ShowToastNoti('error', '', error, 4000, 'topRight');
        }
    });
}

// Update Branch
function UpdateBranch(branchId) {
    const formDataBranch = new FormData();
    formDataBranch.append('Id', branchId);
    formDataBranch.append('Location', $('#location').val());
    formDataBranch.append('Account', $('#account').val());
    formDataBranch.append('Password', $('#password').val());

    $.ajax({
        url: '/Branch/Update',
        type: 'PUT',
        data: formDataBranch,  // Use formDataBranch here
        processData: false,
        contentType: false,
        success: function (response) {
            if (response.result === 1) {
                ShowToastNoti('success', '', response.message, 4000, 'topRight');
                $('#modal_branch').modal('hide');
                RenderBranchTable();
            } else {
                ShowToastNoti('error', '', response.message, 4000, 'topRight');
            }
        },
        error: function (error) {
            ShowToastNoti('error', '', error, 4000, 'topRight');
        }
    });
}

function RemoveBranch(idBranch) {
    if (confirm('Bạn có chắc chắn muốn xóa chi nhánh này không?')) {
        $.ajax({
            url: `/Branch/Remove`,
            type: 'DELETE',
            data: { id: idBranch },
            success: function (response) {
                ShowToastNoti('success', '', response, 4000, 'topRight');
                RenderBranchTable();
            },
            error: function (xhr, status, error) {
                alert('Có lỗi xảy ra khi xóa sản phẩm.');
            }
        });
    }
}

function OnChangeTypeBranchDetail(elm, type) {
    currentType = type;
    $(".nav-detail-branch").removeClass("active");
    $(elm).addClass("active");

    if (type === 'product_branch') {
        RenderTableProductBranch();
    } else if (type === 'staff') {
        RenderTableStaff(idBranch_global);
    } else if (type === 'importProduct') {
        RenderTableImportProduct(idBranch_global);
    } else if (type === 'invoice') {
        RenderTableInvoice(idBranch_global);
    }
}

function RenderTableProductBranch() {
    let tableBody = '';

    $.ajax({
        url: '/Product_Branch/GetListByIdBranch',
        type: 'GET',
        data: { idBranch: idBranch_global },
        success: function (response) {
            if (response.result === 1) {
                if (response.data.length === 0) {
                    tableBody = '<tr><td colspan="4" class="text-center">Không có dữ liệu để hiển thị</td></tr>';
                } else {
                    response.data.forEach(function (pb, index) {
                        tableBody += `
                                    <tr>
                                        <td>${index + 1}</td>
                                        <td>${pb.productName}</td>
                                        <td>${pb.batchNumber}</td>
                                        <td>${pb.quantity}</td>
                                    </tr>`;
                    });
                }

                $('#div_data_detail_branch').html(`
                            <div>
                                <table class="table table-striped">
                                    <thead>
                                        <tr class="bg-primary text-white">
                                            <th>STT</th>
                                            <th>Tên sản phẩm</th>
                                            <th>Số lô hàng</th>
                                            <th>Số lượng hiện tại</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        ${tableBody}
                                    </tbody>
                                </table>
                            </div>
                            `);
            } else {
                ShowToastNoti('error', '', response.message, 4000, 'topRight');
            }
        },
        error: function () {
            ShowToastNoti('error', '', 'Lỗi khi tải dữ liệu', 4000, 'topRight');
        }
    });
}

//+++++++++++++++++++++++Staff Area starts+++++++++++++++++++++++++++++
function RenderTableStaff(idBranch) {
    let tableBody = '';

    $.ajax({
        url: '/User/GetListUserByIdBranch',
        type: 'GET',
        data: { idBranch: idBranch },
        success: function (response) {
            if (response.length === 0) {
                tableBody = '<tr><td colspan="6" class="text-center">Không có dữ liệu để hiển thị</td></tr>';
            }
            else {
                response.forEach(function (staff, index) {
                    tableBody += `
                                    <tr>
                                        <td>${index + 1}</td>
                                        <td>${staff.fullName}</td>
                                        <td>${staff.email}</td>
                                        <td>${staff.dateOfBirth}</td>
                                        <td>${staff.salary.toLocaleString('vi-VN')}</td>
                                        <td>
                                            <a href="javascript:void(0)" onclick="GetUserById(${staff.userId})" class="btn btn-primary">Sửa</a>
                                            <a href="javascript:void(0)" onclick="RemoveStaff(${staff.userId})" class="btn btn-danger">Xóa</a>
                                        </td>
                                    </tr>`;
                });
            }

            $('#div_data_detail_branch').html(`
                            <div>
                                <button type="button" class="btn btn-primary m-4" id="btn_add_branch" data-bs-toggle="modal" onclick="DivActionStaff('addStaff')">
                                    <i class='mdi mdi-account-plus'></i>
                                    Thêm nhân sự
                                </button>
                            </div>
                            <div>
                                <table class="table table-striped">
                                    <thead>
                                        <tr class='bg-primary text-white'>
                                            <th>STT</th>
                                            <th>Họ tên</th>
                                            <th>Email</th>
                                            <th>Ngày sinh</th>
                                            <th>Mức lương</th>
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

function DivActionStaff(type) {
    let buttonStaff = '';

    if (type === 'addStaff') {
        buttonStaff = `<button type="button" id="btn_submit_staff" class="btn btn-primary">Thêm nhân sự</button>`;
    } else if (type === 'updateStaff') {
        buttonStaff = `<button type="button" id="btn_submit_staff" class="btn btn-primary">Cập nhật</button>`;
    }

    $('#div_data_detail_branch').html(`
                <form id="add_form_staff">
                    <div class="mb-3">
                        <label class="form-label">Họ tên</label>
                        <input type="text" class="form-control" id="fullName" required>
                    </div>
                    <div class="mb-3">
                        <label class="form-label">Số điện thoại</label>
                        <input type="text" class="form-control" id="phoneNumber" maxlength="10" required>
                    </div>
                    <div class="mb-3">
                        <label class="form-label">Email</label>
                        <input type="email" class="form-control" id="email" required>
                    </div>
                    <div class="mb-3">
                        <label class="form-label">Giới tính</label>
                        <select class="form-control" id="gender" required>
                            <option value="0">Nam</option>
                            <option value="1">Nữ</option>
                            <option value="2">Khác</option>
                        </select>
                    </div>
                    <div class="mb-3">
                        <label class="form-label">Chức vụ</label>
                        <select class="form-control" id="roleId" required>
                            <option selected disabled>--Chọn chức vụ</option>
                            <option value="1">Quản lý</option>
                            <option value="2">Nhân viên</option>
                        </select>
                    </div>
                    <div class="mb-3">
                        <label class="form-label">Ngày sinh</label>
                        <input type="date" class="form-control" id="dateOfBirth" required>
                    </div>
                    <div class="mb-3">
                        <label class="form-label">Ngày vào làm</label>
                        <input type="date" class="form-control" id="beginDate" required>
                    </div>
                    <div class="mb-3">
                        <label class="form-label">Mức lương</label>
                        <input type="number" class="form-control" id="salary" required>
                    </div>
                    ${buttonStaff}
                </form>
            `);

    $('#btn_submit_staff').on('click', function () {
        if (type === 'addStaff') {
            AddStaff(idBranch_global);
        }
        else if(type === 'updateStaff') {
            UpdateStaff(idBranch_global);
        }
    });
}

function AddStaff(idBranch) {
    var formData = new FormData();
    formData.append('FullName', $('#fullName').val());
    formData.append('PhoneNumber', $('#phoneNumber').val());
    formData.append('Email', $('#email').val());
    formData.append('DateOfBirth', $('#dateOfBirth').val());
    formData.append('Gender', $('#gender').val());
    formData.append('RoleId', $('#roleId').val());
    formData.append('IdBranch', idBranch);
    formData.append('BeginDate', $('#beginDate').val());
    formData.append('Salary', $('#salary').val());

    $.ajax({
        url: '/User/AddStaff',
        type: 'POST',
        data: formData,
        processData: false,
        contentType: false,
        success: function (response) {
            if (response.result === 1) {
                ShowToastNoti('success', '', response.message, 4000, 'topRight');
                RenderTableStaff(idBranch);
            } else {
                ShowToastNoti('error', '', response.message, 4000, 'topRight');
            }
        },
        error: function (error) {
            alert('Có lỗi xảy ra khi thêm nhân viên.');
        }
    });
}

function GetUserById(idStaff) {
    idUser_global = idStaff;
    DivActionStaff('updateStaff');

    $.ajax({
        url: '/User/GetById',
        type: 'GET',
        data: { id: idStaff },
        success: function (response) {
            $('#fullName').val(response.fullName);
            $('#phoneNumber').val(response.phoneNumber);
            $('#email').val(response.email);
            $('#gender').val(response.gender);
            $('#roleId').val(response.roleId);
            $('#dateOfBirth').val(response.dateOfBirth);
            $('#beginDate').val(response.beginDate);
            $('#salary').val(response.salary);
        },
        error: function (error) {
            ShowToastNoti('error', '', 'Unable to fetch branch details', 4000, 'topRight');
        }
    });
}

function UpdateStaff(idBranch) {
    var formData = new FormData();
    formData.append('IdUser', idUser_global);
    formData.append('FullName', $('#fullName').val());
    formData.append('PhoneNumber', $('#phoneNumber').val());
    formData.append('Email', $('#email').val());
    formData.append('DateOfBirth', $('#dateOfBirth').val());
    formData.append('Gender', $('#gender').val());
    formData.append('RoleId', $('#roleId').val());
    formData.append('IdBranch', idBranch);
    formData.append('BeginDate', $('#beginDate').val());
    formData.append('Salary', $('#salary').val());

    $.ajax({
        url: '/User/UpdateStaff',
        type: 'PUT',
        data: formData,
        processData: false,
        contentType: false,
        success: function (response) {
            ShowToastNoti('success', '', response.message, 4000, 'topRight');
            RenderTableStaff(idBranch);
        },
        error: function (error) {
            ShowToastNoti('error', '', error.responseJSON.message , 4000, 'topRight');
        }
    });
}

function RemoveStaff(idStaff) {
    if (confirm('Bạn có chắc chắn muốn xóa nhân sự này không?')) {
        $.ajax({
            url: `/User/DeleteStaff`,
            type: 'DELETE',
            data: { id: idStaff },
            success: function (response) {
                ShowToastNoti('success', '', response, 4000, 'topRight');
                RenderTableStaff(idBranch_global);
            },
            error: function (xhr, status, error) {
                alert('Có lỗi xảy ra khi xóa sản phẩm.');
            }
        });
    }
}
//+++++++++++++++++++++++Staff Area Ends+++++++++++++++++++++++++++++
//++++++++++++++++++++++++Invoice Area Starts++++++++++++++++++++++++
//function RenderTableInvoice(idBranch) {
//    let tableBody = '';

//    $.ajax({
//        url: '/Invoice/GetListInvoiceBranch',
//        type: 'GET',
//        data: { idBranch: idBranch },
//        success: function (response) {
//            if (response.length === 0) {
//                tableBody = '<tr><td colspan="6" class="text-center">Không có dữ liệu để hiển thị</td></tr>';
//            }
//            else {
//                response.forEach(function (invoice, index) {
//                    tableBody += `
//                                    <tr>
//                                        <td>${index + 1}</td>
//                                        <td>${invoice.invoiceNumber}</td>
//                                        <td>${invoice.createdDate}</td>
//                                        <td>${invoice.status.type}</td>
//                                        <td>
//                                            <a href="javascript:void(0)" class="btn btn-primary" onclick="GetDetailsInvoice(${invoice.id})">Chi tiết</a>
//                                        </td>
//                                    </tr>`;
//                });
//            }

//            $('#div_data_detail_branch').html(`
//                            <div>
//                                <table class="table table-striped">
//                                    <thead>
//                                        <tr class='bg-primary text-white'>
//                                            <th>STT</th>
//                                            <th>Mã đơn hàng</th>
//                                            <th>Thời gian</th>
//                                            <th>Trạng thái</th>
//                                            <th>Hành động</th>
//                                        </tr>
//                                    </thead>
//                                    <tbody>${tableBody}</tbody>
//                                </table>
//                            </div>
//                        `);
//        }
//    });
//}

function ChangeStatusInvoice(idInvoice, idStatus) {
    let confirmationMessage = '';
    if (idStatus === 2) {
        confirmationMessage = 'Hoàn tất đơn hàng?';
    } else if (idStatus === 5) {
        confirmationMessage = 'Hủy đơn hàng?';
    }

    if (confirmationMessage && confirm(confirmationMessage)) {
        const data = {
            IdInvoice: idInvoice,
            IdStatus: idStatus,
            EmployeeShip: $('#employeeShip_invoice').val() || null,
            StoreNote: $('#note_store_invoice').val() || null
        };

        $.ajax({
            url: '/Invoice/ChangeStatusInvoice',
            type: 'PUT',
            data: JSON.stringify(data),
            contentType: 'application/json', 
            success: function (response) {
                ShowToastNoti('success', '', 'Trạng thái đã thay đổi thành công!', 4000);
                $('#div_action').addClass('d-none');
                if (idStatus === 2) {
                    $('#invoice_status').text('Đóng gói đơn hàng thành công').addClass('text-success');
                    $('.store-note-invoice').prop('readonly',true);
                } else if (idStatus === 5) {
                    $('#invoice_status').text('Đơn hàng đã hủy').addClass('text-danger');
                }
            },
            error: function (xhr, status, error) {
                console.error('Error:', error);
                ShowToastNoti('error', 'Error', 'Có lỗi xảy ra, vui lòng thử lại!', 4000);
            }
        });
    } else {
        console.log('User canceled the action.');
    }
}
//++++++++++++++++++++++++Invoice Area Ends++++++++++++++++++++++++

//$(document).ready(function () {
//    $('#div_droplist_select_branch').on('click', function () {
//        DropListBranch();
//    });
//});

//function DropListBranch() {
//    $.ajax({
//        url: '/branch/Branch/GetAllBranches', // Fix the URL path
//        type: 'GET',
//        success: function (data) {
//            // Clear the existing options
//            $('#div_droplist_select_branch').empty();

//            // Populate the dropdown with the received data
//            $.each(data, function (index, branch) {
//                $('#div_droplist_select_branch').append(
//                    $('<option></option>').val(branch.id).text(branch.name)
//                );
//            });
//        },
//        error: function (error) {
//            console.log('Error fetching branches:', error);
//        }
//    });
//}

// Render Branch Table
function RenderBranchTable() {
    $('#div_table_product').hide();
    $('#div_table_batch').hide();
    $('#div_table_branch').show();
    let tableBody = '';

    $.ajax({
        url: '/branch/Branch/GetAllBranches',
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
                            <a href="javascript:void(0)" onclick="OpenModalBranch('updateBranch', ${branch.id})" class="btn btn-info">Chi tiết</a>
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
            console.error('Error:', error);
            alert('Có lỗi xảy ra khi tải danh sách chi nhánh.');
        }
    });
}

// Other functions are the same as updated above...


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
                url: '/branch/Branch/GetById',
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

    // Show the modal
    new bootstrap.Modal(document.getElementById('modal_branch')).show();
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
        url: '/branch/Branch/Create',
        type: 'POST',
        data: formData,
        contentType: false,
        processData: false,
        success: function (response) {
            if (response.result === 1) {
                ShowToastNoti('success', '', response.message, 4000, 'topRight');
                $('#modal_branch').modal('hide');  // Correct modal ID
                $('#add_form_branch')[0].reset();   // Correct form ID
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
        url: '/branch/Branch/Update',
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

function RemoveBranch(idBanch) {
    if (confirm('Bạn có chắc chắn muốn xóa chi nhánh này không?')) {
        $.ajax({
            url: `/branch/Branch/Remove`,
            type: 'DELETE',
            data: { id: idBanch },
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


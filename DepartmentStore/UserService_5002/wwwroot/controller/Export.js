function ShowContentExport() {
    $('#div_table_product, #div_table_branch, #div_table_batch,#div_table_promotion').hide();
    $('#div_table_export').show().html(`
        <div class='container-fluid'>
            <div class="row my-4">
                <div class="col-lg-6 mb-3">
                    <label for="fileInput_exportProduct" class="form-label fw-bold">Import sản phẩm từ Excel</label>
                    <div class="input-group">
                        <input type="file" class="form-control" id="fileInput_exportProduct" accept=".xlsx, .xls" />
                        <button class="btn btn-outline-secondary" onclick="UploadExportProductByExcel()">Import</button>
                    </div>
                </div>
                <div class="col-lg-6 d-flex align-items-center">
                    <button class="btn btn-primary w-100 mt-3" onclick="ExportSampleProductFileExcel()">
                        <i class="mdi mdi-download"></i> Tải file mẫu
                    </button>
                </div>
            </div>

            <div class="row mb-4">
                <form id="export_filter" class="d-flex">
                    <div class="col-lg-5 mx-2">
                        <label for="product_select_export" class="form-label fw-bold">Sản phẩm</label>
                        <select id="product_select_export" class="form-select">
                        </select>
                    </div>
                    <div class="col-lg-5 mx-2">
                        <label for="timeInputExport" class="form-label fw-bold">Thời gian</label>
                        <input type="date" class="form-control p-3" id="timeInputExport">
                    </div>
                    <div class="col-lg-2 d-flex align-items-end">
                        <button type="submit" class="btn btn-success w-100">
                            <i class="mdi mdi-filter"></i> Lọc
                        </button>
                    </div>
                </form>
            </div>

            <div class="table-responsive">
                <table class="table table-striped table-hover align-middle">
                    <thead>
                        <tr class="bg-primary text-white text-center">
                            <th>STT</th>
                            <th>Chi nhánh</th>
                            <th>Tên sản phẩm</th>
                            <th>Lô hàng</th>
                            <th>Số lượng xuất</th>
                            <th>Người nhận</th>
                            <th>Ngày xuất</th>
                            <th>Hành động</th>
                        </tr>
                    </thead>
                    <tbody id="export_TableBody">
                    </tbody>
                </table>
            </div>
        </div>
    `);

    LoadProductOptionsExport();
    $('#export_filter').on('submit', function (event) {
        event.preventDefault(); // Prevent the default form submission
        const productId = $('#product_select_export').val();
        const time = $('#timeInputExport').val();
        RenderExportTable(productId, time); // submit in this spot is end in this function, not jump to LoadBatchData()
    });

    LoadAllExport();
}

function LoadProductOptionsExport() {
    $.ajax({
        url: '/product/Product/GetAllProducts',
        type: 'GET',
        dataType: 'json',
        success: function (data) {
            let selectHtml = '<option value="" selected disabled>-- Vui lòng chọn sản phẩm --</option>';
            if (data && data.length > 0) {
                data.forEach(function (product) {
                    selectHtml += `<option value="${product.id}">${product.productName}</option>`;
                });
            } else {
                selectHtml = '<option value="">Không có sản phẩm nào phù hợp</option>';
            }
            $('#product_select_export').html(selectHtml);
        },
        error: function () {
            $('#product_select_export').html('<option value="">Không thể lấy danh sách sản phẩm</option>');
        }
    });
}

function RenderExportTable(productId, time) {
    $.ajax({
        url: `/branch/Product_Branch/GetListByFilter`,
        type: 'POST',
        data: { IdProduct: productId, Time: time },
        success: function (response) {
            let tableBody = response.length === 0
                ? '<tr><td colspan="8" class="text-center">Không có lô hàng để hiển thị</td></tr>'
                : response.map((ex, index) => `
                    <tr>
                        <td>${index + 1}</td>
                        <td>${ex.locationBranch}</td>
                        <td>${ex.productName}</td>
                        <td>${ex.batchNumber}</td>
                        <td>${ex.quantity}</td>
                        <td>${new Date(ex.dateImport).toLocaleDateString()}</td>
                        <td>${ex.consignee}</td>
                        <td>
                            <a href="javascript:void(0)" onclick="OpenModalExport('updateExport', ${ex.id})" class="btn btn-primary">Sửa</a>
                            <a href="javascript:void(0)" onclick="RemoveExport(${ex.id})" class="btn btn-danger">Xóa</a>
                        </td>
                    </tr>
                `).join('');
            $('#export_TableBody').html(tableBody);
        },
        error: function () {
            $('#export_TableBody').html('<tr><td colspan="8" class="text-center text-danger">Không thể lấy danh sách lô hàng</td></tr>');
        }
    });
}

function LoadAllExport() {
    $.ajax({
        url: '/branch/Product_Branch/GetListByFilter',
        type: 'POST',
        data: { IdProduct: null, Time: null },
        success: function (response) {
            let tableBody = response.length === 0
                ? '<tr><td colspan="8" class="text-center">Không có lô hàng để hiển thị</td></tr>'
                : response.map((ex, index) => `
                    <tr>
                        <td>${index + 1}</td>
                        <td>${ex.locationBranch}</td>
                        <td>${ex.productName}</td>
                        <td>${ex.batchNumber}</td>
                        <td>${ex.quantity}</td>
                        <td>${ex.consignee}</td>
                        <td>${new Date(ex.dateImport).toLocaleDateString()}</td>
                        <td>
                            <a href="javascript:void(0)" onclick="OpenModalExport('updateExport', ${ex.id})" class="btn btn-primary">Sửa</a>
                            <a href="javascript:void(0)" onclick="RemoveExport(${ex.id})" class="btn btn-danger">Xóa</a>
                        </td>
                    </tr>
                `).join('');
            $('#export_TableBody').html(tableBody);
        },
        error: function () {
            $('#export_TableBody').html('<tr><td colspan="8" class="text-center text-danger">Không thể lấy danh sách lô hàng</td></tr>');
        }
    });
}

function UploadExportProductByExcel() {
    var fileInput = $('#fileInput_exportProduct')[0].files[0]; // Get the selected file

    if (!fileInput) {
        alert('Please select a file to upload.');
        return;
    }

    var formData = new FormData();
    formData.append('file', fileInput);
    $.ajax({
        url: '/branch/Product_Branch/UploadExportProductByExcel',
        type: 'POST',
        data: formData,
        contentType: false, // Important for file uploads
        processData: false, // Prevent jQuery from processing the FormData
        success: function (response) {
            if (response.result === 1) {
                ShowToastNoti('success', '', response.message, 4000, 'topRight');
                LoadAllExport();
            } else {
                ShowToastNoti('error', '', response.message, 4000, 'topRight');
            }
        },
        error: function (xhr, status, error) {
            alert('An error occurred while processing the request.');
        }
    });
}

function ExportSampleProductFileExcel() {
    window.location.href = '/branch/Product_Branch/ExportSampleProductFileExcel';
}

function OpenModalExport(type, id) {
    let modalExportTitle = $('#modal_title_export');
    const btnExport = $('#btn_export');

    $('#form_export')[0].reset();
    btnExport.off('click');

    if (type == 'updateExport') {
        modalExportTitle.text('Cập nhật');
        btnExport.text('Cập nhật').on('click', function () {
            UpdateExport(id);
        });
    }

    $.ajax({
        url: '/branch/Product_Branch/GetById',
        type: 'GET',
        data: { id: id },
        success: function (response) {
            $('#locationBranch_export').val(response.locationBranch);
            $('#productName_export').val(response.productName);
            $('#batchNumber_export').val(response.batchNumber);
            $('#quantity_export').val(response.quantity);
            $('#dateImport_export').val(response.dateImport);
            $('#consignee_export').val(response.consignee);
            $('#modal_export').modal('show');
        },
        error: function (xhr, status, error) {
            console.error('Error:', error);
            alert('Đã xảy ra lỗi khi tải dữ liệu.');
        }
    });
}

function UpdateExport(id) {
    const formData = new FormData();

    formData.append('Id', id);
    formData.append('LocationBranch', $('#locationBranch_export').val());
    formData.append('ProductName', $('#productName_export').val());
    formData.append('BatchNumber', $('#batchNumber_export').val());
    formData.append('Quantity', $('#quantity_export').val());
    formData.append('DateImport', $('#dateImport_export').val());
    formData.append('Consignee', $('#consignee_export').val());

    $.ajax({
        url: '/branch/Product_Branch/UpdateExport',
        type: 'PUT',
        data: formData,
        processData: false, 
        contentType: false, 
        success: function (response) {
            if (response.result === 1) {
                ShowToastNoti('success', '', response.message, 4000, 'topRight');
                $('#modal_export').modal('hide');
                LoadAllExport();
            }
            else {
                ShowToastNoti('error', '', response.message, 4000, 'topRight');
            }
        },
        error: function (xhr, status, error) {
            alert('Đã xảy ra lỗi khi cập nhật dữ liệu.');
        }
    });
}


function RemoveExport(id) {
    if (confirm('Bạn có chắc chắn muốn xóa lịch sử này?')) {
        $.ajax({
            url: '/branch/Product_Branch/Delete',
            type: 'DELETE',
            data: { id: id },
            success: function (response) {
                ShowToastNoti('success', '', response, 4000, 'topRight');
                LoadAllExport();
            }
        });
    }
}
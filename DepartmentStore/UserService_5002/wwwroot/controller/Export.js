function ShowContentExport() {
    $('#div_table_product, #div_table_branch, #div_table_batch').hide();
    $('#div_table_export').show().html(`
            <div class="row m-3">
            <div class="col-lg-7">
                <button class="btn btn-primary" id="btn_add_export" onclick="OpenModalBatch('addBatch')">Thêm lô hàng</button>
            </div>
            <div>
                <input type="file" id="fileInput_exportProduct" accept=".xlsx, .xls" />
                <button onclick="UploadExportProductByExcel()">Import Excel File</button>
            </div>
            <div class="col-lg-5"> 
                <form id="batch_filter">
                    <div class='row'>
                        <div class='col-lg-5'>
                        <select id="product_select_export" class="form-control">
                        </select>
                        </div>
                        <div class='col-lg-5'>
                            <input type="date" class='form-control' id="timeInput">
                        </div>
                        <div class='col-lg-2'>
                            <button type="submit" id='submit_batchFilter' class='btn btn-success'>
                            <i class='mdi mdi-filter'></i>
                            Lọc
                            </button>
                        </div>
                    </div>
                </form>
            </div>
        </div>
        <div>
            <table class="table table-striped">
                <thead>
                    <tr class='bg-primary text-white'>
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
                <tbody id="export_TableBody"></tbody>
            </table>
        </div>
    `);

    LoadProductOptionsExport();
}

function LoadProductOptionsExport() {
    $.ajax({
        url: '/list/Product/GetAllProducts',
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
                GetProductsByIdCategory(ID_CATEGORY);
            } else {
                ShowToastNoti('error', '', response.message, 4000, 'topRight');
            }
        },
        error: function (xhr, status, error) {
            alert('An error occurred while processing the request.');
        }
    });
}
function GetAllProducts(selectedProductId = null) {
    $.ajax({
        url: '/list/Product/GetAllProducts',
        type: 'GET',
        success: function (data) {
            if (data && data.length > 0) {
                let selectHtml = '<option disabled selected>-- Vui lòng chọn sản phẩm --</option>';
                data.forEach(function (product) {
                    selectHtml += `<option value="${product.id}">${product.productName}</option>`;
                });
                $('#productId_batch').html(selectHtml);

                // Set the selected product if provided
                if (selectedProductId) {
                    $('#productId_batch').val(selectedProductId);

                }
            }
        },
        error: function () {
            let errorHtml = '<option value="">Không thể lấy danh sách sản phẩm</option>';
            $('#productId_batch').html(errorHtml);
        }
    });
}

function ShowContentBatch() {
    $('#div_table_product').hide();
    $('#div_table_branch').hide();
    $('#div_table_batch').show();

    $('#div_table_batch').html(`
        <div class="row m-3">
            <div class="col-lg-10">
                <button class="btn btn-primary" id="btn_add_batch" onclick="OpenModalBatch('addBatch')">Thêm lô hàng</button>
            </div>
            <div class="col-lg-2"> 
                <select id="productSelect" class="form-control">
                    <option value="">-- Vui lòng chọn sản phẩm --</option>
                </select>
            </div>
        </div>
        <div>
            <table class="table table-striped">
                <thead>
                    <tr class='bg-primary text-white'>
                        <th>STT</th>
                        <th>Số lô hàng</th>
                        <th>Tên sản phẩm</th>
                        <th>Số lượng nhập</th>
                        <th>Số lượng còn lại</th>
                        <th>Ngày nhập</th>
                        <th>Người nhận</th>
                        <th>Hành động</th>
                    </tr>
                </thead>
                <tbody id="batchTableBody"></tbody>
            </table>
        </div>
    `);

    $.ajax({
        url: '/list/Product/GetAllProducts',
        type: 'GET',
        dataType: 'json',
        success: function (data) {
            if (data && data.length > 0) {
                let selectHtml = '<option value="">-- Vui lòng chọn sản phẩm --</option>';
                data.forEach(function (product) {
                    selectHtml += `<option value="${product.id}">${product.productName}</option>`;
                });
                $('#productSelect').html(selectHtml);
            }
        },
        error: function () {
            let errorHtml = '<option value="">Không thể lấy danh sách sản phẩm</option>';
            $('#productSelect').html(errorHtml);
        }
    });

    $('#productSelect').on('change', function () {
        const productId = $(this).val();
        if (productId) {
            RenderBatchTable(productId);
        } else {
            $('#batchTableBody').html('<tr><td colspan="7" class="text-center">Vui lòng chọn sản phẩm</td></tr>');
        }
    });
}

function RenderBatchTable(productId) {
    $.ajax({
        url: `/list/Batch/GetListByIdProduct`,
        type: 'GET',
        data: { id: productId },
        success: function (response) {
            let tableBody = '';
            if (response.length === 0) {
                tableBody = '<tr><td colspan="7" class="text-center">Không có lô hàng để hiển thị</td></tr>';
            } else {
                response.forEach(function (batch, index) {
                    tableBody += `
                    <tr>
                        <td>${index + 1}</td>
                        <td>${batch.batchNumber}</td>
                        <td>${batch.productName}</td>
                        <td>${batch.initQuantity}</td>
                        <td>${batch.remainingQuantity}</td>
                        <td>${new Date(batch.importDate).toLocaleDateString()}</td>
                        <td>${batch.receiver}</td>
                        <td>
                            <a href="javascript:void(0)" onclick="OpenModalBatch('updateBatch', ${batch.id})" class="btn btn-primary">Sửa</a>
                            <a href="javascript:void(0)" onclick="RemoveBatch(${batch.id})" class="btn btn-danger">Xóa</a>
                        </td>
                    </tr>
                    `;
                });
            }
            $('#batchTableBody').html(tableBody);
        },
        error: function () {
            let errorRow = '<tr><td colspan="7" class="text-center text-danger">Không thể lấy danh sách lô hàng</td></tr>';
            $('#batchTableBody').html(errorRow);
        }
    });
}

function OpenModalBatch(type, batchId = null) {
    let modalBatchTitle = $('#modal_title_batch');
    let btnBatch = $('#btn_batch');

    $('#add_form_batch')[0].reset();
    btnBatch.off('click');

    if (type === 'addBatch') {
        modalBatchTitle.text('Thêm lô hàng mới');
        btnBatch.text('Thêm').on('click', AddBatch);
        GetAllProducts();
    } else if (type === 'updateBatch') {
        modalBatchTitle.text('Cập nhật lô hàng');
        btnBatch.text('Cập nhật').on('click', function () {
            UpdateBatch(batchId);
        });

        if (batchId) {
            $.ajax({
                url: '/list/Batch/GetById',
                type: 'GET',
                data: { id: batchId },
                success: function (response) {
                    $('#batchNumber').val(response.batchNumber);
                    $('#expiryDate').val(response.expiryDate);
                    $('#initQuantity').val(response.initQuantity);
                    $('#importDate').val(response.importDate);
                    $('#receiver_batch').val(response.receiver);
                    // Load products and set the selected product
                    GetAllProducts(response.idProduct);
                },
                error: function () {
                    ShowToastNoti('error', '', 'Unable to fetch batch details', 4000, 'topRight');
                }
            });
        }
    }

    new bootstrap.Modal(document.getElementById('modal_batch')).show();
}

function AddBatch() {
    // Lấy các giá trị từ form
    const batchNumber = $('#batchNumber').val();
    const idProduct = $('#productId_batch').val();
    const expiryDate = $('#expiryDate').val();
    const initQuantity = $('#initQuantity').val();
    const importDate = $('#importDate').val();
    const receiver_batch = $('#receiver_batch').val();

    // Tạo một FormData để gửi dữ liệu qua AJAX
    const formData = new FormData();
    formData.append('BatchNumber', batchNumber);
    formData.append('IdProduct', idProduct);
    formData.append('ExpiryDate', expiryDate);
    formData.append('InitQuantity', initQuantity);
    formData.append('ImportDate', importDate);
    formData.append('Receiver', receiver_batch);

    // Gửi yêu cầu AJAX để thêm lô hàng
    $.ajax({
        url: '/list/Batch/Create',  
        type: 'POST',
        data: formData,
        contentType: false,
        processData: false,
        success: function (response) {
            if (response.result === 1) {
                ShowToastNoti('success', '', response.message, 4000, 'topRight');
                $('#modal_batch').modal('hide');  
                $('#add_form_batch')[0].reset();   
                RenderBatchTable(idProduct);       
            } else {
                ShowToastNoti('error', '', response.message, 4000, 'topRight');
            }
        },
        error: function (error) {
            ShowToastNoti('error', '', 'Không thể thêm lô hàng', 4000, 'topRight');
        }
    });
}

// Update Branch
function UpdateBatch(idBatch) {
    const formDataBatch = new FormData();
    formDataBatch.append('Id', idBatch);
    formDataBatch.append('BatchNumber', $('#batchNumber').val());
    formDataBatch.append('IdProduct', $('#productId_batch').val());
    formDataBatch.append('ExpiryDate', $('#expiryDate').val());
    formDataBatch.append('InitQuantity', $('#initQuantity').val());
    formDataBatch.append('ImportDate', $('#importDate').val());
    formDataBatch.append('Receiver', $('#receiver_batch').val());

    $.ajax({
        url: '/list/Batch/Update',
        type: 'PUT',
        data: formDataBatch, 
        processData: false,
        contentType: false,
        success: function (response) {
            if (response.result === 1) {
                ShowToastNoti('success', '', "Thêm lô hàng thành công", 4000, 'topRight');
                $('#modal_batch').modal('hide');
                RenderBatchTable($('#productId_batch').val());
            } else {
                ShowToastNoti('error', '', response.message, 4000, 'topRight');
            }
        },
        error: function (error) {
            ShowToastNoti('error', '', error, 4000, 'topRight');
        }
    });
}

function RemoveBatch(idBatch) {
    if (confirm('Bạn có chắc chắn muốn xóa lô hàng này không?')) {
        $.ajax({
            url: `/list/Batch/DeleteById`,
            type: 'DELETE',
            data: { id: idBatch },
            success: function (response) {
                ShowToastNoti('success', '', "Xóa lô hàng thành công", 4000, 'topRight');
                RenderBatchTable(response.idProduct);
            },
            error: function (xhr, status, error) {
                alert('Có lỗi xảy ra khi xóa sản phẩm.');
            }
        });
    }
}
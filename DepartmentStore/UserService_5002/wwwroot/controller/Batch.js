// Put info in add Batch
function GetAllProducts(selectedProductId = null) {
    $.ajax({
        url: '/list/Product/GetAllProducts',
        type: 'GET',
        success: function (data) {
            let selectHtml = '<option disabled selected>-- Vui lòng chọn sản phẩm --</option>';
            if (data && data.length > 0) {
                data.forEach(function (product) {
                    selectHtml += `<option value="${product.id}">${product.productName}</option>`;
                });
            }
            $('#productId_batch').html(selectHtml);

            // Set the selected product if provided
            if (selectedProductId) {
                $('#productId_batch').val(selectedProductId);
            }
        },
        error: function () {
            $('#productId_batch').html('<option value="">Không thể lấy danh sách sản phẩm</option>');
        }
    });
}

function ShowContentBatch() {
    $('#div_table_product, #div_table_branch, #div_table_export').hide();
    $('#div_table_batch').show().html(`
        <div class="row m-3">
            <div class="col-lg-7">
                <button class="btn btn-primary" id="btn_add_batch" onclick="OpenModalBatch('addBatch')">Thêm lô hàng</button>
            </div>
            <div class="col-lg-5"> 
                <form id="batch_filter">
                    <div class='row'>
                        <div class='col-lg-5'>
                        <select id="productSelect" class="form-control">
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

    LoadProductOptions();

    // Handle filter form submission
    $('#batch_filter').on('submit', function (event) {
        event.preventDefault(); // Prevent the default form submission
        const productId = $('#productSelect').val();
        const time = $('#timeInput').val();
        RenderBatchTable(productId, time); // submit in this spot is end in this function, not jump to LoadBatchData()
    });

    LoadBatchData(); // load when batch_filter not submit
}

function LoadProductOptions() {
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
            $('#productSelect').html(selectHtml);
        },
        error: function () {
            $('#productSelect').html('<option value="">Không thể lấy danh sách sản phẩm</option>');
        }
    });
}

function RenderBatchTable(productId, time) {
    $.ajax({
        url: `/list/Batch/GetListByFilter`,
        type: 'POST',
        data: { IdProduct: productId, Time: time },
        success: function (response) {
            let tableBody = response.length === 0
                ? '<tr><td colspan="8" class="text-center">Không có lô hàng để hiển thị</td></tr>'
                : response.map((batch, index) => `
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
                `).join('');
            $('#batchTableBody').html(tableBody);
        },
        error: function () {
            $('#batchTableBody').html('<tr><td colspan="8" class="text-center text-danger">Không thể lấy danh sách lô hàng</td></tr>');
        }
    });
}

function LoadBatchData() {
    $.ajax({
        url: '/list/Batch/GetAll',
        type: 'GET',
        success: function (response) {
            let tableBody = response.length === 0
                ? '<tr><td colspan="8" class="text-center">Không có lô hàng để hiển thị</td></tr>'
                : response.map((batch, index) => `
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
                `).join('');
            $('#batchTableBody').html(tableBody);
        },
        error: function () {
            $('#batchTableBody').html('<tr><td colspan="8" class="text-center">Không thể tải dữ liệu lô hàng</td></tr>');
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
    const formData = new FormData();
    formData.append('BatchNumber', $('#batchNumber').val());
    formData.append('IdProduct', $('#productId_batch').val());
    formData.append('ExpiryDate', $('#expiryDate').val());
    formData.append('InitQuantity', $('#initQuantity').val());
    formData.append('ImportDate', $('#importDate').val());
    formData.append('Receiver', $('#receiver_batch').val());

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
                RenderBatchTable($('#productId_batch').val());
            } else {
                ShowToastNoti('error', '', response.message, 4000, 'topRight');
            }
        },
        error: function () {
            ShowToastNoti('error', '', 'Không thể thêm lô hàng', 4000, 'topRight');
        }
    });
}

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
        type: 'POST',
        data: formDataBatch,
        contentType: false,
        processData: false,
        success: function (response) {
            if (response.result === 1) {
                ShowToastNoti('success', '', response.message, 4000, 'topRight');
                $('#modal_batch').modal('hide');
                RenderBatchTable($('#productId_batch').val());
            } else {
                ShowToastNoti('error', '', response.message, 4000, 'topRight');
            }
        },
        error: function () {
            ShowToastNoti('error', '', 'Không thể cập nhật lô hàng', 4000, 'topRight');
        }
    });
}

function RemoveBatch(batchId) {
    if (confirm('Bạn có chắc chắn muốn xóa lô hàng này?')) {
        $.ajax({
            url: '/list/Batch/Delete',
            type: 'POST',
            data: { id: batchId },
            success: function (response) {
                if (response.result === 1) {
                    ShowToastNoti('success', '', response.message, 4000, 'topRight');
                    LoadBatchData();
                } else {
                    ShowToastNoti('error', '', response.message, 4000, 'topRight');
                }
            },
            error: function () {
                ShowToastNoti('error', '', 'Không thể xóa lô hàng', 4000, 'topRight');
            }
        });
    }
}

function RenderPromotionTable() {
    $('#div_table_product, #div_table_batch, #div_table_customer, #div_table_export,#div_table_branch').hide();
    $('#div_table_promotion').show();
    let tableBody = '';

    $.ajax({
        url: '/Promotion/GetAll',
        type: 'GET',
        success: function (response) {
            if (response.length === 0) {
                tableBody = '<tr><td colspan="7" class="text-center">Không có dữ liệu để hiển thị</td></tr>';
            } else {
                response.forEach(function (promo, index) {
                    tableBody += `
                                <tr>
                                    <td>${index + 1}</td> 
                                    <td>${promo.code}</td>
                                    <td>${promo.percentage}</td>
                                    <td>${promo.remainingQuantity}</td>
                                    <td>${promo.timeUpdate}</td>
                                    <td class="text-center">
                                        <div class="form-check form-switch d-inline-block">
                                            <input class="form-check-input toggle-status-product" type="checkbox" data-product-id="${promo.id}" ${!promo.isActive ? 'checked' : ''}>
                                        </div>
                                    </td>
                                    <td>
                                        <a href="javascript:void(0)" onclick="OpenModalPromotion('updatePromotion', ${promo.id})" class="btn btn-primary">Sửa</a>
                                        <a href="javascript:void(0)" onclick="RemoveBranch(${promo.id})" class="btn btn-danger">Xóa</a>
                                    </td>
                                </tr>
                                `;
                });
            }
            $('#div_table_promotion').html(`
                            <div>
                                <button type="button" class="btn btn-primary m-4" id="btn_add_promotion" data-bs-toggle="modal" onclick="OpenModalPromotion('addPromotion')">
                                    Thêm voucher
                                </button>
                            </div>
                            <div>
                                <table class="table table-striped">
                                    <thead>
                                        <tr class='bg-primary text-white'>
                                            <th>STT</th>
                                            <th>Code</th>
                                            <th>Phần trăm</th>
                                            <th>Số lượng còn lại</th>
                                            <th>Thời gian cập nhật</th>
                                            <th>Trạng thái</th>
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

function OpenModalPromotion(type, promotionId = null) {
    let modalPromotionTitle = $('#modal_title_promotion');
    const btnPromotion = $('#btn_promotion');

    // Reset form
    $('#form_promotion')[0].reset();
    btnPromotion.off('click');  // Clear previous click events

    if (type === 'addPromotion') {
        modalPromotionTitle.text('Thêm khuyến mại mới');
        btnPromotion.text('Thêm').on('click', AddPromotion);  // Bind add promotion handler

    } else if (type === 'updatePromotion') {
        modalPromotionTitle.text('Cập nhật khuyến mại');
        btnPromotion.text('Cập nhật').on('click', function () {
            UpdatePromotion(promotionId);
        });

        // Fetch promotion data if promotionId is provided
        if (promotionId) {
            $.ajax({
                url: '/Promotion/GetById',
                type: 'GET',
                data: { id: promotionId },
                success: function (response) {
                    $('#code').val(response.code);
                    $('#percentage').val(response.percentage);
                    $('#applyFor').val(response.applyFor);
                    $('#minPrice').val(response.minPrice);
                    $('#maxPrice').val(response.maxPrice);
                    $('#initQuantity_promotion').val(response.initQuantity);
                    $('#remainingQuantity').val(response.remainingQuantity);
                    $('#timeUpdate').val(response.timeUpdate); // Make sure the datetime format is correct
                    $('#isActive').prop('checked', response.isActive); // Check the box if the promotion is active
                    $('#promotionType').val(response.idPromotionType); // Assuming you have the promotion type set
                },
                error: function (error) {
                    ShowToastNoti('error', '', 'Unable to fetch promotion details', 4000, 'topRight');
                }
            });
        }
    }
    $('#modal_promotion').modal('show');
//    LoadProductOptionsPromotion();
}
//function LoadProductOptionsPromotion() {
//    $.ajax({
//        url: '/Product/GetAllProducts',
//        type: 'GET',
//        dataType: 'json',
//        success: function (data) {
//            let selectHtml = '<option value="" selected disabled>-- Vui lòng chọn sản phẩm --</option>';
//            if (data && data.length > 0) {
//                data.forEach(function (product) {
//                    selectHtml += `<option value="${product.id}">${product.productName}</option>`;
//                });
//            } else {
//                selectHtml = '<option value="">Không có sản phẩm nào phù hợp</option>';
//            }
//            $('#product_select_export').html(selectHtml);
//        },
//        error: function () {
//            $('#product_select_export').html('<option value="">Không thể lấy danh sách sản phẩm</option>');
//        }
//    });
//}


function AddPromotion() {
    const code = $('#code').val();
    const percentage = $('#percentage').val();
    const applyFor = $('#applyFor').val();
    const minPrice = $('#minPrice').val();
    const maxPrice = $('#maxPrice').val();
    const initQuantity = $('#initQuantity_promotion').val();
    const remainingQuantity = $('#remainingQuantity').val();
    const timeUpdate = $('#timeUpdate').val();
    const isActive = $('#isActive').prop('checked') ? true : false;
    const promotionType = $('#promotionType').val();

    const formData = new FormData();
    formData.append('Code', code);
    formData.append('Percentage', percentage);
    formData.append('ApplyFor', applyFor);
    formData.append('MinPrice', minPrice);
    formData.append('MaxPrice', maxPrice);
    formData.append('InitQuantity', initQuantity);
    formData.append('RemainingQuantity', remainingQuantity);
    formData.append('TimeUpdate', timeUpdate);
    formData.append('IsActive', isActive);
    formData.append('PromotionType', promotionType);

    $.ajax({
        url: '/Promotion/Add',
        type: 'POST',
        data: formData,
        contentType: false,
        processData: false,
        success: function (response) {
            if (response.result === 1) {
                ShowToastNoti('success', '', response.message, 4000, 'topRight');
                $('#modal_promotion').modal('hide');
                $('#form_promotion')[0].reset();
                RenderPromotionTable();
            } else {
                ShowToastNoti('error', '', response.message, 4000, 'topRight');
            }
        },
        error: function (error) {
            ShowToastNoti('error', '', error, 4000, 'topRight');
        }
    });
}


//function HandlePromotionTypeChange() {
//    // Hide all apply_for_* containers
//    const applyForContainers = document.querySelectorAll('[id^="apply_for_"]');
//    applyForContainers.forEach(container => {
//        container.classList.add('d-none');
//    });

//    // Get selected value from promotionType
//    const selectedType = document.getElementById('promotionType').value;

//    // Show the relevant container based on selected value
//    const selectedContainer = document.getElementById(`apply_for_${selectedType}_container`);
//    if (selectedContainer) {
//        selectedContainer.classList.remove('d-none');
//    }
//}

//// Call the function once on page load to ensure the correct div is shown if a default is selected
//document.addEventListener('DOMContentLoaded', handlePromotionTypeChange);

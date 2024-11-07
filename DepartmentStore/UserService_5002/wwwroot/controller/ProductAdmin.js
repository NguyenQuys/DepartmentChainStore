let productIdToAction;
let ID_ROLE;
let ID_TAB;
let ID_CATEGORY;

function HandleTabClick(categoryId, tabId, idRole) {
    ID_ROLE = idRole;
    ID_TAB = tabId;
    ID_CATEGORY = categoryId;
    console.log(ID_TAB);
    GetProductsByIdCategory(categoryId);
    ActiveTogglePills(tabId);
}


function ActiveTogglePills(tabId) {
    $('.nav-link').removeClass('bg-primary text-white');
    $('#' + tabId).addClass('bg-primary text-white');
}

// Fetch products by category ID via AJAX
function GetProductsByIdCategory(id) {
    $.ajax({
        url: `/product/Product/GetProductsByCategory`,
        type: 'GET',
        data: {id:id},
        success: function (response) {
            if (response.result === 1) {
                RenderProductTable(response.data);
            } else {
                ShowToastNoti('error', '', response.message, 4000, '');
            }
        },
        error: function (xhr, status, error) {
            console.error('Error fetching products:', error);
        }
    });
}

// Render the product table
function RenderProductTable(products) {
    $('#div_table_branch, #div_table_batch, #div_table_customer, #div_table_export,#div_table_promotion').hide();
    $('#div_table_product').show();
    let tableBody = '';

    if (products.length === 0) {
        tableBody = '<tr><td colspan="6" class="text-center">Không có sản phẩm để hiển thị</td></tr>';
    } else {
        products.forEach(function (product, index) {
            const actionButtons = ID_ROLE === 1 ? `
                <td>
                    <a href="javascript:void(0)" onclick="OpenModalProduct('updateProduct', ${product.id})" class="btn btn-primary btn-sm me-1">Sửa</a>
                    <a href="javascript:void(0)" onclick="RemoveProduct(${product.id})" class="btn btn-danger btn-sm">Xóa</a>
                </td>
            ` : '';

            const statusToggle = ID_ROLE === 1 ? `
                <td class="text-center">
                    <div class="form-check form-switch d-inline-block">
                        <input class="form-check-input toggle-status-product" type="checkbox" data-product-id="${product.id}" ${!product.isHide ? 'checked' : ''}>
                    </div>
                </td>
            ` : '';

            tableBody += `
                <tr>
                    <td>${index + 1}</td> 
                    <td>${product.productName}</td>
                    <td>${product.price}</td>
                    ${statusToggle}
                    <td>${new Date(product.updatedTime).toLocaleString("en-GB")}</td>
                    ${actionButtons}
                </tr>
            `;
        });
    }

    $('#div_table_product').html(`
        <div class="container-fluid">
            ${ID_ROLE === 1 ? `
                <div class="my-4">
                    <div class="row mb-3">
                        <div class="col-lg-4 my-auto">
                            <button type="button" class="btn btn-primary w-100" id="btn_add_product" data-bs-toggle="modal" onclick="OpenModalProduct('addProduct')">
                                Thêm sản phẩm
                            </button>
                        </div>
                        <div class="col-lg-4 mb-3">
                            <label for="fileInput" class="form-label fw-bold">Import sản phẩm từ Excel</label>
                            <div class="input-group">
                                <input type="file" class="form-control" id="fileInput" accept=".xlsx, .xls" />
                                <button class="btn btn-outline-secondary" onclick="UploadExcelProductFile()">Import</button>
                            </div>
                        </div>
                        <div class="col-lg-4 d-flex align-items-center">
                            <button class="btn btn-primary w-100 mt-3" onclick="DownloadSampleFileProduct()">
                                <i class="mdi mdi-download"></i> Tải file mẫu
                            </button>
                        </div>
                    </div>
                </div>
            ` : ''}

            <div class="table-responsive">
                <table class="table table-striped table-hover align-middle">
                    <thead>
                        <tr class='bg-primary text-white'>
                            <th>STT</th> 
                            <th>Tên sản phẩm</th>
                            <th>Giá</th>
                            ${ID_ROLE === 1 ? '<th class="text-center">Trạng thái</th>' : ''}
                            <th>Cập nhật lần cuối</th>
                            ${ID_ROLE === 1 ? '<th>Hành động</th>' : ''}
                        </tr>
                    </thead>
                    <tbody>${tableBody}</tbody>
                </table>
            </div>
        </div>
    `);
}

$(document).ready(function () {
    $(document).on('click', '.toggle-status-product', function () {
        var productId = $(this).data('product-id'); // Lấy productId từ thuộc tính data của checkbox
        ChangeStatusProduct(productId); 
    });
});

function ChangeStatusProduct(productId) {
    $.ajax({
        url: '/product/Product/ChangeStatusProduct',
        type: 'PUT',
        data: { id: productId },
        success: function (response) {
            ShowToastNoti('success', '', response, 4000,'topRight');
        },
        error: function (xhr, status, error) {
            console.error('Error changing product status:', error);
            alert('Có lỗi xảy ra khi gửi yêu cầu');
        }
    });
}


// Handle the add product form submission
$(document).ready(function () {
    $('#btn_add_product').on('click', function () {
        OpenModalProduct('addProduct');
    });
});

// Add a new product
function addProduct() {
    const productName = $('#productName').val();
    const productPrice = $('#productPrice').val();
    const categoryId = $('#categoryId').val();
    const mainImage = $('#mainImage')[0].files[0];
    const secondaryImages = $('#secondaryImages')[0].files;

    const formData = new FormData();
    formData.append('ProductName', productName);
    formData.append('Price', productPrice);
    formData.append('CategoryId', categoryId);

    if (mainImage) {
        formData.append('MainImage', mainImage);
    }

    for (let i = 0; i < secondaryImages.length; i++) {
        formData.append('SecondaryImages', secondaryImages[i]);
    }

    $.ajax({
        url: '/product/Product/AddProduct',
        type: 'POST',
        data: formData,
        contentType: false,
        processData: false,
        success: function (response) {
            if (response.result === 1) {
                ShowToastNoti('success', '', response.message, 4000, 'topRight');
                $('#modal-product').modal('hide');
                $('#add_form_product')[0].reset();
                GetProductsByIdCategory(categoryId);
            } else {
                ShowToastNoti('error', '', response.message, 4000, 'topRight');
            }
        },
        error: function (error) {
            console.error('Error:', error);
            alert('Có lỗi xảy ra khi thêm sản phẩm.');
        }
    });
}


// Function to open the add/update product modal
function OpenModalProduct(type, productId = null) {
    let modalProductTitle = $('#modal_title_product');
    const btnProduct = $('#btn_product');

    $('#add_form_product')[0].reset(); 

    // Loại bỏ sự kiện click đã được gán trước đó
    btnProduct.off('click');

    if (type === 'addProduct') {
        modalProductTitle.text('Thêm sản phẩm mới');
        btnProduct.text('Thêm sản phẩm').on('click', addProduct);
    } else if (type === 'updateProduct') {
        modalProductTitle.text('Cập nhật sản phẩm');
        btnProduct.text('Cập nhật sản phẩm');

        if (productId) {
            $.ajax({
                url: `/product/Product/GetById?idProduct=${productId}`,
                type: 'GET',
                success: function (response) {
                    $('#productName').val(response.productName);
                    $('#productPrice').val(response.price);
                    $('#categoryId').val(response.categoryId);

                    // Gán sự kiện click để cập nhật sản phẩm sau khi đã tải dữ liệu
                    btnProduct.on('click', function () {
                        updateProduct(response.id);
                    });
                },
                error: function (xhr, status, error) {
                    console.error('Error fetching product:', error);
                    alert('Có lỗi xảy ra khi lấy thông tin sản phẩm.');
                }
            });
        }
    }
    new bootstrap.Modal(document.getElementById('modal-product')).show();
}

// Function to update the product
function updateProduct(productId) {
    const formData = new FormData();
    formData.append('Id', productId); 
    formData.append('ProductName', $('#productName').val());
    formData.append('Price', $('#productPrice').val());
    formData.append('CategoryId', $('#categoryId').val());

    // Handle product images
    const productImages = $('#productImages')[0].files;
    for (let i = 0; i < productImages.length; i++) {
        formData.append('ProductImages', productImages[i]);
    }

    $.ajax({
        url: `/product/Product/UpdateProduct`, 
        type: 'PUT',
        data: formData,
        processData: false, // Prevent jQuery from processing the data
        contentType: false, // Ensure multipart/form-data is used
        success: function (response) {
            ShowToastNoti('success', '', response, 4000, 'topRight');
            $('#modal-product').modal('hide'); 
            GetProductsByIdCategory($('#categoryId').val()); 
        },
        error: function (error) {
            ShowToastNoti('success', '', response, 4000, 'topRight');
        }
    });
}

// Remove a product
function RemoveProduct(idProduct) {
    if (confirm('Bạn có chắc chắn muốn xóa sản phẩm này không?')) {
        $.ajax({
            url: `/product/Product/RemoveProduct`,
            type: 'DELETE',
            data: { idProduct: idProduct },
            success: function (response) {
                ShowToastNoti('success', '', `Xóa sản phẩm ${response.productName} thành công`, 4000, 'topRight');
                GetProductsByIdCategory(response.categoryId); 
            },
            error: function (xhr, status, error) {
                console.error('Error removing product:', error);
                alert('Có lỗi xảy ra khi xóa sản phẩm.');
            }
        });
    }
}

function UploadExcelProductFile() {
    var fileInput = $('#fileInput')[0].files[0]; // Get the selected file

    if (!fileInput) {
        alert('Please select a file to upload.');
        return;
    }

    var formData = new FormData();
    formData.append('file', fileInput); 
    $.ajax({
        url: '/product/Product/UploadProductByExcel', 
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

function DownloadSampleFileProduct() {
    window.location.href = '/product/Product/ExportSampleProductFileExcel';
}



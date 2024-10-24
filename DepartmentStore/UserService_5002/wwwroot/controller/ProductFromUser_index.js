let productIdToAction;

function HandleTabClick(categoryId, tabId) {
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
        url: `/list/Product/GetProductsByCategory/${id}`,
        type: 'GET',
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
    let tableBody = '';

    if (products.length === 0) {
        tableBody = '<tr><td colspan="6" class="text-center">Không có sản phẩm để hiển thị</td></tr>';
    } else {
        products.forEach(function (product) {
            tableBody += `
                <tr>
                    <td>${product.productName}</td>
                    <td>${product.price}</td>
                    <td style="text-align: center; vertical-align: middle;">
                        <div class="form-check form-switch" style="display: inline-block;">
                            <input class="form-check-input toggle-status" type="checkbox" data-product-id="${product.id}" ${!product.isHide ? 'checked' : ''}>
                        </div>
                    </td>
                    <td>${new Date(product.updatedTime).toLocaleString("en-GB")}</td>
                    <td>
                        <a href="#" onclick="OpenModalProduct('updateProduct', ${product.id})" class="btn btn-primary">Sửa</a>
                        <a href="#" onclick="RemoveProduct(${product.id})" class="btn btn-danger">Xóa</a>
                    </td>
                </tr>
            `;
        });
    }

    $('#productByIdCategory').html(`
        <table class="table table-striped">
            <thead>
                <tr class='bg-primary text-white'>
                    <th>Tên sản phẩm</th>
                    <th>Giá</th>
                    <th>Trạng thái</th>
                    <th>Cập nhật lần cuối</th>
                    <th>Hành động</th>
                </tr>
            </thead>
            <tbody>${tableBody}</tbody>
        </table>
    `);
}

$(document).ready(function () {
    $(document).on('click', '.toggle-status', function () {
        var productId = $(this).data('product-id'); // Lấy productId từ thuộc tính data của checkbox
        ChangeStatusProduct(productId); 
    });
});

function ChangeStatusProduct(productId) {
    $.ajax({
        url: '/list/Product/ChangeStatusProduct',
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
    const productImages = $('#productImages')[0].files;

    const formData = new FormData();
    formData.append('ProductName', productName);
    formData.append('Price', productPrice);
    formData.append('CategoryId', categoryId);

    for (let i = 0; i < productImages.length; i++) {
        formData.append('ProductImages', productImages[i]);
    }

    $.ajax({
        url: '/list/Product/AddProduct',
        type: 'POST',
        data: formData,
        contentType: false,
        processData: false,
        success: function (response) {
            if (response.result === 1) {
                ShowToastNoti('success', '', response.message, 4000, 'topRight');
                $('#modal-product').modal('hide');
                $('#add-product-form')[0].reset();
                GetProductsByIdCategory(categoryId); 
            } else {
                alert('Có lỗi xảy ra: ' + response.message);
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
    let modalTitle = $('#modal_product_title');
    const btnProduct = $('#btn_product');

    $('#add-product-form')[0].reset();
    //btnProduct.off('click'); // Remove previous event handlers

    if (type === 'addProduct') {
        modalTitle.text('Thêm sản phẩm mới');
        btnProduct.text('Thêm sản phẩm').on('click', addProduct);
    } else if (type === 'updateProduct') {
        modalTitle.text('Cập nhật sản phẩm');
        btnProduct.text('Cập nhật sản phẩm');

        if (productId) {
            $.ajax({
                url: `/list/Product/GetById?idProduct=${productId}`,
                type: 'GET',
                success: function (response) {
                    if (response.result === 1) {
                        let product = response.data;
                        // Populate the form fields with fetched data
                        $('#productName').val(product.productName);
                        $('#productPrice').val(product.price);
                        $('#categoryId').val(product.categoryId);

                        // Now attach the updateProduct handler to the button after fetching the data
                        btnProduct.on('click', function () {
                            updateProduct(productId);  // Call updateProduct when button is clicked
                        });
                    } else {
                        alert('Có lỗi xảy ra: ' + response.message);
                    }
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
        url: `/list/Product/UpdateProduct/`, 
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
            console.error('Error:', error);
            alert('Có lỗi xảy ra khi cập nhật sản phẩm.');
        }
    });
}

// Remove a product
function RemoveProduct(idProduct) {
    if (confirm('Bạn có chắc chắn muốn xóa sản phẩm này không?')) {
        $.ajax({
            url: `/list/Product/RemoveProduct`,
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

$(document).ready(function () {
    $('#add_product_by_excel').click(function () {
        var fileInput = $('#fileInput')[0].files[0];

        if (!fileInput) {
            alert('Please select an Excel file to upload.');
            return;
        }

        var formData = new FormData();
        formData.append('file', fileInput);

        $.ajax({
            url: '/list/Product/UploadByExcel',
            type: 'POST',
            data: formData,
            contentType: false,
            processData: false,
            success: function (response) {
                ShowToastNoti('success', '', response, 4000, 'topRight');
            },
            error: function (xhr, status, error) {
                console.error('Error uploading product by Excel:', error);
                alert('An error occurred while processing the request.');
            }
        });
    });
});


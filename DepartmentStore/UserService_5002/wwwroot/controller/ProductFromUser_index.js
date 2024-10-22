function HandleTabClick(categoryId, tabId) {
    // Gọi hàm lấy sản phẩm theo ID danh mục
    GetProductsByIdCategory(categoryId);

    // Chuyển đổi tab đang hoạt động
    ActiveTogglePills(tabId);
}

function ActiveTogglePills(tabId) {
    // Xóa class active từ tất cả các liên kết
    $('.nav-link').removeClass('bg-primary text-white');

    // Thêm class active vào tab hiện tại
    $('#' + tabId).addClass('bg-primary text-white');
}

function GetProductsByIdCategory(id) {
    $.ajax({
        url: `/api/ProductApi/ResponseAPIGetProductsByIdCategory/${id}`,
        type: 'GET',
        success: function (response) {
            RenderProductTable(response);
        },
        error: function (xhr, status, error) {
            console.error('Error fetching products:', error);
        }
    });
}

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
                    <td>${product.isHide ? 'Đã bị ẩn' : 'Đang hiển thị'}</td>
                    <td>${new Date(product.updatedTime).toLocaleString("en-GB")}</td>
                    <td>
                        <a href="/Product/Edit/${product.id}" class="btn btn-primary">Edit</a>
                        <a href="/Product/Delete/${product.id}" class="btn btn-danger">Delete</a>
                    </td>
                </tr>
            `;
        });
    }

    $('#productByIdCategory').html(`
        <table class="table table-striped">
            <thead>
                <tr>
                    <th>Tên sản phẩm</th>
                    <th>Giá</th>
                    <th>Trạng thái</th>
                    <th>Cập nhật lần cuối</th>
                    <th>Actions</th>
                </tr>
            </thead>
            <tbody>${tableBody}</tbody>
        </table>
    `);
}

// Xử lý sự kiện khi nút "Lưu thay đổi" được bấm
$('#saveChangesBtn').on('click', function (e) {
    e.preventDefault(); // Ngăn chặn hành vi mặc định của button

    const productName = $('#productName').val();
    const productPrice = $('#productPrice').val();
    const categoryId = $('#categoryId').val();
    const productImages = $('#productImages')[0].files;

    // Tạo một đối tượng FormData để gửi dữ liệu
    const formData = new FormData();
    formData.append('ProductName', productName);
    formData.append('Price', productPrice);
    formData.append('CategoryId', categoryId);

    // Thêm hình ảnh vào FormData
    for (let i = 0; i < productImages.length; i++) {
        formData.append('ProductImages', productImages[i]);
    }

    // Gửi dữ liệu đến server qua AJAX
    $.ajax({
        url: '/list/Product/AddProduct', // Đường dẫn tới API của bạn
        type: 'POST',
        data: formData,
        contentType: false, // Không đặt loại nội dung vì FormData tự động xử lý
        processData: false, // Không xử lý dữ liệu vì chúng ta đang gửi FormData
        success: function (response) {
            if (response.result === 1) {
                alert('Sản phẩm đã được thêm thành công!');
                $('#modal-add-product').modal('hide'); // Đóng modal
                location.reload(); // Reload trang sau khi thêm sản phẩm
            } else {
                alert('Có lỗi xảy ra: ' + response.message);
            }
        },
        error: function (error) {
            console.error('Error:', error);
            alert('Có lỗi xảy ra khi thêm sản phẩm.');
        }
    });
});

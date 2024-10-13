

function HandleTabClick(categoryId, tabId) {
    // Call the function to get products by category ID
    GetProductsByIdCategory(categoryId);

    // Toggle the active class for the selected tab
    ActiveTogglePills(tabId);
}

function ActiveTogglePills(tabId) {
    // Remove active class from all nav links
    $('.nav-link').removeClass('bg-primary text-white');

    // Add active class to the current selected tab
    $('#' + tabId).addClass('bg-primary text-white');
}


function GetProductsByIdCategory(id) {
    $.ajax({
        url: `/ProductFromUser/GetProductsByIdCategory`,
        type: 'GET',
        data: {id:id},
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
                    <td>${product.isHide ? 'Đã bị ẩn': 'Đang hiển thị'}</td>
                    //<td>${new Date(product.updatedTime).toLocaleString("en-GB")}</td>
                    <td>
                        <a href="/Product/Edit/${product.id}" class="btn btn-primary">Edit</a>
                        <a href="/Product/Delete/${product.id}" class="btn btn-danger">Delete</a>
                    </td>
                </tr>
            `;
        });
    }

    $('#productByIdCategory').html(`<table class="table table-striped">
        <thead>
            <tr>
                <th>Tên sản phẩm</th>
                <th>Giá</th>
                <th>Trạng thái</th>
                <th>Actions</th>
            </tr>
        </thead>
        <tbody>${tableBody}</tbody>
    </table>`);
}


// Gọi hàm khi DOM đã sẵn sàng
document.addEventListener('DOMContentLoaded', function () {
    RenderProduct_BranchTab();
});

async function RenderProduct_BranchTab() {
    try {
        const response = await fetch('/Product/Product_BranchTab', {
            method: 'GET', 
        });

        if (!response.ok) {
            throw new Error('HTTP error ' + response.status); 
        }

        const data = await response.json(); 

        const productContainer = document.querySelector('#div_content_store');
        productContainer.classList.add('row'); 
        productContainer.innerHTML = '';  

        let productHTML = `
        <div class="col-lg-7 ftco-animate fadeInUp ftco-animated">
            <div class="row">`;

        data.forEach(product => {
            productHTML += `
                <div class="col-lg-4">
                    <div class="product">
                        <a onclick="AddProductToQueue(${product.id}, '${product.productName}', ${product.price})" class="img-prod">
                            <img class="img-fluid" src="${product.mainImage}" alt="Product Image">
                            <span class="status">30%</span>
                            <div class="overlay"></div>
                        </a>
                        <div class="text py-3 pb-4 px-3 text-center">
                            <h3>${product.productName}</h3>
                            <div class="d-flex">
                                <div class="pricing">
                                    <p class="price">
                                        <span class="mr-2">
                                            ${product.price.toLocaleString('vi-VN')} VND
                                        </span>
                                    </p>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>`;
        });

        productHTML += `
            </div>
        </div>`; 

        const selectedProductHTML = `
        <div class="col-lg-5 bg-secondary">
            <div class="container">
                <div><p class="text-center">Danh sách chọn</p></div>
                <hr>
                <div class="bg-secondary" id="div_selectedProduct"></div>
            </div>
        </div>`;

        productContainer.innerHTML = productHTML + selectedProductHTML;

    } catch (error) {
        console.error('Error fetching products:', error); 
        const productContainer = document.querySelector('#div_content_store');
        productContainer.innerHTML = '<p class="text-danger">Không thể tải sản phẩm. Vui lòng thử lại sau.</p>'; // Hiển thị thông báo lỗi
    }
}

async function RenderTableInvoice() {
    let tableBody = '';
    $('#div_content_store').html(`<div class=d-flex align-items-center justify-content-center>
                                        <div class="spinner-border text-primary" role="status"> <span class="sr-only">Loading...</span></div>
                                  <div>
                                   `);
    try {
        // Gửi yêu cầu GET với Fetch API
        const response = await fetch(`/Invoice/GetListInvoiceBranch?idBranch=${global_idBranch}`, {
            method: 'GET',
            // Các thông số có thể thêm vào nếu cần, ví dụ headers hoặc query params
            // headers: {
            //     'Content-Type': 'application/json',
            //     'Authorization': 'Bearer ' + token,
            // },
            //params: { idBranch: global_idBranch }
        });

        if (!response.ok) {
            throw new Error('Network response was not ok');
        }

        // Chuyển đổi dữ liệu JSON từ phản hồi
        const data = await response.json();

        if (data.length === 0) {
            tableBody = '<tr><td colspan="6" class="text-center">Không có dữ liệu để hiển thị</td></tr>';
        } else {
            data.forEach(function (invoice, index) {
                tableBody += `
                    <tr>
                        <td>${index + 1}</td>
                        <td>${invoice.invoiceNumber}</td>
                        <td>${invoice.createdDate}</td>
                        <td>${invoice.status.type}</td>
                        <td>
                            <a href="javascript:void(0)" class="btn btn-primary" onclick="GetDetailsInvoice(${invoice.id})">Chi tiết</a>
                        </td>
                    </tr>`;
            });
        }

        // Cập nhật nội dung của bảng trong #div_content_store
        document.querySelector('#div_content_store').innerHTML = `
            <div>
                <table class="table table-striped">
                    <thead>
                        <tr class='bg-primary text-white'>
                            <th>STT</th>
                            <th>Mã đơn hàng</th>
                            <th>Thời gian</th>
                            <th>Trạng thái</th>
                            <th>Hành động</th>
                        </tr>
                    </thead>
                    <tbody>${tableBody}</tbody>
                </table>
            </div>
        `;

        // Nếu cần xóa class 'row' (nếu có trong DOM)
        document.querySelector('#div_content_store').classList.remove('row');

    } catch (error) {
        console.error('Error fetching invoices:', error);
        // Xử lý lỗi khi không thể lấy dữ liệu
        document.querySelector('#div_content_store').innerHTML = '<p class="text-danger">Không thể tải danh sách hóa đơn. Vui lòng thử lại sau.</p>';
    }
}


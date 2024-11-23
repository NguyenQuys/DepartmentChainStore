//let listProduct = [];

//function AddProductToQueue(idProduct, productName, price) {
//    let product = listProduct.find(p => p.id === idProduct);

//    if (product) {
//        product.quantity += 1;
//    } else {
//        listProduct.push({
//            id: idProduct,
//            name: productName,
//            price: price,
//            quantity: 1
//        });
//    }

//    renderSelectedProducts();
//}

//function renderSelectedProducts() {
//    // Làm trống div trước khi hiển thị lại
//    $('#div_selectedProduct').html('');
//    let quantityProduct = 0;
//    let sumAllProduct = 0;

//    let table = `
//        <table border="1" style="width: 100%; border-collapse: collapse;">
//            <thead>
//                <tr>
//                    <th style="text-align: left; padding: 8px;">STT</th>
//                    <th style="text-align: left; padding: 8px;">Tên sản phẩm</th>
//                    <th style="text-align: left; padding: 8px;">Số lượng</th>
//                    <th style="text-align: left; padding: 8px;">Đơn giá</th>
//                    <th style="text-align: left; padding: 8px;">Tổng</th>
//                </tr>
//            </thead>
//            <tbody>
//    `;

//    listProduct.forEach((product, index) => {
//        let total = product.price * product.quantity; 
//        sumAllProduct += total;
//        quantityProduct += product.quantity;
//        table += `
//            <tr>
//                <td style="padding: 8px;">${index + 1}</td>
//                <td style="padding: 8px;">${product.name}</td>
//                <td style="padding: 8px;">${product.quantity}</td>
//                <td style="padding: 8px;">${product.price.toLocaleString()}đ</td>
//                <td style="padding: 8px;">${total.toLocaleString()}đ</td>
//            </tr>
//        `;
//    });

//    table += `
//            <tr class="fw-bold text-danger">
//                <td style="padding: 8px;" colspan="2">Tổng</td>
//                <td style="padding: 8px;">${quantityProduct}</td>
//                <td style="padding: 8px;"></td>
//                <td style="padding: 8px;">${sumAllProduct.toLocaleString() }đ</td>
//            </tr>
//            </tbody>
//        </table>
//        <div class='d-flex justify-content-end'>
//            <input type='text' class='form-control m-3' placeholder='Nhập voucher...'>
//            <button type='button'
//            <button type='button' class='btn btn-success m-3'>Hoàn tát</button>
//        </div>
//    `;

//    $('#div_selectedProduct').html(table);
//}

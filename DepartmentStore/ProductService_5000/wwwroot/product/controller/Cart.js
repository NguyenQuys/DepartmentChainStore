$(document).ready(function () {
    var maxQuantity = parseInt($('#quantity').attr('max')); // Get the maximum value from the input's attribute
    var minQuantity = parseInt($('#quantity').attr('min')); // Get the minimum value from the input's attribute

    $('.quantity-right-plus').click(function (e) {
        e.preventDefault();
        var currentQuantity = parseInt($('#quantity').val()) || 0; // Ensure the value is a number
        if (currentQuantity < maxQuantity) { // Check if the quantity is less than max
            $('#quantity').val(currentQuantity + 1);
        } else {
            // Optionally, you can show a message when the max quantity is reached
            ShowToastNoti('error', '', 'Không thể tăng số lượng vượt quá giới hạn', 4000);
        }
    });

    $('.quantity-left-minus').click(function (e) {
        e.preventDefault();
        var currentQuantity = parseInt($('#quantity').val()) || 0; // Ensure the value is a number
        if (currentQuantity > minQuantity) { // Check if the quantity is greater than min
            $('#quantity').val(currentQuantity - 1);
        } else {
            // Optionally, you can show a message when the min quantity is reached
            ShowToastNoti('error', '', 'Số lượng không thể giảm xuống dưới ' + minQuantity, 4000);
        }
    });
});

async function AddToCart(idProduct) {
    let quantity = $('#quantity').val();

    const formData = new FormData();
    formData.append('IdProduct', idProduct);
    formData.append('Quantity', quantity);
    formData.append('IdBranch', ID_BRANCH);

    try {
        const response = await fetch('/Cart/Add', {
            method: 'POST',
            body: formData,
        });

        const result = await response.json(); 
        ShowToastNoti('success', '', result, 4000);
    } catch (error) {
        console.error('Error adding to cart:', error);
        ShowToastNoti('error', '', error.message, 4000);
    }
}


$(document).ready(function () {
    $(document).on('click', '.btn-remove-cart', function () {
        var idProduct = $(this).data('cart-id');
        RemoveCart(idProduct);
    });
});

$(document).ready(function () {
    if ($('#div_table_cart').length) {
        RenderCartTable();
    }

    $('#submitCartButton').on('click', function (e) {
        e.preventDefault();

        let cartItems = [];
        $('#div_table_cart tr').each(function () {
            let checkbox = $(this).find('input[type="checkbox"]');
            if (checkbox.is(':checked')) {
                let productId = checkbox.data('product-id');
                let productName = $(this).find('.product-name h3').text().trim();
                let price = $(this).find('.price-cart-hidden').val();
                let quantity = parseInt($(this).find('#quantity_price').val());
                let mainImage = $(this).find('.img').css('background-image');
                if (mainImage && mainImage.startsWith('url')) {
                    mainImage = mainImage.replace(/^url\(["']?/, '').replace(/["']?\)$/, '');
                    mainImage = mainImage.replace('https://localhost:7076', '');
                }

                if (productId && !isNaN(price) && !isNaN(quantity)) {
                    cartItems.push({
                        idProduct: productId,
                        productName: productName,
                        price: price,
                        quantity: quantity,
                        idBranch: ID_BRANCH,
                        mainImage: mainImage
                    });
                }
            }
        });

        if (cartItems.length > 0) {
            let jsonData = JSON.stringify(cartItems); // Convert to JSON string
            $('#cartData').val(jsonData); // Add JSON data to a hidden input or send it in an AJAX request, etc.
            $('#cartForm').submit();
        } else {
            alert('Giỏ hàng không có sản phẩm được chọn!');
        }

    });

});

// Your RenderCartTable function
function RenderCartTable() {
    fetch('/Cart/GetAll', {
        method: 'GET',
    })
        .then(response => {
            if (!response.ok) {
                throw new Error('Network response was not ok');
            }
            return response.json();
        })
        .then(response => {
            let tableBody = response.length === 0
                ? '<tr><td colspan="8" class="text-center">Không có dữ liệu để hiển thị</td></tr>'
                : response.map(cart => `
                    <tr class="text-center">
                        <td><input type="checkbox" data-product-id="${cart.idProduct}" style="width: 20px; height: 20px;"></td>
                        <td class="image-prod"><div class="img" style="background-image:url(${cart.mainImage});"></div></td>
                        <td class="product-name">
                            <h3>${cart.productName}</h3>
                            <p>Far far away, behind the word mountains, far from the countries</p>
                        </td>
                        <td class="price-cart" id="single_price_cart">${cart.price.toLocaleString('vi-VN')}</td>
                        <input type="hidden" value="${cart.price}" class="price-cart-hidden">
                        <td class="quantity">
                            <div class="input-group mb-3">
                                <input id="quantity_price" type="text" class="quantity form-control input-number" value="${cart.quantity}" readonly>
                            </div>
                        </td>
                        <td class="total" id="total_price_cart">${(cart.price * cart.quantity).toLocaleString('vi-VN')}</td>
                        <td class="product-remove"><a onclick="RemoveCart(${cart.idProduct})"><span class="ion-ios-close"></span></a></td>
                    </tr>
                `).join('');
            document.getElementById('div_table_cart').innerHTML = tableBody;
        })
        .catch(() => {
            document.getElementById('div_table_cart').innerHTML = '<tr><td colspan="8" class="text-center text-danger">Không thể lấy dữ liệu giỏ hàng</td></tr>';
        });
}

async function RemoveCart(idProduct) {
    if (confirm('Bạn có chắc chắn muốn xóa sản phẩm này không?')) {
        try {
            const response = await fetch(`/Cart/Delete?idProduct=${idProduct}`, {
                method: 'DELETE',
            });

            if (!response.ok) {
                throw new Error(`Error: ${response.status}`);
            }

            const result = await response.json();
            ShowToastNoti('success', '', result , 4000, 'topRight');
            RenderCartTable();
        } catch (error) {
            console.error('Error removing cart:', error);
            ShowToastNoti('error', '', error.message, 4000, 'topRight');
        }
    }
}

$(document).ready(function () {
    GetListProduct();
});


// Listen for changes in the category select element using jQuery
$('#category_select_customer').on('change', function () {
    const selectedId = this.value; // 'this' refers to the select element
    GetListProduct(selectedId); // Pass the selected ID to the function
});

// Function to get products based on category
function GetListProduct(idCategory = null) {
    $.ajax({
        url: '/Product/GetProductsByCategory',
        type: 'GET',
        data: { id: idCategory },
        success: function (response) {
            if (response.result === 1) {
                let data = '';
                response.data.forEach(product => {
                    data += `
                        <div class="col-md-6 col-lg-3 ftco-animate fadeInUp ftco-animated">
                            <div class="product">
                                <a href="/Product/GetByIdView?idProduct=${product.id}" class="img-prod">
                                    <img class="img-fluid" src="${product.mainImage}" alt="Product Image">
                                    <span class="status">30%</span>
                                    <div class="overlay"></div>
                                </a>
                                <div class="text py-3 pb-4 px-3 text-center">
                                    <h3>${product.productName}</h3>
                                    <div class="d-flex">
                                        <div class="pricing">
                                            <p class="price"><span class="mr-2">${product.price} VND</span></p>
                                        </div>
                                    </div>
                                    <div class="bottom-area d-flex px-3">
                                        <div class="m-auto d-flex">
                                            <a href="#" class="add-to-cart d-flex justify-content-center align-items-center text-center">
                                                <span><i class="ion-ios-menu"></i></span>
                                            </a>
                                            <a href="#" class="buy-now d-flex justify-content-center align-items-center mx-1">
                                                <span><i class="ion-ios-cart"></i></span>
                                            </a>
                                            <a href="#" class="heart d-flex justify-content-center align-items-center">
                                                <span><i class="ion-ios-heart"></i></span>
                                            </a>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    `;
                });
                $('#div_data_product_index').html(data);
            } else {
                console.error("Error:", response.message);
            }
        },
        error: function (xhr, status, error) {
            console.error("AJAX error:", status, error);
        }
    });
}

$(document).ready(function () {
    $('#search_product_keyword').on('keyup', function () {
        var keyword = $(this).val().trim();

        if (keyword) {
            $.ajax({
                url: '/Product/SearchProduct', // Correct URL path
                type: 'POST',
                data: { productName: keyword },
                success: function (response) {
                    var results = $('#search_results');
                    results.removeClass('d-none').css('z-index', '9').show();

                    results.empty(); // Clear previous results

                    if (response.length > 0) {
                        response.forEach(function (product) {
                            results.append(`
                                <li class="list-group-item">
                                    <img src="${product.mainImage}" alt="${product.productName}" style="max-width: 50px; margin-right: 10px">
                                    <span>${product.productName}</span>
                                </li>
                            `);
                        });
                    } else {
                        results.append('<li class="list-group-item">Không tìm thấy sản phẩm</li>');
                    }
                },
                error: function (xhr, status, error) {
                    console.error("AJAX error:", status, error);
                }
            });
        } else {
            $('#search_results').hide();
        }
    });

    // Hide results when clicking outside
    $(document).on('click', function (e) {
        if (!$(e.target).closest('.search-form, #search_results').length) {
            $('#search_results').hide();
        }
    });

    // Xử lý sự kiện click trên các mục kết quả tìm kiếm
    $('#search_results').on('click', 'li', function () {
        $('#search_product_keyword').val($(this).text());
        $('#search_results').hide(); // Ẩn kết quả sau khi chọn
    });

    // Hiển thị lại kết quả khi bắt đầu nhập lại sau khi chọn
    $('#search_product_keyword').on('focus', function () {
        if ($('#search_results').children().length > 0) {
            $('#search_results').show();
        }
    });

    // Optionally: Handle click on result items
    //$('#search_results').on('click', 'li', function () {
    //    $('#search_product_keyword').val($(this).text());
    //    $('#search_results').hide(); // Hide results after selection
    //});
});

$(document).ready(function () {

    $('.quantity-right-plus').click(function (e) {
        e.preventDefault();
        var currentQuantity = parseInt($('#quantity').val());
        if (currentQuantity < 100) { // Check the maximum value
            $('#quantity').val(currentQuantity + 1);
            if (currentQuantity + 1 > 10) {
                ShowToastNoti('success', '', 'AAAAA', 4000, 'topRight');
            }
        }
    });

    $('.quantity-left-minus').click(function (e) {
        e.preventDefault();
        var currentQuantity = parseInt($('#quantity').val());
        if (currentQuantity > 1) { // Check the minimum value
            $('#quantity').val(currentQuantity - 1);
        }
    });

});
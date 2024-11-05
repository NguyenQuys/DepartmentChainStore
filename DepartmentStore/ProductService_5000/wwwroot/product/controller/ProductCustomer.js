$(document).ready(function () {
    // Load initial product list
    console.log(123);
    GetListProduct();
});

// Listen for changes in the category select element using jQuery
$('#category_select').on('change', function () {
    const selectedId = this.value; // 'this' refers to the select element
    console.log(selectedId);
    GetListProduct(selectedId); // Pass the selected ID to the function
});

// Function to get products based on category
function GetListProduct(idCategory = null) {
    $.ajax({
        url: '/product/Product/GetProductsByCategory',
        type: 'GET',
        data: { id: idCategory },
        success: function (response) {
            if (response.result === 1) {
                let data = '';
                response.data.forEach(product => {
                    data += `
                        <div class="col-md-6 col-lg-3 ftco-animate fadeInUp ftco-animated">
                            <div class="product">
                                <a href="#" class="img-prod">
                                    <img class="img-fluid" src="${product.mainImage}" alt="Product Image">
                                    <span class="status">30%</span>
                                    <div class="overlay"></div>
                                </a>
                                <div class="text py-3 pb-4 px-3 text-center">
                                    <h3><a href="#">${product.productName}</a></h3>
                                    <div class="d-flex">
                                        <div class="pricing">
                                            <p class="price"><span class="mr-2 price-dc">${product.price} VND</span></p>
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

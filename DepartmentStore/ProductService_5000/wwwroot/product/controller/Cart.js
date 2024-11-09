//function AddToCart(idProduct) {
//    let quantity = $('#quantity').val();

//    var formData = new FormData();
//    formData.append('IdProduct', idProduct);
//    formData.append('Quantity', quantity);

//    $.ajax({
//        url: '/Cart/Add',
//        type: 'POST',
//        data: formData,
//        contentType: false,
//        processData: false,
//        success: function (response) {
//            ShowToastNoti('success', '', response, 4000);
//        }
//    });
//}
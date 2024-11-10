function CheckPromotion() {
    let promotionCode = $('#input_promotion_code').val();

    $.ajax({
        url: '/Promotion/GetByPromotionCode',
        type: 'POST',
        data: { code: promotionCode },
        success: function (response) {
            if (response.result === 1) {

            }
        }
    });
}
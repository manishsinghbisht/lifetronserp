var PriceBookLineItem = new Object();

PriceBookLineItem.init = function () {
    $(document).ready();
    var productId = $('#ProductId').val();
    $("select[name*='ProductId']").val(productId);
    //alert($("[name*='ProductId']) option:selected").text());

    var priceBookId = $('#PriceBookId').val();
    $("select[name*='PriceBookId']").val(priceBookId);

    $('input[type="submit"]').click(PriceBookLineItem.Submit);


    $(".stepwizard-step").each(function () {
        $(":button").removeClass("btn-primary");
        $(":button").addClass("btn-default");
    });

    $("#stepWizardPriceBook").removeClass("btn-default");
    $("#stepWizardPriceBook").addClass("btn-primary");

};

PriceBookLineItem.Submit = function () {
    var btn = jQuery(this);
    btn.prop('disabled', true);
    setTimeout(function () {
        btn.prop('disabled', false);
    }, 2000);
    //jQuery("#frm").submit();
    //var form = $(this).closest('form');
    $(this.form).submit();
};



PriceBookLineItem.init();
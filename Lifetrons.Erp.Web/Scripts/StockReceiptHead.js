var StockReceiptHead = new Object();

StockReceiptHead.init = function () {
    $(document).ready(StockReceiptHead.ready);
    $("#btnSearch").click(StockReceiptHead.Search);
    $('input[type="submit"]').click(StockReceiptHead.Submit);
    $('.glyphicon-remove-circle').click(StockReceiptHead.ClearPrevInputControl);
};

StockReceiptHead.Submit = function () {
    var btn = jQuery(this);
    btn.prop('disabled', true);
    setTimeout(function () {
        btn.prop('disabled', false);
    }, 2000);
    //jQuery("#frm").submit();
    //var form = $(this).closest('form');
    $(this.form).submit();
};

StockReceiptHead.Search = function () {
    //////$('#frmSearch').attr('action', 'Invoice/Search');
    //var pathname = window.location.pathname;
    //if (pathname.toLowerCase().indexOf("StockReceipt/") >= 0) {

    //    $('form[name="frmSearch"]').attr('action', 'Search');
    //}
    //else {
    //    $('form[name="frmSearch"]').attr('action', 'StockReceipt/Search');
    //}
    //jQuery("#frmSearch").submit();
    $(this.form).submit();
};

StockReceiptHead.ready = function () {
    $('[data-toggle="tooltip"]').tooltip({ 'placement': 'right' });
    $('[data-toggle1="popover"]').popover({ trigger: 'hover', 'placement': 'top' });

    $('.dateTxt').datetimepicker({
        ////These options can also be set by data-date-OPTION in HTML
        //pickDate: true,                 //en/disables the date picker
        //pickTime: true,                 //en/disables the time picker
        //defaultDate: new Date(),
    });

    $('input.manageMaxLength').maxlength({
        alwaysShow: true,
        threshold: 10,
        warningClass: "label label-success",
        limitReachedClass: "label label-danger",
        separator: ' of ',
        preText: 'You have ',
        postText: ' chars remaining.',
        validate: true
    });
    $('textarea.manageMaxLength').maxlength({
        alwaysShow: true
    });


    $(".stepwizard-step").each(function () {
        $(":button").removeClass("btn-primary");
        $(":button").addClass("btn-default");
    });

    $("#stepWizardStockReceipt").removeClass("btn-default");
    $("#stepWizardStockReceipt").addClass("btn-primary");

    StockReceiptHead.ClearControls();
};

StockReceiptHead.ClearPrevInputControl = function () {
    $(this).prev("input").val('');
    // alert($(this).prev('input').attr('id'));
};

StockReceiptHead.ClearControls = function () {
    $("#Remark").val('');
};

StockReceiptHead.init();
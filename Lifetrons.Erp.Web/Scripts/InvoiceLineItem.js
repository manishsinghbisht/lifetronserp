var InvoiceLineItem = new Object();

InvoiceLineItem.init = function () {
    $(document).ready(InvoiceLineItem.ready);
    $('input[type="submit"]').click(InvoiceLineItem.Submit);
};

InvoiceLineItem.Submit = function () {
    var btn = jQuery(this);
    btn.prop('disabled', true);
    setTimeout(function () {
        btn.prop('disabled', false);
    }, 2000);
    jQuery("#frm").submit();
};

//$("#frm").validate({
//    rules: {
//        Quantity: {
//            required: true
//        },
//        WeightUnitId: {
//            required: function (element) {
//                return $("#Weight").val() != "";
//            }
//        }
//    }
//});

InvoiceLineItem.ready = function () {
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

    $("#WeightUnitId").rules("add", {
        required: function (element) {
            return $("#Weight").val() != "";
        },
        messages: {
            required: "*",
        }
    });

    $(".stepwizard-step").each(function () {
        $(":button").removeClass("btn-primary");
        $(":button").addClass("btn-default");
    });

    $("#stepWizardInvoice").removeClass("btn-default");
    $("#stepWizardInvoice").addClass("btn-primary");

};

InvoiceLineItem.init();
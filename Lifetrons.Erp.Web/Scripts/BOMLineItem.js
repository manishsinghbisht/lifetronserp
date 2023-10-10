var BOMLineItem = new Object();

BOMLineItem.init = function () {
    $(document).ready(BOMLineItem.ready);
     $('input[type="submit"]').click(BOMLineItem.Submit);
};

BOMLineItem.Submit = function () {
    var btn = jQuery(this);
    btn.prop('disabled', true);
    setTimeout(function () {
        btn.prop('disabled', false);
    }, 2000);
    jQuery("#frm").submit();
};

$("#frm").validate({
    rules: {
        Quantity: {
            required: true
        },
        WeightUnitId: {
            required: function (element) {
                return $("#Weight").val() != "";
            }
        },
        AllowedLossWeightUnitId: {
            required: function (element) {
                return $("#AllowedLossWeight").val() != "";
            }
        }
    }
});

BOMLineItem.ready = function () {
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

    $("#stepWizardBOM").removeClass("btn-default");
    $("#stepWizardBOM").addClass("btn-primary");

};

BOMLineItem.init();
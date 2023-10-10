var PlanControl = new Object();

PlanControl.init = function () {
    $(document).ready(PlanControl.ready);
    $("#btnSearch").click(PlanControl.Search);
    $('input[type="submit"]').click(PlanControl.Submit);
    $('.glyphicon-remove-circle').click(PlanControl.ClearPrevInputControl);
};

PlanControl.Submit = function () {
    var btn = jQuery(this);
    btn.prop('disabled', true);
    setTimeout(function () {
        btn.prop('disabled', false);
    }, 2000);
    //jQuery("#frm").submit();
    //var form = $(this).closest('form');
    $(this.form).submit();
};

PlanControl.Search = function () {
    //////$('#frmSearch').attr('action', 'Invoice/Search');
    //var pathname = window.location.pathname;
    //if (pathname.toLowerCase().indexOf("PlanControl/") >= 0) {

    //    $('form[name="frmSearch"]').attr('action', 'Search');
    //}
    //else {
    //    $('form[name="frmSearch"]').attr('action', 'PlanControl/Search');
    //}
    //jQuery("#frmSearch").submit();
    $(this.form).submit();
};

PlanControl.ready = function () {
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

    //////This is causing menu is issues if not #WeightUnitId is not find visible on page, which happens in case of index.
    //$("#WeightUnitId").rules("add", {
    //    required: function (element) {
    //        return $("#Weight").val() != "";
    //    },
    //    messages: {
    //        required: "*",
    //        minlength: jQuery.format("Please, at least {0} characters are necessary")
    //    }
    //});

    $(".stepwizard-step").each(function () {
        $(":button").removeClass("btn-primary");
        $(":button").addClass("btn-default");
    });

    $("#stepWizardForecast").removeClass("btn-default");
    $("#stepWizardForecast").addClass("btn-primary");

    $("#Remark").val('');
};


PlanControl.init();
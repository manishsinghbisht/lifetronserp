var JobReceiptHead = new Object();

JobReceiptHead.init = function () {
    $(document).ready(JobReceiptHead.ready);
    $("#btnSearch").click(JobReceiptHead.Search);
    $('input[type="submit"]').click(JobReceiptHead.Submit);
    $('.glyphicon-remove-circle').click(JobReceiptHead.ClearPrevInputControl);
};

JobReceiptHead.Submit = function () {
    var btn = jQuery(this);
    btn.prop('disabled', true);
    setTimeout(function () {
        btn.prop('disabled', false);
    }, 2000);
    jQuery("#frm").submit();
};

JobReceiptHead.Search = function () {
    ////////$('#frmSearch').attr('action', 'Invoice/Search');
    //var pathname = window.location.pathname;
    //if (pathname.toLowerCase().indexOf("JobReceipt/") >= 0) {

    //    $('form[name="frmSearch"]').attr('action', 'Search');
    //}
    //else {
    //    $('form[name="frmSearch"]').attr('action', 'JobReceipt/Search');
    //}
    //jQuery("#frmSearch").submit();
    $(this.form).submit();
};

JobReceiptHead.ready = function () {
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

    $("#stepWizardJobReceipt").removeClass("btn-default");
    $("#stepWizardJobReceipt").addClass("btn-primary");


    JobReceiptHead.ClearControls();
};

JobReceiptHead.ClearPrevInputControl = function () {
    $(this).prev("input").val('');
    // alert($(this).prev('input').attr('id'));
};

JobReceiptHead.ClearControls = function () {
    $("#Remark").val('');
};

JobReceiptHead.init();
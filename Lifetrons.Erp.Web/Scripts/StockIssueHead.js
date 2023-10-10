var StockIssueHead = new Object();

StockIssueHead.init = function () {
    $(document).ready(StockIssueHead.ready);
    $("#btnSearch").click(StockIssueHead.Search);
    $('input[type="submit"]').click(StockIssueHead.Submit);
    $('.glyphicon-remove-circle').click(StockIssueHead.ClearPrevInputControl);
};

StockIssueHead.Submit = function () {
    var btn = jQuery(this);
    btn.prop('disabled', true);
    setTimeout(function () {
        btn.prop('disabled', false);
    }, 2000);
    //jQuery("#frm").submit();
    //var form = $(this).closest('form');
    $(this.form).submit();
};

StockIssueHead.Search = function () {
    //////$('#frmSearch').attr('action', 'Invoice/Search');
    //var pathname = window.location.pathname;
    //if (pathname.toLowerCase().indexOf("StockIssue/") >= 0) {

    //    $('form[name="frmSearch"]').attr('action', 'Search');
    //}
    //else {
    //    $('form[name="frmSearch"]').attr('action', 'StockIssue/Search');
    //}
    //jQuery("#frmSearch").submit();
    $(this.form).submit();
};

StockIssueHead.ready = function () {
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

    $("#stepWizardStockIssue").removeClass("btn-default");
    $("#stepWizardStockIssue").addClass("btn-primary");


    StockIssueHead.ClearControls();
};

StockIssueHead.ClearPrevInputControl = function () {
    $(this).prev("input").val('');
    // alert($(this).prev('input').attr('id'));
};

StockIssueHead.ClearControls = function () {
    $("#Remark").val('');
};

StockIssueHead.init();
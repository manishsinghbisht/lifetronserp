var JobIssueHead = new Object();

JobIssueHead.init = function () {
    $(document).ready(JobIssueHead.ready);
    $("#btnSearch").click(JobIssueHead.Search);
    $('input[type="submit"]').click(JobIssueHead.Submit);
    $('.glyphicon-remove-circle').click(JobIssueHead.ClearPrevInputControl);
};

JobIssueHead.Submit = function () {
    var btn = jQuery(this);
    btn.prop('disabled', true);
    setTimeout(function () {
        btn.prop('disabled', false);
    }, 2000);
    jQuery("#frm").submit();
};

JobIssueHead.Search = function () {
    ////////$('#frmSearch').attr('action', 'Invoice/Search');
    //var pathname = window.location.pathname;
    //if (pathname.toLowerCase().indexOf("JobIssue/") >= 0) {

    //    $('form[name="frmSearch"]').attr('action', 'Search');
    //}
    //else {
    //    $('form[name="frmSearch"]').attr('action', 'JobIssue/Search');
    //}
    //jQuery("#frmSearch").submit();
    $(this.form).submit();
};

JobIssueHead.ready = function () {
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

    $("#stepWizardJobIssue").removeClass("btn-default");
    $("#stepWizardJobIssue").addClass("btn-primary");

    JobIssueHead.ClearControls();
};

JobIssueHead.ClearPrevInputControl = function () {
    $(this).prev("input").val('');
    // alert($(this).prev('input').attr('id'));
};

JobIssueHead.ClearControls = function () {
    $("#Remark").val('');
};

JobIssueHead.init();
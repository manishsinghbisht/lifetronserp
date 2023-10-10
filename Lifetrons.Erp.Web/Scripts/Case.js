var Case = new Object();

Case.init = function () {
    $(document).ready(Case.ready);
    $("#btnSearch").click(Case.Search);
     $('input[type="submit"]').click(Case.Submit);
};

Case.Submit = function () {
    var btn = jQuery(this);
    btn.prop('disabled', true);
    setTimeout(function () {
        btn.prop('disabled', false);
    }, 2000);
    jQuery("#frm").submit();
};

Case.Search = function () {
    ////////$('#frmSearch').attr('action', 'Case/Search');
    //var pathname = window.location.pathname;
    //if (pathname.toLowerCase().indexOf("case/") >= 0) {

    //    $('form[name="frmSearch"]').attr('action', 'Search');
    //}
    //else {
    //    $('form[name="frmSearch"]').attr('action', 'Case/Search');
    //}
    //jQuery("#frmSearch").submit();
    $(this.form).submit();
};

Case.ready = function () {
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
    Case.ClearControls();
};

Case.ClearControls = function () {
    $("#Desc").val('');
    $("#InternalComments").val('');
};


Case.init();
var Audit = new Object();

Audit.init = function () {
    $(document).ready(Audit.ready);
    $("#btnSearch").click(Audit.Search);
     $('input[type="submit"]').click(Audit.Submit);
};

Audit.Submit = function () {
    var btn = jQuery(this);
    btn.prop('disabled', true);
    setTimeout(function () {
        btn.prop('disabled', false);
    }, 2000);
    jQuery("#frm").submit();
};


Audit.Search = function () {
    ////////$('#frmSearch').attr('action', 'Audit/Search');
    //var pathname = window.location.pathname;
    //if (pathname.toLowerCase().indexOf("audit/") >= 0) {

    //    $('form[name="frmSearch"]').attr('action', 'Search');
    //}
    //else {
    //    $('form[name="frmSearch"]').attr('action', 'Audit/Search');
    //}
    //jQuery("#frmSearch").submit();
    $(this.form).submit();
};

Audit.ready = function () {
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
    Audit.ClearControls();
};

Audit.ClearControls = function () {
   
};



Audit.init();

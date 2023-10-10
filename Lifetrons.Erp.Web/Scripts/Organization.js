var Organization = new Object();

Organization.init = function () {
    $(document).ready(Organization.ready);
    $("#btnSearch").click(Organization.Search);
     $('input[type="submit"]').click(Organization.Submit);
};

Organization.Submit = function () {
    var btn = jQuery(this);
    btn.prop('disabled', true);
    setTimeout(function () {
        btn.prop('disabled', false);
    }, 10000);

    jQuery("#frm").submit();
};

Organization.Search = function () {
    ////////$('#frmSearch').attr('action', 'Invoice/Search');
    //var pathname = window.location.pathname;
    //$('form[name="frmSearch"]').attr('action', 'SearchOrg');
    ////if (pathname.toLowerCase().indexOf("Admin/") >= 0) {

    ////    $('form[name="frmSearch"]').attr('action', 'SearchOrg');
    ////}
    ////else {
    ////    $('form[name="frmSearch"]').attr('action', 'Admin/SearchOrg');
    ////}
    //alert($('form[name="frmSearch"]').attr());
    //jQuery("#frmSearch").submit();
    $(this.form).submit();
};


Organization.ready = function () {
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
};


Organization.init();
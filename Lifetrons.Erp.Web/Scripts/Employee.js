var Employee = new Object();

Employee.init = function () {
    $(document).ready(Employee.ready);
    $("#btnSearch").click(Employee.Search);
    $("#btnShowcaseSearch").click(Employee.ShowcaseSearch);
    $('input[type="submit"]').click(Employee.Submit);
};

Employee.Submit = function () {

    var btn = jQuery(this);
    btn.prop('disabled', true);
    setTimeout(function () {
        btn.prop('disabled', false);
    }, 2000);
    jQuery("#frm").submit();

};

Employee.Search = function () {
    ////////$('#frmSearch').attr('action', 'Invoice/Search');
    //var pathname = window.location.pathname;
    //if (pathname.toLowerCase().indexOf("Employee/") >= 0) {

    //    $('form[name="frmSearch"]').attr('action', 'Search');
    //}
    //else {
    //    $('form[name="frmSearch"]').attr('action', 'Employee/Search');
    //}
    //jQuery("#frmSearch").submit();

    $(this.form).submit();
};

Employee.ShowcaseSearch = function () {
    var pathname = window.location.pathname;
    if (pathname.toLowerCase().indexOf("Employee/") >= 0) {

        $('form[name="frmShowcaseSearch"]').attr('action', 'ShowcaseSearch');
    }
    else {
        $('form[name="frmShowcaseSearch"]').attr('action', 'Employee/ShowcaseSearch');
    }
    jQuery("#frmShowcaseSearch").submit();
};

Employee.ready = function () {
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


Employee.init();
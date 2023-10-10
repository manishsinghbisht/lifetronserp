var Process = new Object();

Process.init = function () {
    $(document).ready(Process.ready);
    $("#btnSearch").click(Process.Search);
     $('input[type="submit"]').click(Process.Submit);
};

Process.Submit = function () {
    var btn = jQuery(this);
    btn.prop('disabled', true);
    setTimeout(function () {
        btn.prop('disabled', false);
    }, 2000);
    //jQuery("#frm").submit();
    //var form = $(this).closest('form');
    $(this.form).submit();
};


Process.Search = function () {
    //////$('#frmSearch').attr('action', 'Process/Search');
    //var pathname = window.location.pathname;
    //if (pathname.toLowerCase().indexOf("Process/") >= 0) {

    //    $('form[name="frmSearch"]').attr('action', 'Search');
    //}
    //else {
    //    $('form[name="frmSearch"]').attr('action', 'Process/Search');
    //}
    //jQuery("#frmSearch").submit();
    $(this.form).submit();
};
Process.ready = function () {
    $('[data-toggle="tooltip"]').tooltip({ 'placement': 'right' });
    $('[data-toggle1="popover"]').popover({ trigger: 'hover', 'placement': 'top' });

    $('.dateTxt').datetimepicker({
        ////These options can also be set by data-date-OPTION in HTML
        //pickDate: true,                 //en/disables the date picker
        //pickTime: true,                 //en/disables the time picker
        //defaultDate: new Date(),
    });

    $(".stepwizard-step").each(function () {
        $(":button").removeClass("btn-primary");
        $(":button").addClass("btn-default");
    });

    $("#stepWizardProcess").removeClass("btn-default");
    $("#stepWizardProcess").addClass("btn-primary");

    Process.ClearControls();
};

Process.ClearControls = function () {
    $("#Desc").val('');
};


Process.init();
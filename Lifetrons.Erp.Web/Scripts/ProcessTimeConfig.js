var ProcessTimeConfig = new Object();

ProcessTimeConfig.init = function () {
    $(document).ready(ProcessTimeConfig.ready);
    $("#btnSearch").click(ProcessTimeConfig.Search);
     $('input[type="submit"]').click(ProcessTimeConfig.Submit);
};

ProcessTimeConfig.Submit = function () {
    var btn = jQuery(this);
    btn.prop('disabled', true);
    setTimeout(function () {
        btn.prop('disabled', false);
    }, 2000);
    //jQuery("#frm").submit();
    //var form = $(this).closest('form');
    $(this.form).submit();
};


ProcessTimeConfig.Search = function () {
    //////$('#frmSearch').attr('action', 'ProcessTimeConfig/Search');
    //var pathname = window.location.pathname;
    //if (pathname.toLowerCase().indexOf("ProcessTimeConfig/") >= 0) {

    //    $('form[name="frmSearch"]').attr('action', 'Search');
    //}
    //else {
    //    $('form[name="frmSearch"]').attr('action', 'ProcessTimeConfig/Search');
    //}
    //jQuery("#frmSearch").submit();
    $(this.form).submit();
};
ProcessTimeConfig.ready = function () {
    $('[data-toggle="tooltip"]').tooltip({ 'placement': 'right' });
    $('[data-toggle1="popover"]').popover({ trigger: 'hover', 'placement': 'top' });

    $('.dateTxt').datetimepicker({
        ////These options can also be set by data-date-OPTION in HTML
        //pickDate: true,                 //en/disables the date picker
        //pickTime: false,                 //en/disables the time picker
        //defaultDate: new Date(),

    });

    
    $('.timeTxt').datetimepicker({
        //These options can also be set by data-date-OPTION in HTML
        pickDate: false,                 //en/disables the date picker
        pickTime: true,                 //en/disables the time picker
        defaultDate: new Date(),
    });

    $(".stepwizard-step").each(function () {
        $(":button").removeClass("btn-primary");
        $(":button").addClass("btn-default");
    });

    $("#stepWizardProcess").removeClass("btn-default");
    $("#stepWizardProcess").addClass("btn-primary");

    ProcessTimeConfig.ClearControls();
};

ProcessTimeConfig.ClearControls = function () {
    $("#Remark").val('');
};


ProcessTimeConfig.init();
var BOM = new Object();

BOM.init = function () {
    $(document).ready(BOM.ready);
    $("#btnSearch").click(BOM.Search);
     $('input[type="submit"]').click(BOM.Submit);
};

BOM.Submit = function () {
    var btn = jQuery(this);
    btn.prop('disabled', true);
    setTimeout(function () {
        btn.prop('disabled', false);
    }, 2000);
    jQuery("#frm").submit();
};


BOM.Search = function () {
    ////////$('#frmSearch').attr('action', 'BOM/Search');
    //var pathname = window.location.pathname;
    //if (pathname.toLowerCase().indexOf("BOM/") >= 0) {

    //    $('form[name="frmSearch"]').attr('action', 'Search');
    //}
    //else {
    //    $('form[name="frmSearch"]').attr('action', 'BOM/Search');
    //}
    //jQuery("#frmSearch").submit();
    $(this.form).submit();
};
BOM.ready = function () {
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

    $("#stepWizardBOM").removeClass("btn-default");
    $("#stepWizardBOM").addClass("btn-primary");

    BOM.ClearControls();
};

BOM.ClearControls = function () {
    $("#Desc").val('');
};


BOM.init();
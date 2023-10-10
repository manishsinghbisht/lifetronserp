var Opportunity = new Object();

Opportunity.init = function () {
    $(document).ready(Opportunity.ready);
    $("#btnSearch").click(Opportunity.Search);
     $('input[type="submit"]').click(Opportunity.Submit);
};

Opportunity.Submit = function () {
    var btn = jQuery(this);
    btn.prop('disabled', true);
    setTimeout(function () {
        btn.prop('disabled', false);
    }, 2000);
    //jQuery("#frm").submit();
    //var form = $(this).closest('form');
    $(this.form).submit();
};


Opportunity.Search = function () {
    ////////$('#frmSearch').attr('action', 'Opportunity/Search');
    //var pathname = window.location.pathname;
    //if (pathname.toLowerCase().indexOf("opportunity/") >= 0) {

    //    $('form[name="frmSearch"]').attr('action', 'Search');
    //}
    //else {
    //    $('form[name="frmSearch"]').attr('action', 'Opportunity/Search');
    //}
    //jQuery("#frmSearch").submit();
    $(this.form).submit();
};
Opportunity.ready = function () {
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

    $("#stepWizardOpportunity").removeClass("btn-default");
    $("#stepWizardOpportunity").addClass("btn-primary");

    Opportunity.ClearControls();
};

Opportunity.ClearControls = function () {
    $("#Desc").val('');
};


Opportunity.init();
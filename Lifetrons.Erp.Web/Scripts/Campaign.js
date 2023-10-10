var Campaign = new Object();

Campaign.init = function () {
    $(document).ready(Campaign.ready);
    $("#btnSearch").click(Campaign.Search);
     $('input[type="submit"]').click(Campaign.Submit);
};

Campaign.Submit = function () {
    var btn = jQuery(this);
    btn.prop('disabled', true);
    setTimeout(function () {
        btn.prop('disabled', false);
    }, 2000);
    jQuery("#frm").submit();
};

Campaign.Search = function () {
    ////////$('#frmSearch').attr('action', 'Campaign/Search');
    //var pathname = window.location.pathname;
    //if (pathname.toLowerCase().indexOf("campaign/") >= 0) {

    //    $('form[name="frmSearch"]').attr('action', 'Search');
    //}
    //else {
    //    $('form[name="frmSearch"]').attr('action', 'Campaign/Search');
    //}
    //jQuery("#frmSearch").submit();
    $(this.form).submit();
};

Campaign.ready = function () {
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

    $("#stepWizardCampaign").removeClass("btn-default");
    $("#stepWizardCampaign").addClass("btn-primary");


    Campaign.ClearControls();
};

Campaign.ClearControls = function () {
    $("#Desc").val('');
};

Campaign.init();
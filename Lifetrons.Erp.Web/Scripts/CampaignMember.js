var CampaignMember = new Object();

CampaignMember.init = function () {
    $(document).ready(CampaignMember.ready);
    $('#LeadId').on('change', CampaignMember.LeadDdlChange);
    $('#ContactId').on('change', CampaignMember.ContactDdlChange);
     $('input[type="submit"]').click(CampaignMember.Submit);
};

CampaignMember.Submit = function () {
    var btn = jQuery(this);
    btn.prop('disabled', true);
    setTimeout(function () {
        btn.prop('disabled', false);
    }, 2000);
    jQuery("#frm").submit();
};

CampaignMember.LeadDdlChange = function () {
    jQuery('#ContactId').val("");
};

CampaignMember.ContactDdlChange = function () {
    jQuery('#LeadId').val("");
};


CampaignMember.ready = function () {
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

};

CampaignMember.init();
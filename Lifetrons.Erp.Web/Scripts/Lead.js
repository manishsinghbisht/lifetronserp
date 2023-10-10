var Lead = new Object();

Lead.init = function () {
    $(document).ready(Lead.ready);
    $('#AddressId').on('change', Lead.AddressDdlChange);
    $("#btnSearch").click(Lead.Search);
     $('input[type="submit"]').click(Lead.Submit);
};

Lead.Submit = function () {
    var btn = jQuery(this);
    btn.prop('disabled', true);
    setTimeout(function () {
        btn.prop('disabled', false);
    }, 2000);
    //jQuery("#frm").submit();
    //var form = $(this).closest('form');
    $(this.form).submit();
};

Lead.Search = function () {
    ////////$('#frmSearch').attr('action', 'Lead/Search');
    //var pathname = window.location.pathname;
    //if (pathname.toLowerCase().indexOf("lead/") >= 0) {

    //    $('form[name="frmSearch"]').attr('action', 'Search');
    //}
    //else {
    //    $('form[name="frmSearch"]').attr('action', 'Lead/Search');
    //}
    //jQuery("#frmSearch").submit();
    $(this.form).submit();
};

Lead.ready = function () {
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

    $("#stepWizardLead").removeClass("btn-default");
    $("#stepWizardLead").addClass("btn-primary");

    Lead.ClearControls();
};

Lead.ClearControls = function () {
    $("#Desc").val('');
};

Lead.AddressDdlChange = function () {
    var controlId = jQuery(this).attr('id');
    var controlName = jQuery(this).attr('name');
    var selectedVal = jQuery('#' + controlId + ' option:selected').val();

    if (selectedVal != "") {
        Lead.PostBackAddress(jQuery(this));
    }
};

Lead.PostBackAddress = function (control) {

    var controlId = jQuery(control).attr('id');
    var controlName = jQuery(control).attr('name');
    var selectedVal = jQuery('#' + controlId + ' option:selected').val();


    var action = "../PostBackAddress";
    //if ($('form[name="foo"]'))
    var parentForm = $('#' + controlId).closest("form");
    if (jQuery(parentForm).attr('action').toLowerCase().indexOf("edit") >= 0) {
        action = "../PostBackAddress";
    } else {
        action = "PostBackAddress";
    }

    $.ajax({
        type: 'POST',
        url: action,
        data: JSON.stringify({ stringifiedParam: selectedVal }),
        contentType: 'application/json',
        beforeSend: function () {
            jQuery(control).attr('disabled', true);
        },
        error: function (xhr, status, error) {
            //do something about the error
            var errMsg = xhr.status + "\r\n" + status + "\r\n" + error;
            alert("Ajax postback error occured: " + errMsg);
            jQuery(control).attr('disabled', false);
        },
        success: function (response)
        {
            Lead.UpdateAddressControls(response);
            jQuery(control).attr('disabled', false);
        },
        complete: function (jqXhR, textStatus) {
            if (textStatus == "success") {

            }
            jQuery(control).attr('disabled', false);
        }
    });
};

Lead.UpdateAddressControls = function (response) {
    jQuery('#AddressToName').val(response.AddressToName);
    jQuery('#AddressLine1').val(response.AddressLine1);
    jQuery('#AddressLine2').val(response.AddressLine2);
    jQuery('#AddressLine3').val(response.AddressLine3);
    jQuery('#AddressCity').val(response.City);
    jQuery('#AddressState').val(response.State);
    jQuery('#AddressPin').val(response.Pin);
    jQuery('#AddressCountry').val(response.Country);
    jQuery('#AddressPhone').val(response.Phone);
    jQuery('#AddressEMail').val(response.EMail);
};

Lead.init();
var Contact = new Object();

Contact.init = function () {
    $(document).ready(Contact.ready);
    $('#MailingAddressId').on('change', Contact.AddressDdlChange);
    $('#OtherAddressId').on('change', Contact.AddressDdlChange);
    $("#btnSearch").click(Contact.Search);
    $('input[type="submit"]').click(Contact.Submit);
    $("#Name").blur(Contact.AutoExtractLastName);
    $('.glyphicon-remove-circle').click(Contact.ClearPrevInputControl);
};

Contact.Submit = function () {
    var btn = jQuery(this);
    btn.prop('disabled', true);
    setTimeout(function () {
        btn.prop('disabled', false);
    }, 2000);
    jQuery("#frm").submit();
};

Contact.Search = function () {
    ////////$('#frmSearch').attr('action', 'Invoice/Search');
    //var pathname = window.location.pathname;
    //if (pathname.toLowerCase().indexOf("contact/") >= 0) {

    //    $('form[name="frmSearch"]').attr('action', 'Search');
    //}
    //else {
    //    $('form[name="frmSearch"]').attr('action', 'Contact/Search');
    //}
    //jQuery("#frmSearch").submit();
    $(this.form).submit();
};

Contact.ready = function () {
    $('[data-toggle="tooltip"]').tooltip({ 'placement': 'right' });
    $('[data-toggle1="popover"]').popover({ trigger: 'hover', 'placement': 'top' });

    $('.dateTxt').datetimepicker({
        ////These options can also be set by data-date-OPTION in HTML
        //pickDate: true,                 //en/disables the date picker
        //pickTime: true,                 //en/disables the time picker
        ////defaultDate: new Date(),
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

    $("#stepWizardContact").removeClass("btn-default");
    $("#stepWizardContact").addClass("btn-primary");


    Contact.InitControls();
};


Contact.ClearPrevInputControl = function () {
    $(this).prev("input").val('');
    // alert($(this).prev('input').attr('id'));
};

Contact.InitControls = function () {
    if ($("#NamePrefix").val() == "") {
        $("#NamePrefix").val('Mr.');
    }
    $("#Desc").val('');
};

Contact.AutoExtractLastName = function () {
   
    var fullName = $("#Name").val().trim();
   
    $("#MailingAddressToName").val(fullName);
    $("#OtherAddressToName").val(fullName);

    var lastSpaceIndex = fullName.lastIndexOf(" ");
    $("#LastName").val(fullName.substring(lastSpaceIndex + 1, fullName.length));
    
    var firstSpaceIndex = fullName.indexOf(" ");
    $("#FirstName").val(fullName.substring(0, firstSpaceIndex));
    $("#MiddleName").val(fullName.substring(firstSpaceIndex + 1, lastSpaceIndex));
    
};


Contact.AddressDdlChange = function () {
    var controlId = jQuery(this).attr('id');
    var controlName = jQuery(this).attr('name');
    var selectedVal = jQuery('#' + controlId + ' option:selected').val();

    if (selectedVal != "") {
        Contact.PostBackAddress(jQuery(this));
    }
};

Contact.PostBackAddress = function (control) {

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
        success: function (response) {

            if (controlId.toLowerCase().indexOf("mailing") >= 0) {
                Contact.UpdateMailingAddressControls(response);
            } else {
                Contact.UpdateOtherAddressControls(response);
            }

            jQuery(control).attr('disabled', false);
        },
        complete: function (jqXhR, textStatus) {
            if (textStatus == "success") {

            }
            jQuery(control).attr('disabled', false);
        }
    });
};

Contact.UpdateMailingAddressControls = function (response) {
    jQuery('#MailingAddressToName').val(response.AddressToName);
    jQuery('#MailingAddressLine1').val(response.AddressLine1);
    jQuery('#MailingAddressLine2').val(response.AddressLine2);
    jQuery('#MailingAddressLine3').val(response.AddressLine3);
    jQuery('#MailingAddressCity').val(response.City);
    jQuery('#MailingAddressState').val(response.State);
    jQuery('#MailingAddressPin').val(response.Pin);
    jQuery('#MailingAddressCountry').val(response.Country);
    jQuery('#MailingAddressPhone').val(response.Phone);
    jQuery('#MailingAddressEMail').val(response.EMail);
};

Contact.UpdateOtherAddressControls = function (response) {
    jQuery('#OtherAddressToName').val(response.AddressToName);
    jQuery('#OtherAddressLine1').val(response.AddressLine1);
    jQuery('#OtherAddressLine2').val(response.AddressLine2);
    jQuery('#OtherAddressLine3').val(response.AddressLine3);
    jQuery('#OtherAddressCity').val(response.City);
    jQuery('#OtherAddressState').val(response.State);
    jQuery('#OtherAddressPin').val(response.Pin);
    jQuery('#OtherAddressCountry').val(response.Country);
    jQuery('#OtherAddressPhone').val(response.Phone);
    jQuery('#OtherAddressEMail').val(response.EMail);
};

Contact.init();
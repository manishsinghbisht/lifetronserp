var WorkOrder = new Object();

WorkOrder.init = function () {
    $(document).ready(WorkOrder.ready);
    $('#BillingAddressId').on('change', WorkOrder.AddressDdlChange);
    $('#ShippingAddressId').on('change', WorkOrder.AddressDdlChange);
    $("#btnSearch").click(WorkOrder.Search);
     $('input[type="submit"]').click(WorkOrder.Submit);
};

WorkOrder.Submit = function () {
    var btn = jQuery(this);
    btn.prop('disabled', true);
    setTimeout(function () {
        btn.prop('disabled', false);
    }, 2000);
    //jQuery("#frm").submit();
    //var form = $(this).closest('form');
    $(this.form).submit();
};

WorkOrder.Search = function () {
    ////////$('#frmSearch').attr('action', 'WorkOrder/Search');
    //var pathname = window.location.pathname;
    //if (pathname.toLowerCase().indexOf("WorkOrder/") >= 0) {

    //    $('form[name="frmSearch"]').attr('action', 'Search');
    //}
    //else {
    //    $('form[name="frmSearch"]').attr('action', 'WorkOrder/Search');
    //}
    //jQuery("#frmSearch").submit();
    $(this.form).submit();
};

WorkOrder.ready = function () {
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

    $("#stepWizardWorkOrder").removeClass("btn-default");
    $("#stepWizardWorkOrder").addClass("btn-primary");

    WorkOrder.ClearControls();
};

WorkOrder.ClearControls = function () {
    $("#Desc").val('');
    $("#ProgressDesc").val('');
};



WorkOrder.AddressDdlChange = function () {
    var controlId = jQuery(this).attr('id');
    var controlName = jQuery(this).attr('name');
    var selectedVal = jQuery('#' + controlId + ' option:selected').val();

    if (selectedVal != "") {
        WorkOrder.PostBackAddress(jQuery(this));
    }
};

WorkOrder.PostBackAddress = function (control) {

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

            if (controlId.toLowerCase().indexOf("billing") >= 0) {
                WorkOrder.UpdateBillingAddressControls(response);
            } else {
                WorkOrder.UpdateShippingAddressControls(response);
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

WorkOrder.UpdateBillingAddressControls = function (response) {
    jQuery('#BillingAddressToName').val(response.AddressToName);
    jQuery('#BillingAddressLine1').val(response.AddressLine1);
    jQuery('#BillingAddressLine2').val(response.AddressLine2);
    jQuery('#BillingAddressLine3').val(response.AddressLine3);
    jQuery('#BillingAddressCity').val(response.City);
    jQuery('#BillingAddressState').val(response.State);
    jQuery('#BillingAddressPin').val(response.Pin);
    jQuery('#BillingAddressCountry').val(response.Country);
    jQuery('#BillingAddressPhone').val(response.Phone);
    jQuery('#BillingAddressEMail').val(response.EMail);
};

WorkOrder.UpdateShippingAddressControls = function (response) {
    jQuery('#ShippingAddressToName').val(response.AddressToName);
    jQuery('#ShippingAddressLine1').val(response.AddressLine1);
    jQuery('#ShippingAddressLine2').val(response.AddressLine2);
    jQuery('#ShippingAddressLine3').val(response.AddressLine3);
    jQuery('#ShippingAddressCity').val(response.City);
    jQuery('#ShippingAddressState').val(response.State);
    jQuery('#ShippingAddressPin').val(response.Pin);
    jQuery('#ShippingAddressCountry').val(response.Country);
    jQuery('#ShippingAddressPhone').val(response.Phone);
    jQuery('#ShippingAddressEMail').val(response.EMail);
};


WorkOrder.init();
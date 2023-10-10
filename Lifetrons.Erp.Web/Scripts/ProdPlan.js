var ProdPlan = new Object();

ProdPlan.init = function () {
    $(document).ready(ProdPlan.ready);
    $("#btnSearch").click(ProdPlan.Search);
    $('input[type="submit"]').click(ProdPlan.Submit);
    $('#ProcessId').on('change', ProdPlan.ProcessDdlChange);
    $('.glyphicon-remove-circle').click(ProdPlan.ClearPrevInputControl);
};

ProdPlan.Submit = function () {
    var btn = jQuery(this);
    btn.prop('disabled', true);
    setTimeout(function () {
        btn.prop('disabled', false);
    }, 2000);
    //jQuery("#frm").submit();
    //var form = $(this).closest('form');
    $(this.form).submit();
};

ProdPlan.Search = function () {
    //////$('#frmSearch').attr('action', 'Invoice/Search');
    //var pathname = window.location.pathname;
    //if (pathname.toLowerCase().indexOf("ProdPlan/") >= 0) {

    //    $('form[name="frmSearch"]').attr('action', 'Search');
    //}
    //else {
    //    $('form[name="frmSearch"]').attr('action', 'ProdPlan/Search');
    //}
    //jQuery("#frmSearch").submit();
    $(this.form).submit();
};

ProdPlan.ready = function () {
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

    $("#stepWizardPlanning").removeClass("btn-default");
    $("#stepWizardPlanning").addClass("btn-primary");

    ProdPlan.ClearControls();
};

ProdPlan.ClearPrevInputControl = function () {
    $(this).prev("input").val('');
    // alert($(this).prev('input').attr('id'));
};

ProdPlan.ClearControls = function () {
    $("#Remark").val('');
};


ProdPlan.ProcessDdlChange = function () {
    
    var controlId = jQuery(this).attr('id');
    var controlName = jQuery(this).attr('name');
    var selectedVal = jQuery('#' + controlId + ' option:selected').val();

    if (selectedVal != "") {
        ProdPlan.PostBackProcess(jQuery(this));
    }
};

ProdPlan.PostBackProcess = function (control) {
   
    var controlId = jQuery(control).attr('id');
    var controlName = jQuery(control).attr('name');
    var selectedVal = jQuery('#' + controlId + ' option:selected').val();


    var action = "../PostBackProcess";
    //if ($('form[name="foo"]'))
    var parentForm = $('#' + controlId).closest("form");
    if (jQuery(parentForm).attr('action').toLowerCase().indexOf("edit") >= 0) {
        action = "../PostBackProcess";
    } else {
        action = "PostBackProcess";
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
            ProdPlan.UpdateUI(response);
            jQuery(control).attr('disabled', false);
        },
        complete: function (jqXhR, textStatus) {
            if (textStatus == "success") {

            }
            jQuery(control).attr('disabled', false);
        }
    });
};

ProdPlan.UpdateUI = function (response) {
    jQuery('#CycleTimeInHour').val(response.CycleTimeInHour);
    jQuery('#CycleCapacity').val(response.CycleCapacity);
};

ProdPlan.init();
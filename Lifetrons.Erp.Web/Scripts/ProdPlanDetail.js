var ProdPlanDetail = new Object();

ProdPlanDetail.init = function () {
    $(document).ready(ProdPlanDetail.ready);
    $("#btnSearch").click(ProdPlanDetail.Search);
    $('#JobNo').on('blur', ProdPlanDetail.ProcessJobNoBlur);
    $('input[type="submit"]').click(ProdPlanDetail.Submit);
    $('.glyphicon-remove-circle').click(ProdPlanDetail.ClearPrevInputControl);
    $('a.info_link').click(ProdPlanDetail.LinkClick);
};

ProdPlanDetail.Submit = function () {
    var btn = jQuery(this);
    btn.prop('disabled', true);
    setTimeout(function () {
        btn.prop('disabled', false);
    }, 2000);
    //jQuery("#frm").submit();
    //var form = $(this).closest('form');
    $(this.form).submit();
};

ProdPlanDetail.Search = function () {
    ////////$('#frmSearch').attr('action', 'Invoice/Search');
    var pathname = window.location.pathname;
    
    //if (pathname.toLowerCase().indexOf("ProdPlanDetail/") >= 0) {

    //    $('form[name="frmSearch"]').attr('action', 'Search');
    //}
    //else {
    //    $('form[name="frmSearch"]').attr('action', 'ProdPlanDetail/Search');
    //}
    $(this.form).submit();
    //jQuery("#frmSearch").submit();
};

ProdPlanDetail.ready = function () {
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

    ////This is causing menu is issues if not #WeightUnitId is not find visible on page, which happens in case of index.
    //$("#WeightUnitId").rules("add", {
    //    required: function (element) {
    //        return $("#Weight").val() != "";
    //    },
    //    messages: {
    //        required: "*",
    //        minlength: jQuery.format("Please, at least {0} characters are necessary")
    //    }
    //});

    $("#Remark").val('');

};

ProdPlanDetail.ClearPrevInputControl = function () {
    $(this).prev("input").val('');
    // alert($(this).prev('input').attr('id'));
};

ProdPlanDetail.ProcessJobNoBlur = function () {

    var controlId = jQuery(this).attr('id');
    var controlName = jQuery(this).attr('name');
    //var selectedVal = jQuery('#' + controlId + ' option:selected').val();
    var selectedVal = jQuery('#' + controlId).val();
    if (selectedVal != "") {
        ProdPlanDetail.PostBackJobNo(jQuery(this));
    }

};

ProdPlanDetail.PostBackJobNo = function (control) {

    var controlId = jQuery(control).attr('id');
    var controlName = jQuery(control).attr('name');
    //var selectedVal = jQuery('#' + controlId + ' option:selected').val();
    var selectedVal = jQuery('#' + controlId).val();

    var action = "../PostBackJobNo";
    //if ($('form[name="foo"]'))
    var parentForm = $('#' + controlId).closest("form");
    if (jQuery(parentForm).attr('action').toLowerCase().indexOf("edit") >= 0) {
        action = "../PostBackJobNo";
    } else {
        action = "PostBackJobNo";
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
            ProdPlanDetail.UpdateUI(response);
            jQuery(control).attr('disabled', false);
        },
        complete: function (jqXhR, textStatus) {
            if (textStatus == "success") {

            }
            jQuery(control).attr('disabled', false);
        }
    });
};

ProdPlanDetail.UpdateUI = function (response) {
    jQuery('#Quantity').val(response.Quantity);
    jQuery('#Weight').val(response.Weight);
    jQuery('#WeightUnitId').val(response.WeightUnitId);
    jQuery('#WeightUnitId option:selected').val(response.WeightUnitId);
    jQuery('#CycleTimeInHour').val(response.CycleTimeInHour);
    jQuery('#CycleCapacity').val(response.CycleCapacity);
};

ProdPlanDetail.LinkClick = function (e) {

    var controlId = jQuery(this).attr('id');
    var controlName = jQuery(this).attr('name');
    var selectedVal = jQuery('#' + controlId).val();
    e.preventDefault();
   
    ProdPlanDetail.SubmitLink(jQuery(this));
    

};

ProdPlanDetail.SubmitLink = function (control) {

    var controlId = jQuery(control).attr('id');
    var controlName = jQuery(control).attr('name');
    var selectedVal = jQuery('#' + controlId).val();

    var action = jQuery(control).attr('href');
    var parentForm = $('#' + controlId).closest("form");

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
            alert("Check if planning record is authorized and quantity is valid. Ajax postback error occured: " + errMsg);
            jQuery(control).attr('disabled', false);
        },
        success: function (response) {
            jQuery(control).addClass("hidden");
            jQuery(control).attr('disabled', true);
        },
        complete: function (jqXhR, textStatus) {
            if (textStatus == "success") {
               
            }
        }
    });
};


ProdPlanDetail.init();
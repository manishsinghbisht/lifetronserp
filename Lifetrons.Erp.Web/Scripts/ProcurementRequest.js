var ProcurementRequest = new Object();

ProcurementRequest.init = function () {
    $(document).ready(ProcurementRequest.ready);
    $('#DepartmentId').on('change', ProcurementRequest.DepartmentDDLChange);
    $("#btnSearch").click(ProcurementRequest.Search);
    $('input[type="submit"]').click(ProcurementRequest.Submit);
    $('.glyphicon-remove-circle').click(ProcurementRequest.ClearPrevInputControl);
};

ProcurementRequest.Submit = function () {
    var btn = jQuery(this);
    btn.prop('disabled', true);
    setTimeout(function () {
        btn.prop('disabled', false);
    }, 2000);
    //jQuery("#frm").submit();
    //var form = $(this).closest('form');
    $(this.form).submit();
};

ProcurementRequest.Search = function () {
    //////$('#frmSearch').attr('action', 'Invoice/Search');
    //var pathname = window.location.pathname;
    //if (pathname.toLowerCase().indexOf("StockIssue/") >= 0) {

    //    $('form[name="frmSearch"]').attr('action', 'Search');
    //}
    //else {
    //    $('form[name="frmSearch"]').attr('action', 'StockIssue/Search');
    //}
    //jQuery("#frmSearch").submit();
    $(this.form).submit();
};

ProcurementRequest.ready = function () {
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

    ProcurementRequest.ClearControls();
};

ProcurementRequest.ClearPrevInputControl = function () {
    $(this).prev("input").val('');
    // alert($(this).prev('input').attr('id'));
};

ProcurementRequest.ClearControls = function () {
    $("#Remark").val('');
};

//// Gets the id of the dropdown changed and postback to appropriate action
ProcurementRequest.DepartmentDDLChange = function () {
    var controlId = jQuery(this).attr('id');
    var controlName = jQuery(this).attr('name');
    var selectedText = jQuery('#' + controlId + ' option:selected').text();
    var selectedVal = jQuery('#' + controlId + ' option:selected').val();
    
    if (selectedVal != "") {
        ProcurementRequest.ReFillEmployeeDropDown(jQuery(this));
    }
};


ProcurementRequest.ReFillEmployeeDropDown = function (control) {
    var controlId = jQuery(control).attr('id');
    var controlName = jQuery(control).attr('name');
    var selectedText = jQuery('#' + controlId + ' option:selected').text();
    var selectedVal = jQuery('#' + controlId + ' option:selected').val();

    var action = "../ProcessJsonResponseForDepartmentChange";
    //if ($('form[name="foo"]'))
    var parentForm = $('#' + controlId).closest("form");
    if (jQuery(parentForm).attr('action').toLowerCase().indexOf("edit") >= 0) {
        action = "../ProcessJsonResponseForDepartmentChange";
    } else {
        action = "ProcessJsonResponseForDepartmentChange";
    }

    $.ajax({
        type: 'POST',
        url: action,
        data: JSON.stringify({ stringifiedParam: selectedVal }),
        contentType: 'application/json',
        beforeSend: function () {
            jQuery(control).attr('disabled', true);
            jQuery('#EmployeeId').attr('disabled', true);
        },
        error: function (xhr, status, error) {
            //do something about the error
            var errMsg = xhr.status + "\r\n" + status + "\r\n" + error;
            alert("Ajax postback error occured: " + errMsg);
            jQuery(control).attr('disabled', false);
            jQuery('#EmployeeId').attr('disabled', false);
        },
        success: function (response) {
            //for (k = 0; k < data.length; k++)
            //    jQuery('#EmployeeId').append("<option value='" + data[k] + "'>" + buildings[k] + "</option>");
            var items = [];
            //items.push("<option>--Choose Your Area--</option>");
            $.each(response, function () {
                items.push("<option value=" + this.Value + ">" + this.Text + "</option>");
            });
            $("#EmployeeId").html(items.join(' '));

            jQuery(control).attr('disabled', false);
            jQuery('#EmployeeId').attr('disabled', false);
        }
    });
};

ProcurementRequest.init();
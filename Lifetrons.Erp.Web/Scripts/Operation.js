var Operation = new Object();

Operation.init = function () {
    $(document).ready(Operation.ready);
    $("#btnSearch").click(Operation.Search);
    $('input[type="submit"]').click(Operation.Submit);
    $('#EnterpriseStageId').on('change', Operation.EnterpriseStageDdlChange);
};

Operation.Submit = function () {
    var btn = jQuery(this);
    btn.prop('disabled', true);
    setTimeout(function () {
        btn.prop('disabled', false);
    }, 2000);
    //jQuery("#frm").submit();
    //var form = $(this).closest('form');
    $(this.form).submit();
    
};


$("#frm").validate({
    rules: {
        Serial: {
            required: true
        },
        CycleTimeInHour: {
            required: true
        },
        CycleCapacity: {
            required: true
        }
    }
});

Operation.Search = function () {
    ////////$('#frmSearch').attr('action', 'Operation/Search');
    //var pathname = window.location.pathname;
    //if (pathname.toLowerCase().indexOf("Operation/") >= 0) {

    //    $('form[name="frmSearch"]').attr('action', 'Search');
    //}
    //else {
    //    $('form[name="frmSearch"]').attr('action', 'Operation/Search');
    //}
    //jQuery("#frmSearch").submit();
    $(this.form).submit();
};
Operation.ready = function () {
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

    $("#stepWizardLOF").removeClass("btn-default");
    $("#stepWizardLOF").addClass("btn-primary");


    Operation.ClearControls();
};

Operation.ClearControls = function () {
    $("#Remark").val('');
};


//// Gets the id of the dropdown changed and postback to appropriate action
Operation.EnterpriseStageDdlChange = function () {
    var controlId = jQuery(this).attr('id');
    var controlName = jQuery(this).attr('name');
    var selectedVal = jQuery('#' + controlId + ' option:selected').val();

    if (selectedVal != "") {
        Operation.LoadProcessDdl(jQuery(this));
    }
};


Operation.LoadProcessDdl = function (control) {
    var controlId = jQuery(control).attr('id');
    var controlName = jQuery(control).attr('name');
    var selectedVal = jQuery('#' + controlId + ' option:selected').val();
    var action = "ProcessJsonResponseForProcessDdl";
    $.ajax({
        type: 'POST',
        url: action,
        data: JSON.stringify({ stringifiedParam: selectedVal }),
        contentType: 'application/json',
        beforeSend: function () {
            jQuery(control).attr('disabled', true);
            jQuery('#ProcessId').attr('disabled', true);
        },
        error: function (xhr, status, error) {
            //do something about the error
            var errMsg = xhr.status + "\r\n" + status + "\r\n" + error;
            alert("Ajax postback error occured: " + errMsg);
            jQuery(control).attr('disabled', false);
            jQuery('#ProcessId').attr('disabled', false);
        },
        success: function (response) {
            //for (k = 0; k < data.length; k++)
            //    jQuery('#RelatedToId').append("<option value='" + data[k] + "'>" + buildings[k] + "</option>");
            var items = [];
            //items.push("<option>--Choose Your Area--</option>");
            //items.push("<option></option>");
            $.each(response, function () {
                items.push("<option value=" + this.Value + ">" + this.Text + "</option>");
            });
            $("#ProcessId").html(items.join(' '));

            jQuery(control).attr('disabled', false);
            jQuery('#ProcessId').attr('disabled', false);
        }
    });
};
Operation.init();
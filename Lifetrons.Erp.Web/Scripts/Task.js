var Task = new Object();

Task.init = function () {
    $(document).ready(Task.ready);
    $('#RelatedToObjectName').on('change', Task.RelatedToObjectNameDDLChange);
    $("#btnSearch").click(Task.Search);
     $('input[type="submit"]').click(Task.Submit);
};

Task.Submit = function () {
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
        //Quantity: {
        //    required: true
        //},
        RelatedToId: {
            required: function (element) {
                return $("#RelatedToObjectName").val() != "";
            }
        }
    }
});

Task.Search = function () {
    //////$('#frmSearch').attr('action', 'Task/Search');
    //var pathname = window.location.pathname;
    //if (pathname.toLowerCase().indexOf("task/") >= 0) {

    //    $('form[name="frmSearch"]').attr('action', 'Search');
    //}
    //else {
    //    $('form[name="frmSearch"]').attr('action', 'Task/Search');
    //}
    //jQuery("#frmSearch").submit();
    $(this.form).submit();
};

Task.ready = function () {
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
    Task.ClearControls();
};

Task.ClearControls = function () {
    $("#Desc").val('');
    $("#ProgressDesc").val('');
};


//// Gets the id of the dropdown changed and postback to appropriate action
Task.RelatedToObjectNameDDLChange = function () {
    var controlId = jQuery(this).attr('id');
    var controlName = jQuery(this).attr('name');
    var selectedVal = jQuery('#' + controlId + ' option:selected').val();

    if (selectedVal != "") {
        Task.ReFillRelatedToDropDown(jQuery(this));
    }
};


Task.ReFillRelatedToDropDown = function (control) {
   
    var controlId = jQuery(control).attr('id');
    var controlName = jQuery(control).attr('name');
    var selectedVal = jQuery('#' + controlId + ' option:selected').val();
    var action = "ProcessJsonResponseForRelatedToDdl";
    $.ajax({
        type: 'POST',
        url: action,
        data: JSON.stringify({ stringifiedParam: selectedVal }),
        contentType: 'application/json',
        beforeSend: function () {
            jQuery(control).attr('disabled', true);
            jQuery('#RelatedToId').attr('disabled', true);
        },
        error: function (xhr, status, error) {
            //do something about the error
            var errMsg = xhr.status + "\r\n" + status + "\r\n" + error;
            alert("Ajax postback error occured: " + errMsg);
            jQuery(control).attr('disabled', false);
            jQuery('#RelatedToId').attr('disabled', false);
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
            $("#RelatedToId").html(items.join(' '));

            jQuery(control).attr('disabled', false);
            jQuery('#RelatedToId').attr('disabled', false);
        }
    });
};

Task.init();

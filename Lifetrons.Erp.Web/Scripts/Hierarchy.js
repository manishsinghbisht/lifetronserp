var Hierarchy = new Object();

Hierarchy.init = function () {
    $(document).ready(Hierarchy.ready);
    $('#DepartmentId').on('change', Hierarchy.DepartmentDDLChange);
    $("#btnSearch").click(Hierarchy.Search);
     $('input[type="submit"]').click(Hierarchy.Submit);
};

Hierarchy.Submit = function () {
    var btn = jQuery(this);
    btn.prop('disabled', true);
    setTimeout(function () {
        btn.prop('disabled', false);
    }, 2000);
    jQuery("#frm").submit();
};


Hierarchy.Search = function () {
    ////////$('#frmSearch').attr('action', 'Hierarchy/Search');
    //var pathname = window.location.pathname;
    //if (pathname.toLowerCase().indexOf("Hierarchy/") >= 0) {

    //    $('form[name="frmSearch"]').attr('action', 'Search');
    //}
    //else {
    //    $('form[name="frmSearch"]').attr('action', 'Hierarchy/Search');
    //}
    //jQuery("#frmSearch").submit();
    $(this.form).submit();
};

Hierarchy.ready = function () {
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
};

//// Gets the id of the dropdown changed and postback to appropriate action
Hierarchy.DepartmentDDLChange = function () {
    var controlId = jQuery(this).attr('id');
    var controlName = jQuery(this).attr('name');
    var selectedText = jQuery('#' + controlId + ' option:selected').text();
    var selectedVal = jQuery('#' + controlId + ' option:selected').val();
    
    if (selectedVal != "") {
        Hierarchy.ReFillTeamDropDown(jQuery(this));
    }
};


Hierarchy.ReFillTeamDropDown = function (control) {

    var controlId = jQuery(control).attr('id');
    var controlName = jQuery(control).attr('name');
    var selectedText = jQuery('#' + controlId + ' option:selected').text();
    var selectedVal = jQuery('#' + controlId + ' option:selected').val();

    var action = "../ProcessJsonResponseForTeamDdl";
    //if ($('form[name="foo"]'))
    var parentForm = $('#' + controlId).closest("form");
    if (jQuery(parentForm).attr('action').toLowerCase().indexOf("edit") >= 0) {
        action = "../ProcessJsonResponseForTeamDdl";
    } else {
        action = "ProcessJsonResponseForTeamDdl";
    }

    $.ajax({
        type: 'POST',
        url: action,
        data: JSON.stringify({ stringifiedParam: selectedVal }),
        contentType: 'application/json',
        beforeSend: function () {
            jQuery(control).attr('disabled', true);
            jQuery('#TeamId').attr('disabled', true);
        },
        error: function (xhr, status, error) {
            //do something about the error
            var errMsg = xhr.status + "\r\n" + status + "\r\n" + error;
            alert("Ajax postback error occured: " + errMsg);
            jQuery(control).attr('disabled', false);
            jQuery('#TeamId').attr('disabled', false);
        },
        success: function (response) {
            //for (k = 0; k < data.length; k++)
            //    jQuery('#TeamId').append("<option value='" + data[k] + "'>" + buildings[k] + "</option>");
            var items = [];
            //items.push("<option>--Choose Your Area--</option>");
            $.each(response, function () {
                items.push("<option value=" + this.Value + ">" + this.Text + "</option>");
            });
            $("#TeamId").html(items.join(' '));

            jQuery(control).attr('disabled', false);
            jQuery('#TeamId').attr('disabled', false);
        }
    });
};

Hierarchy.init();

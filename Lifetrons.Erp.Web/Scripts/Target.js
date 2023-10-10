var Target = new Object();

Target.init = function () {
    $(document).ready(Target.ready);
    $('#ObjectName').on('change', Target.ObjectNameDDLChange);
    $("#btnSearch").click(Target.Search);
     $('input[type="submit"]').click(Target.Submit);
};

Target.Submit = function () {
    var btn = jQuery(this);
    btn.prop('disabled', true);
    setTimeout(function () {
        btn.prop('disabled', false);
    }, 2000);
    //jQuery("#frm").submit();
    //var form = $(this).closest('form');
    $(this.form).submit();
};


Target.Search = function () {
    //////$('#frmSearch').attr('action', 'Target/Search');
    //var pathname = window.location.pathname;
    //if (pathname.toLowerCase().indexOf("target/") >= 0) {

    //    $('form[name="frmSearch"]').attr('action', 'Search');
    //}
    //else {
    //    $('form[name="frmSearch"]').attr('action', 'Target/Search');
    //}
    //jQuery("#frmSearch").submit();
    $(this.form).submit();
};

Target.ready = function () {
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

    $("#stepWizardTarget").removeClass("btn-default");
    $("#stepWizardTarget").addClass("btn-primary");


    Target.ClearControls();
};

Target.ClearControls = function () {
    $("#Desc").val('');
    $("#ProgressDesc").val('');
};


//// Gets the id of the dropdown changed and postback to appropriate action
Target.ObjectNameDDLChange = function () {
    var controlId = jQuery(this).attr('id');
    var controlName = jQuery(this).attr('name');
    var selectedVal = jQuery('#' + controlId + ' option:selected').val();

    if (selectedVal != "") {
        Target.ReFillObjectIdDropDown(jQuery(this));
    }
};


Target.ReFillObjectIdDropDown = function (control) {
   
    var controlId = jQuery(control).attr('id');
    var controlName = jQuery(control).attr('name');
    var selectedVal = jQuery('#' + controlId + ' option:selected').val();
    var action = "ProcessJsonResponseForObjectIdDdl";
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
            jQuery('#ObjectId').attr('disabled', false);
        },
        success: function (response) {
            //for (k = 0; k < data.length; k++)
            //    jQuery('#RelatedToId').append("<option value='" + data[k] + "'>" + buildings[k] + "</option>");
            var items = [];
            items.push("<option>--Choose--</option>");
            $.each(response, function () {
                items.push("<option value=" + this.Value + ">" + this.Text + "</option>");
            });
            $("#ObjectId").html(items.join(' '));

            jQuery(control).attr('disabled', false);
            jQuery('#ObjectId').attr('disabled', false);
        }
    });
};

Target.init();

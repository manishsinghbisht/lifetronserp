var DispatchLineItem = new Object();

DispatchLineItem.init = function () {
    $(document).ready(DispatchLineItem.ready);
    $('input[type="submit"]').click(DispatchLineItem.Submit);
    $('#OrderId').on('change', DispatchLineItem.OrderDDLChange);

};

DispatchLineItem.Submit = function () {
    var btn = jQuery(this);
    btn.prop('disabled', true);
    setTimeout(function () {
        btn.prop('disabled', false);
    }, 2000);
    jQuery("#frm").submit();
};

$("#frm").validate({
    rules: {
        Quantity: {
            required: true
        },
        WeightUnitId: {
            required: function(element) {
                return $("#Weight").val() != "";
            }
        },
        OrderId: {
            required: true
        },
        OrderLineItemId: {
            required: true
            }
    }
});

DispatchLineItem.ready = function () {
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
    $("#Remark").val('');
};


//// Gets the id of the dropdown changed and postback to appropriate action
DispatchLineItem.OrderDDLChange = function () {
    var controlId = jQuery(this).attr('id');
    var controlName = jQuery(this).attr('name');
    var selectedVal = jQuery('#' + controlId + ' option:selected').val();

    if (selectedVal != "") {
        DispatchLineItem.FillOrderLineItemsDropDown(jQuery(this));
    }
};


DispatchLineItem.FillOrderLineItemsDropDown = function (control) {

    var controlId = jQuery(control).attr('id');
    var controlName = jQuery(control).attr('name');
    var selectedVal = jQuery('#' + controlId + ' option:selected').val();
    var action = "ProcessJsonResponseFillOrderLineItemsDropDown";
    $.ajax({
        type: 'POST',
        url: action,
        data: JSON.stringify({ stringifiedParam: selectedVal }),
        contentType: 'application/json',
        beforeSend: function () {
            jQuery(control).attr('disabled', true);
            jQuery('#OrderLineItemId').attr('disabled', true);
        },
        error: function (xhr, status, error) {
            //do something about the error
            var errMsg = xhr.status + "\r\n" + status + "\r\n" + error;
            alert("Ajax postback error occured: " + errMsg);
            jQuery(control).attr('disabled', false);
            jQuery('#OrderLineItemId').attr('disabled', false);
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
            $("#OrderLineItemId").html(items.join(' '));

            jQuery(control).attr('disabled', false);
            jQuery('#OrderLineItemId').attr('disabled', false);
        }
    });
};

DispatchLineItem.init();
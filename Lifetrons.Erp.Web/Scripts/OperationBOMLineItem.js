var OperationBOMLineItem = new Object();

OperationBOMLineItem.init = function () {
    $(document).ready(OperationBOMLineItem.ready);
    $("#btnSearch").click(OperationBOMLineItem.Search);
     $('input[type="submit"]').click(OperationBOMLineItem.Submit);
};

OperationBOMLineItem.Submit = function () {
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
        Quantity: {
            required: true
        },
        WeightUnitId: {
            required: function (element) {
                return $("#Weight").val() != "";
            }
        },
        AllowedLossWeightUnitId: {
            required: function (element) {
                return $("#AllowedLossWeight").val() != "";
            }
        }
    }
});

OperationBOMLineItem.Search = function () {
    ////////$('#frmSearch').attr('action', 'OperationBOMLineItem/Search');
    //var pathname = window.location.pathname;
    //if (pathname.toLowerCase().indexOf("OperationBOMLineItem/") >= 0) {

    //    $('form[name="frmSearch"]').attr('action', 'Search');
    //}
    //else {
    //    $('form[name="frmSearch"]').attr('action', 'OperationBOMLineItem/Search');
    //}
    //jQuery("#frmSearch").submit();
    $(this.form).submit();
};

OperationBOMLineItem.ready = function () {
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

    OperationBOMLineItem.ClearControls();
};

OperationBOMLineItem.ClearControls = function () {
    $("#Desc").val('');
};


OperationBOMLineItem.init();
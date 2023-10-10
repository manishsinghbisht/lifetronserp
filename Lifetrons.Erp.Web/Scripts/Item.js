var Item = new Object();

Item.init = function () {
    $(document).ready(Item.ready);
    $("#btnSearch").click(Item.Search);
    $("#btnShowcaseSearch").click(Item.ShowcaseSearch);
    $('input[type="submit"]').click(Item.Submit);
};

Item.Submit = function () {

    var btn = jQuery(this);
    btn.prop('disabled', true);
    setTimeout(function () {
        btn.prop('disabled', false);
    }, 2000);
    jQuery("#frm").submit();

};

$("#frm").validate({
    rules: {
        Code: {
            required: true
        },
        WeightUnitId: {
            required: function (element) {
                return $("#Weight").val() != "";
            }
        }
    }
});


Item.Search = function () {
    ////////$('#frmSearch').attr('action', 'Invoice/Search');
    //var pathname = window.location.pathname;
    //if (pathname.toLowerCase().indexOf("Item/") >= 0) {

    //    $('form[name="frmSearch"]').attr('action', 'Search');
    //}
    //else {
    //    $('form[name="frmSearch"]').attr('action', 'Item/Search');
    //}
    //jQuery("#frmSearch").submit();
    $(this.form).submit();
};

Item.ShowcaseSearch = function () {
    var pathname = window.location.pathname;
    if (pathname.toLowerCase().indexOf("Item/") >= 0) {

        $('form[name="frmShowcaseSearch"]').attr('action', 'ShowcaseSearch');
    }
    else {
        $('form[name="frmShowcaseSearch"]').attr('action', 'Item/ShowcaseSearch');
    }
    jQuery("#frmShowcaseSearch").submit();
};

Item.ready = function () {
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

    $("#stepWizardItem").removeClass("btn-default");
    $("#stepWizardItem").addClass("btn-primary");

};


Item.init();
var Product = new Object();

Product.init = function () {
    $(document).ready(Product.ready);
    $("#btnSearch").click(Product.Search);
    $("#btnShowcaseSearch").click(Product.ShowcaseSearch);
    $('input[type="submit"]').click(Product.Submit);
};

Product.Submit = function () {

    var btn = jQuery(this);
    btn.prop('disabled', true);
    setTimeout(function () {
        btn.prop('disabled', false);
    }, 2000);

    jQuery("#frm").submit();

};

//$("#frm").validate({

//rules: {
//    Code: {
//        required: true
//    },
//    WeightUnitId: {
//        required: function (element) {
//            return $("#Weight").val() != "";
//        }
//    }
//}
//});


Product.Search = function () {
    //////$('#frmSearch').attr('action', 'Invoice/Search');
    //var pathname = window.location.pathname;
    //if (pathname.toLowerCase().indexOf("product/") >= 0) {

    //    $('form[name="frmSearch"]').attr('action', 'Search');
    //}
    //else {
    //    $('form[name="frmSearch"]').attr('action', 'Product/Search');
    //}
    //jQuery("#frmSearch").submit();
    $(this.form).submit();
};

Product.ShowcaseSearch = function () {
    var pathname = window.location.pathname;
    if (pathname.toLowerCase().indexOf("product/") >= 0) {

        $('form[name="frmShowcaseSearch"]').attr('action', 'ShowcaseSearch');
    }
    else {
        $('form[name="frmShowcaseSearch"]').attr('action', 'Product/ShowcaseSearch');
    }
    jQuery("#frmShowcaseSearch").submit();
};

Product.ready = function () {
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

    $("#stepWizardProduct").removeClass("btn-default");
    $("#stepWizardProduct").addClass("btn-primary");

    //This is causing menu is issues if not #WeightUnitId is not find visible on page, which happens in case of index.
    //$("#WeightUnitId").rules("add", {
    //    required: function (element) {
    //        return $("#Weight").val() != "";
    //    },
    //    messages: {
    //        required: "*",
    //    }
    //});

};


Product.init();
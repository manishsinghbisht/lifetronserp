var PriceBook = new Object();

PriceBook.init = function () {
    $(document).ready(PriceBook.ready);
    $("#btnSearch").click(PriceBook.Search);
    $("#btnSelectSharedWithUser").click(PriceBook.AddSharedWithUser);
    $('input[type="submit"]').click(PriceBook.Submit);
};

PriceBook.Submit = function () {
    var btn = jQuery(this);
    btn.prop('disabled', true);
    setTimeout(function () {
        btn.prop('disabled', false);
    }, 2000);
    //jQuery("#frm").submit();
    //var form = $(this).closest('form');
    $(this.form).submit();
};

PriceBook.AddSharedWithUser = function() {
    var sharedWithUsersList = $("#SharedWith").val();
    sharedWithUsersList = sharedWithUsersList + $("#SharedWithUsersSelectList option:selected").text() + ";";
    $("#SharedWith").val(sharedWithUsersList);
};

PriceBook.Search = function () {
    //////$('#frmSearch').attr('action', 'PriceBook/Search');
    //var pathname = window.location.pathname;
    //if (pathname.toLowerCase().indexOf("priceBook/") >= 0) {

    //    $('form[name="frmSearch"]').attr('action', 'Search');
    //}
    //else {
    //    $('form[name="frmSearch"]').attr('action', 'PriceBook/Search');
    //}
    //jQuery("#frmSearch").submit();
    $(this.form).submit();
};

PriceBook.ready = function () {
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

    $("#stepWizardPriceBook").removeClass("btn-default");
    $("#stepWizardPriceBook").addClass("btn-primary");

    PriceBook.ClearControls();
};

PriceBook.ClearControls = function () {
    $("#Desc").val('');
};

PriceBook.init();
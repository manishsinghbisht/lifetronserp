var Contract = new Object();

Contract.init = function () {
    $(document).ready(Contract.ready);
    $("#btnSearch").click(Contract.Search);
     $('input[type="submit"]').click(Contract.Submit);
};

Contract.Submit = function () {
    var btn = jQuery(this);
    btn.prop('disabled', true);
    setTimeout(function () {
        btn.prop('disabled', false);
    }, 2000);
    jQuery("#frm").submit();
};

Contract.Search = function () {
    ////////$('#frmSearch').attr('action', 'Contract/Search');
    //var pathname = window.location.pathname;
    //if (pathname.toLowerCase().indexOf("contract/") >= 0) {

    //    $('form[name="frmSearch"]').attr('action', 'Search');
    //}
    //else {
    //    $('form[name="frmSearch"]').attr('action', 'Contract/Search');
    //}
    //jQuery("#frmSearch").submit();
    $(this.form).submit();
};
Contract.ready = function () {
    $('[data-toggle="tooltip"]').tooltip({ 'placement': 'right' });
    $('[data-toggle1="popover"]').popover({ trigger: 'hover', 'placement': 'top' });

    $('.dateTxt').datetimepicker({
        ////These options can also be set by data-date-OPTION in HTML
        //pickDate: true,                 //en/disables the date picker
        //pickTime: true,                 //en/disables the time picker
        //defaultDate: new Date(),
    });
};

Contract.init();
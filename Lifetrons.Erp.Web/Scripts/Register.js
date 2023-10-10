var Register = new Object();

Register.init = function () {
    $(document).ready();
  
    //alert($("[name*='ProductId']) option:selected").text());
    //var priceBookId = $('#PriceBookId').val();
    //$("select[name*='PriceBookId']").val(priceBookId);

    $('[data-toggle="tooltip"]').tooltip({ 'placement': 'right' });
    $('[data-toggle1="popover"]').popover({ trigger: 'hover', 'placement': 'top' });

    $('.dateTxt').datetimepicker({
        ////These options can also be set by data-date-OPTION in HTML
        //pickDate: true,                 //en/disables the date picker
        //pickTime: true,                 //en/disables the time picker
        //defaultDate: new Date(),
    });
    $('input[type="submit"]').click(Register.Submit);
    $('input[type="submit"]').prop('disabled', true);
    $('#chkAccept').click(Register.AgreementAccepted);
};

Register.AgreementAccepted = function () {
    if ($("#chkAccept").is(':checked'))
        {
        $('input[type="submit"]').prop('disabled', false);
    } else {
        $('input[type="submit"]').prop('disabled', true);
    }

};


Register.Submit = function () {
    var btn = jQuery(this);
    btn.prop('disabled', true);
    setTimeout(function () {
        btn.prop('disabled', false);
    }, 2000);
    jQuery("#frm").submit();
};



Register.init();
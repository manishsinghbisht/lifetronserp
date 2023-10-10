var Media = new Object();

Media.init = function () {
    $(document).ready(Media.ready);
    $("#btnSearch").click(Media.Search);
    $('input[type="submit"]').click(Media.Submit);
};

Media.Submit = function () {
    var btn = jQuery(this);
    btn.prop('disabled', true);
    setTimeout(function () {
        btn.prop('disabled', false);
    }, 2000);
    jQuery("#frm").submit();
};

Media.Search = function () {
    ////////$('#frmSearch').attr('action', 'Invoice/Search');
    //var pathname = window.location.pathname;
    //if (pathname.toLowerCase().indexOf("Media/") >= 0) {

    //    $('form[name="frmSearch"]').attr('action', 'Search');
    //}
    //else {
    //    $('form[name="frmSearch"]').attr('action', 'Media/Search');
    //}
    //jQuery("#frmSearch").submit();
    $(this.form).submit();
};

Media.ready = function () {
    $('[data-toggle="tooltip"]').tooltip({ 'placement': 'right' });
    $('[data-toggle1="popover"]').popover({ trigger: 'hover', 'placement': 'top' });

    $('.dateTxt').datetimepicker({
        //These options can also be set by data-date-OPTION in HTML
        pickDate: true,                 //en/disables the date picker
        pickTime: true,                 //en/disables the time picker
        defaultDate: new Date(),
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

    $('.fancybox').fancybox();

    //$("#ShowImage").fancybox({
    //    helpers:
    //    {
    //        title:
    //        {
    //            type: 'float'
    //        }
    //    }
    //});

    //$('.imageGallery').fancybox({
    //    fitToView: false,
    //    width: '600px',
    //    height: '400px',
    //    autoSize: false,
    //    closeClick: false,
    //    openEffect: 'none',
    //    closeEffect: 'none',
    //    padding: 0,
    //    closeBtn: false,
    //    'afterClose': function () {
    //        window.location.reload();
    //    },
    //});
};


Media.init();
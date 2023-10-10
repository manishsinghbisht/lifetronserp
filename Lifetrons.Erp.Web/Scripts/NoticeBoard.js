var NoticeBoard = new Object();

NoticeBoard.init = function () {
    $(document).ready(NoticeBoard.ready);
    $("#btnSearch").click(NoticeBoard.Search);
     $('input[type="submit"]').click(NoticeBoard.Submit);
};

NoticeBoard.Submit = function () {
    var btn = jQuery(this);
    btn.prop('disabled', true);
    setTimeout(function () {
        btn.prop('disabled', false);
    }, 2000);
    //jQuery("#frm").submit();
    //var form = $(this).closest('form');
    $(this.form).submit();
};


NoticeBoard.Search = function () {
    ////////$('#frmSearch').attr('action', 'NoticeBoard/Search');
    //var pathname = window.location.pathname;
    //if (pathname.toLowerCase().indexOf("NoticeBoard/") >= 0) {

    //    $('form[name="frmSearch"]').attr('action', 'Search');
    //}
    //else {
    //    $('form[name="frmSearch"]').attr('action', 'NoticeBoard/Search');
    //}
    //jQuery("#frmSearch").submit();
    $(this.form).submit();
};

NoticeBoard.ready = function () {
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
    NoticeBoard.ClearControls();
};

//NoticeBoard.ClearControls = function () {
//    $("#Desc").val('');
//    $("#ProgressDesc").val('');
//};


NoticeBoard.init();

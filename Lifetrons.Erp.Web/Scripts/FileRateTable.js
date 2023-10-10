var FileRateTable = new Object();

FileRateTable.init = function () {
    $(document).ready(FileRateTable.ready);
    $("#btnSearch").click(FileRateTable.Search);
    $('input[type="submit"]').click(FileRateTable.Submit);
    $('.glyphicon-remove-circle').click(FileRateTable.ClearPrevInputControl);
    $('#btnUploadFile').click(FileRateTable.Upload);
};

FileRateTable.Submit = function () {
    var btn = jQuery(this);
    btn.prop('disabled', true);
    setTimeout(function () {
        btn.prop('disabled', false);
    }, 2000);
    jQuery("#frm").submit();
};

FileRateTable.Search = function () {
    ////////$('#frmSearch').attr('action', 'Invoice/Search');
    //var pathname = window.location.pathname;
    //if (pathname.toLowerCase().indexOf("StockIssue/") >= 0) {

    //    $('form[name="frmSearch"]').attr('action', 'Search');
    //}
    //else {
    //    $('form[name="frmSearch"]').attr('action', 'StockIssue/Search');
    //}
    //jQuery("#frmSearch").submit();
    $(this.form).submit();
};

FileRateTable.ready = function () {
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

    FileRateTable.ClearControls();
};

FileRateTable.ClearPrevInputControl = function () {
    $(this).prev("input").val('');
    // alert($(this).prev('input').attr('id'));
};

FileRateTable.ClearControls = function () {
    $("#Desc").val('');
};

FileRateTable.Upload = function () {
    var data = new FormData();
    var files = $("#fileUpload").get(0).files;
    // Add the uploaded image content to the form data collection
    if (files.length > 0) {
        data.append("UploadedImage", files[0]);
    }
    // Make Ajax request with the contentType = false, and procesDate = false
    var ajaxRequest = $.ajax({
        type: "POST",
        url: "UploadFile/" + $('#id').val(),
        contentType: false,
        processData: false,
        data: data
    });

    ajaxRequest.done(function (xhr, textStatus) {
        // Do other operation
        window.location.href = "Index"
    });
};

FileRateTable.init();
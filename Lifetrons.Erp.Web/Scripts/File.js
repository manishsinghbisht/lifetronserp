var File = new Object();

File.init = function (e) {
    $(document).ready(File.ready(e));
    $("#btnSearch").click(File.Search);
    $("#sendButton").click(File.SendFile);
    $('input[type="submit"]').click(File.Submit);
    $('#btnUploadFile').click(File.Upload);
 };

File.Submit = function () {
    var btn = jQuery(this);
    btn.prop('disabled', true);
    setTimeout(function () {
        btn.prop('disabled', false);
    }, 2000);
    jQuery("#frm").submit();
};

File.ready = function (e) {
    $('[data-toggle="tooltip"]').tooltip({ 'placement': 'right' });
    $('[data-toggle1="popover"]').popover({ trigger: 'hover', 'placement': 'top' });

    $('.dateTxt').datetimepicker({
       
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


    $(".partialContents").each(function (index, item) {
        
        var url = $(item).data("url");
        if (url && url.length > 0) {
            $(item).load(url);
        }
    });
    
    File.ClearControls();
};

File.ClearControls = function () {
    $("#Desc").val('');
};

File.Upload = function () {
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

File.init();
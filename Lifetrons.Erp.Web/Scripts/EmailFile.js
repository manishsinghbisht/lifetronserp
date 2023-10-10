var EmailFile = new Object();

EmailFile.init = function (e) {
    $(document).ready(EmailFile.ready(e));
    $("#btnSearch").click(EmailFile.Search);
    $("#sendButton").click(EmailFile.SendEmail);
    $('input[type="submit"]').click(EmailFile.Submit);
 };

EmailFile.Submit = function () {
    var btn = jQuery(this);
    btn.prop('disabled', true);
    setTimeout(function () {
        btn.prop('disabled', false);
    }, 2000);
    jQuery("#frm").submit();
};

EmailFile.ready = function (e) {
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


    $(".partialContents").each(function (index, item) {
        
        var url = $(item).data("url");
        if (url && url.length > 0) {
            $(item).load(url);
        }
    });
    
    EmailFile.ClearControls();
};


EmailFile.ClearControls = function () {
    $("#Desc").val('');
};


EmailFile.SendEmail = function () {

    var btn = jQuery(this);
    var form = $("#emailForm");

    form.submit(function () {

        btn.prop('disabled', true);
        setTimeout(function () {
            btn.prop('disabled', false);
        }, 4000);

        var emailData = {
            toEmail: $("#toEmail").val(),
            ccEmail: $("#ccEmail").val(),
            subject: $("#subject").val(),
            message: $("#message").val()
        };

        $.ajax({
            url: "../../Email/SendMail",
            type: "POST",
            contentType: "application/json; charset=utf-8",
            dataType: "text",
            data: JSON.stringify(emailData),
            success: function (response) {
                $("div[id='mailDiv']").addClass("hidden");
                $("div[id='footerDiv']").addClass("hidden");
                alert(response);
            },
            error: function () {
                alert("There was an error... please try again.");
            }
        });

        return false;
    });
};

EmailFile.init();
var Attendance = new Object();

Attendance.init = function () {
    $(document).ready(Attendance.ready);
    $("#btnSearch").click(Attendance.Search);
    $('input[type="submit"]').click(Attendance.Submit);
    $('.glyphicon-remove-circle').click(Attendance.ClearPrevInputControl);
};

Attendance.Submit = function () {
    var btn = jQuery(this);
    btn.prop('disabled', true);
    setTimeout(function () {
        btn.prop('disabled', false);
    }, 2000);
    jQuery("#frm").submit();
};

Attendance.Search = function () {
    $(this.form).submit();
};

Attendance.ready = function () {
    $('[data-toggle="tooltip"]').tooltip({ 'placement': 'right' });
    $('[data-toggle1="popover"]').popover({ trigger: 'hover', 'placement': 'top' });

    $('.dateTxt').datetimepicker({
        ////These options can also be set by data-date-OPTION in HTML
        //pickDate: true,                 //en/disables the date picker
        //pickTime: true,                 //en/disables the time picker
        ////defaultDate: new Date(),
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
    Attendance.InitControls();
};


Attendance.ClearPrevInputControl = function () {
    $(this).prev("input").val('');
    // alert($(this).prev('input').attr('id'));
};

Attendance.InitControls = function () {
    $("#Remark").val('');
};

Attendance.AutoExtractLastName = function () {
   
    var fullName = $("#Name").val().trim();
   
    $("#MailingAddressToName").val(fullName);
    $("#OtherAddressToName").val(fullName);

    var lastSpaceIndex = fullName.lastIndexOf(" ");
    $("#LastName").val(fullName.substring(lastSpaceIndex + 1, fullName.length));
    
    var firstSpaceIndex = fullName.indexOf(" ");
    $("#FirstName").val(fullName.substring(0, firstSpaceIndex));
    $("#MiddleName").val(fullName.substring(firstSpaceIndex + 1, lastSpaceIndex));
    
};

Attendance.init();
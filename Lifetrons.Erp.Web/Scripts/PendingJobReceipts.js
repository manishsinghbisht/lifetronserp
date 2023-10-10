﻿var PendingJobReceipts = new Object();

PendingJobReceipts.init = function () {
    $(document).ready(PendingJobReceipts.ready);
  
};



PendingJobReceipts.ready = function () {
    $('[data-toggle="tooltip"]').tooltip({ 'placement': 'right' });
    $('[data-toggle1="popover"]').popover({ trigger: 'hover', 'placement': 'top' });

    $('.dateTxt').datetimepicker({
        ////These options can also be set by data-date-OPTION in HTML
        //pickDate: true,                 //en/disables the date picker
        //pickTime: true,                 //en/disables the time picker
        //defaultDate: new Date(),
    });
    
};

PendingJobReceipts.init();

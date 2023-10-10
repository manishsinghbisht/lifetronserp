var Home = new Object();

Home.init = function () {
    $(document).ready();
    var hostname = window.location.hostname;
    var location = window.location.href;
    if (hostname.toLowerCase().indexOf("www") >= 0) {
        var appHomeUrl = $("#AppHomeURL").val();
        window.location.replace(appHomeUrl);
        //window.location.href = "http://Lifetrons.Erp.in";
    }
};

$('input[type = "radio"]').click(function () {

    $(this).parents("form").submit(); // post form

});

Home.init();


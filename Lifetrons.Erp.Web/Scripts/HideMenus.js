var HideMenus = new Object();

HideMenus.init = function () {
    $(document).ready(HideMenus.ready);
};

HideMenus.ready = function () {
    //$("p").addClass("myClass yourClass");
    $('ul[name=MainMenuList]').addClass("hidden");
};

HideMenus.init();
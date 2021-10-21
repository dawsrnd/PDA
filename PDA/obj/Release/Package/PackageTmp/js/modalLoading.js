deleteCookie();

function progress() {
    deleteCookie();
    ShowModalStatic('modalAnimation');

    var loop = setInterval(function () {
        if (IsCookieValid()) {
            $('#modalAnimation').modal('hide'); clearInterval(loop)
        }
    }, 500);
}


function IsCookieValid() {
    var cook = getCookie('generateFlag');
    return cook != '';
}

function deleteCookie() {
    var cook = getCookie('generateFlag');
    if (cook != "") {
        document.cookie = "generateFlag=; Path = /; expires=Thu, 01 Jan 1970 00:00:00 UTC";
    }
}


function getCookie(cname) {
    var name = cname + "=";
    var ca = document.cookie.split(';');
    for (var i = 0; i < ca.length; i++) {
        var c = ca[i];
        while (c.charAt(0) == ' ') {
            c = c.substring(1);
        }
        if (c.indexOf(name) == 0) {
            return c.substring(name.length, c.length);
        }
    }
    return "";
}
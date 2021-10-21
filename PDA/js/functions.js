function ShowModalStatic(id) {
    var myModal = new mdb.Modal(document.getElementById(id), {
        keyboard: false, backdrop: 'static', focus: true
    });

    myModal.show();
}



function ShowToast(type, message) {
    toastr.options.positionClass = 'toast-bottom-center';
    toastr.options.timeOut = 5000;
    switch (type) {
        case 1:
            toastr.error(message);
            break;
        case 2:
            toastr.success(message);
            break;
        case 3:
            toastr.warning(message);
            break;
        case 4:
            toastr.info(message);
            break;
    }
}


function redirectTo(opt) {
    var url = new URL(window.location.href);
    switch (opt) {
        case 1:
            var url = new URL(window.location.href);
            a.href = url.origin + "/Modules/Default.aspx";
            break;
        case 2:
            var a = document.getElementById('lnkMenu');
            a.href = url.origin + "/Modules/Menu.aspx?plant=" + url.searchParams.get("plant")
            break;
        case 3:
            var a = document.getElementById('lnkView');
            a.href = url.origin + "/Modules/Efficiency/ViewResultsEffc.aspx?plant=" + url.searchParams.get("plant")
            break;
    }
}


function redirect(opt) {
    var url = new URL(window.location.href);
    switch (opt) {
        case 1:
        case 2:
        case 3:
            window.location.href = url.origin + "/Modules/Efficiency/ViewResultsEffc.aspx?plant=" + url.searchParams.get("plant");
            break;
    }
}
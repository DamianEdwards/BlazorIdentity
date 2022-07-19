function DismissAlert(divName) {
    var mydiv = document.getElementById(divName);
    mydiv.setAttribute("hidden", "true")
    mydiv.hidden = "true";
    document.getElementById(divName).hidden = true;
}

function ShowAlert(divName) {
//    var mydiv = document.getElementById(divName).hidden = false;
//    mydiv.removeAttribute("hidden");
//    mydiv.hidden = "false";
//    mydiv.setAttribute("hidden", "false")
    document.getElementById(divName).hidden = false;
}
function setCookie(name, val, days) {
    var expires = "";
    if(days){
        var date = new Date();
        date.setTime(date.getTime() + days * 24 * 60 * 60 * 1000);
        expires = "; expires=" + date.toUTCString();
    }
    document.cookie = name + "=" + (val || "") + expires + "; path=/";
}

function getCookie(name){
    var nameEq = name + "=";
    var ca = document.cookie.split(";");
    for(var i = 0; i < ca.length; i++){
        var c = ca[i];
        while(c.charAt(0) == ' '){
            c.substring(1, c.length);
        }
        if(c.indexOf(nameEq) == 0){
            return c.substring(nameEq.length, c.length);
        }
    }
    return null;
}

function deleteCookie(name){
    document.cookie = name + "=; Path=/; Expires=Thu, 01 Jan 1970 00:00:01 GMT";
}
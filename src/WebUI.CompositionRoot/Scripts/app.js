var setFrameSrc = () =>
{
    var url = document.getElementById("url").value;
    document.getElementById("content").setAttribute("src", "/Page?url=" + url);
}

window.onload = () => {
    document.getElementById("content").addEventListener("load", () => {
        var href = document.getElementById("content").contentWindow.location.href;
        href = href.substring(href.indexOf("=") + 1);
        document.getElementById("url").value = href;
    });
    document.getElementById("go").addEventListener("click", setFrameSrc);
    document.getElementById("url").addEventListener("keypress", function (e) {
        var key = e.which || e.keyCode;
        if (key === 13) { // is Enter
            setFrameSrc();
        }
    });
};
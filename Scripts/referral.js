(function (w, d) {
    function host(url) {
        var h = /^.*:\/\/([^\/]+)\/?.*$/.exec(url);
        return h && h.length > 1 ? h[1] : null;
    }
    var r = host(d.referrer),
        c = host(d.URL);
    if (r != c) {
        var date = new Date();
        date.setTime(date.getTime() + 604800000);
        document.cookie = "referrer=" + r + "; expires=" + date.toGMTString() + "; path=/";
    }
})(window, document);
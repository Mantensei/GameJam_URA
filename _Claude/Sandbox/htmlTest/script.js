function loadHTML(url, containerId) {
    return fetch(url)
        .then(function(res) { return res.text(); })
        .then(function(html) {
            var container = document.getElementById(containerId);
            container.outerHTML = html;
        });
}

function scaleApp() {
    var app = document.getElementById('app');
    var scaleX = window.innerWidth / 480;
    var scaleY = window.innerHeight / 270;
    var scale = Math.min(scaleX, scaleY);
    app.style.transform = 'scale(' + scale + ')';
}

window.addEventListener('resize', scaleApp);
window.addEventListener('load', function() {
    Promise.all([
        loadHTML('top-bar.html', 'top-bar-container'),
        loadHTML('bottom-bar.html', 'bottom-bar-container'),
        loadHTML('menu-popup.html', 'menu-popup-container')
    ]).then(scaleApp);
});

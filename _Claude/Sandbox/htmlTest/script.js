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

var popups = [
    'menu-popup',
    'comment-popup',
    'register-popup',
];

var screens = [
    'title-screen',
    'novel-screen',
];

var allOverlays = popups.concat(screens);
var activeIndex = 0;

window.addEventListener('resize', scaleApp);
window.addEventListener('load', function() {
    var loads = [
        loadHTML('top-bar.html', 'top-bar-container'),
        loadHTML('bottom-bar.html', 'bottom-bar-container'),
    ];
    popups.forEach(function(name) {
        loads.push(loadHTML(name + '.html', name + '-container'));
    });
    screens.forEach(function(name) {
        loads.push(loadHTML(name + '.html', name + '-container'));
    });
    Promise.all(loads).then(function() {
        showOverlay(activeIndex);
        scaleApp();
    });
});

function getOverlayElement(name) {
    if (name.indexOf('-popup') !== -1) {
        return document.getElementById(name.replace('-popup', '-overlay'));
    }
    return document.getElementById(name);
}

function showOverlay(idx) {
    allOverlays.forEach(function(name) {
        var el = getOverlayElement(name);
        if (el) el.style.display = 'none';
    });
    var el = getOverlayElement(allOverlays[idx]);
    if (el) el.style.display = 'flex';
}

window.addEventListener('keydown', function(e) {
    if (e.key === 'Tab') {
        e.preventDefault();
        activeIndex = (activeIndex + 1) % allOverlays.length;
        showOverlay(activeIndex);
    }
});

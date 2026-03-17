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
    // 'next-popup',
];

var activePopup = popups[0];

window.addEventListener('resize', scaleApp);
window.addEventListener('load', function() {
    var loads = [
        loadHTML('top-bar.html', 'top-bar-container'),
        loadHTML('bottom-bar.html', 'bottom-bar-container'),
    ];
    popups.forEach(function(name) {
        loads.push(loadHTML(name + '.html', name + '-container'));
    });
    Promise.all(loads).then(function() {
        showPopup(activePopup);
        scaleApp();
    });
});

function showPopup(name) {
    popups.forEach(function(p) {
        var el = document.getElementById(p.replace('-popup', '-overlay'));
        if (el) el.style.display = (p === name) ? 'flex' : 'none';
    });
}

window.addEventListener('keydown', function(e) {
    if (e.key === 'Tab') {
        e.preventDefault();
        var idx = popups.indexOf(activePopup);
        activePopup = popups[(idx + 1) % popups.length];
        showPopup(activePopup);
    }
});

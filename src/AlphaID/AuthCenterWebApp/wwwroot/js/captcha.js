// 图形验证码（Lazy.Captcha.Core）：点击图片换一张；服务端校验失败重新渲染页面时自动换一张。
(function () {
    'use strict';

    function reloadCaptcha() {
        var img = document.getElementById('CaptchaImage');
        if (!img) return;
        var id = img.getAttribute('data-captcha-id') || 'default';
        img.src = '/captcha?id=' + encodeURIComponent(id) + '&t=' + Date.now();
    }

    window.reloadCaptcha = reloadCaptcha;

    document.addEventListener('DOMContentLoaded', function () {
        var img = document.getElementById('CaptchaImage');
        if (img) img.addEventListener('click', reloadCaptcha);
    });
})();

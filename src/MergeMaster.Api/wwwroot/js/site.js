// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
// site.js
document.querySelectorAll('form[data-disable-on-submit]').forEach(form => {
    form.addEventListener('submit', () => {
        form.querySelector('button[type=submit]').disabled = true;
        form.querySelector('[data-loading-spin]')?.classList.add('infinite-spin');
    });
});

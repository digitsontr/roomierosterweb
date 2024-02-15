// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.


window.config = {
    accessToken: document.cookie.split('AccessToken=').slice(-1)[0].split(';')[0],
    baseUrl: 'https://localhost:7032/'
}
// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

const checkEnvironment = () =>{
    if(window.location.host.includes('local') || window.location.host.includes('develop')){
        return 'Development'
    }

    return 'Production';
}


window.config = {
    accessToken: document.cookie.split('AccessToken=').slice(-1)[0].split(';')[0],
    baseUrl: checkEnvironment() === 'Production' ?'https://api.roomieroster.com/' :'https://develop-api.roomieroster.com/'
}
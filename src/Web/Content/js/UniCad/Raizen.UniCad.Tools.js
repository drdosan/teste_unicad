var namespace;
(function () {

    'use strict';
    var logarErro;
    function LogarErro(mensagem) {
        if (window.console &&
            window.console.log &&
            window.console.log instanceof Function) {
            window.console.log(mensagem);
        }
    }

    var _ns = namespace('Raizen.UniCad.Tools');

    _ns.LogarErro = logarErro;

}());
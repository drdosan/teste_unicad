
(function () {
    'use strict';
    
    var namespace;
    var Raizen;
    var Messenger;


    var _ns = namespace('Raizen.UniCad.AJAX');

    var showLoading = function (target) {
        if (!target || $(target).length <= 0) {
            $("#page").css({ opacity: 0.50 });
            $('#divLoadingFiltros').show();
        }
        else {
            var _parent = $(target).parents('.box-body').get(0);
            if (_parent) {
                $(_parent).css({ opacity: 0.50 });
                $('#divLoadingFiltros').clone().show().appendTo(_parent);
            }
        }
    }
    var hideLoading = function (target) {
        if (!target || $(target).length <= 0) {
            $("#page").css({ opacity: 100.0 });
            $('#divLoadingFiltros').hide();
        } else {
            var _parent = $($(target).parents('.box-body').get(0));
            if (_parent) {
                _parent.css({ opacity: 100.0 });
                _parent.find('#divLoadingFiltros').remove();
            }
        }

        setTimeout(function () {
            $("button[data-loading-text]").button("reset");
        }, 1000);
    }
    var configurar = function () {
        //CONFIGURA O COMPORTAMENTO QUANDO UMA REQUISIÇÃO AJAX FOR INICIADA
        $(document).ajaxStart(function (xhr) {
            //LogarErro('Iniciando requisições');
        });
        //CONFIGURA O COMPORTAMENTO QUANDO TODAS AS REQUISIÇÕES AJAX ATIVAS FOREM CONCLUÍDAS
        $(document).ajaxStop(function (xhr) {
            Raizen.UniCad.View.Helper.CarregarDataTable();
            hideLoading();
            //LogarErro('Finalizando requisições');
        });
        //CONFIGURA O COMPORTAMENTO QUANDO ALGUMA REQUISIÇÃO AJAX DISPARAR ERRO
        $(document).ajaxError(function (event, xhr, settings, exception) {
            //LogarErro({
            //    event: event,
            //    xhr: xhr,
            //    settings: settings,
            //    exception: exception
            //});
            //hideLoading();
            //Messenger.options = {
            //    extraClasses: 'messenger-fixed messenger-on-bottom messenger-on-right',
            //    theme: 'flat'
            //}
            //Call
            var _errorMessage;
            if (xhr.responseJSON)
                _errorMessage = xhr.responseJSON.Message || xhr.responseJSON.ExceptionMessage;
            else if (xhr.responseText)
                try {
                    _errorMessage = JSON.parse(xhr.responseText).ExceptionMessage;
                } catch (e) {
                    _errorMessage = exception;
                }
            else if (exception) {
                _errorMessage = "Erro: " + exception;
            }

            Messenger().post({
                message: _errorMessage + " (" + xhr.status + ")",
                type: 'error',
                showCloseButton: true
            });
        });
        //TRATAMENTO DE EXCEÇÕES DO SSO
        $.ajaxSetup({
            cache: false,
            statusCode: {
                350: function (jqXHR, textStatus, errorThrown) {
                    var response = $.parseJSON(jqXHR.responseText);
                    if (response != null && response.ssoResponse == true) {
                        window.document.location.href = response.redirectUrl;
                    }
                }
            }
        });
    }

    _ns.ShowLoading = showLoading;
    _ns.HideLoading = hideLoading;

    configurar();
}());
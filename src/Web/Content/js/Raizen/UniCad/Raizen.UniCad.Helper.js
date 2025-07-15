(function () {
    'use strict';

    //CONSTANTES

    var _required = 'data-val-required';
    var _numeric = 'data-val-number';
    var _date = 'data-val-date';
    var _attrDataId = 'data-id';
    var _attrDataNome = 'data-nome';
    var _attrDataAction = 'data-action';
    var _attrDataTarget = 'data-target';
    var bootbox;
    var Messenger;
    var RaizenHelpers;
    var namespace

    //FUNÇÕES 'PRIVADAS'
    var validarRequerido = function (e) {
        var $e = $(e);
        var _val = $(e).val();
        if ($e.attr(_required) && (!_val ||
                ($e.attr('type') === 'hidden' && _val === '0')
            )) {
            return false;
        }
        return true;
    };

    var exibirErro = function (mensagem) {
        //CONFIGURANDO MESSAGERIA
        Messenger.options = {
            extraClasses: 'messenger-fixed messenger-on-bottom messenger-on-right',
            theme: 'flat'
        }
        //EXIBIR MENSAGEM
        Messenger().post({
            message: mensagem,
            type: 'error',
            showCloseButton: true
        });
    };

    var exibirErroCampo = function (e, valAttr) {

        var $e = $(e);

        var $container = $e.parents('.form-group');
        var _controlName = $container.find('label').text();
        $container.addClass('has-error');

        if (_controlName) {
            exibirErro(_controlName + ": " + $e.attr(valAttr));
        } else {
            exibirErro($e.attr(valAttr));
        }

        $("button[data-loading-text]").button("reset");
    };

    var formatIBM = function ($campo) {
        var _valor = $campo.val();
        if (_valor == '' || _valor == 0)
            return;        
        var regex = /^[0-9\b]+$/;
        if (regex.test(_valor)) {
            while (_valor.length < 10) {
                _valor = '0' + _valor;
            }
            $campo.val(_valor);
        }
    };


    var validarData = function (e) {

        var $e = $(e);
        var _val = $(e).val();

        if ($e.attr(_date)) {
            var _dateValue;
            if (_val) {
                try {
                    _dateValue = new Date(_val);
                } catch (e) {
                    return false;
                }
            }
        }
        return true;

    };

    var validarNumerico = function (e) {
        var $e = $(e);
        var _val = $(e).val();

        if ($e.attr(_numeric)) {
            var _number;
            if (_val) {
                try {
                    _number = parseFloat(_val.replaceAll(" ", ""));
                    if (_number === 0)
                        return true;
                    if (!_number || _number === NaN)
                        return false;
                } catch (e) {
                    return false;
                }
            }
        }
        return true;
    };

    var validar = function (obj) {

        var _valido;
        var $obj = $(obj);
        //LIMPANDO TODAS AS MARCAÇÕES DE ERRO
        $obj.find('.has-error').removeClass('has-error');

        //VALIDANDO CAMPOS REQUERIDOS
        $obj.find('[type!=hidden][data-val=true]').each(function (i, e) {
            var $e = $(e);

            //VALIDAR REQUERIDO
            if (!validarRequerido(e)) {
                _valido = false;
                exibirErroCampo(e, _required);
            }
                //VALIDAR TIPO NUMERAL
            else if (!validarNumerico(e)) {
                _valido = false;
                exibirErroCampo(e, _numeric);
            }
                //VALIDAR TIPO DATA
            else if (!validarData(e)) {
                _valido = false;
                exibirErroCampo(e, _date);
            }


        });

        //RETONANDO
        //SE NÃO FOI ATRIBUIDO
        return _valido === undefined && _valido !== false;
    };







    var validaCurrency = function ($campo) {

        var _valor = $campo.val() || "";
        if (_valor == "") {
            $campo.val("0,00")
        }

    };

    var exibeNomePorIBM = function ($campo) {

        var _valor = $campo.val() || "";
        var _name = $campo.attr("name");

        $campo.parent().parent().parent().removeClass("has-warning");
        $("[data-element-ref='" + _name + "']").html("&nbsp;");
        if (_valor) {

            var _classOld = $(".input-group-addon i", $campo.parent()).data("icon-init") || $(".input-group-addon i", $campo.parent()).attr("class");
            $(".input-group-addon i", $campo.parent()).attr("class", "fa fa-refresh fa-spin");


            $.ajax({
                url: $.rootScript + 'Cliente/ObterClientePorIBM',
                data: {
                    ibm: _valor
                },
                type: 'POST',
                cache: false,
                traditional: true,
                success: function (jsonResult) {

                    if (jsonResult.erro == false) {
                        $("[data-element-ref='" + _name + "']").html(jsonResult.razaosocial);
                        $(".input-group-addon i", $campo.parent()).attr("class", _classOld);
                    } else {
                        exibirErro(jsonResult.msg);
                        $(".input-group-addon i", $campo.parent()).attr("data-icon-init", _classOld)
                        $(".input-group-addon i", $campo.parent()).attr("class", "fa fa-exclamation-triangle");
                        $campo.parent().parent().parent().addClass("has-warning");
                    }
                },
                error: function (jsonResult) {
                    exibirErro(jsonResult.msg);
                    $(".input-group-addon i", $campo.parent()).attr("data-icon-init", _classOld)
                    $(".input-group-addon i", $campo.parent()).attr("class", "fa fa-exclamation-triangle");
                    $campo.parent().parent().parent().addClass("has-warning");
                }
            });

        }



    };

    var incluirPartial = function (content, target) {

        var $content = $('<div></div>').append(content);
        //GET ALL SCRIPTS TO PUT IN THE END OF BODY
        var _scripts = $content.find('script').each(function () {
            //VERIFICA SE O SCRIPT NÃO FOI ADICIONADO AO CORPO DO DOCUMENTO
            var $this = $(this);
            var _src = $this.attr('src');
            if (!_src || $(document).find('script[src="' + _src + '"]').length <= 0) {
                $('body').append($this);
            }
        });
        //GET ALL STYLES AND LINKS TO PUT IN THE END OF HEAD
        var _styles = $content.find('style, link');
        $('head').append(_styles);
        //INSERT THE OTHER CONTENT TO THE TARGET CONTAINER
        var _content = $content.children().not('script, style, link');
        if (target)
            $(target).html(_content);
        return _content;
    };

    var configurar = function ($parent) {

        if (!$parent) $parent = $(document);

        //CONFIGURA TODAS AS TABELAS PARA DATATABLES
        //carregarDataTable($parent);

        //CONFIGURA PARA O FORMATO DE IBM TODOS OS CAMPOS COM O ATRIBUTO 'data-ibm'
        $parent.find("input[data-ibm='True']").focusout(function () { formatIBM($(this)); });

        //Sem espaço em branco
        $("input[data-nospace]", $parent).keypress(function (e) {
            if (e.which === 32) return false;
            return true;
        }).blur(function () {
            $(this).val($(this).val().replace(/ /g, ''));
        });

        $("input[data-upper]", $parent)
            .keyup(function () { $(this).val($(this).val().toUpperCase()); })
            .blur(function () { $(this).val($(this).val().toUpperCase()); });

        //Somente letras
        $("input[data-letter]", $parent)
            .keyup(function () { this.value = this.value.replace(/[^a-zA-Z]/g, ''); })
            .blur(function () { this.value = this.value.replace(/[^a-zA-Z]/g, ''); });

        //CONFIGURA PARA EXIBIR O NOME DO CLIENTE PELO IBM COM O ATRIBUTO 'data-ibm-pesquisa'
        $parent.find("input[data-ibm-pesquisa='True']").focusout(function () { exibeNomePorIBM($(this)); });
        $("[data-element-ref]", $("input[data-ibm-pesquisa='False']").parent().parent()).parent().remove();

        //CONFIGURA OS MENUS SUSPENSOS PARA O PADRÃO
        $parent.find('select[data-selectize]').selectize();

        //CONFIGURA OS CAMPOS DE VALOR MONETÁRIO
        $.each($("input[data-currency-field]"), function (index, element) {
            $(element).maskMoney({
                showSymbol: false,
                symbol: "R$",
                decimal: ",",
                thousands: ".",
                allowNegative: true,
                allowZero: true,
                precision: $(element).data('currency-precision') || 2
            });
        }
        );

        $parent.find('[data-currency-field]').focusout(function () { validaCurrency($(this)); });

        $("[data-currency-field-inline='True']").addClass("input-sm");

        //CONFIGURA INTERIO COM SEPARADOR MILHARES
        $.each($("input[data-integer-field]"), function (index, element) {
            $(element).maskMoney({ showSymbol: false, symbol: "R$", decimal: ",", thousands: ".", allowNegative: true, allowZero: true, precision: 0 });
        }
       );
        //CONFIGURA OS CAMPOS DE VALOR NUMÉRICO
        $parent.find('[data-number-field]').keyup(function () { this.value = this.value.replace(/[^0-9\.]/g, ''); });
        $parent.find('[data-number-field]').focusout(function () { this.value = this.value.replace(/[^0-9\.]/g, ''); });

        //DESABILITANDO O ENVIO DE FORMULÁRIOS
        $parent.find('form').unbind('submit');

        //ATRIBUINDO TRATAMENTO DE ENVIO
        $parent.find('form').submit(function (evento) {
            var _this = this;

            //$('input[data-mask]').unmask();
            $.each($("input[data-currency-field='True']"), function (index, element) {
                var _valor = $(element).val();
                _valor = _valor.replace(/\./g, '');
                $(element).val(_valor);
            });
            $.each($("input[data-integer-field='True']"), function (index, element) {
                var _valor = $(element).val();
                _valor = _valor.replace(/\./g, '');
                $(element).val(_valor);
            });

            if (!$(_this).attr('novalidate') && !validar(_this)) {
                evento.preventDefault();
                return;
            }
            if ($(this).attr('data-ajax') === 'true') {
                evento.preventDefault();
                $(_this).SendFormAsync(evento);
                return;
            }
            return;

        });
        //POPOVER
        //configurarPopOver($parent.find('.pop-hover, .pop-left, .pop-top, .pop-bottom, .pop'));
        //CONFIGURAR BOTÂO LIMPAR FORMULARIO
        $parent.find("[type='reset']").click(function () {

            $parent.find('select[data-selectize]').each(function (i, e) {
                $(e).get(0).selectize.clear();
            });

            $parent.find("[data-element-ref]").html("&nbsp;");
        });
        //CONFIGURA LOADING NOS BOTÕES
        $parent.find("button[data-loading-text]").click(function () {
            var a = $(this);
            a.button("loading");
        });


        RaizenHelpers.prototype.IniciarDatePickers();

    };





    var delecaoPrompt = function () {

        var $this = $(this);

        var _id = $this.attr(_attrDataId);
        var _nome = $this.attr(_attrDataNome);
        var _action = $this.attr(_attrDataAction);
        var _target = $this.attr(_attrDataTarget);
        var _callback = $this.attr('data-callback');
        if (!_callback) _callback = null;

        if (!_id) throw 'Não há como identificar o registro para exclusão.';
        if (!_action) throw 'Não há rota definida para a operação.'

        var _mensagem;
        if (!_nome) _mensagem = 'Deseja excluir o registro?';
        else _mensagem = 'Deseja excluir "' + _nome + '"?';

        if (!_target) _target = '#grid';

        var _quest = bootbox.dialog({
            message: _mensagem,
            title: 'Excluir',
            buttons: {
                cancelar: {
                    label: 'Cancelar',
                    className: 'btn-default',
                    callback: function () {
                        //$('.bootbox').modal('hide');
                        $(_quest).modal('hide');
                    }
                },
                ok: {
                    label: 'Ok',
                    className: 'btn-primary',
                    callback: function () {
                        $.ajax({
                            url: _action,
                            method: 'POST',
                            data: { id: parseInt(_id) },
                            success: function (data) {
                                $(_target).html(data);
                                //CALLBACK
                                if (_callback)
                                    var _f = JSON.parse(_callback);
                            }
                        });
                    }
                }

            }
        });

    };

    var formModal = function () {
        var $this = $(this);

        var _form = $this.attr('data-form');
        if (!_form) throw 'Não há formulário associado à ação.'

        var _callback = $this.attr('data-callback');
        if (!_callback) _callback = null;

        var _classname = $this.attr('data-classname') || "";
        var _titulo = $this.attr('data-titulo') || "Cadastrar";

        $.ajax({
            url: _form,
            method: 'GET',
            success: function (data) {
                var _conteudo = incluirPartial(data);
                var _diag = bootbox.dialog({
                    title: _titulo,
                    message: _conteudo,
                    className: _classname
                });
                //CONFIGURAÇÕES ISOLADAS AO ESCOPO DA PARTIAL VIEW
                configurar($(_diag));
                //BOTÃO FECHAR MODAL
                window.setTimeout(function () {
                    $(_diag).find('[data-modal-hide]').click(function () {
                        _diag.modal('hide');
                    });
                }, 1000);
                //CALLBACK
                if (_callback)
                    var _f = JSON.parse(_callback);
            }
        });

    };

    var carregarPartial = function (conf) {
        if (!conf) throw 'Conf cant be null';
        if (!conf.Url) throw 'Url cant be null';
        if (!conf.Target) throw 'Target cant be null';

        $.ajax({
            url: conf.Url,
            async: true,
            data: conf.Data ? conf.Data : {},
            beforeSend: function () {
                $(conf.Target).html($('#loading-indicator').html());
            },
            error: function () {
                $(conf.Target)
                    .html('<div class="alert alert-danger"><h5><span class="fa fa-exclamation-circle"></span> Erro ao carregar o painel</h5></div>');
            },
            success: function (data) {
                incluirPartial(data, conf.Target);
                if (conf.Target)
                    configurar($(conf.Target));
                if (conf.Callback)
                    conf.Callback();
            }
        });
    };





    var exibirSucesso = function (mensagem) {
        //CONFIGURANDO MESSAGERIA
        Messenger.options = {
            extraClasses: 'messenger-fixed messenger-on-bottom messenger-on-right',
            theme: 'flat'
        }
        //EXIBIR MENSAGEM
        Messenger().post({
            message: mensagem,
            type: 'success',
            showCloseButton: true
        });
    };




    var validarNumericoGeral = function (e) {
        var $e = $(e);
        var _val = $(e).val();

        var _number;
        if (_val) {
            try {
                _number = parseFloat(_val.replaceAll(" ", ""));

                if (_number === 0)
                    return true;
                if (!_number || _number === NaN)
                    return false;
            } catch (e) {
                return false;
            }
        }

        return true;
    };





    var configurarPopOver = function ($obj) {

        var _conf = {};

        if ($obj.hasClass('pop-left')) _conf.placement = 'left';
        else if ($obj.hasClass('pop-top')) _conf.placement = 'top';
        else if ($obj.hasClass('pop-bottom')) _conf.placement = 'bottom';

        if ($obj.hasClass('pop-hover')) _conf.trigger = 'hover';

        $obj.popover(_conf);

    };



    //DEFINIÇÃO DE NAMESPACE
    var _ns = namespace('Raizen.UniCad.Helper');

    //FUNÇÕES PÚBLICAS
    _ns.CarregarPartial = carregarPartial;
    _ns.IncluirPartial = incluirPartial;
    _ns.Configurar = configurar;
    _ns.ExibirErro = exibirErro;
    _ns.ExibirSucesso = exibirSucesso;
    _ns.ValidarNumericoGeral = validarNumericoGeral;
    _ns.ValidaCurrency = validaCurrency;
    //_ns.CarregarDataTable = carregarDataTable;
    _ns.ConfigurarPopOver = configurarPopOver;
    _ns.FormatIBM = formatIBM;

    //EXECUTA CONFIGURAÇÕES NA PRIMEIRA EXECUÇÃO
    $(function () {
        configurar();
        //CONFIGURATION OS BOTÕES DE EXCLUSÃO DO GRID
        //TODOS OS 'ON' DEVEM SEGUIR ABAIXO, O BLOCO DE CONFIGURAÇÃO SERVE APENAS
        //PARA CHAMADAS QUE INDEPENDEM DE EVENTOS, OU QUANDO 'ON' NÃO É UM CENÁRIO VÁLIDO
        $(document).on('click', '[data-botao-exluir]', delecaoPrompt);
        $(document).on('click', '[data-grid-form-buttom]', formModal);
        $(document).popover({
            selector: '.pop-hover',
            trigger: 'hover',
            container: 'body'
        });
    });

}());

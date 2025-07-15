/// Classe responsável em conter os programas java Scripts que interagem com os objetos Helpers do MVC
/// Qualquer customização para CRUD's especificos deve ser tratado em um JS a parte
/// JavaScript Design Patterns (Prototype Pattern) - ver Learning JavaScript Design Patterns A book by Addy Osmani Volume 1.5.2
(function () {
    this.namespace = function (namespaceString) {
        var parts = namespaceString.split('.'),
            parent = window,
            currentPart = '';
        for (var i = 0, length = parts.length; i < length; i++) {
            currentPart = parts[i];
            parent[currentPart] = parent[currentPart] || {};
            parent = parent[currentPart];
        }
        return parent;
    }
})(window);

function RaizenHelpers() {
    this.IdUltimoControle = "";
    this.IdFormulario = "";
    this.urlValidacao = "";
    this.IdModalCrud = "";
    this.ContemErros = "N";
    this.MensagemOperacao = "";
};

/// Função que ocorre no evento change do controle
RaizenHelpers.prototype.ValidarModel = function ValidarModel(obj) {

    //Remove mensagem se ainda estiver no client
    $(obj).popover('destroy');


    //Limpa os controles apontados com erro
    $('.form-group.has-error').each(function () {
        $(this).removeClass('form-group has-error');
        $(this).addClass('form-group');
    });


    //Serializa o formulário para enviar como Model
    var dados = $('#' + this.IdFormulario).serialize();

    //Realiza a chamada Ajax como GET e solicita a validação dos campos
    $.ajax({
        url: this.urlValidacao,
        data: JSON.stringify(dados),
        contentType: 'application/json',
        type: 'GET',
        success: function (response) {

            //Percorre a resposta atraz de mensagens de erros sobre os controles da Model
            $.each(response, function () {
                var idControle = '#' + this['IdControle'].replace('lbl_', '');

                //Configura o popover do BootStrap
                $(idControle).popover(
                                    {
                                        html: true,
                                        placement: 'top',
                                        content: this['MensagemValidacao']
                                    });
                //Excecuta o popover
                $(idControle).popover('show');

                if ($(idControle).is(':checkbox') == false && $(idControle).is(':radio') == false) {

                    $(idControle).parents('div').each(function () {
                        if ($(this).hasClass('form-group')) {
                            $(this).removeClass('form-group');
                            $(this).addClass('form-group  has-error');
                            return;
                        }
                    });

                }
            });
        }
    });

    raizenCoreJs.raizenHelpers.AjustarCheckRadioBoxForm('#' + this.IdFormulario);
};



///Função de ajuste para tratar campos bool (checkbox e radio's) Bug do Jquery que seraliza esse tipo com valor incorreto
RaizenHelpers.prototype.AjustarCheckRadioBoxForm = function AjustarCheckRadioBoxForm(idForm) {
    var checkboxes = $(idForm).find('input[type="checkbox"]');
    $.each(checkboxes, function () {
        $(this).popover('hide');
    });

    var radios = $(idForm).find('input[type="radio"]');
    $.each(radios, function () {
        $(this).popover('hide');
    });
};

RaizenHelpers.prototype.AtualizarFormularioSucess = function AtualizarFormularioSucess() {

    if (this.ContemErros == 'S') {
        raizenCoreJs.raizenHelpers.ExibirMensagensValidacao();
    }
    else {
        raizenCoreJs.raizenHelpers.LimparBody();
    }
};


RaizenHelpers.prototype.ExibirMensagensValidacao = function ExibirMensagensValidacao() {
    if (this.ContemErros == 'S') {
        $('#divMensagemValidacao li').each(function () {
            var controleLabel = $(this).attr('Id');

            var mensagemErro = $(controleLabel).html() + ': ' + $(this).attr('title');

            $(this).html('<span>' + $(controleLabel).html() + '</span>: ' + this.title);
            $(controleLabel).parent().addClass('form-group has-error');

            raizenCoreJs.raizenHelpers.ExibirMensagemErro(mensagemErro);
        });
    }
};

RaizenHelpers.prototype.ExibirMensagemErro = function ExibirMensagemErro(mensagem) {
    var mytheme = 'future';
    var mypos = 'messenger-on-bottom messenger-on-right';

    //Set theme
    Messenger.options = {
        extraClasses: 'messenger-fixed ' + mypos,
        theme: mytheme
    }
    Messenger().run({
        errorMessage: mensagem,
        showCloseButton: true,
        action: function (opts) { return opts.error(); }
    });
};

RaizenHelpers.prototype.ExibirMensagemResultadoOperacao = function ExibirMensagemResultadoOperacao() {
    var mytheme = 'future';
    var mypos = 'messenger-on-bottom messenger-on-right';
    //Set theme
    Messenger.options = {
        extraClasses: 'messenger-fixed ' + mypos,
        theme: mytheme
    }
    if (raizenCoreJs.raizenHelpers.ContemErros == 'N') {
        Messenger().post({
            message: raizenCoreJs.raizenHelpers.MensagemOperacao,
            showCloseButton: true
        });
    }
    else {
        Messenger().run({
            errorMessage: raizenCoreJs.raizenHelpers.MensagemOperacao,
            showCloseButton: true,
            action: function (opts) { return opts.error(); }
        });

    }
};

RaizenHelpers.prototype.FecharMensagemValidacao = function FecharMensagemValidacao() {
    $('#divMensagemValidacao').fadeOut(2600);
};

RaizenHelpers.prototype.FecharConfirm = function FecharConfirm() {
    $('#ModalConfirm').modal('hide');
};

RaizenHelpers.prototype.FecharConfirmEnviar = function FecharConfirm() {
    $('#ModalConfirmEnviar').modal('hide');
};


RaizenHelpers.prototype.FecharDelete = function FecharDelete() {
    $('#ModalDelete').modal('hide');
    $('body').removeClass('modal-open');
    $('.modal-backdrop').remove();
};

RaizenHelpers.prototype.FecharConfirmJustify = function FecharConfirm() {
    $('#ModalConfirmJustify').modal('hide');
};

RaizenHelpers.prototype.FecharConfirmAnexo = function FecharConfirmAnexo() {
    $('#ModalConfirmAnexo').modal('hide');
};

RaizenHelpers.prototype.FecharModal = function FecharModal(modal) {
    $('#' + modal).modal('hide');
    $('body').removeClass('modal-open');
    $('.modal-backdrop').remove();
};

RaizenHelpers.prototype.AbrirConfirmEnviarAprovacao = function AbrirConfirmEnviarAprovacao(func, message) {
    $('#confirmFunctionEnviarAprovacao').val(func);
    if (message != '') {
        $('#MessageConfirmEnviar').html(message);
    }
    $('#ModalConfirmEnviar').modal('show');
};


RaizenHelpers.prototype.AbrirConfirm = function AbrirConfirm(func, message, idPais) {
    $('#confirmFunction').val(func);
    if (message != '') {
        $('#MessageConfirm1').html(message);
    }

    if (idPais === 2) { // Para o Brasil essa regriga é ignorada, é usado o valor default já em Português
        $("#ModalConfirm .modal-content .modal-footer button:nth-child(1)").html('Si');
        $("#ModalConfirm .modal-content .modal-footer button:nth-child(2)").html('No');
    }

    $('#ModalConfirm').modal('show');
};



RaizenHelpers.prototype.AbrirDelete = function AbrirDelete(func, message) {
    $('#deleteFunction').val(func);
    if (message != '') {
        $('#MessageDelete1').html(message);
    }
    $('#ModalDelete').modal('show');
};

RaizenHelpers.prototype.AbrirConfirmJustify = function AbrirConfirmJustify(func) {
    $('#confirmFunctionJustify').val(func);
    $('#ModalConfirmJustify').modal('show');
};

RaizenHelpers.prototype.AbrirConfirmAnexo = function AbrirConfirmAnexo(func, message) {
    $('#confirmFunctionAnexo').val(func);
    $('#MessageConfirm').html(message);
    $('#divAnexoModalConfirm').empty();
    $('#divAnexoModalConfirm').append('<div class="col-md-12"><label class="control-label" id="lbl_Licenca_ResponsavelOperacao">Anexo</label><input id="input-701-2" style="padding:0"  type="file" name="txtAnexo" class="" data-buttonText="Selecione" data-iconName="glyphicon glyphicon-file"><input type="hidden" id="AnexoLicencaAnexo-2" /></div>')

    $('#input-701-2').on('fileclear', function () {
        $('#AnexoLicencaAnexo-2').val('');
    });

    $("#input-701-2").fileinput({
        uploadUrl: raizenCoreJs.raizenHelpers.urlAnexoArquivo,
        dropZoneEnabled: false,
        showUpload: false,
        showPreview: false,
        browseLabel: '',
        uploadLabel: '',
        removeLabel: '',
        initialCaption: "Tamanho máximo: 20MB",
        layoutTemplates: {
            progress: ''
        },
    });

    $('#input-701-2').on('fileloaded', function (event, file, previewId, index, reader) {

        var formData = new FormData();

        formData.append('file', file);
        raizenCoreJs.raizenHelpers.AbrirLoading();
        $.ajax({
            url: raizenCoreJs.raizenHelpers.urlAnexoArquivo,
            data: formData,
            cache: false,
            contentType: false,
            processData: false,
            type: 'POST',
            success: function (response) {
                raizenCoreJs.raizenHelpers.FecharLoading();

                if (response == 'O Arquivo selecionado está vazio!') {
                    $('.glyphicon.glyphicon-trash').trigger('click');
                    ShowErrorMenssage(response);
                }
                else if (response == 'Tamanho Arquivo') {
                    ShowErrorMenssage('O arquivo deve ser menor que 20 MB, por favor, tente novamente', 20);
                    $('.glyphicon.glyphicon-trash').trigger('click');
                }
                else if (response.indexOf('Formato de Arquivo Inválido') >= 0) {
                    ShowErrorMenssage(response, 20);
                    $('.glyphicon.glyphicon-trash').trigger('click');
                }
                else {
                    $('#AnexoLicencaAnexo-2').val(response);
                }
            },
            error: function (response) {
                ShowErrorMenssage('O arquivo deve ser menor que 20 MB, por favor, tente novamente', 20);
            }
        });
    });



    $('#ModalConfirmAnexo').modal('show');
};

RaizenHelpers.prototype.AbrirModal = function AbrirModal(modal, func) {
    modal = $(modal);
    modal.find('input[type=hidden]').val(func);
    $(modal).modal('show');
};

RaizenHelpers.prototype.LimparBody = function LimparBody() {
    if ($('body').is('.modal-open')) {
        $('body').removeClass('modal-open');
    }
    $('.modal-backdrop').hide();
};

RaizenHelpers.prototype.ExecutarFuncaoDelete = function ExecutarFuncaoDelete() {
    var ok = false;
    var execute = $('#deleteFunction').val();
    if (execute != "") {
        var fn = new Function(execute);
        ok = fn();
        raizenCoreJs.raizenHelpers.FecharDelete();
    }
};

RaizenHelpers.prototype.ExecutarFuncaoConfirm = function ExecutarFuncaoConfirm() {
    var ok = false;
    var execute = $('#confirmFunction').val();
    if (execute != "") {
        var fn = new Function(execute);
        ok = fn();
        raizenCoreJs.raizenHelpers.FecharConfirm();
    }
};

RaizenHelpers.prototype.ExecutarFuncaoConfirmEnviarAprovacao = function ExecutarFuncaoConfirmEnviarAprovacao() {
    var ok = false;
    var execute = $('#confirmFunctionEnviarAprovacao').val();
    if (execute != "") {
        var fn = new Function(execute);
        ok = fn();
        raizenCoreJs.raizenHelpers.FecharConfirm();
    }
};

RaizenHelpers.prototype.ExecutarFuncaoConfirmJustify = function ExecutarFuncaoConfirmJustify() {
    var ok = false;
    var execute = $('#confirmFunctionJustify').val();
    if (execute != "") {
        var fn = new Function(execute);
        ok = fn();
    }
    raizenCoreJs.raizenHelpers.FecharConfirmJustify();
};

RaizenHelpers.prototype.ExecutarFuncaoConfirmAnexo = function ExecutarFuncaoConfirmAnexo() {
    var ok = false;
    var execute = $('#confirmFunctionAnexo').val();
    if (execute != "") {
        var fn = new Function(execute);
        ok = fn();
    }
    raizenCoreJs.raizenHelpers.FecharConfirmAnexo();
};

RaizenHelpers.prototype.ExecutarFuncaoModal = function ExecutarFuncaoModal(modal) {
    modal = $(modal);
    var execute = modal.find('input[type=hidden]').val();
    if (execute != "") {
        var fn = new Function(execute);
        fn();
    }
    raizenCoreJs.raizenHelpers.FecharModal(modal);
};

//Analisa quais controles input text possuem o data-type  = date
//Cria datepicker do bootstrap v 3.0.3 nesses controles
RaizenHelpers.prototype.IniciarDatePickers = function IniciarDatePickers() {
    $("input[data-type='date']:not(#Placa_DataNascimento):not(.dataValidade)").each(function () {
        //Verifica se o controle em questão está em modo leitura
        if ($(this).attr('readOnly') && $(this).attr('readOnly') == 'readonly') {
            return;
        }

        $(this).datepicker({
            dateFormat: 'dd/mm/yy',
            dayNames: ['Domingo', 'Segunda', 'Terça', 'Quarta', 'Quinta', 'Sexta', 'Sábado'],
            dayNamesMin: ['D', 'S', 'T', 'Q', 'Q', 'S', 'S', 'D'],
            dayNamesShort: ['Dom', 'Seg', 'Ter', 'Qua', 'Qui', 'Sex', 'Sáb', 'Dom'],
            monthNames: ['Janeiro', 'Fevereiro', 'Março', 'Abril', 'Maio', 'Junho', 'Julho', 'Agosto', 'Setembro', 'Outubro', 'Novembro', 'Dezembro'],
            monthNamesShort: ['Jan', 'Fev', 'Mar', 'Abr', 'Mai', 'Jun', 'Jul', 'Ago', 'Set', 'Out', 'Nov', 'Dez'],
            nextText: 'Próximo',
            prevText: 'Anterior'
        });
        //$(this).datepicker({
        //    format: "dd/mm/yyyy",
        //    language: "pt-BR",
        //    autoclose: true
        //});

        //Verifica se o controle está configurado para ser validado no momento de alteração
        //Se estiver implementa no evento fechar do datepicker a chamada ao métodod e validação
        if ($(this).attr('Validar') == 'True') {
            $(this).datepicker()
            .on('hide', function (e) {
                raizenCoreJs.raizenHelpers.ValidarModel(this);
            });
        }

    });



};

RaizenHelpers.prototype.EnterPagina = function EnterPagina(e) {
    //if (e && e.keyCode == 13) {
    //    $('#' + raizenCoreJs.raizenHelpers.IdFormulario).submit();
    //}
};

RaizenHelpers.prototype.EnterSalvar = function EnterSalvar(e) {
    //if (e && e.keyCode == 13) {
    //    raizenCoreJs.raizenCRUD.Salvar();
    //}
};


function ShowErrorMenssage(msg, hideAfter) {

    if (hideAfter == null)
        hideAfter = 10;

    Messenger().post({
        message: msg,
        showCloseButton: true,
        hideAfter: hideAfter,
        type: 'error'
    });
}


//Exibe mensagens laterais
function ShowMessage(msg, hideAfter) {

    if (hideAfter == null)
        hideAfter = 10;

    Messenger().post({
        message: msg,
        showCloseButton: true,
        hideAfter: hideAfter
    });
}

//Exibe mensagens laterais
function ShowMessageSucess(msg, func, icone, width) {
    if (func != null && func != '') {
        $('#BtnModalFunction').attr('onclick', func);
    }
    else {
        $('#BtnModalFunction').attr('onclick', "raizenCoreJs.raizenHelpers.FecharModal('modalSucess');");
    }

    if (icone == "fa-exclamation-triangle") {
        $('#iconeModel').removeClass("fa-check");
        $('#iconeModel').addClass(icone);
        $('#iconeModel').css('color', 'rgb(255, 190, 0)');
    }
    else {
        $('#iconeModel').removeClass("fa-exclamation-triangle");
        $('#iconeModel').addClass("fa-check");
        $('#iconeModel').css('color', 'green');

    }

    $('#MessageModel').html(raizenCoreJs.raizenHelpers.MensagemOperacao);
    if (width != null && width != ''){
        $('#modalSucess').modal({ backdrop: 'static', keyboard: false });
        $('#modalSucess .modal-dialog').attr('style','width:'+ width + ' !important' );
    }
    else
        $('#modalSucess').modal({ backdrop: 'static', keyboard: false });

    $('#MessageModel').html(msg);

    if (width != null && width != '') {
        $('#modalSucess').modal({ backdrop: 'static', keyboard: false });
        $('#modalSucess .modal-dialog').attr('style','width:'+ width + ' !important' );
    }
    else
        $('#modalSucess').modal({ backdrop: 'static', keyboard: false });
}

RaizenHelpers.prototype.EnterPesquisar = function EnterPesquisar(e) {
    if (e && e.keyCode == 13) {
        raizenCoreJs.raizenCRUD.RealizarPesquisa();
    }
};

RaizenHelpers.prototype.AbrirLoading = function AbrirLoading() {
    $("#page").css({ opacity: 0.25 });
    $('#loading-indicator').show();
};

RaizenHelpers.prototype.FecharLoading = function FecharLoading() {
    $("#page").css({ opacity: 100.0 });
    $('#loading-indicator').hide();
};

RaizenCoreJs.prototype.raizenHelpers = new RaizenHelpers();

/*
    Funções para trabalhar com valores decimais.
*/

//Função para inverter uma string. EX: ABCD irá retornar DCBA.
RaizenHelpers.prototype.InverterString = function (valor) {
    return valor.split('').reverse().join('');
};

//Função para substituir caracteres de uma string pelos caracteres desejados (replace do JS subst. somente o primeiro).
RaizenHelpers.prototype.ReplaceAll = function (conteudo, substituir, substituto) {
    conteudo = conteudo.toString();

    if (!conteudo || conteudo.length == 0 || !substituir) {
        return '';
    }

    while (conteudo.indexOf(substituir) > -1) {
        conteudo = conteudo.replace(substituir, substituto)
    }

    return conteudo;
};

//Função para converter um valor string em decimal para poder somar.
RaizenHelpers.prototype.ConverterMoedaParaFloat = function (valor, prefixo) {
    //Limpar R$ ou qualquer prefixo.
    if (prefixo && prefixo != '') {
        valor = this.ReplaceAll(valor, prefixo, '');
    }

    valor = $.trim(valor);

    if (valor != '') {
        valor = this.ReplaceAll(this.ReplaceAll(valor, '.', ''), ',', '.');
        valor = parseFloat(valor);
    }
    else {
        valor = parseFloat('0');
    }

    return valor;
};

function TruncarFloat(numero, casasDecimais) {
    var numeroStr = numero.toString();
    var indiceDecimal = numeroStr.indexOf('.');
    var substrLength = indiceDecimal == -1 ? numeroStr.length : 1 + indiceDecimal + casasDecimais;
    var numeroRecortado = numeroStr.substr(0, substrLength);
    var retorno = isNaN(numeroRecortado) ? 0 : numeroRecortado;
    return parseFloat(retorno);
}

//Função para converter um valor string em moeda. Permite adicionar um separador e escolher a qtd de casas decimais.
/*
    truncar = true, usa funçao especial para realizar truncamento.
    truncar = false, usa toFixed(N) para arredondar valores.
*/
RaizenHelpers.prototype.ConverterFloatParaMoeda = function (valor, prefixo, casasDecimais, truncar) {
    if (valor == undefined || valor == null || valor === '') {
        return (prefixo ? prefixo + '' : '');
    }

    if (!casasDecimais) {
        casasDecimais = 2; //Default.
    }

    if (typeof (valor) === 'string') {
        valor = this.ConverterMoedaParaFloat(valor, '');
    }

    //Formatar para valor monetário.
    if (!truncar) {
        valor = valor.toFixed(casasDecimais);
    }
    else {
        valor = TruncarFloat(valor, casasDecimais).toString();
        /*
            Completar com zeros para que o número tenha o formado desejado.
            O parseFloat faz com que 0.040 seja transformado em 0.04, que não
            é o comportamento desejado.
        */
        if (valor.indexOf('.') == -1) {
            valor += '.';
            for (var i = 0; i < casasDecimais; i++) {
                valor += '0';
            }
        }
        else {
            if (valor.split('.')[1].length < casasDecimais) {
                var qtdCasasCompletar = casasDecimais - valor.split('.')[1].length;
                for (var i = 0; i < qtdCasasCompletar; i++) {
                    valor += '0';
                }
            }
        }
    }

    var numeroQuebrado = valor.split('.');

    //Ajustar parte inteira (colocando um '.' a cada 3 caracteres).
    var parteInteira = numeroQuebrado[0];

    if (parteInteira.length > 3) {
        var qtdBlocosParteInteira = parseInt(parteInteira.length / 3);
        parteInteira = this.InverterString(parteInteira);
        numeroQuebrado[0] = '';

        for (var i = 0; i < qtdBlocosParteInteira; i++) {
            if (i > 0) {
                numeroQuebrado[0] = numeroQuebrado[0] + '.';
            }
            numeroQuebrado[0] = numeroQuebrado[0] + parteInteira.substr((i * 3), 3);
        }

        //Se sobrar resto, ler a string até o final.
        var resto = parseInt(parteInteira.length % 3);
        if (resto > 0) {
            numeroQuebrado[0] = numeroQuebrado[0] + '.' + parteInteira.substr((qtdBlocosParteInteira * 3), resto);
        }

        numeroQuebrado[0] = this.InverterString(numeroQuebrado[0]);
    }

    //Concatenar parte inteira e decimal.
    valor = numeroQuebrado.join(',');

    //Adicionar prefixo, se necessário.
    if (prefixo && prefixo != '') {
        valor = prefixo + valor;
    }

    return valor;
};


jQuery.fn.ForceNumericOnly =
function () {

    ctrlKey = 17,
    cmdKey = 91,
    vKey = 86,
    cKey = 67;

    return this.each(function () {
        $(this).keydown(function (e) {
            if (e.keyCode == ctrlKey || e.keyCode == cmdKey) ctrlDown = true;
        }).keyup(function (e) {
            if (e.keyCode == ctrlKey || e.keyCode == cmdKey) ctrlDown = false;
        });


        $(this).keydown(function (e) {
            var key = e.charCode || e.keyCode || 0;
            // allow backspace, tab, delete, enter, arrows, numbers and keypad numbers ONLY
            // home, end, period, and numpad decimal
            return (
                ctrlDown && (e.keyCode == vKey || e.keyCode == cKey) ||
                key == 8 ||
                key == 9 ||
                key == 13 ||
                key == 46 ||
                key == 110 ||
                key == 190 ||
                (key >= 35 && key <= 40) ||
                (key >= 48 && key <= 57) ||
                (key >= 96 && key <= 105));
        });
    });
};

function UnidadeAutoComplete(nomeCampo, url, ibmNome, tipoConsulta) {
    $("#UnidadeAuto").autocomplete({
        minLength: 3,
        source: function (request, response) {
            $.ajax({
                url: url,
                type: "POST", dataType: "json",
                data: { nome: request.term, ibmNome: ibmNome, tipoConsulta: tipoConsulta },
                success: function (data) {
                    response($.map(data, function (item) {
                        return { label: item.Name, value: item.Name, codigo: item.ID };
                    }));
                }
            });
        },
        select: function (event, ui) {
            $("#" + nomeCampo).val(ui.item.codigo);
            $("#" + nomeCampo + "Nome").val(ui.item.label);
        }
    });
}

RaizenHelpers.prototype.AbrirConfirmExclusaoPlaca = function AbrirConfirmExclusaoPlaca(func1, func2, message, somenteRede) {

    $('#option1Function').val(func1);
    $('#option2Function').val(func2);

    if (message != '') {
        $('#MessageConfirmExclusaoPlaca').html(message);
    }
    if (somenteRede) {
        $('#buttonApenasCliente').hide();
    }

    $('#ModalConfirmExclusaoPlaca').modal('show');
};

RaizenHelpers.prototype.ExecutarFuncaoConfirmExclusao = function ExecutarFuncaoConfirmExclusao() {
    var ok = false;
    var execute = $('#option1Function').val();
    if (execute != "") {
        var fn = new Function(execute);
        ok = fn();
        $('#ModalConfirmExclusaoPlaca').modal('hide');
    }
}

RaizenHelpers.prototype.ExecutarFuncaoConfirmExclusaoRede = function ExecutarFuncaoConfirmExclusaoRede() {
    var ok = false;
    var execute = $('#option2Function').val();
    if (execute != "") {
        var fn = new Function(execute);
        ok = fn();
        $('#ModalConfirmExclusaoPlaca').modal('hide');
    }
}

RaizenHelpers.prototype.FecharConfirmExclusao = function FecharConfirmExclusao() {
    $('#ModalConfirmExclusaoPlaca').modal('hide');
}

//RaizenHelpers.prototype.ExcluirPlaca = function ExcluirPlaca(placaClientes) {
//    raizenCoreJs.raizenHelpers.AbrirLoading();
//    $.ajax({
//        type: "POST",
//        url: this.urlExcluirPlaca,
//        data: { placaClientes: placaClientes },
//        success: function (retorno) {
//            raizenCoreJs.raizenHelpers.FecharLoading();
//            ShowMessageSucess(retorno);
//            raizenCoreJs.raizenCRUD.RealizarPesquisa();
//        },
//        error: function (partialView) {
//            ShowMessage(partialView);
//        }
//    });
//}


function Placa() {
    this.urlAdicionarCliente = "";
    this.urlAdicionarSeta = "";
    this.urlInativarSeta = "";
    this._idPais = 1;
}

var raizenCoreJs;
var RaizenCoreJs;
var ShowMessage;
var ShowMessageSucess;


function GetMessageError(msgPortugues, msgEspanhol) {
    switch (RaizenCoreJs.prototype.Composicao._idPais) {

        case 1:
            return msgPortugues;

        case 2:
            return msgEspanhol;

        default:
            return msgPortugues;
    }
}

RaizenCoreJs.prototype.Placa = new Placa();

$(document).ready(function () {
    $("#Placa_Tara").ForceNumericOnly();
    $("#Placa_Renavam").mask("00000000000");
    $("#Placa_NumeroEixos").ForceNumericOnly();
    $("#Placa_NumeroEixosPneusDuplos").ForceNumericOnly();
    $("#Placa_NumeroEixosDistanciados").ForceNumericOnly();

    if ($('#Placa_EixosDistanciados').val() == "True") {
        $('#NumeroEixosDistanciados').fadeIn();
    }
    else {
        $('#NumeroEixosDistanciados').fadeOut();
    }

    if ($('#Placa_EixosPneusDuplos').val() == "True") {
        $('#NumeroEixosPneusDuplos').fadeIn();
    }
    else {
        $('#NumeroEixosPneusDuplos').fadeOut();
    }


    $('.nav-tabs.placas li a').on('shown.bs.tab', function () {
        if ($('.nav-tabs.placas li').last().hasClass("active")) {
            $('.btnNext').hide();
        }
        else {
            $('.btnNext').show();
        }
    });

    $('.btnNext').click(function () {
        $('.nav-tabs.placas > .active').next('li').find('a').trigger('click');
    });

    $('.btnPrevious').click(function () {
        $('.nav-tabs.placas > .active').prev('li').find('a').trigger('click');
    });

    //$('#Placa_DataNascimento').unmask();
    //$('#Placa_DataNascimento').mask('99/99/9999');

    var elemento = $('#Placa_IDTipoVeiculo').val();
    if (elemento === "2" || elemento === "4" || elemento === "6" || elemento === "9") {
        $('.tipoCarretaTruck').fadeIn();
    }
    else {
        $('.tipoCarretaTruck').fadeOut();
    }
});

Placa.prototype.mudarMascara = function mudarMascara(valor) {
    try {
        $("#Placa_PlacaBrasil_CPFCNPJ").unmask();
    } catch (e) { }

    var tamanho = valor.length;
    if (tamanho == 11) {
        $("#Placa_PlacaBrasil_CPFCNPJ").val(valor.substring(0, 3) +
            "." +
            valor.substring(3, 6) +
            "." +
            valor.substring(6, 9) +
            "-" +
            valor.substring(9, 11));
        mostrarCpf();
    } else if (tamanho == 14) {
        $("#Placa_PlacaBrasil_CPFCNPJ").val(valor.substring(0, 2) +
            "." +
            valor.substring(2, 5) +
            "." +
            valor.substring(5, 8) +
            "/" +
            valor.substring(8, 12) +
            "-" +
            valor.substring(12, 14));
        mostrarCnpj();
    }
}

Placa.prototype.mudarMascaraArgentina = function mudarMascaraArgentina(valor) {
    try {
        $("#Placa_PlacaArgentina_CUIT").unmask();
    } catch (e) { }

    var tamanho = valor.length;
 
    $("#Placa_PlacaArgentina_CUIT").val(valor.substring(0, 2) +
        "-" +
        valor.substring(2, 10) +
        "-" +
        valor.substring(10, 11));

    var tipo = valor.substring(0, 2);
}

function mostrarCpf() {
    $('#Placa_RazaoSocial').val('');
    $('#razaoSocialPlaca').hide();
    $('#dataNascimentoPlaca').fadeIn();
}

function mostrarCnpj() {
    $('#Placa_DataNascimento').val('');
    $('#dataNascimentoPlaca').hide();
    $('#razaoSocialPlaca').fadeIn();
}

Placa.prototype.adicionarSeta = function adicionarSeta() {
    var incluir = true;
    var lista = $('.setaRow.id.visivel');
    if (lista.length < 5) {
        $.ajax({
            type: "POST",
            async: false,
            url: this.urlAdicionarSeta,
            data: { colunas: $('.coluna.visivel').length, sequencial: $('#setaTable tr:not(.lacre)').length, multiseta: $('#Placa_MultiSeta').val(), linhaNegocio: $('#Placa_LinhaNegocio').val() },
            success: function (partialView) {
                $('#setaTable tbody').append(partialView);
            },
            error: function (partialView) {
                ShowMessage(partialView);
            }
        });
    }
    else {
        ShowMessage(GetMessage('Apenas 5 setas são permitidas!', '¡Solo se permiten 5 flechas!'));
    }
};

Placa.prototype.ExibirEixosPneusDuplos = function ExibirEixosPneusDuplos() {
    if ($('#Placa_EixosPneusDuplos').val() == "True") {
        $('#NumeroEixosPneusDuplos').fadeIn();
    }
    else {
        $('#NumeroEixosPneusDuplos').fadeOut();
    }
}

Placa.prototype.ExibirEixosDistanciados = function ExibirEixosDistanciados() {
    if ($('#Placa_EixosDistanciados').val() == "True") {
        $('#NumeroEixosDistanciados').fadeIn();
    }
    else {
        $('#NumeroEixosDistanciados').fadeOut();
    }
}

Placa.prototype.ExibirSetas = function ExibirSetas() {
    var elemento = $('#Placa_IDTipoVeiculo').val();
    if (elemento == "2" || elemento == "4") {
        $('.tipoCarretaTruck').fadeIn();
    }
    else {
        $('.tipoCarretaTruck').fadeOut();
    }
}

Placa.prototype.ExibirSeta = function ExibirSeta(elemento) {
    if (elemento.value == "True") {
        $('.setaRow').removeClass('invisivel').addClass('visivel');
        $('td.visivel .checkPrincipal').prop('checked', true);
    }
    else {
        $('.setaRow').removeClass('visivel').addClass('invisivel');
        $('.setaRow input').val('');
        $('.checkPrincipal').prop('checked', false);
        for (var i = $('.lacres').length; i > 1; i--) {
            $('.linha-seta-' + i).remove();
        }
    }
}

Placa.prototype.inativarCompartimento = function inativarCompartimento(input) {
    var IsInativo = $(input).siblings('#IsInativo').val();
    var lista = $(input).closest("#setaTable").find('.coluna.visivel');
    var valor = 'True';
    if (IsInativo == 'True'){
        valor = 'False';
        var titulo = $(input).closest("#setaTable").find("th.coluna.visivel.compartimento" + lista.length).html().replace(' (Inativo)','');
        $(input).closest("#setaTable").find("th.coluna.visivel.compartimento" + lista.length).html(titulo);
    }
    else
        $(input).closest("#setaTable").find("th.coluna.visivel.compartimento" + lista.length).append(' (Inativo)');

    $(input).closest("#setaTable").find('.compartimento' + lista.length +'.visivel').each(function (i, obj) {
        $(this).find('input[type=hidden]').val(valor);
    });

    $(input).siblings('#IsInativo').val(valor);
}

Placa.prototype.adicionarCompartimento = function adicionarCompartimento() {

    var lista = $('.coluna.visivel');
    if (lista.length < 10) {
        $(".compartimento" + ($('.coluna.visivel').length + 1)).removeClass('invisivel').addClass('visivel');
        $(".compartimento" + ($('.coluna.visivel').length)).find('input[type=text]').val('0');
        $("#linha-seta-1").find('.check' + ($('.coluna.visivel').length)).prop('checked', true);
    }
    else
        ShowMessage(GetMessage("Apenas 10 compartimentos são permitidos!", "¡Solo se permiten 10 compartimentos!"));
}

Placa.prototype.removerCompartimento = function removerCompartimento() {
    if ($('.coluna.visivel').length > 1) {
        $('.compartimento' + $('.coluna.visivel').length + ' input').val('');
        $('.check' + ($('.coluna.visivel').length)).prop('checked', false);
        $('.compartimento' + $('.coluna.visivel').length).removeClass('visivel').addClass('invisivel');
    }
}

Placa.prototype.removerSeta = function removerSeta(id) {
    var linhaNegocio = $('#Placa_LinhaNegocio').val()
    if (linhaNegocio == 2) {
        if ($('#setaTable tr').length > 2) {
            $('#linha-seta-' + ($('#setaTable tr').length - 1)).remove();
        }
    }
    else {
        if ($('#setaTable tr:not(.lacre)').length > 2) {
            $('.linha-seta-' + ($('#setaTable tr:not(.lacre)').length - 1)).remove();
        }
    }
};

function checkPrincipal(elemento, comp) {
    $(elemento).closest("#setaTable").find('.check' + comp).prop('checked', false);
    $(elemento).prop('checked', true);
};

Placa.prototype.Salvar = function Salvar(idPais) {
    raizenCoreJs.raizenCRUD.ExibirControlesOcultos();
    var dados = $('.frmEdicaoPlaca').serialize();
    raizenCoreJs.raizenCRUD.OcultarControlesOcultos();
    raizenCoreJs.raizenHelpers.AbrirLoading();

    if (idPais == 2) {
        var cuit = $('[name="Placa.PlacaArgentina.CUIT"]').val();
		
		if (cuit != '' && cuit != undefined) {
			cuit = cuit.replace('-', '').replace('-', '');
		}
		
        var razaoSocial = $('[name="Placa.RazaoSocial"]').val();
    }

    $.ajax({
        url: this.urlSalvar,
        data: dados,
        type: 'POST',
        success: function (response) {
            $("#containerEdicaoPlaca").html(response);

            if (raizenCoreJs.raizenMensagens.ContemErros == "N") {

				if (idPais == 2) {
					raizenCoreJs.Composicao.carregarCuitAr(cuit, razaoSocial);
				}

                var tipoComposicao = $('#Composicao_IDTipoComposicao').val();
                var placa1 = $("#Composicao_IDPlaca1").val();
                var placa2 = $("#Composicao_IDPlaca2").val();
                var placa3 = $("#Composicao_IDPlaca3").val();
                var placa4 = $("#Composicao_IDPlaca4").val();

                var flag = false;
                if (tipoComposicao == 1)
                    flag = true;
                else if (tipoComposicao == 2 && placa1 != "" && placa2 != "")
                    flag = true;
                else if (tipoComposicao == 3 && placa1 != "" && placa2 != "" && placa3 != "")
                    flag = true;
                else if (tipoComposicao == 4 && placa1 != "" && placa2 != "" && placa3 != "" && placa4 != "")
                    flag = true;
                if (!flag) {
                    ShowMessage(GetMessageError("Dados da placa salvos com sucesso, continue com a criação/edição da composição.", "Los datos de la patente se guardaron correctamente, continúe con la creación/edición de la composición."));
                }
                $('#modalcontainerEdicao').modal('hide');

                //atp
                $("input[data-currency-field]").unbind(".maskMoney");
                $("input[data-currency-field]").maskMoney({
                    showSymbol: false,
                    symbol: "R$",
                    decimal: ",",
                    thousands: ".",
                    allowNegative: true,
                    allowZero: true,
                    precision: 2
                });

                raizenCoreJs.Composicao.MensagemSalvarPlaca();
            }
            else {
                raizenCoreJs.raizenMensagens.ExibirMensagemOperacao();
            }

            if (idPais == 2) {
                raizenCoreJs.Composicao.carregarCuitAr(cuit, razaoSocial);
            }

            raizenCoreJs.Composicao.carregarQuantidadesEixos();
            
            raizenCoreJs.raizenHelpers.FecharLoading();
        }
    });
};

Placa.prototype.removerCliente = function removerCliente(id) {
    $('#linha-cliente-' + id).remove();
};

Placa.prototype.adicionarCliente = function adicionarCliente() {
    var incluir = true;
    var lista = $('#clientesTable tbody .IDCliente');
    for (var i = 0; i < lista.length; i++) {
        if (lista[i].value == $('#Cliente').val()) {
            incluir = false;
        }
    }
    if (incluir) {
        if ($('#Cliente').val() != '') {
            $.ajax({
                type: "POST",
                async: false,
                url: this.urlAdicionarCliente,
                data: { idCliente: $('#Cliente').val() },
                success: function (partialView) {
                    $('#clientesTable tbody').append(partialView);
                    $("#ClienteAuto").val('');
                    VerificarSeDeveLimpar();

                },
                error: function (partialView) {
                    ShowMessage(partialView);
                }
            });
        }
    }
    else {
        ShowMessage(GetMessageError('Este Cliente já foi adicionado!', '¡Este cliente ya fue agregado!'));
    }
};

function GetMessage(msgPortugues, msgEspanhol) {
    switch (RaizenCoreJs.prototype.Composicao._idPais) {

        case 1:
            return msgPortugues;

        case 2:
            return msgEspanhol;

        default:
            return msgPortugues;
    }
}

Placa.prototype.carregarDocumentos = function carregarDocumentos(idPais, i) {
    if (idPais === 1) {
        if (($('#Placa_IDCategoriaVeiculo').val() == '') || ($('#Placa_Operacao').val() == '') || (($('#Placa_IDTipoProduto').val() == '') && ($('#Placa_IDTipoVeiculo').val() != 1 && $('#Placa_IDTipoVeiculo').val() != 3))) {
            return;
        }
    } else {
        if (($('#Placa_IDCategoriaVeiculo').val() == '') || ($('#Placa_Operacao').val() == '') || (($('#Placa_IDTipoProduto').val() == '') && ($("#Placa_IDTipoVeiculo").val() != 5 && $("#Placa_IDTipoVeiculo").val() != 7 ))) {
            return;
        }
    }

    raizenCoreJs.raizenHelpers.AbrirLoading();
    $.ajax({
        type: "POST",
        url: this.urlListarDocumentos,
        data: { Aprovar: $('#Aprovar').val(), IDTipoVeiculo: $('#Placa_IDTipoVeiculo').val(), IDCategoriaVeiculo: $('#Placa_IDCategoriaVeiculo').val(), Operacao: $('#Placa_Operacao').val(), LinhaNegocio: $('#Placa_LinhaNegocio').val(), IDTipoProduto: $('#Placa_IDTipoProduto').val(), idPais: idPais, idTipoComposicao: $("#Composicao_IDTipoComposicao").val(), numero: $('#placaAdicionar').val() },
        success: function (partialView) {
            $('#documentos').html(partialView);
            raizenCoreJs.raizenHelpers.FecharLoading();
        },
        error: function (partialView) {
            ShowMessage(partialView);
            raizenCoreJs.raizenHelpers.FecharLoading();
        }
    });
}

Placa.prototype.carregarDocumentosPorPlaca = function carregarDocumentosPorPlaca(idPais, idPlaca, i) {
    if (idPais === 1) {
        if (($('#Placa_IDCategoriaVeiculo').val() == '') || ($('#Placa_Operacao').val() == '') || (($('#Placa_IDTipoProduto').val() == '') && ($('#Placa_IDTipoVeiculo').val() != 1 && $('#Placa_IDTipoVeiculo').val() != 3))) {
            return;
        }
    } else {
        if (($('#Placa_IDCategoriaVeiculo').val() == '') || ($('#Placa_Operacao').val() == '') || (($('#Placa_IDTipoProduto').val() == '') && ($("#Placa_IDTipoVeiculo").val() != 5 && $("#Placa_IDTipoVeiculo").val() != 7))) {
            return;
        }
    }

    raizenCoreJs.raizenHelpers.AbrirLoading();
    $.ajax({
        type: "POST",
        url: this.urlListarDocumentos,
        data: { Aprovar: $('#Aprovar').val(), IDTipoVeiculo: $('#Placa_IDTipoVeiculo').val(), IDCategoriaVeiculo: $('#Placa_IDCategoriaVeiculo').val(), Operacao: $('#Placa_Operacao').val(), LinhaNegocio: $('#Placa_LinhaNegocio').val(), IDTipoProduto: $('#Placa_IDTipoProduto').val(), idPais: idPais, idTipoComposicao: $("#Composicao_IDTipoComposicao").val(), numero: $('#placaAdicionar').val(), idPlaca: idPlaca },
        success: function (partialView) {
            $('#documentos').html(partialView);
            raizenCoreJs.raizenHelpers.FecharLoading();
        },
        error: function (partialView) {
            ShowMessage(partialView);
            raizenCoreJs.raizenHelpers.FecharLoading();
        }
    });
}

function VerificarSeDeveLimpar() {
    if ($("#ClienteAuto").val() == '') {
        $("#Cliente").val('');
        $("#ClienteNome").val('');
    }
}
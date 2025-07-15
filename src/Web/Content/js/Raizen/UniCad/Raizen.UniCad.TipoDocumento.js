function TipoDocumento() {
    this.urlAdicionarComposicaoMotorista = "";
    this.urlAdicionarTipoProduto = "";
    this.urlGetComposicoes = "";
    this.urlSelecionarCategoria = "";
    this.composicoes = [];
}

var raizenCoreJs;
var RaizenCoreJs;
var ShowMessage;
var TipoComposicao;
var TipoProdutos;

var FADE_IN_TIME = 500;

var Paises = {
    BRASIL: 1,
    ARGENTINA: 2
};

var TipoCadastro = {
    VEICULO: 1,
    MOTORISTA: 2
};

var $selectizePlacas = null;
var $selectizeProdutos = null;
var $selectizeTipoComposicao = null;

var PlacasPorTipoComposicao = function (numeroPlacas) {
    var ret = [];
    for (var i = 0; i < numeroPlacas; i++) {
        var number = i + 1;

        switch (number) {
            case 1:
                ret.push({ Value: "Placa" + number, Text: "Tractor" });
                break;
            case 2:
                ret.push({ Value: "Placa" + number, Text: "Semiremolque 1" });
                break;
            case 3:
                ret.push({ Value: "Placa" + number, Text: "Semiremolque 2" });
                break;
            default:
                ret.push({ Value: "Placa" + number, Text: "Patente " + number });
                break;
        }

    }
    return ret;
};

RaizenCoreJs.prototype.TipoDocumento = new TipoDocumento();

$(document).ready(function () {
    $("#TipoDocumento_QtdDiasBloqueio").ForceNumericOnly();

    HabilitarAlertas();

    $("#TipoDocumento_qtdeAlertas").change(function () {
        HabilitarAlertas();
    });

    $selectizePlacas = $('#PlacaComposicao').selectize({
        labelField: 'Text',
        valueField: 'Value',
        searchField: 'Text',
        create: false,
        options: []
    })[0].selectize;

    $selectizeProdutos = $('#TipoProduto').selectize({
        labelField: 'Nome',
        valueField: 'ID',
        searchField: 'Nome',
        create: false,
        options: []
    })[0].selectize;

    $selectizeTipoComposicao = $('#TipoComposicao').selectize({
        labelField: 'Nome',
        valueField: 'ID',
        searchField: 'Nome',
        create: false,
        options: []
    })[0].selectize;

    $selectizeTipoComposicao = $('#TipoVeiculo').selectize({
        labelField: 'Nome',
        valueField: 'ID',
        searchField: 'Nome',
        create: false,
        options: []
    })[0].selectize;

    $("#Composicao").change(function () {
        $selectizePlacas.clearOptions();
        var composicao = TipoComposicao.find(function (x) { return x.Id == $('#Composicao').val(); });
        if (!composicao) { return; }
        var placas = PlacasPorTipoComposicao(composicao.Placas);
        $selectizePlacas.addOption(placas);
    });

    $("#TipoDocumento_IDPais").change(function () {
        TipoDocumento.prototype.exibeRows();
        TipoDocumento.prototype.preencheComboProduto();
    });
});

TipoDocumento.prototype.preencheComboProduto = function () {
    var idPais = $("#TipoDocumento_IDPais").val();
    var produtos = TipoProdutos.filter(function (x) {
        return x.Pais == idPais;
    });

    $selectizeProdutos.clearOptions();
    $selectizeProdutos.addOption(produtos);
};

TipoDocumento.prototype.exibeRows = function () {
    var valorPais = $('#TipoDocumento_IDPais').val();
    var valorTipoCadastro = $("#TipoDocumento_tipoCadastro").val();

    if (valorPais == Paises.BRASIL && valorTipoCadastro == TipoCadastro.VEICULO) {
        $('#linhaTipoVeiculo').show(FADE_IN_TIME);
        $('#linhaTipoProduto').show(FADE_IN_TIME);
        $('#linhaComposicao').hide(FADE_IN_TIME);
        $('#linhaTipoComposicaoMotorista').hide(FADE_IN_TIME);
        return;
    }

    if (valorPais == Paises.ARGENTINA && valorTipoCadastro == TipoCadastro.MOTORISTA) {
        $('#linhaTipoVeiculo').hide(FADE_IN_TIME);
        $('#linhaTipoProduto').show(FADE_IN_TIME);
        $('#linhaComposicao').hide(FADE_IN_TIME);
        $('#linhaTipoComposicaoMotorista').show(FADE_IN_TIME);
        return;
    }

    if (valorPais == Paises.ARGENTINA && valorTipoCadastro == TipoCadastro.VEICULO) {
        $('#linhaTipoVeiculo').hide(FADE_IN_TIME);
        $('#linhaTipoProduto').show(FADE_IN_TIME);
        $('#linhaComposicao').show(FADE_IN_TIME);
        $('#linhaTipoComposicaoMotorista').hide(FADE_IN_TIME);
        return;
    }

    $('#linhaTipoVeiculo').hide(FADE_IN_TIME);
    $('#linhaTipoProduto').hide(FADE_IN_TIME);
    $('#linhaComposicao').hide(FADE_IN_TIME);
    $('#linhaTipoComposicaoMotorista').hide(FADE_IN_TIME);
}

TipoDocumento.prototype.exibePossuiVencimento = function exibePossuiVencimento() {
    $('#divPossuiVencimento').show();
}

function HabilitarAlertas() {
    raizenCoreJs.raizenHelpers.AbrirLoading();
    var valor = $("#TipoDocumento_qtdeAlertas").val();
    if (valor == 1) {
        $("#TipoDocumento_Alerta1").prop('disabled', false);
        $("#TipoDocumento_Alerta2").prop('disabled', true);
        $("#TipoDocumento_Alerta2").val("0");
    }
    else if (valor == 2) {
        $("#TipoDocumento_Alerta1").prop('disabled', false);
        $("#TipoDocumento_Alerta2").prop('disabled', false);
    }
    else {
        $("#TipoDocumento_Alerta1").prop('disabled', true);
        $("#TipoDocumento_Alerta2").prop('disabled', true);
        $("#TipoDocumento_Alerta1").val("0");
        $("#TipoDocumento_Alerta2").val("0");
    }
    raizenCoreJs.raizenHelpers.FecharLoading();
}

TipoDocumento.prototype.escoderQtd = function escoderQtd(elemento) {
    if (elemento.value == '0') {
        $('#QtdDiasBloqueio').fadeIn();
    }
    else {
        $('#QtdDiasBloqueio').fadeOut();
    }
}

TipoDocumento.prototype.esconderAcaoImediata = function esconderAcaoImediata(elemento) {
    if (elemento.value == '1' || elemento.value == '2') {
        $('#divAcaoImediata').fadeIn();
    }
    else {
        $('#divAcaoImediata').fadeOut();
    }
}

TipoDocumento.prototype.adicionarTipoProduto = function adicionarTipoProduto() {
    /* Códigos Pais
        1 - Brasil
        2 - Argentina
    */
    var incluir = true;
    var lista = $('#TipoProdutoTable tbody .IDTipoProduto');
    for (var i = 0; i < lista.length; i++) {
        if (lista[i].value == $('#TipoProduto').val()) {
            incluir = false;
        }
    }

    if (incluir) {
        if ($('#TipoProduto').val() != '') {
            $.ajax({
                type: "POST",
                url: this.urlAdicionarTipoProduto,
                data: {
                    idTipoProduto: $('#TipoProduto').val(),
                    idPais: Paises.BRASIL
                },
                success: function (partialView) {
                    $('#TipoProdutoTable tbody').append(partialView);
                    var select = $("#TipoProduto").selectize();
                    var selectize = select[0].selectize;
                    selectize.setValue('');

                },
                error: function (partialView) {
                    ShowMessage(partialView);
                }
            });
        }
    }
    else {
        ShowMessage('Este produto já foi adicionado!')
    }
};

TipoDocumento.prototype.listarComposicoes = function () {
    var table = $('#lista-composicao');
    table.empty();

    table.append(this.composicoes.map(function (comp, index) {
        var temp = $('#linha-composicao').html().format(comp.nomeComposicao + ' - ' + comp.nomePlaca, comp.idPlaca, comp.idComposicao, index, comp.nomePlaca, comp.nomeComposicao);
        return $(temp);
    }));
};

TipoDocumento.prototype.adicionarComposicao = function () {
    var placa = $('#PlacaComposicao').val();
    var composicao = $('#Composicao').val();

    if (!placa || !composicao) {
        ShowMessage('Selecione Placa e Composição');
        return;
    }

    if (this.composicoes.length == 0) {
        this.carregarComposicao();
    }

    if (this.composicoes.filter(function (x) { return x.idPlaca == placa && x.idComposicao == composicao; }).length) {
        ShowMessage('Este tipo de composição já foi adicionado!');
        // qual('Composição + Placa já existente, impossível continuar!');
        return;
    }

    this.composicoes.push({ idPlaca: placa, nomePlaca: $('#PlacaComposicao').text(), idComposicao: composicao, nomeComposicao: $('#Composicao option:selected').text() });
    this.listarComposicoes();
};

TipoDocumento.prototype.removerComposicao = function (idPlaca, idComposicao) {
    this.composicoes = this.composicoes.filter(function (comp) {
        return comp.idComposicao != idComposicao || comp.idPlaca != idPlaca;
    });

    this.listarComposicoes();
};

TipoDocumento.prototype.carregarComposicao = function () {
    var table = $('#lista-composicao tr');

    if (table.length > 0) {

        var _composicoes = this.composicoes;
        table.each(function (index, tr) {
            _composicoes.push({ idPlaca: $(tr).find('input[name*="IdPlaca"]').val(), nomePlaca: $(tr).find('input[name*="NomePlaca"]').val(), idComposicao: $(tr).find('input[name*="IdComposicao"]').val(), nomeComposicao: $(tr).find('input[name*="NomeComposicao"]').val() });
        });

        this.composicoes = _composicoes;

        this.listarComposicoes();
    }
};

TipoDocumento.prototype.editarRegistro = function (appId, paisId, action) {
    var urlGet = this.urlGetComposicoes;

    console.log('urlGet : ' + urlGet);

    raizenCoreJs.raizenCRUD.EditarRegistro(appId, action);
    $('#modalcontainerEdicao').off('modal.shown').on('modal.shown', function () {
        raizenCoreJs.raizenHelpers.AbrirLoading();

        $.get(urlGet, { docId: appId, paisId: paisId })
            .done(function (response) {
                raizenCoreJs.TipoDocumento.composicoes = response.map(function (x) { return { idPlaca: x.IdPlaca, idComposicao: x.IdComposicao, nomePlaca: x.NomePlaca, nomeComposicao: x.NomeComposicao }; });
                raizenCoreJs.TipoDocumento.listarComposicoes();
                raizenCoreJs.raizenHelpers.FecharLoading();
            })
            .fail(function (error) {
                raizenCoreJs.raizenHelpers.FecharLoading();
                console.log(error);
            });
    });
};

TipoDocumento.prototype.removerTipoProduto = function removerTipoProduto(id) {
    $('#linha-produto-' + id).remove();
};

TipoDocumento.prototype.adicionarTipoVeiculo = function adicionarTipoVeiculo() {
    var incluir = true;
    var lista = $('#TipoVeiculoTable tbody .IDTipoVeiculo');
    for (var i = 0; i < lista.length; i++) {
        if (lista[i].value == $('#TipoVeiculo').val()) {
            incluir = false;
        }
    }

    if (incluir) {
        if ($('#TipoVeiculo').val() != '') {
            $.ajax({
                type: "POST",
                url: this.urlAdicionarTipoVeiculo,
                data: { idTipoVeiculo: $('#TipoVeiculo').val() },
                success: function (partialView) {
                    $('#TipoVeiculoTable tbody').append(partialView);
                    var select = $("#TipoVeiculo").selectize();
                    var selectize = select[0].selectize;
                    selectize.setValue('');
                },
                error: function (partialView) {
                    ShowMessage(partialView);
                }
            });
        }
    }
    else {
        ShowMessage('Este tipo de veículo já foi adicionado!');
    }
};

TipoDocumento.prototype.adicionarTipoComposicaoMotorista = function () {
    /* Códigos Pais
        1 - Brasil
        2 - Argentina
    */
    var lista = $('#TipoComposicaoMotoristaTable tbody .IdTipoComposicao');
    for (var i = 0; i < lista.length; i++) {
        if (lista[i].value == $("#TipoComposicao").val()) {
            ShowMessage('Este tipo de composição já foi adicionado!');
            return;
        }
    }

    if (!$('#TipoComposicao').val()) { return; }

    $.ajax({
        type: "POST",
        url: this.urlAdicionarComposicaoMotorista,
        data: {
            idTipoComposicao: $('#TipoComposicao').val(),
            idPais: Paises.BRASIL
        },
        success: function (partialView) {
            $('#TipoComposicaoMotoristaTable tbody').append(partialView);
            $("#TipoComposicao").val(null);
        },
        error: function (partialView) {
            ShowMessage(partialView);
        }
    });
};

TipoDocumento.prototype.removerTipoComposicaoMotorista = function (id) {
    $('#linha-composicao-motorista-' + id).remove();
};

TipoDocumento.prototype.removerTipoVeiculo = function removerTipoVeiculo(id) {
    $('#linha-veiculo-' + id).remove();
};

TipoDocumento.prototype.Salvar = function Salvar() {
    raizenCoreJs.raizenCRUD.ExibirControlesOcultos();
    var dados = $('#frmEdicao').serialize();
    raizenCoreJs.raizenCRUD.OcultarControlesOcultos();
    raizenCoreJs.raizenHelpers.AbrirLoading();

    $.ajax({
        url: this.urlSalvar,
        data: dados,
        type: 'POST',
        success: function (response) {
            $("#containerEdicao").html(response);
            if (raizenCoreJs.raizenMensagens.ContemErros == "N") {
                raizenCoreJs.raizenCRUD.Voltar();
            }
            raizenCoreJs.raizenHelpers.FecharLoading();
        }
    });
};
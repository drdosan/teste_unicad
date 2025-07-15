function EdicaoMotoristaArgentina() {
    var urlListarDocumentos = '';
    this.urlAdicionarTipoComposicao = "";
    this.urlAdicionarTipoProduto = "";
}

var raizenCoreJs;
var RaizenCoreJs;
var ShowMessage;
var RaizenHelpers;
var ShowMessageSucess;
var VerificarSeDeveLimpar;
var tipoProdutoList = [];
var tipoComposicaoList = [];

RaizenCoreJs.prototype.EdicaoMotoristaArgentina = new EdicaoMotoristaArgentina();

var Paises = {
    BRASIL: 1,
    ARGENTINA: 2
};

$(document).ready(function () {
    var phone = $("#Motorista_Telefone").val();
    if (phone != '' && phone != null) {
        phone = phone.replace(/\D/g, '');
        var tamanho = phone.length;
        if (tamanho < 11) {
            $("#Motorista_Telefone").mask("(99) 9999-9999");
        } else {
            $("#Motorista_Telefone").mask("(99) 99999-9999");
        }
    };
    $('#Motorista_PIS').unmask();
    $("#Motorista_PIS").mask("99999999999");
    $('#Motorista_Nascimento').unmask();
    $('#Motorista_Nascimento').mask('99/99/9999');


    $("#Motorista_Telefone").keydown(function (key) {
        if (key.key == "0" ||
            key.key == "1" ||
            key.key == "2" ||
            key.key == "3" ||
            key.key == "4" ||
            key.key == "5" ||
            key.key == "6" ||
            key.key == "7" ||
            key.key == "8" ||
            key.key == "9"
        ) {
            mascaraTelefone();
        }
    });

    //$("#Motorista_Telefone").change(function () {
    //    {
    //        mascaraTelefone();
    //    }
    //});

    $('#tabMotorista input[id=Motorista_MotoristaArgentina_DNI]').mask('99.999.999');
    $('.btnNext').click(function () {
        $('.nav-tabs > .active').next('li').find('a').trigger('click');
    });

    $('.btnPrevious').click(function () {
        $('.nav-tabs > .active').prev('li').find('a').trigger('click');
    });

    //$("#Motorista_CNH").ForceNumericOnly();

});

function mascaraTelefone() {
    try {
        $("#Motorista_Telefone").unmask();
        var phone = $("#Motorista_Telefone").val().replace(/\D/g, '');
    } catch (e) { alert(e); }

    var tamanho = phone.length;
    if (tamanho < 10) {
        $("#Motorista_Telefone").mask("(99) 9999-9999");
    } else {
        $("#Motorista_Telefone").mask("(99) 99999-9999");
    }
}
EdicaoMotoristaArgentina.prototype.carregarDocumentos = function carregarDocumentos(aprovar, recarregar) {
    raizenCoreJs.raizenHelpers.AbrirLoading();
    
    if (!recarregar) {
        raizenCoreJs.EdicaoMotoristaArgentina.popularListaDocumentos();
    }

    $.ajax({
        type: "POST",
        url: this.urlListarDocumentos,
        data: {
            IDEmpresa: $('#Motorista_IDEmpresa').val(),
            Operacao: $('#Motorista_Operacao').val(),
            Aprovar: aprovar,
            IDMotorista: $('#Motorista_ID').val() == 0 ? $('#Motorista_IDMotorista').val() : $('#Motorista_ID').val(),
            tipoProdutoList: tipoProdutoList,
            tipoComposicaoList: tipoComposicaoList
        },
        success: function (partialView) {
            $('#documentos').html(partialView);
        },
        error: function (partialView) {
            ShowMessage(partialView);
        }
    });
    raizenCoreJs.raizenHelpers.FecharLoading();
}

EdicaoMotoristaArgentina.prototype.popularListaDocumentos = function popularListaDocumentos() {

    $("#TipoProdutoMotoristaTable tr .IDTipoProduto").each(function (index, element) {
        if ($.inArray(element.getAttribute("value"), tipoProdutoList) == -1 && element.getAttribute("value") !== 0)
            tipoProdutoList.push(element.getAttribute("value"));
    });

    $('#TipoComposicaoMotoristaTable tr .IDTipoComposicao').each(function (index, element) {
        if ($.inArray(element.getAttribute("value"), tipoComposicaoList) == -1 && element.getAttribute("value") !== 0)
            tipoComposicaoList.push(element.getAttribute("value"));
    });
}


EdicaoMotoristaArgentina.prototype.carregarProdutos = function carregarProdutos() {
    raizenCoreJs.raizenHelpers.AbrirLoading();
    $.ajax({
        type: "POST",
        url: this.urlListarProdutos,
        data: {
            IdEmpresa: $('#Motorista_IDEmpresa').val(),
            operacao: $('#Motorista_Operacao').val(),
            idMotorista: $('#Motorista_ID').val(),
            produtoCarregado: $('#Filtro_IdTipoProduto').val()
        },
        success: function (partialView) {
            $('#tiposProduto').html(partialView);
        },
        error: function (partialView) {
            ShowMessage(partialView);
        }
    });
    raizenCoreJs.raizenHelpers.FecharLoading();
}

EdicaoMotoristaArgentina.prototype.adicionarTipoProduto = function adicionarTipoProduto(aprovar) {
    var incluir = true;
    var lista = $('#TipoProdutoMotoristaTable tbody .IDTipoProduto');
    for (var i = 0; i < lista.length; i++) {
        if (lista[i].value == $('#Filtro_IdTipoProduto').val()) {
            incluir = false;
        }
    }

    if (incluir) {
        if ($('#Filtro_IdTipoProduto').val() != '') {

            var dados = $('#frmEdicao').serialize();

            //Persistência Parcial de Dados - Temporário
            //var tiposProduto = [];
            //$('TipoProdutoMotoristaTable tbody tr').each(
            //    function (index) {
            //        tiposProduto.append(this.getElementsByClassName("Nome"), this.getElementsByClassName("IDTipoProduto"));
            //    }

            //);

            $.ajax({
                type: "POST",
                url: this.urlAdicionarTipoProduto,
                data: dados + '&idTipoProduto=' + $('#Filtro_IdTipoProduto').val() + '&idPais=2',

                //data: {
                //    modelMotorista: dados,
                //    idTipoProduto: $('#Filtro_IdTipoProduto').val(),
                //    idPais: Paises.ARGENTINA
                //},
                success: function (partialView) {
                    $('#TipoProdutoMotoristaTable tbody').append(partialView);
                    var select = $("#TipoProduto").selectize();
                    if ($.inArray($('#Filtro_IdTipoProduto').val(), tipoProdutoList) == -1)
                        tipoProdutoList.push($('#Filtro_IdTipoProduto').val());

                    raizenCoreJs.EdicaoMotoristaArgentina.carregarDocumentos(aprovar);
                    $("#Filtro_IdTipoProduto").val(null);

                },
                error: function (partialView) {
                    ShowMessage(partialView);
                }
            });
        }
    }
    else {
        ShowMessage('¡Este producto ya ha sido agregado!')
    }


};

EdicaoMotoristaArgentina.prototype.removerTipoProduto = function removerTipoProduto(id, aprovar) {
    $('#TipoProdutoMotoristaTable tbody .IDTipoProduto').filter(function () { return this.value == id }).remove();
    tipoProdutoList = $.grep(tipoProdutoList, function (value) {
        return value != id;
    });

    $('#linha-produto-' + id).remove();
    raizenCoreJs.EdicaoMotoristaArgentina.carregarDocumentos(aprovar);

};

EdicaoMotoristaArgentina.prototype.adicionarTipoComposicao = function adicionarTipoComposicao(aprovar) {
    var lista = $('#TipoComposicaoMotoristaTable tbody .IDTipoComposicao');
    for (var i = 0; i < lista.length; i++) {
        if (lista[i].value == $("#Filtro_IdTipoComposicao").val()) {
            ShowMessage('¡Este tipo de composición ya se ha agregado!');
            return;
        }
    }

    if (!$('#Filtro_IdTipoComposicao').val()) { return; }

    var dados = $('#frmEdicao').serialize();

    $.ajax({
        type: "POST",
        url: this.urlAdicionarTipoComposicao,
        data: dados + '&idTipoComposicao=' + $('#Filtro_IdTipoComposicao').val() + '&idPais=2',
        //data: {
        //    idTipoComposicao: $('#Filtro_IdTipoComposicao').val(),
        //    idPais: Paises.ARGENTINA
        //},
        success: function (partialView) {
            $('#TipoComposicaoMotoristaTable tbody').append(partialView);
            if ($.inArray($('#Filtro_IdTipoComposicao').val(), tipoComposicaoList) == -1)
                tipoComposicaoList.push($('#Filtro_IdTipoComposicao').val());

            raizenCoreJs.EdicaoMotoristaArgentina.carregarDocumentos(aprovar);
            $("#Filtro_IdTipoComposicao").val(null);

        },
        error: function (partialView) {
            ShowMessage(partialView);
        }
    });



};

EdicaoMotoristaArgentina.prototype.removerTipoComposicaoMotorista = function removerTipoComposicaoMotorista(id, aprovar) {
    $('#TipoComposicaoMotoristaTable tbody .IDTipoComposicao').filter(function () { return this.value == id }).remove();
    tipoComposicaoList = $.grep(tipoComposicaoList, function (value) {
        return value != id;
    });
    $('#linha-composicao-motorista-' + id).remove();
    raizenCoreJs.EdicaoMotoristaArgentina.carregarDocumentos(aprovar);

};


EdicaoMotoristaArgentina.prototype.carregarComposicoes = function carregarComposicoes() {
    raizenCoreJs.raizenHelpers.AbrirLoading();
    $.ajax({
        type: "POST",
        url: this.urlListarComposicoes,
        data: {
            IdEmpresa: $('#Motorista_IDEmpresa').val(),
            operacao: $('#Motorista_Operacao').val(),
            idMotorista: $('#Motorista_ID').val(),
            composicaoUtilizada: $('#Filtro_IdTipoComposicao').val()
        },
        success: function (partialView) {
            $('#tiposComposicao').html(partialView);
        },
        error: function (partialView) {
            ShowMessage(partialView);
        }
    });
    raizenCoreJs.raizenHelpers.FecharLoading();
}

EdicaoMotoristaArgentina.prototype.Reprovar = function Reprovar() {
    if ($('#Motorista_Justificativa').val() == '') {
        ShowMessage('¡Informe una justificación!');
        $('#Motorista_Justificativa').focus();
    }
    else {
        $("#Reprovar").val('true');
        RaizenHelpers.prototype.AbrirConfirm('return raizenCoreJs.EdicaoMotoristaArgentina.Salvar(4, false)');
    }
}



EdicaoMotoristaArgentina.prototype.fecharModalMotorista = function fecharModalMotorista() {
    $('#modalcontainerEdicao').modal('hide');
    $('#frmClientes').html('');
    $("#ModalMotorista").html(null);
}

EdicaoMotoristaArgentina.prototype.Salvar = function Salvar(status, comRessalvas) {

    if ((status == 4 || (status == 2 && comRessalvas)) && $('#Motorista_Justificativa').val() == '') {
        ShowMessage("¡Por favor informe la justificación!");
        return;
    }

    raizenCoreJs.raizenCRUD.ExibirControlesOcultos();
    $('#idTransportadora').val($('#Motorista_IDTransportadora').val());
    $('#frmClientes').html($('#frmEdicao').html());

    var dados = $('#frmEdicao').serialize();
    raizenCoreJs.raizenCRUD.OcultarControlesOcultos();
    raizenCoreJs.raizenHelpers.AbrirLoading();

    raizenCoreJs.EdicaoMotoristaArgentina.popularListaDocumentos();

    var tipoProdutoD = [];

    tipoProdutoList.forEach(function (val, i) {
        tipoProdutoD.push({ Key: val, Value: $('#Filtro_IdTipoProduto option[value="' + val + '"]:eq(0)').text() });
    });

    var tipoComposicaoD = [];

    tipoComposicaoList.forEach(function (val, i) {
        tipoComposicaoD.push({ Key: val, Value: $('#Filtro_IdTipoComposicao option[value="' + val + '"]:eq(0)').text() });
    });

    var tiposProduto = '&' + $.param({ tipoProdutoList: tipoProdutoD });
    var tiposComposicao = '&' + $.param({ tipoComposicaoList: tipoComposicaoD });

    $.ajax({
        url: this.urlSalvar,
        data: dados + '&status=' + status + '&comRessalvas=' + comRessalvas + '&idPais=2' + tiposProduto + tiposComposicao,
        type: 'POST',
        success: function (response) {

            $("#containerEdicaoMotoristaArgentina").html(response);

            if (raizenCoreJs.raizenMensagens.ContemErros == "N") {
                if (status == 1) {
                    ShowMessageSucess(
                        "El registro de Conductor se envió para análisis.<br/><br/>" +
                        "Una vez analizado, recibirá un correo electrónico confirmando si ha sido aprobado o rechazado.<br/><br/>" +
                        "El Conductor solo debe dirigirse al Depósito después de que se apruebe el registro.<br/><br/>" +
                        "En caso de duda, póngase en contacto con Raízen");
                } else {
                    ShowMessageSucess(raizenCoreJs.raizenMensagens.MensagemOperacao);
                }

                $('#modalcontainerEdicao').modal('hide');
                $('#frmClientes').html('');
                $('#Motorista_DNIEdicao').val('');
                $('#btnEdit').hide();
                $('#btnClone').hide();
                $('#btnNovo').show();
                $('#checkJaExiste').remove();
                $('#checkNaoExiste').remove();
                $('#lblNome').text('');
                $('#validado').val('0');
                $('#idTransportadora').val('');
                raizenCoreJs.raizenCRUD.RealizarPesquisa();
            }
            else {
                $('#retorno').val('0');
                raizenCoreJs.raizenMensagens.ExibirMensagemOperacao();
            }
            raizenCoreJs.raizenHelpers.FecharLoading();
        }
    });
};


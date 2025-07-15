
function Composicao() {
    this.urlAdicionarCliente = "";
    this.urlAdicionarSeta = "";
    this.urlListarDocumentos = "";
    this.urlSolicitarAprovacao = "";
    this.urlLista = "";
    this.urlVerificarAlteracoes = "";
    this.urlSalvar = "";
    this._idPais = 1;
    this._idTipoComp = 0;
}

var raizenCoreJs;
var RaizenCoreJs;
var ShowMessage;
var RaizenHelpers;
var ShowMessageSucess;
var ShowErrorMenssage;



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

function HabilitarArrendamento(primeiraVez) {
    if (primeiraVez == 0) {
        var valor = $("#Arrendamento").val().toUpperCase();
        if (valor == 'TRUE') {

            var operacao = $('#Composicao_Operacao').val();
            var linhaNegocio = $('#Composicao_IDEmpresa').val();

            if (linhaNegocio == '') {
                ShowMessage(GetMessage('Favor Selecione a linha de negócio', 'Por favor seleccione una línea de negocio'));
                $("#Arrendamento").val('');
                return;
            }

            if (operacao == '') {
                ShowMessage(GetMessage('Favor Selecione a operação', 'Por favor seleccione la operación'));
                $("#Arrendamento").val('');
                return;
            }


            $("#Composicao_CPFCNPJArrendamento").prop('disabled', false);
            $("#Composicao_RazaoSocialArrendamento").prop('disabled', false);
        }
        else {
            $("#Composicao_CPFCNPJArrendamento").val('');
            $("#Composicao_RazaoSocialArrendamento").val('');
            $("#Composicao_CPFCNPJArrendamento").prop('disabled', true);
            $("#Composicao_RazaoSocialArrendamento").prop('disabled', true);
        }
        $('#isArrendamento').val(valor);
    }
    else {
        var valorAnterior = $('#isArrendamento').val();
        if (valorAnterior == 'TRUE') {


            $("#Arrendamento").val('True');
            $("#Composicao_CPFCNPJArrendamento").prop('disabled', false);
            $("#Composicao_RazaoSocialArrendamento").prop('disabled', false);
        }
        else {
            if (valorAnterior == 'FALSE') {
                $("#Arrendamento").val('False');
            }
            else {
                $("#Arrendamento").val('');
            }
            $("#Composicao_CPFCNPJArrendamento").prop('disabled', true);
            $("#Composicao_RazaoSocialArrendamento").prop('disabled', true);
        }
        $('#isArrendamento').val(valorAnterior);
    }


}

function VerificarSeDeveLimpar() {
    if ($("#ClienteAuto").val() == '') {
        $("#Cliente").val('');
        $("#ClienteNome").val('');
    }
}

function VerificarSeDeveLimparTransp() {
    if ($("#TransportadoraAuto").val() == '') {
        $("#Transportadora").val('');
        $("#TransportadoraNome").val('');
    }
}

Composicao.prototype.LimparFiltros = function LimparFiltros() {
    $("#Filtro_Placa").val('');
    var isLinhaNegocioDisabled = $("#Filtro_IDEmpresa").prop('disabled');
    if (!isLinhaNegocioDisabled || isLinhaNegocioDisabled == undefined)
        $("#Filtro_IDEmpresa").val($("#target option:first").val());
    $("#Filtro_Operacao").val($("#target option:first").val());
    $("#Filtro_Chamado").val('');

    var $select = $("#Filtro_IDTipoComposicao").selectize();
    var control = $select[0].selectize;
    control.clear();

    var isStatusEmAprovacao = $("#statusOriginalEmAprovacao").val();
    var $selectStatus = $("#Filtro_IDStatus").selectize();
    var controlStatus = $selectStatus[0].selectize;
    if (isStatusEmAprovacao)
        controlStatus.setValue(1);
    else
        controlStatus.setValue('');

    $("#ClienteAuto").val('');
    $("#TransportadoraAuto").val('');
    $("#Filtro_DataInicio").val('');
    $("#Filtro_DataFim").val('');
}

function FormatarCnpj() {

    if ($("#Composicao_CPFCNPJ").val() != undefined) {
        var tamanho = $("#Composicao_CPFCNPJ").val().length;

        if (tamanho <= 11) {
            $("#Composicao_CPFCNPJ").mask("999.999.999-99");
            mostrarCpfComp();
        } else {
            $("#Composicao_CPFCNPJ").mask("99.999.999/9999-99");
            mostrarCnpjComp();
        }
    }

    //if ($("#Composicao_CPFCNPJArrendamento").val() != undefined) {
    //    var tamanhoArr = $("#Composicao_CPFCNPJArrendamento").val().length;

    //    if (tamanhoArr <= 11) {
    //        $("#Composicao_CPFCNPJArrendamento").mask("999.999.999-99");
    //    } else {
    //        $("#Composicao_CPFCNPJArrendamento").mask("99.999.999/9999-99");
    //    }
    //}
}

$(document).ready(function () {
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

    try {
        $(function () {
            $("#Arrendamento").change(function () {
                HabilitarArrendamento(0);
            });
        });
        HabilitarArrendamento(1);
    }
    catch (e) { }

    try {
        $("#Composicao_CPFCNPJ").unmask();
        $("#Composicao_CPFCNPJArrendamento").unmask();
    } catch (e) { }

    //FormatarCnpj();
    if ($("#Composicao_CPFCNPJ").val() != undefined) {
        var tamanho = $("#Composicao_CPFCNPJ").val().length;

        if (tamanho <= 11) {
            $("#Composicao_CPFCNPJ").mask("999.999.999-99");
            mostrarCpfComp();
        } else {
            $("#Composicao_CPFCNPJ").mask("99.999.999/9999-99");
            mostrarCnpjComp();
        }
    }

    $("#Composicao_CPFCNPJ").keydown(function (key) {
        if (key.key == "0" ||
            key.key == "1" ||
            key.key == "2" ||
            key.key == "3" ||
            key.key == "4" ||
            key.key == "5" ||
            key.key == "6" ||
            key.key == "7" ||
            key.key == "8" ||
            key.key == "9") {
            try {
                $("#Composicao_CPFCNPJ").unmask();
            } catch (e) { }

            var tamanho = $("#Composicao_CPFCNPJ").val().length;

            if (tamanho < 11) {
                $("#Composicao_CPFCNPJ").mask("999.999.999-99");
                mostrarCpfComp();
            } else {
                $("#Composicao_CPFCNPJ").mask("99.999.999/9999-99");
                mostrarCnpjComp();
            }
        }
    });



    //if ($("#Composicao_CPFCNPJArrendamento").val() != undefined && $("#Composicao_CPFCNPJArrendamento").val() != "") {
    //    mudarMascara(valor);
    //}

    $('.btnNext').click(function () {
        $('.nav-tabs > .active').next('li').find('a').trigger('click');
    });

    $('.btnPrevious').click(function () {
        $('.nav-tabs > .active').prev('li').find('a').trigger('click');
    });
    raizenCoreJs.Composicao.empresaChange();
});

function mostrarCpfComp() {
    $('#Composicao_RazaoSocial').val('');
    $('#razaoSocial').hide();
    $('#dataNascimento').fadeIn();
}

function mostrarCnpjComp() {
    $('#Composicao_DataNascimento').val('');
    $('#dataNascimento').hide();
    $('#razaoSocial').fadeIn();
}

RaizenCoreJs.prototype.Composicao = new Composicao();

Composicao.prototype.Reprovar = function Reprovar() {

    if (RaizenCoreJs.prototype.Composicao._idPais === 2) {
        $("#ModalConfirm .modal-dialog .modal-content .modal-footer button:nth-child(1)").html("Si");
        $("#ModalConfirm .modal-dialog .modal-content .modal-footer button:nth-child(2)").html("No");
    }

    if ($('#Composicao_Justificativa').val() == '') {
        ShowMessage(GetMessage('Informe uma justificativa!', 'Informa una justificación'));
        //$('.nav-tabs > .active').next('li').find('a').trigger('click');
        //$('#Composicao_Justificativa').focus();
    }
    else {
        $("#Reprovar").val('true');
        $('#MessageConfirm1').html(GetMessage('Deseja realmente reprovar essa composição?', '¿Queres realmente reprobar esta composición?'));

        RaizenHelpers.prototype.AbrirConfirm('return raizenCoreJs.Composicao.Salvar(4, false)');
    }
}

Composicao.prototype.FocarSAP = function FocarSAP(str) {
    $('#MessageConfirm1').html(str);
    debugger;
    RaizenHelpers.prototype.AbrirConfirm('return raizenCoreJs.Composicao.Salvar(2, false, true)');
}

Composicao.prototype.SalvarComposicao = function SalvarComposicao(status, comRessalvas, forcar) {
    if (comRessalvas && $('#Composicao_Justificativa').val() === '') {
        ShowMessage(GetMessage('Informe uma justificativa!', '¡Informe una justificación!'), '');
        return;
    }

    if (RaizenCoreJs.prototype.Composicao._idPais === 1) { /* Brasil */
        if ($('#Arrendamento').val() !== 'True' && $('#Arrendamento').val() !== 'False') {
            ShowMessage(GetMessage('Informe se possui Arrendamento!', '¡Dime si tienes un alquiler!'));
            return;
        } else if ($('#Arrendamento').val() === 'True' && ($('#Composicao_CPFCNPJArrendamento').val() === '' || $('#Composicao_RazaoSocialArrendamento').val() === '')) {
            ShowMessage(GetMessage('Informe os dados de Arrendamento!', 'Ingrese los datos de alquiler!'));
            return;
        }
    }

    $("#composicaoSalvar").find("#btnCrudSalvar").prop('disabled', true);

    raizenCoreJs.raizenCRUD.ExibirControlesOcultos();
    var dados = $('#frmEdicao').serialize();
    raizenCoreJs.raizenCRUD.OcultarControlesOcultos();

    var idStatus = $('#Composicao_IDStatus').val();
    var lista = RaizenCoreJs.prototype.Composicao.urlLista;
    //se for aprovação validar se houve alteração
    if (idStatus == 2) {
        $.ajax({
            url: this.urlVerificarAlteracoes,
            type: 'POST',
            data: dados,
            success: function (response) {
                if (response.Data.isOutrosDadosAlterados == true)
                    RaizenHelpers.prototype.AbrirConfirmEnviarAprovacao('return Composicao.prototype.Salvar(' + status + ',' + comRessalvas + ',' + forcar + ', true)');
                else if (response.Data.isClientesAlterados == true)
                    return Composicao.prototype.Salvar(status, comRessalvas, forcar, false);
                else
                    window.location.href = lista;
            }
        });
    }
    else
        Composicao.prototype.Salvar(status, comRessalvas, forcar, false);
    $("#composicaoSalvar").find("#btnCrudSalvar").prop('disabled', false);
}

Composicao.prototype.Salvar = function Salvar(status, comRessalvas, forcar, comAlteracoes) {  
    $("#composicaoSalvar").find("#btnCrudSalvar").prop('disabled', true);

    raizenCoreJs.raizenCRUD.ExibirControlesOcultos();
    var dados = $('#frmEdicao').serialize();
    raizenCoreJs.raizenCRUD.OcultarControlesOcultos();
    $("#ModalPlaca").html(null);
    raizenCoreJs.raizenHelpers.AbrirLoading();

    var urlSalvarComposicao = RaizenCoreJs.prototype.Composicao.urlSalvar;

    var lista = RaizenCoreJs.prototype.Composicao.urlLista;

    if (forcar === null) {
        forcar = false;
    } else {
        forcar = true;
    }

    if (comAlteracoes === undefined) {
        comAlteracoes = false;
    }

    $.ajax({
        url: urlSalvarComposicao,
        data: dados + '&status=' + status + '&comRessalvas=' + comRessalvas + '&forcar=' + forcar + '&comAlteracoes=' + comAlteracoes,
        type: 'POST',
        success: function (response) {
            $("#containerEdicao").html(response);

            $('#hdfpostou').val(true);

            if (raizenCoreJs.raizenMensagens.ContemErros == "N") {
                if (raizenCoreJs.raizenMensagens.MensagemOperacao.includes("Aprovação automática")) {
                    var message = GetMessage(
                        'Réplica realizada com sucesso!<br/><br/> O veículo está disponível no Csonline para ingresso dos pedidos.<br/><br/>  Em caso de dúvidas, entrar em contato com a nossa Central de Atendimento através do telefone 0300 789 8282 / 0800 789 8282.',
                        '¡La replicación se completó exitosamente!<br/><br/> El vehículo está disponible en Csonline para entrada de pedidos. <br/><br/> En caso de duda, póngase en contacto con Raízen.'
                    );
                    ShowMessageSucess(message, "window.location.href = '" + lista + "'");
                } else if (status === 1) { // Aprovado = 2, Bloqueado = 5, Em aprovacao = 1, Reprovado = 4
                    var message = GetMessage(
                        'O cadastro do veículo foi encaminhado para análise e será aprovado ou rejeitado em até 24 horas.<br/><br/> O veículo somente deverá se direcionar para a base, após o cadastro aprovado no sistema e placa disponível no Csonline para ingresso dos pedidos.<br/><br/>  Em caso de dúvidas, entrar em contato com a nossa Central de Atendimento através do telefone 0300 789 8282 / 0800 789 8282.',
                        'El registro de vehículo se envió para análisis. <br/><br/> Una vez analizado, recibirá un correo electrónico confirmando si ha sido aprobado o rechazado. <br/><br/> El Vehículo solo debe dirigirse al Depósito después de que se apruebe el registro. <br/><br/> En caso de duda, póngase en contacto con Raízen.'
                    );
                    ShowMessageSucess(message, "window.location.href = '" + lista + "'");
                } else {
                    ShowMessageSucess(raizenCoreJs.raizenMensagens.MensagemOperacao,
                        "window.location.href = '" + lista + "'");
                }
            }
            else {
                if (raizenCoreJs.raizenMensagens.MensagemOperacao.indexOf('Deseja continuar') >= 0) {
                    raizenCoreJs.Composicao.FocarSAP(raizenCoreJs.raizenMensagens.MensagemOperacao);
                }
                else {
                    raizenCoreJs.raizenHelpers.FecharModal('confirmFunctionEnviarAprovacao');
                    raizenCoreJs.raizenMensagens.ExibirMensagemOperacao();
                }
                $("#composicaoSalvar").find("#btnCrudSalvar").prop('disabled', false);
            }
            raizenCoreJs.raizenHelpers.FecharLoading();
        }
    });
};

Composicao.prototype.novaPlaca = function novaPlaca(i, idPais) {
    var tipoVeiculo;
    $('#placaAdicionar').val(i);

    var linhaNegocio = $('#Composicao_IDEmpresa').val();
    if (linhaNegocio == "") {
        ShowErrorMenssage(GetMessage('Favor selecione a linha de negócio', 'Por favor selecciona la línea de negocio'));
        return;
    }

    if ($('#Composicao_Operacao').val() == "") {
        ShowMessage(GetMessage('Favor selecione a Operação', 'Por favor seleciona la Operación'));
        return;
    }

    console.log("teste - " + i);

    if (idPais === 1) { //Brasil
        if ($('#Composicao_IDTipoComposicao').val() == 1) {
            tipoVeiculo = 4;
        } else {
            if (i == 4 || (i == 3 && $('#Composicao_IDTipoComposicao').val() == 3)) {
                tipoVeiculo = 2;
            }
            else {
                tipoVeiculo = i;
            }
        }
    } else
    {
        var idTipoComp = RaizenCoreJs.prototype.Composicao._idTipoComp;
        console.log("teste idTipoComp - " + idTipoComp );

        if (idTipoComp > 0) {
            if (i === 1 && idTipoComp != 10) {
                tipoVeiculo = 5;
            }
            else if (idTipoComp == 10) {
                console.log("teste1111");
                tipoVeiculo = 9;//truck argentina
            }
            else {
                tipoVeiculo = 6;
            }
        }
    }

    raizenCoreJs.raizenHelpers.AbrirLoading();

    $.ajax({
        url: this.urlNovaPlaca,
        type: 'GET',
        data: { tipoVeiculo: tipoVeiculo, idTipoParteVeiculo: i, operacaoFrete: $('#Composicao_Operacao').val(), idTipoComposicao: $('#Composicao_IDTipoComposicao').val(), linhaNegocio: linhaNegocio, idPais: idPais },
        success: function (response) {
            $("#ModalPlaca").html(null);
            $("#ModalPlaca").html(response);
            $("#modalcontainerEdicao").modal('show');
            $("#containerEdicaoPlaca").show();
            $('#Placa_PlacaVeiculo').val($('#Composicao_Placa' + i).val());

            raizenCoreJs.raizenHelpers.FecharLoading();
        }
    });
}

Composicao.prototype.EditarPlaca = function EditarPlaca(i, aprovar) {
    $('#placaAdicionar').val(i);
    raizenCoreJs.raizenHelpers.AbrirLoading();
    var idComp = $('#Composicao_ID').val();
    if (idComp == 0 || idComp == '')
        idComp = $('#Composicao_IDComposicao').val();

    var linhaNegocio = $('#Composicao_IDEmpresa').val();
    if (linhaNegocio == "") {
        ShowErrorMenssage(GetMessage('Favor Selecione a linha de negócio', 'Por favor seleccione una línea de negocio'));
        raizenCoreJs.raizenHelpers.FecharLoading();
        return;
    }
    $.ajax({
        url: this.urlEditarPlaca,
        data: { Id: $('#Composicao_IDPlaca' + i).val(), IdplacaOficial: $('#Composicao_PlacaOficial' + i).val(), Aprovar: aprovar, operacaoComposicao: $('#Composicao_Operacao').val(), idComposicao: idComp, linhaNegocio: linhaNegocio, numero: i, idTipoComposicao: $('#Composicao_IDTipoComposicao').val() },
        type: 'GET',
        success: function (response) {
            if (response.mensagem != '' && response.mensagem != null) {
                ShowErrorMenssage(response.mensagem)
            }
            else {
                $("#ModalPlaca").html(null);
                $("#ModalPlaca").html(response);
                $("#modalcontainerEdicao").modal('show');
                $("#containerEdicaoPlaca").show();
            }
            raizenCoreJs.raizenHelpers.FecharLoading();
        }
    });
}

Composicao.prototype.empresaChange = function empresaChange() {
    if ($("#Composicao_IDEmpresa").val() == 3 || $("#UsuarioPerfil").val() == "transportadora") {
        $('#Composicao_Operacao').val('CIF');
        $('#Composicao_Operacao').attr('disabled', 'disabled');
    }
    else if ($('#Composicao_IDEmpresa').attr('disabled') != 'disabled') {
        $('#Composicao_Operacao').removeAttr('disabled');
    }
}

function mascaraData(campoData) {
    var data = campoData.value;
    if (data.length == 2) {
        data = data + '/';
        campoData.value = data;
        return true;
    }
    if (data.length == 5) {
        data = data + '/';
        campoData.value = data;
        return true;
    }
}

Composicao.prototype.LimparFiltros = function LimparFiltros() {
    $("#Filtro_Placa").val('');
    var isLinhaNegocioDisabled = $("#Filtro_IDEmpresa").prop('disabled');
    if (!isLinhaNegocioDisabled || isLinhaNegocioDisabled == undefined)
        $("#Filtro_IDEmpresa").val($("#target option:first").val());
    $("#Filtro_Operacao").val($("#target option:first").val());
    $("#Filtro_Chamado").val('');



    var $select = $("#Filtro_IDTipoComposicao").selectize();
    var control = $select[0].selectize;
    control.clear();

    var isStatusEmAprovacao = $("#statusOriginalEmAprovacao").val();
    var $selectStatus = $("#Filtro_IDStatus").selectize();
    var controlStatus = $selectStatus[0].selectize;
    if (isStatusEmAprovacao)
        controlStatus.setValue(1);
    else
        controlStatus.setValue('');

    $("#ClienteAuto").val('');
    $("#TransportadoraAuto").val('');
    $("#Filtro_DataInicio").val('');
    $("#Filtro_DataFim").val('');
}

Composicao.prototype.AbrirEixo = function AbrirEixo(placa, i) {
    raizenCoreJs.raizenHelpers.AbrirLoading();
    var tipoComposicao = $("#Composicao_IDTipoComposicao").val();
    $('#modalEixo').hide();
    $.ajax({
        url: this.urlObterEixos,
        data: { tipoComposicao: $('#Composicao_IDTipoComposicao').val() },
        type: 'POST',
        success: function (response) {
            $('[id="Composicao_IDTipoComposicaoEixo"]').val('');
            $('#modalEixo').html(response);
            $('#modalEixo').modal('show');
            $('#btnAddEixo').removeAttr('disabled');

            raizenCoreJs.raizenHelpers.FecharLoading();
        },
        error: function (e) {
            ShowMessage(e);
            raizenCoreJs.raizenHelpers.FecharLoading();
        }
    });
}

Composicao.prototype.SelecionarEixo = function SelecionarEixo(id) {
    $('[id="Composicao_IDTipoComposicaoEixo"]').val(id);
    $('#modalEixo').modal('hide');
}

Composicao.prototype.placaChange = function placaChange(placa, i, change, msg) {
    var placaLenghtVerification = false;

    if (RaizenCoreJs.prototype.Composicao._idPais === 1)
        placaLenghtVerification = $(placa).val().length == 7;
    else if (RaizenCoreJs.prototype.Composicao._idPais === 2)
        placaLenghtVerification = $(placa).val().length >= 6 && $(placa).val().length <= 7;

    var idEmpresa = $('#Composicao_IDEmpresa').val();

    if (idEmpresa == undefined || idEmpresa == '') {
        ShowErrorMenssage(GetMessage('Favor Selecione a linha de negócio', 'Por favor seleccione una línea de negocio'));
        $(placa).val('');
        return false;
    }

    if (change || event.keyCode == 8 || (event.keyCode >= 48 && event.keyCode <= 57) || (event.keyCode >= 65 && event.keyCode <= 90) || (event.keyCode >= 97 && event.keyCode <= 122)) {
        if ($(placa).val() != undefined && placaLenghtVerification) {
            raizenCoreJs.raizenHelpers.AbrirLoading();
            $.ajax({
                url: this.urlVerificarPlaca,
                data: { placa: $(placa).val(), numero: i, tipoComposicao: $('#Composicao_IDTipoComposicao').val(), composicao: $('[name=ChavePrimaria]').val(), operacao: $('#Composicao_Operacao').val(), IDEmpresa: $('#Composicao_IDEmpresa').val() },
                type: 'POST',
                success: function (response) {

                    if (response.Placa.ID == 0) {
                        $('#Composicao_IDPlaca' + i).val('');
                        $('#btnAddPlaca' + i).show();
                        $('#btnEditPlaca' + i).hide();

                        if (response.Mensagem != '' && response.Mensagem != null) {
                            ShowMessage(response.Mensagem);
                        }
                        else {
                            ShowMessage(GetMessage('Placa não cadastrada no Sistema. Selecione + para incluir.', 'Patente no registrada en el sistema. Seleccione + para incluirla'));
                        }
                        raizenCoreJs.raizenHelpers.FecharLoading();
                    }
                    else {
                        if (response.Placa.Status == 'Bloqueada') {
                            ShowMessage(GetMessage('Esta placa está bloqueada.', 'Esta patente está bloqueada.'));
                        }
                        else if (msg != "false" && response.Mensagem != '' && response.Mensagem !== null && response.MensagemId !== 1 /*EnumMensagemPlaca*/) {
                            //$(placa).val('');
                            ShowMessage(response.Mensagem);
                        }
                        else {
                            if (msg != "false" && $('#Composicao_IDPlaca' + i).val() != response.Placa.ID && response.MensagemId === 1 /*EnumMensagemPlaca*/) {
                                ShowMessage(response.Mensagem);
                            }
                            $('#btnAddPlaca' + i).hide();
                            $('#btnEditPlaca' + i).show();
                            $('#Composicao_IDPlaca' + i).val(response.Placa.ID);

                            raizenCoreJs.Composicao.carregarQuantidadesEixos();
                        }
                    }
                    raizenCoreJs.raizenHelpers.FecharLoading();
                },
                error: function (e) {
                    ShowMessage(e);
                    raizenCoreJs.raizenHelpers.FecharLoading();
                }
            });
        }
        else {
            $('#Composicao_IDPlaca' + i).val('');
            $('#btnAddPlaca' + i).show();
            $('#btnEditPlaca' + i).hide();
            raizenCoreJs.Composicao.carregarQuantidadesEixos();
        }
    }
}

Composicao.prototype.VisualizarDocumentos = function VisualizarDocumentos(id) {
    raizenCoreJs.raizenHelpers.AbrirLoading();
    $.ajax({
        type: "GET",
        url: this.urlVisualizarDocumentos,
        data: { idComposicao: id },
        success: function (retorno) {
            $('#divModal').html(retorno);
            $('#containerVisualizar').modal('show');
            raizenCoreJs.raizenHelpers.FecharLoading();
        },
        error: function (partialView) {
            ShowMessage(partialView);
            raizenCoreJs.raizenHelpers.FecharLoading();
        }
    });
}

Composicao.prototype.VisualizarCapacidade = function VisualizarCapacidade(id) {
    raizenCoreJs.raizenHelpers.AbrirLoading();
    $.ajax({
        type: "GET",
        url: this.urlVisualizarCapacidade,
        data: { idComposicao: id },
        success: function (retorno) {
            $('#divModal').html(retorno);
            $('#containerVisualizar').modal('show');
            $('button[id="add"]').attr('disabled', 'disabled');
            $('button[id="Remover"]').attr('disabled', 'disabled');
            $('.visivel input[type="text"]').attr('disabled', 'disabled');
            $('.invisivel input[type="text"]').attr('disabled', 'disabled');
            $('.checkPrincipal').attr('disabled', 'disabled');
            raizenCoreJs.raizenHelpers.FecharLoading();
        },
        error: function (partialView) {
            ShowMessage(partialView);
            raizenCoreJs.raizenHelpers.FecharLoading();
        }
    });
}

Composicao.prototype.InativarCompartimento = function InativarCompartimento(id) {
    raizenCoreJs.raizenHelpers.AbrirLoading();
    $.ajax({
        type: "GET",
        url: this.urlInativarCompartimento,
        data: { idComposicao: id },
        success: function (retorno) {
            $('#divModal').html(retorno);
            $('#containerEixos').modal('show');
            $('button[id="add"]').attr('disabled', 'disabled');
            $('button[id="Remover"]').attr('disabled', 'disabled');
            $('.visivel input[type="text"]').attr('disabled', 'disabled');
            $('.invisivel input[type="text"]').attr('disabled', 'disabled');
            $('.checkPrincipal').attr('disabled', 'disabled');
            $('#IdComposicaoAlterar').val(id);
            raizenCoreJs.raizenHelpers.FecharLoading();
        },
        error: function (partialView) {
            ShowMessage(partialView);
            raizenCoreJs.raizenHelpers.FecharLoading();
        }
    });
}

Composicao.prototype.AlterarSeta = function AlterarSeta(id) {
    raizenCoreJs.raizenHelpers.AbrirLoading();
    $.ajax({
        type: "GET",
        url: this.urlAlterarSeta,
        data: { idComposicao: id },
        success: function (retorno) {
            $('#divModal').html(retorno);
            $('#containerEixos').modal('show');
            $('button[id="add"]').attr('disabled', 'disabled');
            $('button[id="Remover"]').attr('disabled', 'disabled');
            $('.visivel input[type="text"]').attr('disabled', 'disabled');
            $('.invisivel input[type="text"]').attr('disabled', 'disabled');
            $('#IdComposicaoAlterar').val(id);
            raizenCoreJs.raizenHelpers.FecharLoading();
        },
        error: function (partialView) {
            ShowMessage(partialView);
            raizenCoreJs.raizenHelpers.FecharLoading();
        }
    });
}

Composicao.prototype.SalvarAlterarSeta = function SalvarAlterarSeta(id) {
    raizenCoreJs.raizenHelpers.AbrirLoading();

    var dados = $('#frmSeta').serialize();

    $.ajax({
        type: "POST",
        url: this.urlAlterarSetaSalvar,
        data: dados + '&idComposicao=' + $('#IdComposicaoAlterar').val(),
        success: function (retorno) {
            ShowMessageSucess(retorno);

            if (retorno == "Seta alterada com sucesso!") {
                $('#containerEixos').modal('hide');
            }

            raizenCoreJs.raizenHelpers.FecharLoading();
        },
        error: function (partialView) {
            ShowMessage(partialView);
            raizenCoreJs.raizenHelpers.FecharLoading();
        }
    });
}

Composicao.prototype.SalvarInativarCompartimento = function SalvarInativarCompartimento() {
    raizenCoreJs.raizenHelpers.AbrirLoading();

    var dados = $('#frmInativarCompartimento').serialize();

    $.ajax({
        type: "POST",
        url: this.urlInativarCompartimentoSalvar,
        data: dados + '&idComposicao=' + $('#IdComposicaoAlterar').val(),
        success: function (retorno) {
            ShowMessageSucess(retorno);

            if (retorno == "Compartimento alterado com sucesso!") {
                $('#containerEixos').modal('hide');
            }

            raizenCoreJs.raizenHelpers.FecharLoading();
        },
        error: function (partialView) {
            ShowMessage(partialView);
            raizenCoreJs.raizenHelpers.FecharLoading();
        }
    });
}

Composicao.prototype.VisualizarDocumentos = function VisualizarDocumentos(id) {
    raizenCoreJs.raizenHelpers.AbrirLoading();
    $.ajax({
        type: "GET",
        url: this.urlVisualizarDocumentos,
        data: { idComposicao: id },
        success: function (retorno) {
            $('#divModal').html(retorno);
            $('#containerVisualizar').modal('show');
            raizenCoreJs.raizenHelpers.FecharLoading();
        },
        error: function (partialView) {
            ShowMessage(partialView);
            raizenCoreJs.raizenHelpers.FecharLoading();
        }
    });
}

Composicao.prototype.Bloquear = function Bloquear(id) {
    raizenCoreJs.raizenHelpers.AbrirLoading();
    $.ajax({
        type: "GET",
        url: this.urlBloquear,
        data: { idComposicao: id },
        success: function (retorno) {
            $('#divModal').html(retorno);
            $('#containerVisualizar').modal('show');
            raizenCoreJs.raizenHelpers.FecharLoading();
        },
        error: function (partialView) {
            ShowMessage(partialView);
            raizenCoreJs.raizenHelpers.FecharLoading();
        }
    });
}

Composicao.prototype.SalvarBloquear = function SalvarBloquear(id) {

    if ($('#Justificativa').val() == '') {
        ShowMessage(GetMessage("Favor informar a justificativa!", "¡Por favor informa la justificación!"));
        return;
    }

    raizenCoreJs.raizenHelpers.AbrirLoading();
    $.ajax({
        type: "POST",
        url: this.urlSalvarBloquear,
        data: { idComposicao: id, justificativa: $('#Justificativa').val(), bloqueado: $('input[name=Bloqueado]:checked').val() },
        success: function (retorno) {
            if (retorno == GetMessage("Gravado com sucesso!", "¡Grabado con éxito!")) {
                ShowMessageSucess(retorno);
                raizenCoreJs.raizenHelpers.FecharModal('containerVisualizar');
                raizenCoreJs.raizenCRUD.RealizarPesquisa();
            }
            else {
                ShowMessage(retorno);
            }
            raizenCoreJs.raizenHelpers.FecharLoading();
        },
        error: function (partialView) {
            ShowMessage(partialView);
            raizenCoreJs.raizenHelpers.FecharLoading();
        }
    });
}

Composicao.prototype.Checklist = function Checklist(id) {
    raizenCoreJs.raizenHelpers.AbrirLoading();
    $.ajax({
        type: "GET",
        url: this.urlChecklist,
        data: { idComposicao: id },
        success: function (retorno) {
            $('#divModal').html(retorno);
            $('#containerVisualizar').modal('show');
            raizenCoreJs.raizenHelpers.FecharLoading();
        },
        error: function (partialView) {
            ShowMessage(partialView);
            raizenCoreJs.raizenHelpers.FecharLoading();
        }
    });
}

Composicao.prototype.SalvarConfirmacaoCheckList = function SalvarConfirmacaoCheckList(id, isReplicarEab) {
    raizenCoreJs.raizenHelpers.AbrirLoading();
    $.ajax({
        type: "POST",
        url: this.urlSalvarChecklist,
        data: { idComposicao: id, justificativa: $('#Justificativa').val(), data: $('#Data').val(), anexo: $('#Anexo-34').val(), aprovado: $('input[name=Aprovado]:checked').val(), isReplicarEab: isReplicarEab },
        success: function (retorno) {

            if (retorno == "Gravado com sucesso!") {
                raizenCoreJs.raizenHelpers.FecharModal('containerVisualizar');
                raizenCoreJs.raizenCRUD.RealizarPesquisa();
                ShowMessageSucess(retorno);
            }
            else {
                ShowMessage(retorno);
            }
            raizenCoreJs.raizenHelpers.FecharLoading();
        },
        error: function (partialView) {
            ShowMessage(partialView);
            raizenCoreJs.raizenHelpers.FecharLoading();
        }

    });

    raizenCoreJs.raizenHelpers.FecharModal('ModalConfirmAmbas');
}

Composicao.prototype.SalvarChecklist = function SalvarChecklist(id) {

    if ($('#Anexo-34').val() == '' && $('input[name="Aprovado"]:checked').val() == 'true') {
        ShowMessage(GetMessage("Favor informar um anexo!", "¡Por favor introduzca un archivo adjunto!"));
        return;
    }

    if ($('#Data').val() == '' && $('input[name="Aprovado"]:checked').val() == 'true') {
        ShowErrorMenssage("Favor informar a Data!");
        return;
    }
    if ($('#Data').val() != '') {
        var ano = new Date($('#Data').val()).getFullYear();
        if (ano <= 1800 || ano >= 2900) {
            ShowErrorMenssage("A Data deve ser maior que 1800 e menor que 2900!");
            return;
        }
    }

    if ($('#Justificativa').val() == '' && $('input[name="Aprovado"]:checked').val() == 'false') {
        ShowErrorMenssage(GetMessage('Favor informar a justificativa!', '¡Por favor informa la justificación!'));
        return;
    }
    var idEmpresa = $("#IdEmpresaComposicao").val();
    //Ambas -- toda empresa ambas é CIF
    if (idEmpresa == 3) {
        $("#idComposicaoChecklist").val(id);
        //$('#modalConfirmAmbas').html(retorno);
        $('#ModalConfirmAmbas').modal('show');

    } else {
        raizenCoreJs.Composicao.SalvarConfirmacaoCheckList(id, true);
    }
}

Composicao.prototype.ExcluirComposicaoConfirm = function ExcluirComposicaoConfirm(id, somenteComposicao, idPais) {

    if (typeof idPais === "undefined") {
        idPais = 1;
    }

    const message = GetMessage("Deseja realmente excluir essa composição?", "¿Queres realmente eliminar esta composición?");
    $('#MessageConfirm1').html(message);

    RaizenHelpers.prototype.AbrirConfirm('return raizenCoreJs.Composicao.ExcluirComposicao(' + id + ', ' + somenteComposicao + ')', message, idPais);
}

Composicao.prototype.ExcluirComposicao = function ExcluirComposicao(id, somenteComposicao) {
    raizenCoreJs.raizenHelpers.AbrirLoading();
    $.ajax({
        type: "POST",
        url: this.urlExcluirComposicao,
        data: { id: id, somenteComposicao: somenteComposicao },
        success: function (retorno) {
            raizenCoreJs.raizenHelpers.FecharLoading();
            ShowMessageSucess(retorno);
            raizenCoreJs.raizenCRUD.RealizarPesquisa();
        },
        error: function (partialView) {
            ShowMessage(partialView);
        }
    });
}

function carregarCamposEmComum(retorno) {
    $('[id="Composicao_EixosComposicao"]').val(retorno.NumeroEixos);
    $('[id="Composicao_EixosPneusDuplos"]').val(retorno.NumeroEixosPneusDuplos);
    $('[id="Composicao_EixosDistanciados"]').val(retorno.NumeroEixosDistanciados);

    var tara = retorno.Tara.toFixed(2).toString();
    if (tara !== null && tara.indexOf('.') > 0)
        tara = tara.replace(".", ",");
    $('[id="Composicao_TaraComposicao"]').val(tara);
    $('[id="Composicao_IDCategoriaVeiculo"]').val(retorno.IDCategoriaVeiculo);

    if (RaizenCoreJs.prototype.Composicao._idPais === 1) {
        $('#Composicao_CPFCNPJ').val(retorno.CPFCNPJ);
    }
    else if (RaizenCoreJs.prototype.Composicao._idPais === 2) {
        $('#Composicao_CPFCNPJ').val(retorno.Cuit);
    }

    $('#Composicao_RazaoSocial').val(retorno.RazaoSocial);
    $('#Composicao_DataNascimento').val(retorno.Datas);
}

function carregarQuantidadesEixosBrasil(retorno) {
    carregarCamposEmComum(retorno);

    $('[id="Composicao_ufCRLV"]').val(retorno.IDEstado);
}

function carregarQuantidadesEixosArgentina(retorno) {
    carregarCamposEmComum(retorno);
    $('[id="Composicao_IDCategoriaVeiculo"]').val(retorno.IDCategoriaVeiculo);

    retorno.PBTC = retorno.PBTC.toFixed(2).toString().replace('.', ',');

    $('[name="Composicao.PBTC"]').val(retorno.PBTC);
}

function FormatarCuitCnpj(idPais) {
    if (idPais === 1) {
        FormatarCnpj();
    } else {
        $("#Composicao_CPFCNPJ").mask("99-99999999-9");
    }
}

Composicao.prototype.carregarQuantidadesEixos = function carregarQuantidadesEixos() {
    $.ajax({
        type: "POST",
        url: this.urlObterQuantidades,
        data: { placa1: $('#Composicao_IDPlaca1').val(), placa2: $('#Composicao_IDPlaca2').val(), placa3: $('#Composicao_IDPlaca3').val(), placa4: $('#Composicao_IDPlaca4').val(), IDEmpresa: $('#Composicao_IDEmpresa').val() },
        success: function (retorno) {
            switch (retorno.IdPais) {
                case 1: // Brasil
                    carregarQuantidadesEixosBrasil(retorno);
                    break;

                case 2: // Argentina
                    carregarQuantidadesEixosArgentina(retorno);
                    break;

                default:
                    carregarQuantidadesEixosBrasil(retorno);
                    break;
            }

            FormatarCuitCnpj(retorno.IdPais);

            raizenCoreJs.Composicao.carregarDocumentos();
        },
        error: function (partialView) {
            ShowMessage(partialView);
        }
    });
}

Composicao.prototype.carregarCuit = function carregarCuit(cuit, razaoSocial) {
    $('[name="Composicao.CPFCNPJ"]').val(cuit);
    $('[name="Composicao.CUIT"]').val(cuit);
    $('[name="Composicao.RazaoSocial"]').val(razaoSocial);
}

Composicao.prototype.carregarCuitAr = function carregarCuitAr(cuit, razaoSocial) {
    $('[name="Composicao.CPFCNPJ"]').val(cuit);
    $('[name="Composicao.RazaoSocial"]').val(razaoSocial);

    $('[name="Composicao.CPFCNPJ"]').mask("99-99999999-9");
}

Composicao.prototype.checarPlacaRepetida = function checarPlacaRepetida(placa, isFromPlaca) {

    var placaLength = 7;

    if (this._idPais == 2)
        placaLength = 6;

    if ($(placa).val().length >= placaLength) {
        var tipo = $('#Composicao_IDTipoComposicao').val();
        //bitrem ou dolly
        if (tipo == 3 || tipo == 4) {
            var placa1 = $('#Composicao_Placa2').val().toUpperCase();

            if (tipo == 3)
                var placa2 = $('#Composicao_Placa3').val().toUpperCase();
            else
                var placa2 = $('#Composicao_Placa4').val().toUpperCase();
            var retorno = false;

            if (isFromPlaca)
                retorno = placa1 != "" ? placa1 == $(placa).val().toUpperCase() : placa2 == $(placa).val().toUpperCase();
            else
                retorno = placa1 != "" ? placa1 == placa2 : false;

            if (retorno) {
                ShowErrorMenssage('A placa da 1º e da 2º carreta não podem ser iguais. Informe outra placa!');
                $(placa).val('');
                $(placa).focus();
                return;
            }
        }       

        if (tipo == 8 || tipo == 9) {
            var placa1 = $('#Composicao_Placa2').val().toUpperCase();
            var placa2 = $('#Composicao_Placa3').val().toUpperCase();

            var retorno = false;

            if (isFromPlaca)
                retorno = placa1 != "" ? placa1 == $(placa).val().toUpperCase() : placa2 == $(placa).val().toUpperCase();
            else
                retorno = placa1 != "" ? placa1 == placa2 : false;

            if (retorno) {
                ShowErrorMenssage('La patente del 1º e 2º semirremolque no pueden ser iguales, ¡Informa otra patente!');
                $(placa).val('');
                $(placa).focus();
                return;
            }
        }
    }
}

Composicao.prototype.carregarDocumentos = function carregarDocumentos() {
    $.ajax({
        type: "POST",
        url: this.urlListarDocumentos,
        data: { placa1: $('#Composicao_IDPlaca1').val(), placa2: $('#Composicao_IDPlaca2').val(), placa3: $('#Composicao_IDPlaca3').val(), placa4: $('#Composicao_IDPlaca4').val() },
        success: function (partialView) {
            $('#documentosComposicao').html(partialView);
        },
        error: function (partialView) {
            ShowMessage(partialView);
        }
    });
}

Composicao.prototype.Exportar = function Exportar() {
    var dados = $('#frmPesquisa').serialize();
    raizenCoreJs.raizenHelpers.AbrirLoading();
    window.location = raizenCoreJs.Composicao.urlExportar + "?data=" + dados;
    raizenCoreJs.raizenHelpers.FecharLoading();
}

Composicao.prototype.MensagemSalvarPlaca = function MensagemSalvarPlaca() {
    var idStatus = $('#Composicao_IDStatus').val();
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
    if (flag && $('#Aprovar').val() == 'False' && idStatus !== '2' /* aprovado */)
        ShowMessageSucess(GetMessage('Informe se o veículo é arrendado ou não e SALVE a composição', 'Informa si el vehículo es alquilado o no, y SALVA la composición'), null, "fa-exclamation-triangle");
};

Composicao.prototype.ExcluirPlacaConfirm = function ExcluirPlacaConfirm(id) {

    $.ajax({
        url: this.urlVerificarClientePermissao,
        type: 'GET',
        data: { id: id },
        success: function (response) {

            var result = response.Data;
            let message = GetMessage("Tem certeza que deseja excluir essa placa?", "¿Queres realmente eliminar esta placa?");

            if (result.quantidade <= 0) {
                message = GetMessage("Você não possui vínculo com essa placa", "No tienes enlace a esta placa");
                ShowErrorMenssage(message);
            }
            else if (result.quantidade === 1) {
                RaizenHelpers.prototype.AbrirConfirm('return raizenCoreJs.Composicao.ExcluirPlaca(' + id + ',' + result.idPlaca + ',' + result.placasUsuario + ')', message, RaizenCoreJs.prototype.Composicao._idPais);
            } else {
                RaizenHelpers.prototype.AbrirConfirmExclusaoPlaca(
                    'return raizenCoreJs.Composicao.ExcluirPlaca(' + id + ',' + result.idPlaca + ', [' + result.placasUsuario + '])',
                    'return raizenCoreJs.Composicao.ExcluirPlaca(' + id + ',' + result.idPlaca + ', [' + result.placasRede + '])',
                    message, result.placasUsuario <= 0);
            }
        },
        error: function (e) {
            ShowMessage(e);
        }
    });
};

Composicao.prototype.ExcluirPlaca = function ExcluirPlaca(idComposicao, idPlaca, placaClientes) {
    raizenCoreJs.raizenHelpers.AbrirLoading();
    $.ajax({
        type: "POST",
        url: this.urlExcluirPlaca,
        data: {
            idComposicao: idComposicao,
            idPlaca: idPlaca,
            placaClientes: placaClientes
        },
        success: function (retorno) {
            raizenCoreJs.raizenHelpers.FecharLoading();
            raizenCoreJs.raizenCRUD.RealizarPesquisa();
            ShowMessage(retorno);
        },
        error: function (partialView) {
            raizenCoreJs.raizenHelpers.FecharLoading();
            ShowMessage(partialView);
        }
    });
}
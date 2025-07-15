function AgendamentoTreinamento() {
    this.urlLista = "";
    this.urlHorarios = "";
    this.urlObterComposicao = "";
    this.urlInscrever = "";
    this.urlDownload = "";
    this.urlBuscarMotorista = "";
    this.urlEditarMotorista = "";
    this.urlBuscarTipoAgenda = "";
    this.urlBuscarTerminal = "";
    this.urlBuscarEmpresasCongeneres = "";
    this.urlVerificarCpfCongenereJaCadastrado = "";
}

$(document).ready(function () {
    $('#Filtro_CPF').mask('999.999.999-99')

});

var raizenCoreJs;
var RaizenCoreJs;
var ShowMessage;
var RaizenHelpers;
var ShowMessageSucess;
var ShowErrorMenssage;
var ValidarCPF;

AgendamentoTreinamento.prototype.carregarTerminal = function carregarTerminal() {
    var operacao = $('#AgendamentoTreinamento_Operacao').val();
    var idterminal = $("#idTerminalTreinamento").val();
   
    console.log(idterminal);
    if (operacao != '') {
        raizenCoreJs.raizenHelpers.AbrirLoading();
        var url = this.urlBuscarTerminal;
        $.ajax({
            url: url,
            data: { Operacao: operacao },
            type: 'GET',
            success: function (response) {
                if (response.result == "sucesso") {
                    var $select = $("#AgendamentoTreinamento_IDTerminal").selectize();
                    var control = $select[0].selectize;
                    control.clear();
                    control.clearOptions();
                    $.each(response.list, function () {
                        control.addOption({ value: this.Value, text: this.Text })
                    });
                    console.log(idterminal);
                    if (idterminal != 0)
                        control.setValue(idterminal, false);
                }
                else {
                    ShowErrorMenssage('Falha ao tentar buscar terminal: ' + response.message + ' , por favor, tente novamente.');
                }

            }
        });
        raizenCoreJs.AgendamentoTreinamento.carregarNovosCampos(operacao);
        raizenCoreJs.raizenHelpers.FecharLoading();
    }
}

AgendamentoTreinamento.prototype.verificarCpf = function verificarCpf() {
    var operacao = $('#AgendamentoTreinamento_Operacao').val();
    var cpf = $('#AgendamentoTreinamento_CPFCongenere').val();
    var idempresa = $('#AgendamentoTreinamento_IDEmpresa').val();
    var data = $('#AgendamentoTreinamento_Data').val();
    var idtipoAgenda = $('#AgendamentoTreinamento_IDTipoTreinamento').val();
    var idTerminal = $('#AgendamentoTreinamento_IDTerminal').val();
    var idTipoTreinamento = $("#AgendamentoTreinamento_IDTipoTreinamento").val();
    var isEditar = $('#isEditar').val();

    if (idempresa == '') {
        ShowErrorMenssage("Selecione uma linha de negócio");
    }
    if (data == '') {
        ShowErrorMenssage("Selecione uma data");
    }
    if (idtipoAgenda == '') {
        ShowErrorMenssage("Selecione um tipo de agenda");
    }
    if (idTerminal == '') {
        ShowErrorMenssage("Selecione um terminal");
    }
    if (idTipoTreinamento == '') {
        ShowErrorMenssage("Selecione um tipo de treinamento");
    }


    if (operacao == 'CON' &&
        cpf.replace(".", "").replace(".", "").replace("-", "").length > 10) {
        if (!ValidarCPF(cpf)) {
            ShowErrorMenssage("CPF Inválido");
            return;
        }

        raizenCoreJs.raizenHelpers.AbrirLoading();
        var url = this.urlVerificarCpfCongenereJaCadastrado;
        $.ajax({
            type: "GET",
            url: url,
            data: {
                cpf: cpf,
                idEmpresa: idempresa,
                data: data,
                idtipoAgenda: idtipoAgenda,
                idTerminal: idTerminal,
                isEditar: isEditar
            },
            success: function (retorno) {
                if (retorno.situacao == "jaExisteAgendamentoTeorico") {
                    ShowErrorMenssage("Já existe agendamento ativo para esse motorista!");
                    $('#AgendamentoTreinamento_CPFCongenere').val("");
                    $('#AgendamentoTreinamento_CPFCongenere').focus()
                }
                else if (retorno.situacao == "jaExisteAgendamentoPratico") {
                    ShowErrorMenssage("Este motorista já possui agendamento para este dia.");
                    $('#AgendamentoTreinamento_CPFCongenere').val("");
                    $('#AgendamentoTreinamento_CPFCongenere').focus()
                }
            }
        });
        raizenCoreJs.raizenHelpers.FecharLoading();
    }
}

AgendamentoTreinamento.prototype.listarEmpresas = function listarEmpresas() {
    var operacao = $('#AgendamentoTreinamento_Operacao').val();
    var idEmpresaCongenere = $('#idEmpresaCongenere').val();
    if (operacao == "CON") {
        var idTerminal = $('#AgendamentoTreinamento_IDTerminal').val();
        if (idTerminal != '') {
            raizenCoreJs.raizenHelpers.AbrirLoading();
            var url = this.urlBuscarEmpresasCongeneres;
            $.ajax({
                url: url,
                data: { idTerminal: idTerminal },
                type: 'GET',
                success: function (response) {
                    if (response.result == "sucesso") {
                        var $select = $("#AgendamentoTreinamento_IDEmpresaCongenere").selectize();
                        var control = $select[0].selectize;
                        control.clear();
                        control.clearOptions();
                        $.each(response.list,
                            function () {
                                control.addOption({ value: this.Value, text: this.Text })
                            });
                        if (idEmpresaCongenere != 0)
                            control.setValue(idEmpresaCongenere, false);
                    } else {
                        ShowErrorMenssage('Falha ao tentar buscar terminal: ' +
                            response.message +
                            ' , por favor, tente novamente.');
                    }

                }
            });
            raizenCoreJs.AgendamentoTreinamento.carregarNovosCampos(operacao);
            raizenCoreJs.raizenHelpers.FecharLoading();
        }
    }
}

AgendamentoTreinamento.prototype.carregarNovosCampos = function carregarNovosCampos(operacao) {
    if (operacao == "CON") {
        $("#naoCongenere").fadeOut(100);
        $("#congenere").fadeIn(1000);
    } else {
        $("#congenere").fadeOut(100);
        $("#naoCongenere").fadeIn(1000);
    }
}

AgendamentoTreinamento.prototype.carregarTipoAgenda = function carregarTipoAgenda() {
    var tipoTreinamento = $('#AgendamentoTreinamento_IDTipoTreinamento').val();
    var idTipo = $('#idTipo').val();
    if (tipoTreinamento != '' && tipoTreinamento != null) {
        raizenCoreJs.raizenHelpers.AbrirLoading();
        var url = this.urlBuscarTipoAgenda;
        $.ajax({
            url: url,
            data: { Id: tipoTreinamento },
            type: 'GET',
            success: function (response) {
                if (response.result == "sucesso") {
                    var $select = $("#AgendamentoTreinamento_IDTipo").selectize();
                    var control = $select[0].selectize;
                    control.clear();
                    control.clearOptions();
                    $.each(response.list, function () {
                        control.addOption({ value: this.Value, text: this.Text })
                    });
                    if (idTipo != 0)
                        control.setValue(idTipo, false);
                }
                else {
                    ShowErrorMenssage('Falha ao tentar buscar tipo de agenda: ' + response.message + ' , por favor, tente novamente.');
                }
                raizenCoreJs.raizenHelpers.FecharLoading();
            }
        });
    }
}


AgendamentoTreinamento.prototype.EditarMotorista = function EditarMotorista(act) {
    var url = this.urlEditarMotorista;
    window.location.href = url;
}

AgendamentoTreinamento.prototype.LimparFiltros = function LimparFiltros() {
    $("#Filtro_Motorista").val('');
    $("#Filtro_CPF").val('');
    var isLinhaNegocioDisabled = $("#Filtro_IDEmpresa").prop('disabled');
    if (!isLinhaNegocioDisabled || isLinhaNegocioDisabled == undefined)
        $("#Filtro_IDEmpresa").val($("#target option:first").val());
    $("#Filtro_Operacao").val($("#target option:first").val());
    $("#Filtro_Chamado").val('');

    var $select = $("#Filtro_IDTipoAgendamentoTreinamento").selectize();
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

RaizenCoreJs.prototype.AgendamentoTreinamento = new AgendamentoTreinamento();

AgendamentoTreinamento.prototype.LimparFiltros = function LimparFiltros() {
    $("#Filtro_Placa").val('');
    var isLinhaNegocioDisabled = $("#Filtro_IDEmpresa").prop('disabled');
    if (!isLinhaNegocioDisabled || isLinhaNegocioDisabled == undefined)
        $("#Filtro_IDEmpresa").val($("#target option:first").val());
    $("#Filtro_Operacao").val($("#target option:first").val());
    $("#Filtro_Chamado").val('');

    var $select = $("#Filtro_IDTipoAgendamentoTreinamento").selectize();
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

AgendamentoTreinamento.prototype.Inscrever = function Inscrever(idHorario) {
    RaizenHelpers.prototype.AbrirConfirm('return raizenCoreJs.AgendamentoTreinamento.funcInscrever(' + idHorario + ')', 'Deseja realmente se inscrever nesse horário?');
}

AgendamentoTreinamento.prototype.funcInscrever = function funcInscrever(idHorario) {
    var operacao = $('#AgendamentoTreinamento_Operacao').val();
    console.log(operacao);
    if (operacao == "CON") {
        if ($('#AgendamentoTreinamento_CPFCongenere').val() == '') {
            ShowMessage('Por favor digite um CPF');
            return;
        }
        if ($('#AgendamentoTreinamento_NomeMotoristaCongenere').val() == '') {
            ShowMessage('Por favor digite o nome do motorista congênere');
            return;
        }
        if ($('#AgendamentoTreinamento_IDEmpresaCongenere').val() == '') {
            ShowMessage('Por favor digite uma empresa congênere');
            return;
        }
    } else {
        var idMotorista = $('#AgendamentoTreinamento_IDMotorista').val();
        if (idMotorista == '' || idMotorista == null || idMotorista == 0) {
            ShowMessage('Por favor digite um CPF');
            return;
        }
    }
    $('#AgendamentoTreinamento_IDAgendamentoTerminalHorario').val(idHorario);

    raizenCoreJs.raizenCRUD.ExibirControlesOcultos();
    var dados = $('#frmEdicao').serialize();
    raizenCoreJs.raizenCRUD.OcultarControlesOcultos();

    raizenCoreJs.raizenHelpers.AbrirLoading();

    $.ajax({
        url: this.urlInscrever,
        data: dados,
        type: 'POST',
        success: function (response) {
            if (response.Mensagem != '' && response.Mensagem != null) {

                if ($.inArray('não existem mais vagas disponíveis', response.Mensagem) == -1) {
                    raizenCoreJs.AgendamentoTreinamento.empresaOperacaoTerminalChange(false);
                }
                else {
                    $('#AgendamentoTreinamento_CPF').val("");
                    $('#lblNome').html("");
                }

                ShowMessage(response.Mensagem);
            }
            else {
                ShowMessageSucess("Agendamento concluído com sucesso!",
                        "raizenCoreJs.raizenCRUD.Voltar();$('#modalSucess').modal('hide');");
            }
            raizenCoreJs.raizenHelpers.FecharLoading();
        }
    });
};

AgendamentoTreinamento.prototype.empresaOperacaoTerminalChange = function empresaOperacaoTerminalChange(change) {

    var idEmpresa = $('#AgendamentoTreinamento_IDEmpresa').val();
    var operacao = $('#AgendamentoTreinamento_Operacao').val();
    var idTerminal = $('#AgendamentoTreinamento_IDTerminal').val();
    var idTipoTreinamento = $('#AgendamentoTreinamento_IDTipo').val();
    var data = $('#AgendamentoTreinamento_Data').val();
    var id = $('#ChavePrimaria').val();
    var url = this.urlHorarios;

    if (idEmpresa != '' && operacao != '' && idTerminal != '' && data != '' && idTipoTreinamento != '') {
        raizenCoreJs.raizenHelpers.AbrirLoading();
        $.ajax({
            url: url,
            data: { IDEmpresa: idEmpresa, Operacao: operacao, IDTerminal: idTerminal, IDTipoTreinamento: idTipoTreinamento, Data: data, ID: id },
            type: 'POST',
            success: function (response) {
                $("#containerHorarios").html(response);
                $("#AgendamentoTreinamento_CPF").val('');
                $('#btnNovo').hide();
                raizenCoreJs.raizenHelpers.FecharLoading();
            }
        });
    }
}


AgendamentoTreinamento.prototype.Exportar = function Exportar() {
    var dados = $('#frmPesquisa').serialize();
    raizenCoreJs.raizenHelpers.AbrirLoading();
    window.location = raizenCoreJs.AgendamentoTreinamento.urlExportar + "?data=" + dados;
    raizenCoreJs.raizenHelpers.FecharLoading();
}

AgendamentoTreinamento.prototype.Imprimir = function Imprimir(id) {
    var url = this.urlDownload;
    $.ajax({
        cache: false,
        url: raizenCoreJs.AgendamentoTreinamento.urlGerarPdf,
        data: { id: id },
        success: function (d) {
            window.location = url + '?fileGuid=' + d.FileGuid
                              + '&filename=' + d.FileName;
        },
        error: function () {
            alert("Error");
        }
    });
    //window.location = raizenCoreJs.Motorista.urlGerarPdf + "/?id=" + id;
}

function ValidarCPF(cpf) {
    var Soma;
    var Resto;
    Soma = 0;
    var strCPF = cpf.replace('.', '').replace('.', '').replace('-', '');
    if (strCPF === "00000000000"
        || strCPF === "11111111111"
        || strCPF === "22222222222"
        || strCPF === "33333333333"
        || strCPF === "44444444444"
        || strCPF === "55555555555"
        || strCPF === "66666666666"
        || strCPF === "77777777777"
        || strCPF === "88888888888"
        || strCPF === "99999999999") return false;

    for (var i = 1; i <= 9; i++) Soma = Soma + parseInt(strCPF.substring(i - 1, i)) * (11 - i);
    Resto = (Soma * 10) % 11;

    if ((Resto == 10) || (Resto == 11)) Resto = 0;
    if (Resto != parseInt(strCPF.substring(9, 10))) return false;

    Soma = 0;
    for (var i = 1; i <= 10; i++) Soma = Soma + parseInt(strCPF.substring(i - 1, i)) * (12 - i);
    Resto = (Soma * 10) % 11;

    if ((Resto == 10) || (Resto == 11)) Resto = 0;
    if (Resto != parseInt(strCPF.substring(10, 11))) return false;
    return true;
}

AgendamentoTreinamento.prototype.buscarMotorista = function buscarMotorista(e, isOrigemLinhaNegocio) {
    var ctrlDown = false,
        ctrlKey = 17,
        cmdKey = 91,
        vKey = 86,
        bkpKey = 8,
        cKey = 67;

    if (isOrigemLinhaNegocio || ((e.key >= 0 && e.key <= 9) || (e.keyCode == ctrlKey || e.keyCode == cmdKey || e.keyCode == bkpKey))) {
        $('#btnNovo').hide();
        $('#lblNome').text("");
        $("#AgendamentoTreinamento_IDMotorista").val("");

        if (!isOrigemLinhaNegocio && $('#AgendamentoTreinamento_IDEmpresa').val() == '') {
            ShowMessage('Por favor selecione uma linha de negócio');
            $('#AgendamentoTreinamento_IDEmpresa').focus();
            return;
        }

        if (!isOrigemLinhaNegocio && $('#AgendamentoTreinamento_Operacao').val() == '') {
            ShowMessage('Por favor selecione uma operação');
            $('#AgendamentoTreinamento_Operacao').focus();
            return;
        }

        if ($('#AgendamentoTreinamento_CPF').val().replace(".", "").replace(".", "").replace("-", "").length > 10) {

            if (!ValidarCPF($('#AgendamentoTreinamento_CPF').val())) {
                ShowErrorMenssage("CPF Inválido");
                return;
            }

            var cpf = $('#AgendamentoTreinamento_CPF').val();
            var idempresa = $('#AgendamentoTreinamento_IDEmpresa').val();
            var operacao = $('#AgendamentoTreinamento_Operacao').val();
            var data = $('#AgendamentoTreinamento_Data').val();
            var idtipoAgenda = $('#AgendamentoTreinamento_IDTipoTreinamento').val();
            var idTerminal = $('#AgendamentoTreinamento_IDTerminal').val();
            var isEditar = $('#isEditar').val();
            //se a chamada vier de outro campo fora o cpf validar todos os campos 
            //caso contrário apenas linha de negócio e operação para buscar apenas o motorista (não validar ainda a regra de data de treinamento)
            if ((isOrigemLinhaNegocio && cpf != '' && idempresa != '' && operacao != '' && idtipoAgenda != '') || (!isOrigemLinhaNegocio && idempresa != '' && operacao != '')) {
                raizenCoreJs.raizenHelpers.AbrirLoading();
                var url = this.urlBuscarMotorista;
                $.ajax({
                    type: "GET",
                    url: url,
                    data: { cpf: cpf, idEmpresa: idempresa, operacao: operacao, data: data, idtipoAgenda: idtipoAgenda, idTerminal: idTerminal, isEditar: isEditar },
                    success: function (retorno) {
                        if (retorno.situacao == "inapto") {
                            ShowErrorMenssage("Esse motorista não está apto para o agendamento! Favor verificar o cadastro.");
                            $('#AgendamentoTreinamento_CPF').val("");
                            $('#AgendamentoTreinamento_CPF').focus()
                        }
                        else if (retorno.situacao == "novo") {
                            ShowMessage("Esse motorista não tem cadastro no sistema! Caso deseje cadastrar, clique em \"+\"");
                            $('#btnNovo').show();
                        }
                        else if (retorno.situacao == "agendado") {
                            $('#lblNome').text(retorno.nome);
                            $("#AgendamentoTreinamento_IDMotorista").val(retorno.id);
                            ShowMessageSucess("Esse motorista possui treinamento válido até  " + retorno.data + ". Caso queira realizar outro treinamento, favor seguir com o agendamento.", null, "fa-exclamation-triangle");
                        }
                        else if (retorno.situacao == "jaExisteAgendamentoTeorico") {
                            ShowErrorMenssage("Já existe agendamento ativo para esse motorista!");
                            $('#AgendamentoTreinamento_CPF').val("");
                            $('#AgendamentoTreinamento_CPF').focus()
                        }
                        else if (retorno.situacao == "jaExisteAgendamentoPratico") {
                            ShowErrorMenssage("Este motorista já possui agendamento para este dia.");
                            $('#AgendamentoTreinamento_CPF').val("");
                            $('#AgendamentoTreinamento_CPF').focus()
                        }
                        else if (retorno.situacao == "naoExisteTeorico") {
                            ShowErrorMenssage("O motorista informado não está apto para a realização de treinamento prático, pois ainda não realizou o treinamento teórico! Favor verificar a situação.");
                            $('#AgendamentoTreinamento_CPF').val("");
                            $('#AgendamentoTreinamento_CPF').focus()
                        }
                        else {
                            $('#lblNome').text(retorno.nome);
                            $("#AgendamentoTreinamento_IDMotorista").val(retorno.id);
                        }
                    },
                    error: function (retorno) {
                        ShowMessage(retorno);
                    }

                });
            }
            raizenCoreJs.raizenHelpers.FecharLoading();
        }
    }
};

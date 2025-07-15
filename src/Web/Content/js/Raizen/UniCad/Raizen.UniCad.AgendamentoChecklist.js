function AgendamentoChecklist() {
    this.urlLista = "";
    this.urlHorarios = "";
    this.urlObterComposicao = "";
    this.urlInscrever = "";
    this.urlDownload = "";
    this.urlNovaComposicao = "";
    this.urlBuscarTerminal = "";
    this.urlBuscarEmpresasCongeneres = "";
}

var raizenCoreJs;
var RaizenCoreJs;
var ShowMessage;
var RaizenHelpers;
var ShowMessageSucess;
var ShowErrorMenssage;

AgendamentoChecklist.prototype.listarEmpresas = function listarEmpresas() {
    var operacao = $('#AgendamentoChecklist_Operacao').val();
    var idEmpresaCongenere = $('#idEmpresaCongenere').val();
    if (operacao == "CON") {
        var idTerminal = $('#AgendamentoChecklist_IDTerminal').val();
        if (idTerminal != '') {
            raizenCoreJs.raizenHelpers.AbrirLoading();
            var url = this.urlBuscarEmpresasCongeneres;
            $.ajax({
                url: url,
                data: { idTerminal: idTerminal },
                type: 'GET',
                success: function (response) {
                    if (response.result == "sucesso") {
                        var $select = $("#AgendamentoChecklist_IDEmpresaCongenere").selectize();
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
            raizenCoreJs.AgendamentoChecklist.carregarNovosCampos(operacao);
            raizenCoreJs.raizenHelpers.FecharLoading();
        }
    }
}


AgendamentoChecklist.prototype.carregarTerminal = function carregarTerminal() {
    var operacao = $('#AgendamentoChecklist_Operacao').val();
    var idterminal = $("#idTerminalChecklist").val();
   
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
                    var $select = $("#AgendamentoChecklist_IDTerminal").selectize();
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
        raizenCoreJs.AgendamentoChecklist.carregarNovosCampos(operacao);
        raizenCoreJs.raizenHelpers.FecharLoading();
    }
}

AgendamentoChecklist.prototype.carregarNovosCampos = function carregarNovosCampos(operacao) {
    if (operacao == "CON") {
        $("#naoCongenere").fadeOut(100);
        $("#congenere").fadeIn(1000);
    } else {
        $("#congenere").fadeOut(100);
        $("#naoCongenere").fadeIn(1000);
    }
}

AgendamentoChecklist.prototype.LimparFiltros = function LimparFiltros() {
    $("#Filtro_Placa").val('');
    var isLinhaNegocioDisabled = $("#Filtro_IDEmpresa").prop('disabled');
    if (!isLinhaNegocioDisabled || isLinhaNegocioDisabled == undefined)
        $("#Filtro_IDEmpresa").val($("#target option:first").val());
    $("#Filtro_Operacao").val($("#target option:first").val());
    $("#Filtro_Chamado").val('');

    var $select = $("#Filtro_IDTipoAgendamentoChecklist").selectize();
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

RaizenCoreJs.prototype.AgendamentoChecklist = new AgendamentoChecklist();


AgendamentoChecklist.prototype.LimparFiltros = function LimparFiltros() {
    $("#Filtro_Placa").val('');
    var isLinhaNegocioDisabled = $("#Filtro_IDEmpresa").prop('disabled');
    if (!isLinhaNegocioDisabled || isLinhaNegocioDisabled == undefined)
        $("#Filtro_IDEmpresa").val($("#target option:first").val());
    $("#Filtro_Operacao").val($("#target option:first").val());
    $("#Filtro_Chamado").val('');

    var $select = $("#Filtro_IDTipoAgendamentoChecklist").selectize();
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

AgendamentoChecklist.prototype.placaChange = function placaChange(campo, event) {
    var idEmpresa = $('#AgendamentoChecklist_IDEmpresa').val();
    var operacao = $('#AgendamentoChecklist_Operacao').val();

    if (event.key == "Control")
        return;

    if (campo.value.length == 7 && idEmpresa != '' && operacao != '') {
        raizenCoreJs.raizenHelpers.AbrirLoading();
        $.ajax({
            url: this.urlObterComposicao,
            data: { Placa: campo.value, IDEmpresa: idEmpresa, Operacao: operacao },
            type: 'POST',
            dataType: "json",
            success: function (response) {
                if (response != null && response.Placas == undefined && response.includes('veículo não está apto')) {
                    $('#btnAddPlaca1').hide();
                    $("#AgendamentoChecklist_Placa").val('');
                    $("#placas").html('');
                    ShowErrorMenssage(response);
                }
                else if (response == null || response.Placas == undefined) {
                    $("#placas").html('');
                    $('#btnAddPlaca1').fadeIn();
                    if (response.includes('Já existe agendamento')) {
                        $("#AgendamentoChecklist_Placa").val('');
                        $('#btnAddPlaca1').hide();
                        $("#placas").html('');
                    }
                    ShowErrorMenssage(response);
                }
                else {
                    $('#btnAddPlaca1').hide();
                    if (response.Mensagem != null && response.Mensagem.includes('possui checklist')) {
                        ShowMessageSucess(response.Mensagem, null, "fa-exclamation-triangle");
                    }

                    $("#placas").html(response.Placas);
                    $("#AgendamentoChecklist_IDComposicao").val(response.IDComposicao);

                }
                raizenCoreJs.raizenHelpers.FecharLoading();
            }
        });
    }

}

AgendamentoChecklist.prototype.empresaOperacaoTerminalChange = function empresaOperacaoTerminalChange(change) {

    var idEmpresa = $('#AgendamentoChecklist_IDEmpresa').val();
    var operacao = $('#AgendamentoChecklist_Operacao').val();
    var idTerminal = $('#AgendamentoChecklist_IDTerminal').val();
    var data = $('#AgendamentoChecklist_Data').val();
    var idAgendamentoTerminalHorario = $('#AgendamentoChecklist_IDAgendamentoTerminalHorario').val();
    if (change) {
        if (idEmpresa != '' && operacao != '') {
            $("#placas").html('');
            $('#AgendamentoChecklist_Placa').val('');
            $('#AgendamentoChecklist_Placa').prop("disabled", false);
        }
        else {
            $('#AgendamentoChecklist_Placa').prop("disabled", true);
            $('#AgendamentoChecklist_Placa').val('');
            $("#placas").html('');
        }
    }

    if (idEmpresa != '' && operacao != '' && idTerminal != '' && data != '') {
        raizenCoreJs.raizenHelpers.AbrirLoading();
        var url = RaizenCoreJs.prototype.AgendamentoChecklist.urlHorarios;
        $.ajax({
            url: url,
            data: { IDEmpresa: idEmpresa, Operacao: operacao, IDTerminal: idTerminal, Data: data, idAgendamentoTerminalHorario: idAgendamentoTerminalHorario },
            type: 'POST',
            success: function (response) {
                $("#containerHorarios").html(response);
                raizenCoreJs.raizenHelpers.FecharLoading();
            }
        });
    }
}

AgendamentoChecklist.prototype.Exportar = function Exportar() {
    var dados = $('#frmPesquisa').serialize();
    raizenCoreJs.raizenHelpers.AbrirLoading();
    window.location = raizenCoreJs.AgendamentoChecklist.urlExportar + "?data=" + dados;
    raizenCoreJs.raizenHelpers.FecharLoading();
}

AgendamentoChecklist.prototype.Imprimir = function Imprimir(id) {
    var url = this.urlDownload;
    $.ajax({
        cache: false,
        url: raizenCoreJs.AgendamentoChecklist.urlGerarPdf,
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

AgendamentoChecklist.prototype.InscreverConfirm = function InscreverConfirm(idHorario) {
    if ($('#AgendamentoChecklist_PlacaCongenere').val() == '' 
            && $('#AgendamentoChecklist_IDComposicao').val() == "0") {
        ShowErrorMenssage("Informe uma placa válida.");
        return;
    }
    $('#MessageConfirm1').html('Deseja realmente se inscrever nesse horário?');
    RaizenHelpers.prototype.AbrirConfirm('return raizenCoreJs.AgendamentoChecklist.Inscrever(' + idHorario + ')');
}
AgendamentoChecklist.prototype.novaPlaca = function novaPlaca() {
    window.location = this.urlNovaComposicao;
}

AgendamentoChecklist.prototype.Inscrever = function Inscrever(idHorario) {

    $('#AgendamentoChecklist_IDAgendamentoTerminalHorario').val(idHorario);

    raizenCoreJs.raizenCRUD.ExibirControlesOcultos();
    var dados = $('#frmEdicao').serialize();
    raizenCoreJs.raizenCRUD.OcultarControlesOcultos();

    raizenCoreJs.raizenHelpers.AbrirLoading();

    var lista = this.urlLista;

    $.ajax({
        url: this.urlInscrever,
        data: dados,
        type: 'POST',
        success: function (response) {
            if (response == null || response.AgendamentoChecklist == null || response.AgendamentoChecklist.ID == "0" || (response.Mensagem != '' && response.Mensagem != null)) {
                ShowMessage(response.Mensagem);
                AgendamentoChecklist.prototype.empresaOperacaoTerminalChange(false);
                //raizenCoreJs.AgendamentoChecklist.empresaOperacaoTerminalChange(false);
            }
            else {
                ShowMessageSucess("Agendamento concluído com sucesso!",
                        "raizenCoreJs.raizenCRUD.Voltar();$('#modalSucess').modal('hide');");
            }

            raizenCoreJs.raizenHelpers.FecharLoading();
        }
    });
};
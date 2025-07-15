function AgendamentoTerminal() {
    this.urlAdicionarHorario = '';
    this.urlEditar = '';
    this.urlNovo = '';
    this.urlPesquisarHorarios = '';
    this.urlClonar = '';
    this.urlValidarExistemItensRelacionados = '';
    this.urlSalvar = '';
    this.urlVerificarSeEhPool = '';
}

var raizenCoreJs;
var RaizenCoreJs;
var ShowMessage;
var RaizenHelpers;
var ShowErrorMenssage;
var ShowMessageSucess;
var funcClonar;

RaizenCoreJs.prototype.AgendamentoTerminal = new AgendamentoTerminal();


function mascaraData(campoData, event) {
    if (event.key == 'Backspace')
        return true;
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

AgendamentoTerminal.prototype.fecharModal = function fecharModal() {
    $('body').removeClass('modal-open');
    $('.modal-backdrop').remove();
    $('#modalcontainerEdicao').modal('hide');
    $("#ModalAgendamento").html(null);
    raizenCoreJs.raizenCRUD.RealizarPesquisa();
}

AgendamentoTerminal.prototype.fecharModalClone = function fecharModalClone() {
    $('body').removeClass('modal-open');
    $('.modal-backdrop').remove();
    $('#modalcontainerClonar').modal('hide');
    $("#ModalAgendamentoClonar").html(null);
    raizenCoreJs.raizenCRUD.RealizarPesquisa();
}
AgendamentoTerminal.prototype.verificarSeEhPool = function verificarSeEhPool() {
    raizenCoreJs.raizenHelpers.AbrirLoading();
    var terminal = $('#AgendamentoTerminal_IDTerminal').val();
    var url = RaizenCoreJs.prototype.AgendamentoTerminal.urlVerificarSeEhPool;
    if (terminal != '') {
        $.ajax({
            url: url,
            data: { idTerminal: terminal },
            type: 'GET',
            success: function (response) {
                console.log(response);
                if (response == 'True') {
                    $("#AgendamentoTerminalHorario_Operacao").append(
                        $('<option></option>').val("CON").html("Congênere"))
                } else {
                    $("#AgendamentoTerminalHorario_Operacao option[value='CON']").remove();
                }
            }
        });
    }
    raizenCoreJs.raizenHelpers.FecharLoading();
}

AgendamentoTerminal.prototype.pesquisarHorarios = function pesquisarHorarios() {

    raizenCoreJs.raizenHelpers.AbrirLoading();
    var terminal = $('#AgendamentoTerminal_IDTerminal').val();
    var tipoAgenda = $('#AgendamentoTerminal_IDTipoAgenda').val();
    var data = $('#AgendamentoTerminal_Data').val();
    var ativo = $('#AgendamentoTerminal_Ativo').val();

    //só irá pesquisar se todos os campos estiverem preenchidos
    if (terminal != '' && tipoAgenda != '' && data != '' && ativo != '') {
        var dados = $('#frmEdicao').serialize() + '&isPesquisa=True';
        $.ajax({
            url: this.urlPesquisarHorarios,
            data: dados,
            type: 'GET',
            success: function (response) {
                $('#horarios').html(response);
            }
        });
    }
    raizenCoreJs.raizenHelpers.FecharLoading();
}

function funcEditarHorario(id) {
    raizenCoreJs.raizenHelpers.AbrirLoading();
    $('#idHoraAgenda').val(id);
    $('#AgendamentoTerminalHorario_HoraInicio').val($('tr[id=linhaHorario-' + id + ']').find('input[id=HoraInicio]').val().substring(0, 5))
    $('#AgendamentoTerminalHorario_HoraFim').val($('tr[id=linhaHorario-' + id + ']').find('input[id=HoraFim]').val().substring(0, 5))
    $('#AgendamentoTerminalHorario_IDEmpresa').val($('tr[id=linhaHorario-' + id + ']').find('input[id=IdLinhaNegocios]').val());
    $('#AgendamentoTerminalHorario_Operacao').val($('tr[id=linhaHorario-' + id + ']').find('input[id=Operacao]').val());
    $('#AgendamentoTerminalHorario_Vagas').val($('tr[id=linhaHorario-' + id + ']').find('input[id=NumVagas]').val());
    raizenCoreJs.raizenHelpers.FecharLoading();
}


AgendamentoTerminal.prototype.editarHorario = function editarHorario(id) {
    var url = this.urlValidarExistemItensRelacionados;
    $.ajax({
        url: url,
        data: { id: id },
        type: 'GET',
        success: function (response) {
            if (response) {
                $('#MessageConfirm1').html('Existem itens associadas a esta agenda. Deseja editar?');
                RaizenHelpers.prototype.AbrirConfirm('return funcEditarHorario(' + id + ')');
            }
            else {
                funcEditarHorario(id);
            }
        }
    });
}

AgendamentoTerminal.prototype.salvar = function salvar() {
    var ativo = $('#AgendamentoTerminal_Ativo').val();
    if (ativo == '' || ativo == null) {
        ShowErrorMenssage("Status: Preenchimento obrigatório!")
        $('#AgendamentoTerminal_Ativo').focus();
        return;
    }

    $('#AgendamentoTerminalHorario_HoraInicio').val('00:00:00');
    $('#AgendamentoTerminalHorario_HoraFim').val('00:00:00');
    if ($('#AgendamentoTerminalHorario_IDEmpresaUsuario').val() == '' || $('#AgendamentoTerminalHorario_IDEmpresaUsuario').val() == "0")
        $('#AgendamentoTerminalHorario_IDEmpresa').val('');
    if ($('#AgendamentoTerminalHorario_OperacaoUsuario').val() == '' || $('#AgendamentoTerminalHorario_OperacaoUsuario').val() == null)
        $('#AgendamentoTerminalHorario_Operacao').val('');
    $('#AgendamentoTerminalHorario_Vagas').val('0');

    raizenCoreJs.raizenCRUD.ExibirControlesOcultos();
    var dados = $('#frmEdicao').serialize();
    raizenCoreJs.raizenCRUD.OcultarControlesOcultos();
    var url = this.urlSalvar;
    raizenCoreJs.raizenHelpers.AbrirLoading();
    //se for edição capturar o id
    var id = $('#idHoraAgenda').val();

    
    $.ajax({
        url: url,
        data: dados + "&AgendamentoTerminalHorario.idHoraAgenda=" + id + "&isSalvar=True",
        type: 'GET',
        success: function (response) {
            $('#ModalAgendamento').html(response);
            if (raizenCoreJs.raizenMensagens.ContemErros == "N") {
                ShowMessageSucess("Registro atualizado com sucesso!");
                raizenCoreJs.AgendamentoTerminal.fecharModal();
            }
            else {
                raizenCoreJs.raizenMensagens.ExibirMensagemOperacao();
            }
            raizenCoreJs.raizenHelpers.FecharLoading();
        }
    });
}

AgendamentoTerminal.prototype.adicionarHorario = function adicionarHorario() {

    var dados = $('#frmEdicao').serialize();

    var HoraInicio = $('#AgendamentoTerminalHorario_HoraInicio').val();
    var horaInicioIsValid = (HoraInicio.search(/^\d{2}:\d{2}$/) != -1) &&
            (HoraInicio.substr(0, 2) >= 0 && HoraInicio.substr(0, 2) <= 24) &&
            (HoraInicio.substr(3, 2) >= 0 && HoraInicio.substr(3, 2) <= 59);
    var HoraFim = $('#AgendamentoTerminalHorario_HoraFim').val();
    var horaFimIsValid = (HoraFim.search(/^\d{2}:\d{2}$/) != -1) &&
            (HoraFim.substr(0, 2) >= 0 && HoraFim.substr(0, 2) <= 24) &&
            (HoraFim.substr(3, 2) >= 0 && HoraFim.substr(3, 2) <= 59);

    if (!horaInicioIsValid) {
        ShowErrorMenssage('Horário início inválido');
        return;
    }

    if (!horaFimIsValid) {
        ShowErrorMenssage('Horário fim inválido');
        return;
    }

    raizenCoreJs.raizenHelpers.AbrirLoading();
    //se for edição capturar o id
    var id = $('#idHoraAgenda').val();
    console.log(dados);
    $.ajax({
        url: this.urlAdicionarHorario,
        data: dados + "&AgendamentoTerminalHorario.idHoraAgenda=" + id,
        type: 'POST',
        success: function (response) {
            $('#ModalAgendamento').html(response);
            if (raizenCoreJs.raizenMensagens.ContemErros == "N") {
                $('#AgendamentoTerminalHorario_HoraInicio').val('00:00:00');
                $('#AgendamentoTerminalHorario_HoraFim').val('00:00:00');
                if ($('#AgendamentoTerminalHorario_IDEmpresaUsuario').val() == '' || $('#AgendamentoTerminalHorario_IDEmpresaUsuario').val() == "0")
                    $('#AgendamentoTerminalHorario_IDEmpresa').val('');
                if ($('#AgendamentoTerminalHorario_OperacaoUsuario').val() == '' || $('#AgendamentoTerminalHorario_OperacaoUsuario').val() == null)
                    $('#AgendamentoTerminalHorario_Operacao').val('');
                $('#AgendamentoTerminalHorario_Vagas').val('0');
                ShowMessageSucess('Alteração realizada com sucesso!');

            }
            else {
                raizenCoreJs.raizenMensagens.ExibirMensagemOperacao();
            }
            raizenCoreJs.raizenHelpers.FecharLoading();
        }
    });
}
funcClonar = function funcClonar(id) {
    raizenCoreJs.raizenHelpers.AbrirLoading();
    var dates = $('#dateRange').multiDatesPicker('getDates', 'object', 'picked')

    if (dates == null || dates == '') {
        ShowErrorMenssage('Por favor selecione ao menos uma data');
        raizenCoreJs.raizenHelpers.FecharLoading();
        return;
    }

    var lista = raizenCoreJs.raizenCRUD.urlPesquisa;
    var i;
    for (i = 0; i < dates.length; ++i) {
        dates[i] = dates[i].getFullYear() + "-" + (dates[i].getMonth() + 1) + '-' + dates[i].getDate(); //getMonth + 1: zero-index based : Java implementation
        console.log(dates[i]);
    }
    var url = RaizenCoreJs.prototype.AgendamentoTerminal.urlClonar;
    $.ajax({
        url: url,
        data: { id: id, datas: dates },
        type: 'POST',
        success: function (response) {
            if (response == '') {
                ShowMessageSucess('Agenda replicada com sucesso!', "window.location.href = '" + lista + "'", '', '40%');
            }
            else {
                ShowMessageSucess(response, "window.location.href = '" + lista + "'", "fa-exclamation-triangle", '40%');
            }
            raizenCoreJs.raizenHelpers.FecharLoading();
        }
    });
}
AgendamentoTerminal.prototype.Clonar = function Clonar(id) {
    RaizenHelpers.prototype.AbrirConfirm('return funcClonar(' + id + ')', 'Você deseja realmente clonar esse registro?');
};
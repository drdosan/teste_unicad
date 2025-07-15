function ControleAgendamentos() {
    this.urlImprimir = '';
    this.urlGerarPdf = '';
    this.urlSalvarPresenca = '';
    this.urlEditarControle = '';
    this.urlLista = '';
}

var raizenCoreJs;
var RaizenCoreJs;
var ShowMessage;
var RaizenHelpers;
var ShowMessageSucess;
var ShowErrorMenssage;

RaizenCoreJs.prototype.ControleAgendamentos = new ControleAgendamentos();

ControleAgendamentos.prototype.ControlePresenca = function ControlePresenca(data, idTerminal, idTipoAgenda, vagas, vagasDisponiveis) {
    var url = this.urlEditarControle;
    raizenCoreJs.raizenHelpers.AbrirLoading();
    $.ajax({
        url: url,
        data: { data: data, idTerminal: idTerminal, idTipoAgenda: idTipoAgenda, vagas: vagas, vagasDisponiveis: vagasDisponiveis },
        type: 'GET',
        success: function (response) {
            $("#ModalControleAgendamento").html(null);
            $("#ModalControleAgendamento").html(response);
            $("#modalContainerControlaAgendamento").modal('show');
            $("#containerEdicao").show();
            raizenCoreJs.raizenHelpers.FecharLoading();
        }
    });
}

ControleAgendamentos.prototype.salvarPresenca = function salvarPresenca() {
    var lista = this.urlLista;
    var url = this.urlSalvarPresenca;
    raizenCoreJs.raizenHelpers.AbrirLoading();
    raizenCoreJs.raizenCRUD.ExibirControlesOcultos();
    var dados = $('#frmEdicao').serialize();
    console.log(dados);
    raizenCoreJs.raizenCRUD.OcultarControlesOcultos();
    $.ajax({
        url: url,
        data: dados,
        type: 'POST',
        success: function (response) {
            if (response.retorno)
                ShowMessageSucess("Controle atualizado com sucesso.",
                    "window.location.href = '" + lista + "'");
            raizenCoreJs.raizenHelpers.FecharLoading();
        },
        error: function (response) {
            ShowErrorMenssage(response.mensagem);
        }
    });
}


ControleAgendamentos.prototype.Imprimir = function Imprimir(data, idTerminal, idTipoAgenda, vagas, vagasDisponiveis) {
    var url = this.urlImprimir;
    $.ajax({
        cache: false,
        url: this.urlGerarPdf,
        data: { data: data, idTerminal: idTerminal, idTipoAgenda: idTipoAgenda, vagas: vagas, vagasDisponiveis: vagasDisponiveis },
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

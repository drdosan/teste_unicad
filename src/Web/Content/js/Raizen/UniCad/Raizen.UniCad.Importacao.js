function Importacao() {
    this.urlProcessar = "";
    this.urlVerificar = "";
    this.id = "";
    this.myTimeout = [];
}

var raizenCoreJs;
var RaizenCoreJs;
var ShowMessage;
var RaizenHelpers;



RaizenCoreJs.prototype.Importacao = new Importacao();

Importacao.prototype.Processar = function Processar(id) {
    var objImportacao = this;
    $.ajax({
        url: this.urlProcessar,
        data: { id: id },
        type: 'GET',
        success: function (response) {
            ShowMessage('Processamento iniciado!');
            raizenCoreJs.raizenCRUD.RealizarPesquisa();
        },
        error: function (error) {
            ShowMessage(error);
        }
    });

    setTimeout(objImportacao.myTimeout.push({ id: id, fn: setInterval(function () { objImportacao.ObterStatus(id); }, 1000) }), 1000);
    
}

Importacao.prototype.ObterStatus = function ObterStatus(id) {
    var objImportacao = this;
    objImportacao.id = id;
    $.ajax({
        url: this.urlVerificar,
        data: { id: id },
        type: 'GET',
        success: function (response) {
            $('.progress-' + objImportacao.id).css('width', response + '%').attr('aria-valuenow', response);
            if (response == 100) {
                clearInterval(objImportacao.myTimeout.filter(function (item) { return item.id == objImportacao.id })[0].fn);
                raizenCoreJs.raizenCRUD.RealizarPesquisa();
            }
        },
        error: function (error) {
            ShowMessage(error);
            clearInterval(objImportacao.myTimeout.filter(function (item) { return item.id == objImportacao.id })[0].fn);
            raizenCoreJs.raizenCRUD.RealizarPesquisa();
        }
    });
}
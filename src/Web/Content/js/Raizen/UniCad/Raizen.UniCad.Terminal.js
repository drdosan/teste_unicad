function Terminal() {
    this.urlAdicionarCliente = "";
}
var raizenCoreJs;
var RaizenCoreJs;
var ShowMessage;
var RaizenHelpers;
var ShowErrorMenssage;
var ShowMessageSucess;

RaizenCoreJs.prototype.Terminal = new Terminal();

Terminal.prototype.PoolChange = function PoolChange() {
    if ($("#Terminal_isPool option:selected").text() == "Sim")
        $("#linhaCliente").fadeIn();    
    else
        $("#linhaCliente").fadeOut();    
}

Terminal.prototype.adicionarCliente = function adicionarCliente() {
    console.log(this.urlAdicionarCliente);
    var incluir = true;
    var lista = $('#clientesTable tbody .IDCliente');
    for (var i = 0; i < lista.length; i++) {
        console.log(lista[i].value);
        if (lista[i].value == $('#Cliente').val()) {
            incluir = false;
        }
    }
    if (incluir) {
        if ($('#Cliente').val() != '') {
            $.ajax({
                type: "POST",
                url: this.urlAdicionarCliente,
                data: { nome: $('#Cliente').val() },
                success: function (partialView) {
                    $('#clientesTable tbody').append(partialView);
                    $("#Cliente").val('');

                },
                error: function (partialView) {
                    ShowMessage(partialView);
                }
            });
        }
    }
    else {
        ShowMessage('Essa empresa já foi adicionado!')
    }
};


Terminal.prototype.removerCliente = function removerCliente(id) {
    $('#linha-' + id).remove();
};
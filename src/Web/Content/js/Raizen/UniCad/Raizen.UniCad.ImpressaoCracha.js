function ImpressaoCracha() {
    this.urlImprimirCracha = "";
    this.urlVisualizarCracha = "";
    this._idPais = 1;
}

var RaizenCoreJs;
var ShowMessage;
var RaizenHelpers;
var ShowErrorMenssage;
var ShowMessageSucess;

var Paises = {
    BRASIL: 1,
    ARGENTINA: 2
};

function GetMessage(msgPortugues, msgEspanhol) {
    switch (RaizenCoreJs.prototype.ImpressaoCracha._idPais) {

        case 1:
            return msgPortugues;

        case 2:
            return msgEspanhol;

        default:
            return msgPortugues;
    }
}




RaizenCoreJs.prototype.ImpressaoCracha = new ImpressaoCracha();
//RaizenCoreJs.prototype.DownLoadCracha = new DownLoadCracha(id);
//RaizenCoreJs.prototype.VisualizarCracha = new VisualizarCracha(id);



ImpressaoCracha.prototype.VisualizarCracha = function VisualizarCracha() {


    if ($('#Cliente').val() != '') {

        event.preventDefault();
        // capture o formulário
        var form = $('#frmPesquisa')[0];
        // crie um FormData {Object}
        var data = new FormData(form);
        // caso queira adicionar um campo extra ao FormData
        // data.append("customfield", "Este é um campo extra para teste");

        // desabilitar o botão de "submit" para evitar multiplos envios até receber uma resposta
        $(".btnCracha").prop("disabled", true);

        // processar
        $.ajax({
            type: "POST",
            enctype: 'multipart/form-data',
            url: this.urlVisualizarCracha,
            data: data,
            processData: false, // impedir que o jQuery tranforma a "data" em querystring
            contentType: false, // desabilitar o cabeçalho "Content-Type"
            cache: false, // desabilitar o "cache"
            timeout: 600000, // definir um tempo limite (opcional)
            // manipular o sucesso da requisição
            success: function (result) {
                if (result.success) {
                    var iframe = $('<iframe>');
                    iframe.attr('src', 'data:application/pdf;base64,' + result.bytes + '#toolbar=0&navpanes=0&scrollbar=0');
                    iframe.attr('style', 'height:320px; width:800px');
                    //iframe.attr('style', 'width:1000px');

                    $('#dvExibirCracha').html('');
                    $('#dvExibirCracha').append(iframe);
                } else
                {
                    ShowMessage(result.responseText);

                }
            },
            // manipular erros da requisição
            error: function (partialView) {
                ShowMessage(partialView);
               
            }
        });

        $(".btnCracha").prop("disabled", false);





    }

}

















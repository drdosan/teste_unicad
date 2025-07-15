function Usuario() {
    this.urlAdicionarCliente = "";
}

var raizenCoreJs;
var RaizenCoreJs;
var ShowMessage;

RaizenCoreJs.prototype.Usuario = new Usuario();

function VerificarSeDeveLimpar() {
    if ($("#ClienteAuto").val() == '') {
        $("#Cliente").val('');
        $("#ClienteNome").val('');
    }
}

Usuario.prototype.adicionarCliente = function adicionarCliente() {
    var incluir = true;
    var lista = $('#clientesTable tbody .IDCliente');
    for (var i = 0; i < lista.length; i++) {
        if (lista[i].value == $('#Cliente').val()) {
            incluir = false;
        }
    }
    if (incluir) {
        if ($('#Cliente').val() != '') {
            $.ajax({
                type: "POST",
                url: this.urlAdicionarCliente,
                data: { idCliente: $('#Cliente').val() },
                success: function (partialView) {
                    $('#clientesTable tbody').append(partialView);
                    $("#ClienteAuto").val('');
                    VerificarSeDeveLimpar();

                },
                error: function (partialView) {
                    ShowMessage(partialView);
                }
            });
        }
    }
    else {
        ShowMessage('Este Cliente já foi adicionado!')
    }
};


Usuario.prototype.removerCliente = function removerCliente(id) {
    $('#linha-' + id).remove();
};

function VerificarSeDeveLimparTransp() {
    if ($("#TransportadoraAuto").val() == '') {
        $("#Transportadora").val('');
        $("#TransportadoraNome").val('');
    }
}

Usuario.prototype.adicionarTransportadora = function adicionarTransportadora() {
    var incluir = true;
    var lista = $('#TransportadorasTable tbody .IDTransportadora');
    for (var i = 0; i < lista.length; i++) {
        if (lista[i].value == $('#Transportadora').val()) {
            incluir = false;
        }
    }
    if (incluir) {
        if ($('#Transportadora').val() != '') {
            $.ajax({
                type: "POST",
                url: this.urlAdicionarTransportadora,
                data: { idTransportadora: $('#Transportadora').val() },
                success: function (partialView) {
                    $('#TransportadorasTable tbody').append(partialView);
                    $("#TransportadoraAuto").val('');
                    VerificarSeDeveLimparTransp();
                },
                error: function (partialView) {
                    ShowMessage(partialView);
                }
            });
        }
    }
    else {
        ShowMessage('Esta transportadora já foi adicionada!')
    }
};


Usuario.prototype.removerTransportadora = function removerTransportadora(id) {
    $('#linha-' + id).remove();
};

Usuario.prototype.AtualizarLinhaNegocio = function AtualizarLinhaNegocio() {
    $('#TransportadorasTable tbody').empty();
    $('#clientesTable tbody').val('');
    $('#Transportadora').val('');
    $('#TransportadoraNome').val('');
    $('#TransportadoraAuto').val('');
    $('#Cliente').val('');
    $('#ClienteNome').val('');
    $('#ClienteAuto').val('');
}

Usuario.prototype.OcultarCampoLinhaNegocio = function OcultarCampoLinhaNegocio() {
    var selecionado = $("#Usuario_IDEmpresa").selectize();
    var selectize = selecionado[0].selectize;
    if ($("#Usuario_Perfil option:selected").text() == 'Cliente EAB') {
        selectize.setValue('1');
        selectize.disable();
    }
    else if ($("#Usuario_Perfil option:selected").text() == 'Cliente ACS' || $("#Usuario_Perfil option:selected").text() == 'Cliente ACS Argentina') {
        selectize.setValue('2');
        selectize.disable();
    }
    else
        selectize.enable();
}

Usuario.prototype.OcultarCamposPerfilOperacao = function OcultarCamposPerfilOperacao() {
    event.preventDefault();
    var select = $("#Usuario_Operacao").selectize();
    var selectize = select[0].selectize;
    selectize.enable();

    if ($('#Usuario_Externo').val() == "false")//interno
    {
        $("#linhaCliente").fadeOut();
        $("#linhaTransportadora").fadeOut();

        $('#Usuario_Login').fadeIn();
        $('#lbl_Usuario_Login').fadeIn();
        $('#lbl_Usuario_Email').text('Email');
    }
    else //externo
    {
        //R1) Ocultar/Exibir os campos Login/Email/Linha transportadora/Linha cliente na tela de "Edição de Usuário"
        if ($("#Usuario_Perfil option:selected").text() != "Cliente ACS" && $("#Usuario_Perfil option:selected").text() != "Cliente ACS Argentina") {
            $('#Usuario_Login').fadeOut();
            $('#lbl_Usuario_Login').fadeOut();
            $('#lbl_Usuario_Email').text('Login(Email)');
        }
        if ($("#Usuario_Perfil option:selected").text() == "Programação") {
            if ($("#Usuario_Operacao").val() == "CIF")//cif
            {
                $("#linhaTransportadora").fadeIn();
                $("#linhaCliente").fadeOut();
            }
            else if ($("#Usuario_Operacao").val() == "FOB")// fob
            {
                $("#linhaCliente").fadeIn();
                $("#linhaTransportadora").fadeOut();
            }
            else if ($("#Usuario_Operacao").val() == "Ambas")// Ambas
            {
                $("#linhaCliente").fadeIn();
                $("#linhaTransportadora").fadeIn();
            }
        }
        else if ($("#Usuario_Perfil option:selected").text().trim() == "Transportadora" || $("#Usuario_Perfil option:selected").text().trim() == "Transportadora Argentina")//Transportadora
        {
            if (selectize.getValue() != 'CIF')
                selectize.setValue('CIF');

            selectize.disable();
            $("#linhaTransportadora").fadeIn();
            $("#linhaCliente").fadeOut();
        }
        else if ($("#Usuario_Perfil option:selected").text().trim() == "Cliente EAB" || $("#Usuario_Perfil option:selected").text().trim() == "Cliente ACS" || $("#Usuario_Perfil option:selected").text().trim() == "Cliente ACS Argentina")//Cliente ACS
        {
            if (selectize.getValue() != 'FOB')
                selectize.setValue('FOB');

            selectize.disable();
            $("#linhaTransportadora").fadeOut();
            $("#linhaCliente").fadeIn();
        }
        else {
            $("#linhaCliente").fadeOut();
            $("#linhaTransportadora").fadeOut();
        }
    }
};


Usuario.prototype.resetarSenha = function resetarSenha(id) {
    $.ajax({
        type: "POST",
        url: this.urlResetarSenha,
        data: { id: id },
        success: function (partialView) {
            ShowMessage(partialView)
        },
        error: function (partialView) {
            ShowMessage(partialView);
        }
    });
}


Usuario.prototype.Salvar = function Salvar() {

    raizenCoreJs.raizenCRUD.ExibirControlesOcultos();
    var dado = $('#frmEdicao').serialize();
    raizenCoreJs.raizenCRUD.OcultarControlesOcultos();

    raizenCoreJs.raizenHelpers.AbrirLoading();

    $.ajax({
        url: this.urlSalvar,
        data: dado,
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
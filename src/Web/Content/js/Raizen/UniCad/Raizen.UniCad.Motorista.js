function Motorista() {
    this.urlBuscarMotorista = "";
    this.urlEditarMotorista = "";
    this.urlAtivarMotorista = "";
    this.urlBloquearMotorista = "";
    this.urlSalvarAtivar = "";
    this.urlSalvarBloquear = "";
    this.urlVisualizarDocumentos = "";
    this.urlCarteirinha = "";
    this.urlGerarPdf = "";
    this.urlAdicionarTerminal = "";
    this.urlTreinamento = "";
    this.urlSalvarTreinamento = "";
    this.urlUploadDocumentos = "";
    this.urlSalvarDocumentos = "";
    this.urlSalvarPermissoes = "";
    this.urlAdicionarCliente = "";
    this.urlExcluirMotorista = "";
    this._idPais = 1;
}

var raizenCoreJs;
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
    switch (RaizenCoreJs.prototype.Motorista._idPais) {

        case 1:
            return msgPortugues;

        case 2:
            return msgEspanhol;

        default:
            return msgPortugues;
    }
}

$(document).ready(function () {

    if ($("#Motorista_CPFEdicao").val() == "") {
        $("#Motorista_CPFEdicao").mask("999.999.999-99");
        $("#Filtro_CPF").mask("999.999.999-99");
    }
    else if ($("#Motorista_DNIEdicao").val() == "") {
        $("#Motorista_DNIEdicao").mask("99.999.999");
        $("#Filtro_DNI").mask("99.999.999");
    }


    $('.btnNext').click(function () {
        $('.nav-tabs > .active').next('li').find('a').trigger('click');
    });

    $('.btnPrevious').click(function () {
        $('.nav-tabs > .active').prev('li').find('a').trigger('click');
    });

    $('#btnEdit').hide();
});


RaizenCoreJs.prototype.Motorista = new Motorista();


Motorista.prototype.adicionarCliente = function adicionarCliente() {
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
                async: false,
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

        ShowMessage(RaizenCoreJs.prototype.Motorista._idPais == 1 ? 'Este Cliente já foi adicionado!' : '¡Este cliente ya se ha agregado!');
    }
};

Motorista.prototype.removerCliente = function removerCliente(id) {
    $('#linha-cliente-' + id).remove();
};

Motorista.prototype.fecharModalMotorista = function fecharModalMotorista() {
    $('#modalcontainerEdicao').modal('hide');
    $('#frmClientes').html('');
    $("#ModalMotorista").html(null);
}


Motorista.prototype.LimparFiltros = function LimparFiltros() {
    $("#Filtro_Nome").val('');
    $("#Filtro_Chamado").val('');

    $("#Filtro_DataInicio").val('');
    $("#Filtro_DataFim").val('');

    if (RaizenCoreJs.prototype.Motorista._idPais == 1) {
        $("#Filtro_CNH").val('');
        $("#Filtro_CPF").val('');
        $("#Filtro_RG").val('');
    }
    else if (RaizenCoreJs.prototype.Motorista._idPais == 2) {
        $("#Filtro_DNI").val('');
        $("#Filtro_Apellido").val('');
    }


    var isLinhaNegocioDisabled = $("#Filtro_IDEmpresa").prop('disabled');
    if (!isLinhaNegocioDisabled || isLinhaNegocioDisabled == undefined)
        $("#Filtro_IDEmpresa").val($("#target option:first").val());

    var isOperacaoDisabled = $("#Filtro_Operacao").prop('disabled');
    if (!isOperacaoDisabled || isOperacaoDisabled == undefined)
        $("#Filtro_Operacao").val($("#target option:first").val());

    $("#Filtro_Ativo").val($("#target option:first").val());

    var isStatusEmAprovacao = $("#statusOriginalEmAprovacao").val();
    //var selectStatus = $("#Filtro_IDStatus");//.selectize();
    //var controlStatus = selectStatus[0].selectize;
    if (isStatusEmAprovacao)
        $('#Filtro_IDStatus').val(1);
    else
        $('#Filtro_IDStatus').val('');

    $("#ClienteAuto").val('');
    $("#TransportadoraAuto").val('');
    $("#Filtro_DataInicio").val('');
    $("#Filtro_DataFim").val('');
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

Motorista.prototype.GerarCarteirinha = function GerarCarteirinha(id) {
    var url = this.urlDowloadCarteirinha;
    $.ajax({
        cache: false,
        url: raizenCoreJs.Motorista.urlGerarPdf,
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


Motorista.prototype.Excluir = function Excluir(id, status) {
    RaizenHelpers.prototype.AbrirConfirm('return raizenCoreJs.Motorista.FuncaoExcluir(' + id + ',' + status + ')', RaizenCoreJs.prototype.Motorista._idPais == 1 ? 'Você deseja realmente excluir esse registro?' : '¿Realmente quieres eliminar este registro?');
}

Motorista.prototype.FuncaoExcluir = function FuncaoExcluir(id, status) {
    raizenCoreJs.raizenHelpers.AbrirLoading();
    var lista = raizenCoreJs.raizenCRUD.urlPesquisa;
    $.ajax({
        url: this.urlExcluirMotorista,
        data: { id: id, status: status },
        type: 'GET',
        success: function (response) {
            if (response.retorno == "") {
                $('#motorista-linha-' + id).remove();
                ShowMessageSucess(RaizenCoreJs.prototype.Motorista._idPais == 1 ? 'Registro excluído com sucesso!' : '¡Registro eliminado con éxito!');
            }
            else {
                ShowErrorMenssage(response.retorno);
            }
            raizenCoreJs.raizenHelpers.FecharLoading();
        }
    });
}


Motorista.prototype.Editar = function Editar(id, aprovar) {
    raizenCoreJs.raizenHelpers.AbrirLoading();


    var dados = {};

    if (RaizenCoreJs.prototype.Motorista._idPais == 1)
        dados = { id: id, aprovar: aprovar };
    else if (RaizenCoreJs.prototype.Motorista._idPais == 2)
        dados = { id: id, aprovar: aprovar, acao: 2 };

    $.ajax({
        url: this.urlEditarMotorista,
        data: dados,
        type: 'GET',
        success: function (response) {
            $("#_Permissao").html(null);
            $("#ModalMotorista").html(null);
            $("#ModalMotorista").html(response);
            $("#modalcontainerEdicao").modal('show');
            $("#containerEdicao").show();
            raizenCoreJs.raizenHelpers.FecharLoading();
        }
    });
}

var ValidarCPF = function ValidarCPF(cpf) {
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

var ValidarDNI = function ValidarDNI(dni) {
    // Validacao com letra de verificação

    // str = dni.toUpperCase().replace(/\s/, '');
    // var dni_letters = "TRWAGMYFPDXBNJZSQVHLCKE";
    // var letter = dni_letters.charAt( parseInt( dni, 10 ) % 23 );
    // return (letter == str.charAt(8));

    //Validacao sem letra de verificação
    dni = dni.replace(".", "").replace(".", "").replace("-", "");
    var ex_regular_dni = /^\d{8}(?:[-\s]\d{4})?$/;
    if (ex_regular_dni.test(dni) == true) {
        return true;
    } else {
        return false;
    }
};


Motorista.prototype.ClonarMotorista = function ClonarMotorista(id, idEmpresa) {
    raizenCoreJs.raizenHelpers.AbrirLoading();
    $.ajax({
        url: this.urlEditarMotorista,
        data: { id: id, Cpf: null, idEmpresa: idEmpresa, Acao: 3, naoAprovado: null },
        type: 'GET',
        success: function (response) {
            $("#ModalMotorista").html(null);
            $("#ModalMotorista").html(response);
            $("#modalcontainerEdicao").modal('show');
            $("#containerEdicao").show();
            raizenCoreJs.raizenHelpers.FecharLoading();
        }
    });
}

Motorista.prototype.EditarMotorista = function EditarMotorista(act) {

    if ($('#LinhaNegocio').val() == '') {
        ShowErrorMenssage(RaizenCoreJs.prototype.Motorista._idPais == 1 ? "Por favor selecione uma linha de negócio" : "Por favor seleccione una línea de negocio.");
        return;
    }

    var idEmpresa = $('#LinhaNegocio').val();
    var naoAprovado = $('#naoAprovado').val();

    if (RaizenCoreJs.prototype.Motorista._idPais == 1) {
        if ($('#Motorista_CPFEdicao').val() == '') {
            ShowErrorMenssage('Por favor digite um CPF');
            return;
        }

        var cpf = $('#Motorista_CPFEdicao').val();
        if (!ValidarCPF(cpf)) {
            ShowErrorMenssage('CPF Inválido');
            return;
        }

        var dados = { id: null, Cpf: cpf, idEmpresa: idEmpresa, Acao: act, naoAprovado: naoAprovado };

    }
    else if (RaizenCoreJs.prototype.Motorista._idPais == 2) {
        if ($('#Motorista_DNIEdicao').val() == '') {
            ShowErrorMenssage('Por favor complete el campo de DNI');
            return;
        }

        var dni = $('#Motorista_DNIEdicao').val();
        if (!ValidarDNI(dni)) {
            ShowErrorMenssage('DNI Inválido');
            return;
        }

        dados = { id: null, Dni: dni, idEmpresa: idEmpresa, Acao: act, naoAprovado: naoAprovado };

    }

    raizenCoreJs.raizenHelpers.AbrirLoading();

    $.ajax({
        url: this.urlEditarMotorista,
        data: dados,
        type: 'GET',
        success: function (response) {
            if (response.status != 'e') {
                $("#ModalMotorista").html(null);
                $("#ModalMotorista").html(response);
                $("#modalcontainerEdicao").modal('show');
                $("#containerEdicao").show();
            }
            else {
                ShowErrorMenssage(response.result);
            }
            raizenCoreJs.raizenHelpers.FecharLoading();
        },
        error: function (response) {
            ShowMessage(response);
            raizenCoreJs.raizenHelpers.FecharLoading();
        }
    });
}

Motorista.prototype.UploadDocumentos = function UploadDocumentos(id) {
    raizenCoreJs.raizenHelpers.AbrirLoading();
    $.ajax({
        type: "GET",
        url: this.urlUploadDocumentos,
        data: { id: id },
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

Motorista.prototype.VisualizarDocumentos = function VisualizarDocumentos(id) {
    raizenCoreJs.raizenHelpers.AbrirLoading();
    $.ajax({
        type: "GET",
        url: this.urlVisualizarDocumentos,
        data: {
            id: id,
            idPais: RaizenCoreJs.prototype.Motorista._idPais
        },
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

Motorista.prototype.Ativar = function Ativar(id) {
    raizenCoreJs.raizenHelpers.AbrirLoading();
    $.ajax({
        type: "GET",
        url: this.urlAtivarMotorista,
        data: { id: id },
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

Motorista.prototype.Carteirinha = function Carteirinha(id) {
    raizenCoreJs.raizenHelpers.AbrirLoading();
    $.ajax({
        type: "GET",
        url: this.urlCarteirinha,
        data: { id: id },
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

Motorista.prototype.Bloquear = function Bloquear(id) {
    raizenCoreJs.raizenHelpers.AbrirLoading();
    $.ajax({
        type: "GET",
        url: this.urlBloquearMotorista,
        data: { id: id },
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

Motorista.prototype.Treinamento = function Treinamento(id) {
    raizenCoreJs.raizenHelpers.AbrirLoading();
    $.ajax({
        type: "GET",
        url: this.urlTreinamento,
        data: { id: id },
        success: function (retorno) {
            $('#ModalMotorista').html(null);
            $('#ModalMotorista').html(retorno);
            $('#modalcontainerEdicao').modal('show');
            $('#containerEdicaoTreinamento').show();
            raizenCoreJs.raizenHelpers.FecharLoading();
        },
        error: function (partialView) {
            ShowMessage(partialView);
            raizenCoreJs.raizenHelpers.FecharLoading();
        }
    });
}

Motorista.prototype.SalvarAtivar = function SalvarAtivar(id) {

    if ($('#Justificativa').val() == '') {
        ShowMessage(RaizenCoreJs.prototype.Motorista._idPais == 1 ? "Favor informar a justificativa!" : "¡Por favor informe la justificación!");
        return;
    }

    raizenCoreJs.raizenHelpers.AbrirLoading();
    $.ajax({
        type: "POST",
        url: this.urlSalvarAtivar,
        data: { id: id, justificativa: $('#Justificativa').val(), ativo: $('input[name=Ativo]:checked').val() },
        success: function (retorno) {
            if (retorno == (RaizenCoreJs.prototype.Motorista._idPais == 1 ? "Gravado com sucesso!" : "¡Grabado con éxito!")) {
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


Motorista.prototype.Salvar = function Salvar(id, status, comRessalvas) {

    raizenCoreJs.raizenHelpers.AbrirLoading();
    var lista = this.urlMotorista;
    $.ajax({
        url: this.urlAprovar,
        data: { id: id, status: status, comRessalvas: comRessalvas },
        type: 'POST',
        success: function (response) {
            $("#containerEdicao").html(response);

            if (raizenCoreJs.raizenMensagens.ContemErros == "N") {
                ShowMessageSucess(raizenCoreJs.raizenMensagens.MensagemOperacao, "window.location.href = '" + lista + "'");
            }
            else {
                raizenCoreJs.raizenMensagens.ExibirMensagemOperacao();
            }
            raizenCoreJs.raizenHelpers.FecharLoading();
        }
    });
};

Motorista.prototype.SalvarDocumentos = function SalvarDocumentos(id) {
    var dados = $('#frmDocumentos').serialize();
    raizenCoreJs.raizenHelpers.AbrirLoading();
    $.ajax({
        type: "POST",
        url: this.urlSalvarDocumentos,
        data: dados,
        success: function (retorno) {
            if (retorno == "S") {
                ShowMessageSucess(RaizenCoreJs.prototype.Motorista._idPais == 1 ? 'Gravado com sucesso' : '¡Grabado con éxito!');
                raizenCoreJs.raizenHelpers.FecharModal('containerVisualizar');
            }
            else {
                ShowErrorMenssage(retorno);
            }
            raizenCoreJs.raizenHelpers.FecharLoading();
        },
        error: function (partialView) {
            ShowErrorMenssage(partialView);
            raizenCoreJs.raizenHelpers.FecharLoading();
        }
    });
}

Motorista.prototype.SalvarBloquear = function SalvarBloquear(id) {

    debugger;

    if ($('#Justificativa').val() == '') {
        ShowMessage(GetMessage("Favor informar a justificativa!", "¡Por favor informe la justificación!"));
        return;
    }

    raizenCoreJs.raizenHelpers.AbrirLoading();
    $.ajax({
        type: "POST",
        url: this.urlSalvarBloquear,
        data: { id: id, justificativa: $('#Justificativa').val(), bloqueio: $('input[name=Bloqueado]:checked').val() },
        success: function (retorno) {
            if (retorno == GetMessage('Gravado com sucesso!', '¡Grabado con éxito!')) {
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
Motorista.prototype.buscarMotorista = function buscarMotorista(e, isOrigemLinhaNegocio) {

    var ctrlDown = false,
        ctrlKey = 17,
        cmdKey = 91,
        vKey = 86,
        bkpKey = 8,
        cKey = 67;

    if (isOrigemLinhaNegocio || ((e.key >= 0 && e.key <= 9) || (e.keyCode == ctrlKey || e.keyCode == cmdKey || e.keyCode == bkpKey)) || (RaizenCoreJs.prototype.Motorista._idPais == 1 ? true : e.keyCode >= 65 && e.keyCode <= 90 || e.keyCode >= 97 && e.keyCode <= 122)) {
        $('#validado').val('0');
        if ($('#LinhaNegocio').val() == '') {
            ShowMessage(RaizenCoreJs.prototype.Motorista._idPais == 1 ? 'Por favor selecione uma linha de negócio' : 'Por favor seleccione una línea de negocio.');
            $('#LinhaNegocio').focus();
            return;
        }


        var validaTamanho =
            RaizenCoreJs.prototype.Motorista._idPais == 1
                ? $('#Motorista_CPFEdicao').val().replace(".", "").replace(".", "").replace("-", "").length > 10
                : $('#Motorista_DNIEdicao').val().replace(".", "").replace(".", "").replace("-", "").length > 7;


        if (validaTamanho) {

            if (RaizenCoreJs.prototype.Motorista._idPais == 1) {
                if (!ValidarCPF($('#Motorista_CPFEdicao').val())) {
                    ShowErrorMenssage("CPF Inválido");
                    return;
                }

                var dados = { cpf: $('#Motorista_CPFEdicao').val(), idEmpresa: $('#LinhaNegocio').val() };

            }
            else if (RaizenCoreJs.prototype.Motorista._idPais == 2) {
                if (!ValidarDNI($('#Motorista_DNIEdicao').val())) {
                    ShowErrorMenssage("DNI Inválido");
                    return;
                }


                dados = { dni: $('#Motorista_DNIEdicao').val(), idEmpresa: $('#LinhaNegocio').val() };
            }

            $('#validado').val('1');

            $('#btnEdit').prop('enabled', false);
            $('#btnNovo').prop('enabled', false);
            $('#btnClone').prop('enabled', false);
            $.ajax({
                type: "GET",
                url: this.urlBuscarMotorista,
                data: dados,
                success: function (retorno) {

                    $('#checkJaExiste').remove();
                    $('#checkNaoExiste').remove();
                    if (retorno.retorno == 'aprovado') {

                        if (RaizenCoreJs.prototype.Motorista._idPais == 1)
                            $("#lbl_Motorista_CPFEdicao").after("<span id='checkJaExiste' title='Motorista já cadastrado' class='fa fa-check' style='color:#36940d;padding-left: 3px;'></span>");
                        else if (RaizenCoreJs.prototype.Motorista._idPais == 2)
                            $("#lbl_Motorista_DNIEdicao").after("<span id='checkJaExiste' title='Conductor ya registrado' class='fa fa-check' style='color:#36940d;padding-left: 3px;'></span>");

                        $('#btnEdit').show();
                        $('#btnClone').hide();
                        $('#btnNovo').hide();
                        ShowErrorMenssage(RaizenCoreJs.prototype.Motorista._idPais == 1 ? "Este motorista já está cadastrado! Caso queria conferir, clicar no ícone 'Lápis'" : "¡Este conductor ya está registrado! Para ver las informaciones haga click en el icono Lápiz");
                        $('#lblNome').text(retorno.nome);
                        $('#naoAprovado').val('0');
                    } else if (retorno.retorno == 'bloqueado') {
                        if (RaizenCoreJs.prototype.Motorista._idPais == 1)
                            $("#lbl_Motorista_CPFEdicao").after("<span id='checkJaExiste' title='Motorista já cadastrado - bloqueado' class='fa fa-check' style='color:#36940d;padding-left: 3px;'></span>");
                        else if (RaizenCoreJs.prototype.Motorista._idPais == 2)
                            $("#lbl_Motorista_DNIEdicao").after("<span id='checkJaExiste' title='Conductor ya registrado - bloqueado' class='fa fa-check' style='color:#36940d;padding-left: 3px;'></span>");

                        $('#btnEdit').hide();
                        $('#btnClone').hide();
                        $('#btnNovo').hide();
                        $('#lblNome').text(retorno.nome);
                        ShowErrorMenssage(RaizenCoreJs.prototype.Motorista._idPais == 1 ? "Esse motorista está bloqueado, não é permitido incluir uma solicitação até que o mesmo seja desbloqueado." : "Este conductor está bloqueado, no está permitido registrar una solicitud hasta que sea desbloqueado");
                        $('#naoAprovado').val('1');
                    } else if (retorno.retorno == 'naoAprovado') {

                        if (RaizenCoreJs.prototype.Motorista._idPais == 1)
                            $("#lbl_Motorista_CPFEdicao").after("<span id='checkJaExiste' title='Motorista já cadastrado - não aprovado' class='fa fa-check' style='color:#36940d;padding-left: 3px;'></span>");
                        else if (RaizenCoreJs.prototype.Motorista._idPais == 2)
                            $("#lbl_Motorista_DNIEdicao").after("<span id='checkJaExiste' title='Conductor ya registrado - no aprobado' class='fa fa-check' style='color:#36940d;padding-left: 3px;'></span>");

                        $('#btnEdit').hide();
                        $('#btnClone').hide();
                        $('#btnNovo').hide();
                        $('#lblNome').text(retorno.nome);
                        ShowErrorMenssage(RaizenCoreJs.prototype.Motorista._idPais == 1 ? "Esse motorista está em aprovação, não é permitido incluir uma solicitação até que o mesmo seja aprovado." : "Este conductor está en aprobación, no está permitido registrar una solicitud hasta que sea desbloqueado");
                        $('#naoAprovado').val('1');
                    } else if (retorno.retorno == 'reprovado' && retorno.isFob == true) {
                        // Motorista FOB Reprovado: permite edição.

                        if (RaizenCoreJs.prototype.Motorista._idPais == 1)
                            $("#lbl_Motorista_CPFEdicao").after("<span id='checkJaExiste' title='Motorista já cadastrado - reprovado' class='fa fa-check' style='color:#36940d;padding-left: 3px;'></span>");
                        else if (RaizenCoreJs.prototype.Motorista._idPais == 2)
                            $("#lbl_Motorista_DNIEdicao").after("<span id='checkJaExiste' title='Controlador ya registrado - fallido' class='fa fa-check' style='color:#36940d;padding-left: 3px;'></span>");

                        $('#btnEdit').show();
                        $('#btnClone').hide();
                        $('#btnNovo').hide();
                        $('#lblNome').text(retorno.nome);
                        ShowErrorMenssage(RaizenCoreJs.prototype.Motorista._idPais == 1 ? "Esse motorista está reprovado, Caso queria conferir, clicar no ícone 'Lápis'." : "Este conductor está reprobado, para ver las informaciones haga click en el icono Lápiz");
                        $('#naoAprovado').val('1');
                    } else if (retorno.retorno == 'reprovado' && retorno.isFob == false) {

                        if (RaizenCoreJs.prototype.Motorista._idPais == 1)
                            $("#lbl_Motorista_CPFEdicao").after("<span id='checkJaExiste' title='Motorista já cadastrado - reprovado' class='fa fa-check' style='color:#36940d;padding-left: 3px;'></span>");
                        else if (RaizenCoreJs.prototype.Motorista._idPais == 2)
                            $("#lbl_Motorista_DNIEdicao").after("<span id='checkJaExiste' title='Controlador ya registrado - fallido' class='fa fa-check' style='color:#36940d;padding-left: 3px;'></span>");

                        $('#btnEdit').hide();
                        $('#btnClone').hide();
                        $('#btnNovo').hide();
                        $('#lblNome').text(retorno.nome);
                        ShowErrorMenssage(RaizenCoreJs.prototype.Motorista._idPais == 1 ? "Esse motorista está reprovado, não é permitido incluir uma solicitação até que o mesmo seja editado." : "Este conductor está reprobado, no está permitido registrar una solicitud hasta que sea corregido");
                        $('#naoAprovado').val('1');
                    } else if (retorno.retorno == 'jaExisteOutraEmpresa') {

                        if (RaizenCoreJs.prototype.Motorista._idPais == 1)
                            $("#lbl_Motorista_CPFEdicao").after("<span id='checkJaExiste' title='Motorista já cadastrado para outra linha de negócios' class='fa fa-check' style='color:#36940d;padding-left: 3px;'></span>");
                        else if (RaizenCoreJs.prototype.Motorista._idPais == 2)
                            $("#lbl_Motorista_DNIEdicao").after("<span id='checkJaExiste' title='Conductor ya registrado para otra línea de negocios' class='fa fa-check' style='color:#36940d;padding-left: 3px;'></span>");

                        $('#btnClone').show();
                        $('#btnNovo').hide();
                        $('#btnEdit').hide();
                        $('#btnNovo').hide();
                        $('#lblNome').text(retorno.nome);
                        ShowErrorMenssage(RaizenCoreJs.prototype.Motorista._idPais == 1 ? "Motorista já cadastrado para outra linha de negócios! Clique em '+' para incluí-lo para esta linha de negócio." : "Conductor ya está registrado en otra línea de negocio. Haga click en ‘+’ para añadirlo en esta línea de negocio");
                        $('#naoAprovado').val('1');
                    } else {

                        if (RaizenCoreJs.prototype.Motorista._idPais == 1)
                            $("#lbl_Motorista_CPFEdicao").after("<span id='checkNaoExiste' title='Motorista não cadastrado' class='fa fa-exclamation' style='color:#ff0000;padding-left: 3px;'></span>");
                        else if (RaizenCoreJs.prototype.Motorista._idPais == 2)
                            $("#lbl_Motorista_DNIEdicao").after("<span id='checkNaoExiste' title='Conductor no registrado' class='fa fa-exclamation' style='color:#ff0000;padding-left: 3px;'></span>");

                        $('#btnNovo').show();
                        $('#btnClone').hide();
                        $('#btnEdit').hide();
                        $('#lblNome').text('');
                        $('#naoAprovado').val('0');
                        ShowMessage(RaizenCoreJs.prototype.Motorista._idPais == 1 ? 'Motorista não cadastrado no Sistema. Selecione + para incluir.' : 'Conductor no registrado en el Sistema. Haga click en ‘+’ para registrarlo');
                    }
                    raizenCoreJs.raizenHelpers.FecharLoading();
                },
                error: function (retorno) {
                    ShowMessage(retorno);
                    raizenCoreJs.raizenHelpers.FecharLoading();
                }
            });
        }
        else {
            $('#btnNovo').hide();
            $('#btnClone').hide();
            $('#btnEdit').hide();
            $('#btnNovo').show();
            $('#checkJaExiste').remove();
            $('#checkNaoExiste').remove();
            $('#lblNome').text('');
        }

        $('#btnClone').prop('disabled', false);
        $('#btnEdit').prop('disabled', false);
        $('#btnNovo').prop('disabled', false);
        raizenCoreJs.raizenHelpers.FecharLoading();
    }
};


Motorista.prototype.removerTerminal = function removerTerminal(id) {
    $('#linha-terminal-' + id).remove();
};


Motorista.prototype.adicionarTerminal = function adicionarTerminal() {
    var incluir = true;
    var lista = $('#terminaisTable tbody .ID');
    for (var i = 0; i < lista.length; i++) {
        if (lista[i].value == $('#Terminal').val()) {
            incluir = false;
        }
    }
    if (incluir) {
        if ($('#Terminal').val() != '') {
            $.ajax({
                type: "POST",
                async: false,
                url: this.urlAdicionarTerminal,
                data: { idTerminal: $('#Terminal').val() },
                success: function (partialView) {
                    $('#terminaisTable tbody').append(partialView);
                    $("#TerminalAuto").val('');
                    VerificarSeDeveLimpar();

                },
                error: function (partialView) {
                    ShowMessage(partialView);
                }
            });
        }
    }
    else {
        ShowMessage(RaizenCoreJs.prototype.Motorista._idPais == 1 ? 'Este Terminal já foi adicionado!' : '¡Este terminal ya ha sido agregado!');
    }
};


Motorista.prototype.SalvarTreinamento = function SalvarTreinamento() {

    if ((($("[name='TreinamentoView.TreinamentoAprovado']:checked").val() == undefined || $("[name='TreinamentoView.TreinamentoAprovado']:checked").val() == '')) && ($('#TreinamentoView_dataValidade').val() != '' || $('#Anexo-34').val() != '')) {
        ShowMessage(RaizenCoreJs.prototype.Motorista._idPais == 1 ? 'Informe se o treinamento foi Aprovado/Reprovado!' : '¡Informe si la capacitación ha sido aprobada / reprobada!');
        return;
    }
    //else 
    if ($("[name='TreinamentoView.TreinamentoAprovado']:checked").val() == 'true' && ($('#TreinamentoView_dataValidade').val() == '' || $('#Anexo-34').val() == '')) {
        ShowMessage(RaizenCoreJs.prototype.Motorista._idPais == 1 ? 'Inclua um anexo, e a Data de Vencimento do Treinamento!' : '¡Incluya un archivo adjunto y la fecha de vencimiento del entrenamiento!');
        return;
    }
    else if ($("[name='TreinamentoView.TreinamentoAprovado']:checked").val() == 'false' && $('#TreinamentoView_Justificativa').val() == '') {
        ShowMessage(RaizenCoreJs.prototype.Motorista._idPais == 1 ? 'Informe a justificativa!' : 'Ingrese la justificación!');
        return;
    }

    var dados = $('#frmTreinamento').serialize();
    raizenCoreJs.raizenHelpers.AbrirLoading();
    $.ajax({
        type: "POST",
        url: this.urlSalvarTreinamento,
        data: dados,
        success: function (response) {
            $("#ModalMotorista").html(response);
            if (raizenCoreJs.raizenMensagens.ContemErros == "N") {
                raizenCoreJs.raizenHelpers.FecharModal('containerVisualizar');
                raizenCoreJs.raizenCRUD.RealizarPesquisa();
                ShowMessageSucess(raizenCoreJs.raizenMensagens.MensagemOperacao);
            }
            else {
                raizenCoreJs.raizenMensagens.ExibirMensagemOperacao();
            }
            raizenCoreJs.raizenHelpers.FecharLoading();
        }
    });
};

function VerificarSeDeveLimpar() {
    if ($("#TerminalAuto").val() == '') {
        $("#Terminal").val('');
        $("#TerminalNome").val('');
    }

    if ($("#TransportadoraAuto").val() == '') {
        $("#Transportadora").val('');
        $("#TransportadoraNome").val('');
    }
};

Motorista.prototype.SalvarPermissao = function SalvarPermissao() {

    var lista = "";
    var operacao = $('#Motorista_Operacao').val();
    if (operacao == 'CIF') {
        var transp = $('#Motorista_IDTransportadora').val();
        if (transp == '') {
            ShowErrorMenssage(RaizenCoreJs.prototype.Motorista._idPais == 1 ? 'É necessário associar uma transportadora.' : 'Se necesita relacionar una transportadora');
            $('#Motorista_Transportadora').focus();
            return;
        }
    }
    //Cliente
    else if (operacao == "FOB") {
        lista = $('#clientesTable tbody .IDCliente');
        if (lista.length == 0) {
            ShowErrorMenssage(RaizenCoreJs.prototype.Motorista._idPais == 1 ? 'É necessário associar um cliente.' : 'Se necesita relacionar un cliente');
            $('#ClienteAuto').focus();
            return;
        }
    }
    var dados = $('#frmEdicao').serialize();
    lista = this.urlMotorista;
    raizenCoreJs.raizenHelpers.AbrirLoading();
    $.ajax({
        url: this.urlSalvarPermissoes,
        data: dados,
        type: 'POST',
        success: function (response) {
            $("#containerEdicao").html(response);

            if (raizenCoreJs.raizenMensagens.ContemErros == "N") {
                ShowMessageSucess(raizenCoreJs.raizenMensagens.MensagemOperacao, "window.location.href = '" + lista + "'");
            }
            else {
                $('#retorno').val('0');
                raizenCoreJs.raizenMensagens.ExibirMensagemOperacao();
            }
            raizenCoreJs.raizenHelpers.FecharLoading();
        }
    });
}

Motorista.prototype.Exportar = function Exportar() {
    var dados = $('#frmPesquisa').serialize();
    raizenCoreJs.raizenHelpers.AbrirLoading();
    window.location = raizenCoreJs.raizenCRUD.urlExportar + "?data=" + dados;
    raizenCoreJs.raizenHelpers.FecharLoading();
};
function EdicaoMotorista() {
    var urlListarDocumentos = '';
}

var raizenCoreJs;
var RaizenCoreJs;
var ShowMessage;
var RaizenHelpers;
var ShowMessageSucess;
var VerificarSeDeveLimpar;
RaizenCoreJs.prototype.EdicaoMotorista = new EdicaoMotorista();

$(document).ready(function () {
    var phone = $("#Motorista_Telefone").val();
    if (phone != '' && phone != null) {
        phone = phone.replace(/\D/g, '');
        var tamanho = phone.length;
        if (tamanho < 11) {
            $("#Motorista_Telefone").mask("(99) 9999-9999");
        } else {
            $("#Motorista_Telefone").mask("(99) 99999-9999");
        }
    };
    $('#Motorista_PIS').unmask();
    $("#Motorista_PIS").mask("99999999999");
    $('#Motorista_MotoristaBrasil_Nascimento').unmask();
    $('#Motorista_MotoristaBrasil_Nascimento').mask('99/99/9999');


    $("#Motorista_Telefone").keydown(function (key) {
        if (key.key == "0" ||
            key.key == "1" ||
            key.key == "2" ||
            key.key == "3" ||
            key.key == "4" ||
            key.key == "5" ||
            key.key == "6" ||
            key.key == "7" ||
            key.key == "8" ||
            key.key == "9"
        ) {
            mascaraTelefone();
        }
    });

    //$("#Motorista_Telefone").change(function () {
    //    {
    //        mascaraTelefone();
    //    }
    //});

    $('#tabMotorista input[id=Motorista_MotoristaBrasil_CPF]').mask('999.999.999-99')
    $('.btnNext').click(function () {
        $('.nav-tabs > .active').next('li').find('a').trigger('click');
    });

    $('.btnPrevious').click(function () {
        $('.nav-tabs > .active').prev('li').find('a').trigger('click');
    });

    //$("#Motorista_CNH").ForceNumericOnly();

});

function mascaraTelefone() {
    try {
        $("#Motorista_Telefone").unmask();
        var phone = $("#Motorista_Telefone").val().replace(/\D/g, '');
    } catch (e) { alert(e); }

    var tamanho = phone.length;
    if (tamanho < 10) {
        $("#Motorista_Telefone").mask("(99) 9999-9999");
    } else {
        $("#Motorista_Telefone").mask("(99) 99999-9999");
    }
}
EdicaoMotorista.prototype.carregarDocumentos = function carregarDocumentos(aprovar) {
    raizenCoreJs.raizenHelpers.AbrirLoading();
    var idMotorista = $('#Motorista_ID').val();
    var action = $('#IdOperacaoCRUDEdit').val();

    $.ajax({
        type: "POST",
        url: this.urlListarDocumentos,
        data: { IDEmpresa: $('#Motorista_IDEmpresa').val(), Operacao: $('#Motorista_Operacao').val(), Aprovar: aprovar, IDMotorista: idMotorista, Acao: action },
        success: function (partialView) {
            $('#documentos').html(partialView);
        },
        error: function (partialView) {
            ShowMessage(partialView);
        }
    });
    raizenCoreJs.raizenHelpers.FecharLoading();
}


EdicaoMotorista.prototype.Reprovar = function Reprovar() {
    if ($('#Motorista_Justificativa').val() == '') {
        ShowMessage('Informe uma justificativa!');
        $('#Motorista_Justificativa').focus();
    }
    else {
        $("#Reprovar").val('true');
        RaizenHelpers.prototype.AbrirConfirm('return raizenCoreJs.EdicaoMotorista.Salvar(4, false)');
    }
}



EdicaoMotorista.prototype.fecharModalMotorista = function fecharModalMotorista() {
    $('#modalcontainerEdicao').modal('hide');
    $('#frmClientes').html('');
    $("#ModalMotorista").html(null);
}

EdicaoMotorista.prototype.Salvar = function Salvar(status, comRessalvas, pais) {

    if ((status == 4 || (status == 2 && comRessalvas)) && $('#Motorista_Justificativa').val() == '') {
        ShowMessage("Favor informar a justificativa!");
        return;
    }

    raizenCoreJs.raizenCRUD.ExibirControlesOcultos();
    $('#idTransportadora').val($('#Motorista_IDTransportadora').val());
    $('#frmClientes').html($('#frmEdicao').html());
    var dados = $('#frmEdicao').serialize();
    raizenCoreJs.raizenCRUD.OcultarControlesOcultos();
    raizenCoreJs.raizenHelpers.AbrirLoading();

    $.ajax({
        url: this.urlSalvar,
        data: dados + '&status=' + status + '&comRessalvas=' + comRessalvas + '&idPais=1',
        type: 'POST',
        success: function (response) {
            $("#containerEdicaoMotorista").html(response);

            
            for (var i = 0; i < raizenCoreJs.raizenMensagens.ValidacoesModel.length; i++) {
                if (raizenCoreJs.raizenMensagens.ValidacoesModel[i].IdControle == 'lbl_Motorista_Email') {
                    $('#Motorista_Email').addClass("input-validation-error ");
                }

                if (raizenCoreJs.raizenMensagens.ValidacoesModel[i].IdControle == 'lbl_Motorista_Telefone') {
                    $('#Motorista_Telefone').addClass("input-validation-error ");
                }
            }


            if (raizenCoreJs.raizenMensagens.ContemErros == "N") {

                if (raizenCoreJs.raizenMensagens.MensagemOperacao == 'ALTERACAO_EMAIL_TELEFONE') {
                    ShowMessageSucess('Telefone e ou Email alterados com sucesso.');
                }
                else
                    if (raizenCoreJs.raizenMensagens.MensagemOperacao == 'APROVACAO_AUTOMATICA') {
                        ShowMessageSucess('Réplica realizada com sucesso!<br/><br/> O motorista está disponível no Csonline para ingresso dos pedidos.<br/><br/>  Em caso de dúvidas, entrar em contato com a nossa Central de Atendimento através do telefone 0300 789 8282 / 0800 789 8282.');
                    }
                    else if (status === 1) {
                        ShowMessageSucess("O cadastro do motorista foi encaminhado para análise e será aprovado ou rejeitado em até 24 horas.<br/><br/>" +
                            "Em caso de dúvidas, entrar em contato com a nossa Central de Atendimento através do telefone 0300 789 8282 / 0800 789 8282.");
                    } else {
                        ShowMessageSucess(raizenCoreJs.raizenMensagens.MensagemOperacao);
                    }

                $('#modalcontainerEdicao').modal('hide');
                $('#frmClientes').html('');
                $('#Motorista_CPFEdicao').val('');
                $('#btnEdit').hide();
                $('#btnClone').hide();
                $('#btnNovo').show();
                $('#checkJaExiste').remove();
                $('#checkNaoExiste').remove();
                $('#lblNome').text('');
                $('#validado').val('0');
                $('#idTransportadora').val('');
                raizenCoreJs.raizenCRUD.RealizarPesquisa();
            }
            else {
                $('#retorno').val('0');
                raizenCoreJs.raizenMensagens.ExibirMensagemOperacao();
            }
            raizenCoreJs.raizenHelpers.FecharLoading();
        }
    });
};

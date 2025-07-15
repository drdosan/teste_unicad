/// Classe responsável em conter os programas java Scripts que implementam travas, eventos, funções validadoras aos controles indicados no Custom Helpers do Framework
/// Qualquer customização para CRUD's especificos deve ser tratado em um JS a parte
/// JavaScript Design Patterns (Prototype Pattern) - ver Learning JavaScript Design Patterns A book by Addy Osmani Volume 1.5.2

function RaizenEnginePolices() {
    this.AcoesController = "";
};


RaizenEnginePolices.prototype.RedirectErroPadrao = function RedirectErroPadrao(url) {

    window.document.location.href = url;
};

RaizenEnginePolices.prototype.InicializarBloqueadores = function InicializarBloqueadores() {

    $('*[bloqueador-controles="true"]').each(function () {
        $(this).focus(function () {

            raizenCoreJs.raizenEnginePolices.AdicionarBloqueios();

        });
    });
};

RaizenEnginePolices.prototype.AdicionarBloqueios = function AdicionarBloqueios() {
    $('*[bloquear-controle="true"]').each(function () {
        $(this).prop('disabled', 'disabled');
    });
};

RaizenEnginePolices.prototype.RemoverBloqueadores = function RemoverBloqueadores() {
    $('*[bloquear-controle="true"]').each(function () {
        $(this).removeAttr("disabled");
    });
};

RaizenEnginePolices.prototype.AplicarPermissaoAcoes = function AplicarPermissaoAcoes(container) {

    var idBusca = "#" + container + " *[AcaoPermissao]";

    $(idBusca).each(function () {

        var retorno = raizenCoreJs.raizenEnginePolices.ValidarPermissaoAcao($(this).attr("AcaoPermissao"));
        if (retorno == "N") {
            $(this).prop('disabled', 'disabled');
        }
    });

};

RaizenEnginePolices.prototype.ValidarPermissaoAcao = function ValidarPermissaoAcao(tag) {

    var retorno = "N";
    $.each(raizenCoreJs.raizenEnginePolices.AcoesController, function () {
        if (this.TagAcao == tag) {
            retorno = "S";
            return;
        }
    });

    return retorno;
}





RaizenCoreJs.prototype.raizenEnginePolices = new RaizenEnginePolices();
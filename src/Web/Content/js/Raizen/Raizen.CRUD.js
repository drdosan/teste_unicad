///Classe Js reponsável por interagir com os controles, atributos e operações que envolvam os formulários de CRUD
///definidos como padrões pela Raízen. 
///Qualquer customização para CRUD's especificos deve ser tratado em um JS a parte
/// JavaScript Design Patterns (Prototype Pattern) - ver Learning JavaScript Design Patterns A book by Addy Osmani Volume 1.5.2
function RaizenCRUD() {
  this.urlPesquisa = "";
  this.urlSalvar = "";
  this.urlExcluir = "";
  this.urlNovo = "";

  this.controlesOcultosValidacao = "";
};

RaizenCRUD.prototype.RealizarPesquisa = function RealizarPesquisa() {
    
    raizenCoreJs.raizenHelpers.AbrirLoading();
    var paginador = $('#paginador').html();
    if (paginador != undefined) {
        $('#PaginadorDados_Status').val(raizenCoreJs.raizenDominios.estadoPaginacao.RenovandoConsulta);
    }

    $('#' + raizenCoreJs.raizenHelpers.IdFormulario).find('#IdOperacaoCRUD').val(raizenCoreJs.raizenDominios.operacaoCrud.List);

    var dados = $('#frmPesquisa').serialize();

    $.ajax({
        url: this.urlPesquisa,
        data: dados,
        contentType: 'application/json',
        type: 'GET',
        success: function (response) {

            $("#containerPesquisa").html(null);
            $("#containerPesquisa").html(response);
            raizenCoreJs.raizenHelpers.FecharLoading();
        }
    });
};

RaizenCRUD.prototype.RealizarPesquisaPaginando = function RealizarPesquisaPaginando() {

    var dados = $('#frmPesquisa').serialize();

    $.ajax({
        url: this.urlPesquisa,
        data: dados,
        contentType: 'application/json',
        type: 'GET',
        success: function (response) {
            $("#containerPesquisa").html(null);
            $("#containerPesquisa").html(response);
        }
    });
};

RaizenCRUD.prototype.EditarRegistro = function EditarRegistro(IdRegistro, url, Bloqueado) {

    raizenCoreJs.raizenHelpers.IdFormulario = "frmEdicao";
    $('#frmEdicao').find("#ChavePrimaria").val(IdRegistro);

    $.get(url,
    {
        "Id": IdRegistro,
        "Bloqueado": Bloqueado
    },
    function (data) {
        $("#containerEdicao").html(null);
        $("#containerEdicao").html(data);
        $("#ToolBoxCrudPesquisa").show();
        $("#modalcontainerEdicao").modal('show');
        $("#containerEdicao").show();
    });
};

RaizenCRUD.prototype.Voltar = function Voltar() {
    $("#containerEdicao").hide();
    $("#modalcontainerEdicao").modal('hide');
    raizenCoreJs.raizenCRUD.RealizarPesquisaPaginando();
};

RaizenCRUD.prototype.Salvar = function Salvar() {

    raizenCoreJs.raizenCRUD.ExibirControlesOcultos();
    var dados = $('#frmEdicao').serialize();
    raizenCoreJs.raizenCRUD.OcultarControlesOcultos();

    raizenCoreJs.raizenHelpers.AbrirLoading();

    $.ajax({
        url: this.urlSalvar,
        data: dados,
        contentType: 'application/json',
        type: 'GET',
        success: function (response) {
            $("#containerEdicao").html(response);
            if (raizenCoreJs.raizenMensagens.ContemErros == "N") {
                raizenCoreJs.raizenCRUD.Voltar();
            }
            raizenCoreJs.raizenHelpers.FecharLoading();
        }
    });
};

RaizenCRUD.prototype.OcultarControlesOcultos = function OcultarControlesOcultos() {

    if (this.controlesOcultosValidacao.length > 0) {
        $.each(this.controlesOcultosValidacao, function (i, item) {
            if (item.propriedade == 'readonly') {
                $("#" + item.IdControle).attr("readonly", "readonly");
            }

            if (item.propriedade == 'disabled') {
                $("#" + item.IdControle).attr("disabled", "disabled");
            }

            if (item.propriedade == 'hide') {
                $("#" + item.IdControle).hide();
            }   
        });
    }
};

RaizenCRUD.prototype.ExibirControlesOcultos = function ExibirControlesOcultos() {

    if (this.controlesOcultosValidacao.length > 0) {
        $.each(this.controlesOcultosValidacao, function (i, item) {

            if (item.propriedade == 'readonly') {
                $("#" + item.IdControle).removeAttr("readonly");
            }

            if (item.propriedade == 'disabled') {
                $("#" + item.IdControle).removeAttr("disabled");
            }

            if (item.propriedade == 'hide') {
                $("#" + item.IdControle).show();
            }

        });
    }
};

RaizenCRUD.prototype.ExcluirRegistro = function ExcluirRegistro(IdRegistro) {
    RaizenHelpers.prototype.AbrirDelete('return raizenCoreJs.raizenCRUD.Excluir(' + IdRegistro + ')','Deseja realmente excluir esse registro?');
};

RaizenCRUD.prototype.Excluir = function Excluir(IdRegistro) {    
    $.get(this.urlExcluir,
    {
        "Id": IdRegistro
    },
    function (data) {
        if (data == "False") {
            ShowMessage("Este registro está sendo utilizado por outra entidade!");
            return false;
        }
        else {
            raizenCoreJs.raizenCRUD.RealizarPesquisaPaginando();
            return true;
        }
    });

};

RaizenCRUD.prototype.Novo = function Novo() {

    raizenCoreJs.raizenHelpers.IdFormulario = "frmEdicao";
    $.get(this.urlNovo,
    {
        
    },
    function (data) {
        $("#containerEdicao").html(data);
        $("#modalcontainerEdicao").modal('show');
        $("#containerEdicao").show();

        //CONFIGURA OS MENUS SUSPENSOS PARA O PADRÃO
        //$('select[data-selectize]').selectize(
        ////    {
        ////    onChange: function (value) {
        ////        var obj = $(this)[0];
        ////        alert(obj.$input["0"].id);
        ////    }
        ////}
        //);
    });
};


RaizenCRUD.prototype.LimparFiltros = function (resetarHiddens) {
    $('#frmPesquisa').trigger('reset');

    //Manualmente resetar textboxes em casos de partial views que voltam do servidor.
    $('#frmPesquisa :text').val('');
    $('#frmPesquisa').find('[type="number"]').val('');

    //Resetar combos criadas com selectize.
    $('#frmPesquisa [data-selectize="True"]').each(function (index, item) {
        var $select = $(item).selectize();
        var control = $select[0].selectize;
        control.clear();
    });

    //Resetar datepicker textbox.
    $('#frmPesquisa [data-type="date"]').val('');

    //Resetar radios.
    $('#frmPesquisa input:radio').each(function (index, item) {
        $(item).removeAttr('checked');

        //Limpar radios estilo button group.
        $(item).parent().removeClass('active');

        //Setar valor default nos radios.
        var grupoRadios = $(item).parents('.radio-groups').first();
        var valorDefault = grupoRadios.attr('data-default-value');

        var itemDefault = grupoRadios.find('[value="' + valorDefault + '"]');
        itemDefault.attr('checked', 'checked');
        itemDefault.parent().addClass('active');
    });

    if (resetarHiddens) {
        $('#frmPesquisa :hidden').val('');
    }
};


RaizenCoreJs.prototype.raizenCRUD = new RaizenCRUD();



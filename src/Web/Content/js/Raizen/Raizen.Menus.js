/// Classe responsável em conter os programas java Scripts que os menus Bootstrap de nagevação entre páginas e dentro de páginas
/// Qualquer customização para CRUD's especificos deve ser tratado em um JS
/// JavaScript Design Patterns (Prototype Pattern) - ver Learning JavaScript Design Patterns A book by Addy Osmani Volume 1.5.2

function RaizenMenus() {

    this.ModoExibicaoTreeView = raizenCoreJs.raizenDominios.modoExibicaoTreeView.TotalmenteAberta;
    this.UrlGetItemMenu = "";
    this.UrlSetItemMenu = "";

}

RaizenMenus.prototype.UltimoItemSelecionado = function UltimoItemSelecionado(id) {

    var apiUrl = raizenCoreJs.raizenMenus.UrlSetItemMenu + "?Id=" + id;
    $.ajax({
        url: apiUrl,
        type: 'GET',
        cache: false,
        statusCode: {
            200: function (data) {
            }, // Successful DELETE
            404: function (data) {

            }, 
            400: function (data) {
                
            } 
        } // statusCode
    }); //    
};

RaizenMenus.prototype.ManterPosicaoMenu = function ManterPosicaoMenu() {
    var apiUrl = raizenCoreJs.raizenMenus.UrlGetItemMenu;
    $.ajax({
        url: apiUrl,
        type: 'GET',
        cache: false,
        statusCode: {
            200: function (data) {
                jQuery('#' + data).find('.sub').show();
            },
            404: function (data) {

            },
            400: function (data) {

            }
        }
    }); // 


};

RaizenMenus.prototype.ConfigurarTreeView = function ConfigurarTreeView() {

    $('.tree-toggle').click(function () {
        $(this).parent().children('ul.tree').toggle(200);

    });

    if (this.ModoExibicaoTreeView == raizenCoreJs.raizenDominios.modoExibicaoTreeView.TotalmenteFechada) {
        $('.nav.tree').each(function () {
            $(this).hide();
        });
    }

    if (this.ModoExibicaoTreeView == raizenCoreJs.raizenDominios.modoExibicaoTreeView.TotalmenteAberta) {
        $('.nav.tree').each(function () {
            $(this).show();
        });
    }

    if (this.ModoExibicaoTreeView == raizenCoreJs.raizenDominios.modoExibicaoTreeView.ParcialmenteAberta) {

        $('.nav.tree').each(function () {
            $(this).hide();
        });

        $('*[modoexibicaofilhos="' + raizenCoreJs.raizenDominios.modoExibicaoFilhosTreeView.Aberto + '"]').each(function () {
            $(this).parent().children('ul.tree').show();
        });

        $('*[modoexibicaofilhos="' + raizenCoreJs.raizenDominios.modoExibicaoFilhosTreeView.Fechado + '"]').each(function () {
            $(this).parent().children('ul.tree').hide();
        });

    }

    if ($('#UsarIcones').val() == 'False') {
        $('.tree-toggle').find('span').hide();
    }
};

RaizenCoreJs.prototype.raizenMenus = new RaizenMenus();
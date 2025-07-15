function RaizenPaginadorComponente() {
    this.objJquery = null;
}

RaizenPaginadorComponente.prototype.SetObjeto = function SetObjeto(IdControle) {
    this.objJquery = $("#" + IdControle);
};

RaizenPaginadorComponente.prototype.PrimeiraPagina = function PrimeiraPagina(IdControle) {
    raizenCoreJs.raizenPaginadorComponente.SetObjeto(IdControle);
    this.objJquery.find("#PaginaAtual").val(1);
    raizenCoreJs.raizenPaginadorComponente.ExecutarPaginacao(1);
};

RaizenPaginadorComponente.prototype.PaginaAnterior = function PaginaAnterior(IdControle) {

    raizenCoreJs.raizenPaginadorComponente.SetObjeto(IdControle);

    var pagAtual = parseInt(this.objJquery.find("#PaginaAtual").val()) - 1;

    this.objJquery.find("#PaginaAtual").val(pagAtual);

    if (pagAtual <= 0) {
        this.objJquery.find("#PaginaAtual").val(1);
    }

    raizenCoreJs.raizenPaginadorComponente.ExecutarPaginacao(this.objJquery.find("#PaginaAtual").val());
};

RaizenPaginadorComponente.prototype.ProximaPagina = function ProximaPagina(IdControle) {

    raizenCoreJs.raizenPaginadorComponente.SetObjeto(IdControle);
    var pagAtual = parseInt(this.objJquery.find("#PaginaAtual").val());
    pagAtual += 1;

    var qtdePaginas = parseInt(this.objJquery.find("#QtdePaginas").val());

    if (pagAtual > qtdePaginas) {
        pagAtual = qtdePaginas;
    }
    this.objJquery.find("#PaginaAtual").val(pagAtual);
    raizenCoreJs.raizenPaginadorComponente.ExecutarPaginacao(pagAtual);
};

RaizenPaginadorComponente.prototype.UltimaPagina = function UltimaPagina(IdControle) {
    raizenCoreJs.raizenPaginadorComponente.SetObjeto(IdControle);

    this.objJquery.find("#PaginaAtual").val(this.objJquery.find("#QtdePaginas").val());

    raizenCoreJs.raizenPaginadorComponente.ExecutarPaginacao(this.objJquery.find("#PaginaAtual").val());
};

RaizenPaginadorComponente.prototype.SelecaoMaunalPagina = function SelecaoMaunalPagina(numeroPagina, IdControle) {

    raizenCoreJs.raizenPaginadorComponente.SetObjeto(IdControle);

    this.objJquery.find("#PaginaAtual").val(parseInt(numeroPagina));
    var pagAtual = parseInt(this.objJquery.find("#PaginaAtual").val());

    if (pagAtual <= 0) {
        pagAtual = 1;
    }

    var qtdePaginas = parseInt(this.objJquery.find("#QtdePaginas").val());

    if (pagAtual > qtdePaginas) {
        pagAtual = qtdePaginas;
    }

    this.objJquery.find("#PaginaAtual").val(pagAtual);
    raizenCoreJs.raizenPaginadorComponente.ExecutarPaginacao(pagAtual);
};

RaizenPaginadorComponente.prototype.SelecaoConjuntoPaginas = function SelecaoConjuntoPaginas(numeroConjunto, IdControle) {

    
    raizenCoreJs.raizenPaginadorComponente.SetObjeto(IdControle);
    if (numeroConjunto != "") {
        this.objJquery.find("#QtdeItensPagina").val(numeroConjunto);

        //Quando selecione um novo valor de retorno no combo preciso recalcular a quantidade e renovar as páginas portando é como se 
        //Fizesse uma nova consulta.

        this.objJquery.find("#Status").val(raizenCoreJs.raizenDominios.estadoPaginacao.RenovandoConsulta);
    }

    raizenCoreJs.raizenPaginadorComponente.PrimeiraPagina(IdControle);
};

RaizenPaginadorComponente.prototype.ZerarPaginador = function ZerarPaginador(IdControle) {
    raizenCoreJs.raizenPaginadorComponente.SetObjeto(IdControle);
    this.objJquery.find("#Status").val(raizenCoreJs.raizenDominios.estadoPaginacao.RenovandoConsulta);
};

RaizenPaginadorComponente.prototype.ExecutarPaginacao = function ExecutarPaginacao(pagina) {

    this.objJquery.find("#Status").val(raizenCoreJs.raizenDominios.estadoPaginacao.Paginando);
    $('#' + raizenCoreJs.raizenHelpers.IdFormulario).find('#IdOperacaoCRUD').val(raizenCoreJs.raizenDominios.operacaoCrud.List);

    var execute = this.objJquery.find("#FuncaoRealizaPaginacao").val();
    if (execute != "") {
        var fn = new Function(execute);
        fn();
    }
};

RaizenCoreJs.prototype.raizenPaginadorComponente = new RaizenPaginadorComponente();
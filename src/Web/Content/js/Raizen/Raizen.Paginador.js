function RaizenPaginador() {

    this.PaginaAtual = 1;
    this.QtdePaginas = 1;
    this.QtdeItensPagina = 1;
    this.QtdeTotalRegistros = 1;
    this.Status = 2;

}

RaizenPaginador.prototype.PrimeiraPagina = function PrimeiraPagina() {
    raizenCoreJs.raizenPaginador.ExecutarPaginacao(1);
};

RaizenPaginador.prototype.PaginaAnterior = function PaginaAnterior() {

    this.PaginaAtual = parseInt(this.PaginaAtual) - 1;

    if (this.PaginaAtual <= 0) {
        this.PaginaAtual = 1;
    }

    raizenCoreJs.raizenPaginador.ExecutarPaginacao(this.PaginaAtual);
};

RaizenPaginador.prototype.ProximaPagina = function ProximaPagina() {

    this.PaginaAtual = parseInt(this.PaginaAtual);
    this.PaginaAtual += 1;

    if (this.PaginaAtual > this.QtdePaginas) {
        this.PaginaAtual = this.QtdePaginas;
    }
    raizenCoreJs.raizenPaginador.ExecutarPaginacao(this.PaginaAtual);
};

RaizenPaginador.prototype.UltimaPagina = function UltimaPagina() {
    this.PaginaAtual = this.QtdePaginas;
    raizenCoreJs.raizenPaginador.ExecutarPaginacao(this.PaginaAtual);
};

RaizenPaginador.prototype.SelecaoMaunalPagina = function SelecaoMaunalPagina(numeroPagina) {

    this.PaginaAtual = parseInt(numeroPagina);

    if (this.PaginaAtual <= 0) {
        this.PaginaAtual = 1;
    }

    if (this.PaginaAtual > this.QtdePaginas) {
        this.PaginaAtual = this.QtdePaginas;
    }

    raizenCoreJs.raizenPaginador.ExecutarPaginacao(this.PaginaAtual);   
};

RaizenPaginador.prototype.SelecaoConjuntoPaginas = function SelecaoConjuntoPaginas(numeroConjunto) {
    
    if (numeroConjunto != "") {
        $('#PaginadorDados_QtdeItensPagina').val(numeroConjunto);
        this.QtdeItensPagina = numeroConjunto;

        //Quando selecione um novo valor de retorno no combo preciso recalcular a quantidade e renovar as páginas portando é como se 
        //Fizesse uma nova consulta.

        $('#PaginadorDados_Status').val(raizenCoreJs.raizenDominios.estadoPaginacao.RenovandoConsulta);
        this.Status = raizenCoreJs.raizenDominios.estadoPaginacao.RenovandoConsulta;
    }

    raizenCoreJs.raizenPaginador.PrimeiraPagina();
};

RaizenPaginador.prototype.ExecutarPaginacao = function ExecutarPaginacao(pagina) {

    $('#PaginadorDados_Status').val(raizenCoreJs.raizenDominios.estadoPaginacao.Paginando);
    $('#PaginadorDados_PaginaAtual').val(pagina.toString());

    $('#' + raizenCoreJs.raizenHelpers.IdFormulario).find('#IdOperacaoCRUD').val(raizenCoreJs.raizenDominios.operacaoCrud.List);

    raizenCoreJs.raizenCRUD.RealizarPesquisaPaginando();
};

RaizenCoreJs.prototype.raizenPaginador = new RaizenPaginador();
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Raizen.UniCad.Web.Models;
using Raizen.Framework.Web.MVC.Bases;
using Raizen.Framework.Web.MVC.Filters;
using Raizen.UniCad.Model;
using Raizen.UniCad.Model.Filtro;
using Raizen.Framework.Models;
using Raizen.Framework.Web.MVC.Models;
using Raizen.UniCad.BLL;
using Raizen.Framework.Web.MVC.Utils;
using Raizen.Framework.Utils.Extensions;
using Raizen.Framework.UserSystem.Client;

namespace Raizen.UniCad.Web.Controllers
{
    public class ProdutoController : BaseUniCadController
    {
        #region Constantes
        private readonly ProdutoBusiness ProdutoBLL = new ProdutoBusiness();
        private readonly TipoProdutoBusiness TipoProdutoBLL = new TipoProdutoBusiness();
        private const string NomeFiltro = "Filtro_Produto";
        private const string NomePaginador = "Paginador_Produto";
        private const string TotalRegistros = "totalRegistros_Produto";
        #endregion

        #region Index
        [HttpGet]
        public ActionResult Index()
        {
            return this.CarregarDefault();
        }

        #endregion

        #region Novo
        [HttpGet]
        [AjaxOnlyAttribute]
        public ActionResult Novo()
        {
            ModelProduto Model = new ModelProduto();
            Model.Produto = new Produto();
            Model.Produto.Status = true;
            Model.Resultado = new ResultadoOperacao();
            var partialView = PartialView("_Edicao", Model);
            partialView.ViewData.Add(new KeyValuePair<string, object>("Edicao", false));

            return partialView;
        }
        #endregion

        #region Editar
        [HttpGet]
        [AjaxOnlyAttribute]
        public ActionResult Editar(string Id)
        {
            ModelProduto Model = new ModelProduto();
            var partialView = PartialView("_Edicao", Model);

            if (!string.IsNullOrEmpty(Id))
            {
                Model.Operacao = OperacoesCRUD.Editando;
                Model.ChavePrimaria = Id;
                Model.Resultado = new ResultadoOperacao();

                Model.Produto = new Produto();
                Model.Produto.ID = int.Parse(Model.ChavePrimaria);
                Model.Produto = this.ProdutoBLL.Selecionar(Model.Produto.ID);

                var tipoProduto = this.TipoProdutoBLL.Selecionar(Model.Produto.IDTipoProduto);
                Model.Filtro = new ProdutoFiltro();
                Model.Filtro.Pais = tipoProduto.Pais;
                Model.Filtro.IDTipoProduto = tipoProduto.ID;
            }

            partialView.ViewData.Add(new KeyValuePair<string, object> ("ListarDados", true));

            var tiposID = TipoProdutoBLL.Listar().Where(tp => tp.Pais == Model.Filtro.Pais).Select(tp => tp.ID).ToList();
            partialView.ViewData.Add(new KeyValuePair<string, object>("IDTipos", tiposID));

            return partialView;
        }

        #endregion

        #region CarregarDefault
        private ActionResult CarregarDefault()
        {
            ModelProduto model = new ModelProduto();
            model = ProdutoLayout(model);
            model.Operacao = OperacoesCRUD.List;
            model.Filtro = new ProdutoFiltro();
            model.Filtro.Status = true;
            model.Resultado = new ResultadoOperacao();
            model.PaginadorDados = new PaginadorModel();
            model.PaginadorDados.Status = EstadoPaginador.RenovandoConsulta;
            this.ListarPaginador(model);

            return View("Index", model);
        }

        #endregion

        #region ProdutoLayout
        private ModelProduto ProdutoLayout(ModelProduto model)
        {
            model.ConfiguracaoLayout.UtilizaComponenteBusca = true;
            model.ConfiguracaoLayout.UtilizaEmailUsuario = true;
            model.ConfiguracaoLayout.UtilizaListaAplicacoes = false;
            model.ConfiguracaoLayout.UtilizaMenuEsquerdo = true;
            model.ConfiguracaoLayout.UtilizaNotificacaoUsuario = true;
            model.ConfiguracaoLayout.UtilizaPerfilUsuario = true;
            model.ConfiguracaoLayout.UtilizaStatusTime = false;
            model.ConfiguracaoLayout.UtilizaTarefaUsuario = false;
            return model;
        }
        #endregion

        #region Listar
        private void ListarPaginador(ModelProduto Model)
        {
            if (Model.PaginadorDados.Status == EstadoPaginador.RenovandoConsulta && Model.Operacao != OperacoesCRUD.Editando)
            {
                this.ListarRenovandoConsulta(Model);
            }
            else
            {
                this.ListarPaginando(Model);
            }
        }

        private void ListarRenovandoConsulta(ModelProduto Model)
        {
            base.ArmazenarDados<ProdutoFiltro>(Model.Filtro, NomeFiltro);

            this.AtualizarQtdeRegPaginador(Model);
            int totalReg = base.RetornaDados<int>(TotalRegistros);

            Model.PaginadorDados = ModelUtils.IniciarPaginador(Model.PaginadorDados, totalReg);
            base.ArmazenarDados<int>(totalReg, TotalRegistros);
            base.ArmazenarDados<PaginadorModel>(Model.PaginadorDados, NomePaginador);

            if (totalReg > 0)
            {
                Model.ListaProduto = this.ProdutoBLL.ListarProduto(Model.Filtro, Model.PaginadorDados);
            }
        }

        private void AtualizarQtdeRegPaginador(ModelProduto Model)
        {
            Model.Filtro = base.RetornaDados<ProdutoFiltro>(NomeFiltro);
            base.ArmazenarDados<ProdutoFiltro>(Model.Filtro, NomeFiltro);

            int totalRegistros = this.ProdutoBLL.ListarProdutoCount(Model.Filtro);
            base.ArmazenarDados<int>(totalRegistros, TotalRegistros);
        }

        private void Listar(ModelProduto Model)
        {
            this.ListarPaginador(Model);
            Model.Resultado = base.ProcessarResultado(!Model.ListaProduto.IsNullOrEmpty(), Model.Operacao);
        }
        #endregion

        #region Pesquisar
        [HttpGet]
        [AjaxOnlyAttribute]
        public ActionResult Pesquisar(ModelProduto Model)
        {
            this.PrepararPaginadorOperacoes(Model);
            this.Listar(Model);

            var partialView = PartialView("_Pesquisa", Model);

            partialView.ViewData.Add(new KeyValuePair<string, object>("ListarDados", true));

            if (Model.Filtro.Pais != 0 && Model.Filtro.Pais != null)
                partialView.ViewData.Add(new KeyValuePair<string, object>("ListarTodos", false));

            var tiposID = TipoProdutoBLL.Listar().Where(FiltrarPorPais(Model)).Select(tp => tp.ID).ToList();
            partialView.ViewData.Add(new KeyValuePair<string, object>("IDTipos", tiposID));

            return partialView;
        }

        private static Func<TipoProduto, bool> FiltrarPorPais(ModelProduto Model)
        {
            if (Model.Filtro.Pais == 0 || Model.Filtro.Pais == null)
                return tp => true;

            return tp => tp.Pais == Model.Filtro.Pais;
        }

        #endregion

        #region Paginacao

        private void PrepararPaginadorOperacoes(ModelProduto Model)
        {
            if (Model.PaginadorDados == null)
            {
                Model.PaginadorDados = base.RetornaDados<PaginadorModel>(NomePaginador);
                base.ArmazenarDados<PaginadorModel>(Model.PaginadorDados, NomePaginador);
            }
            else
            {
                base.ArmazenarDados<PaginadorModel>(Model.PaginadorDados, NomePaginador);
            }

            //Sifnifica que não tem registro ainda
            if (Model.PaginadorDados == null)
            {
                Model.PaginadorDados = new PaginadorModel();
            }

            Model.PaginadorDados.ConjuntoPaginas = ModelUtils.ListarConjuntoPaginas();
        }

        private void ListarPaginando(ModelProduto Model)
        {
            Model.Filtro = base.RetornaDados<ProdutoFiltro>(NomeFiltro);
            base.ArmazenarDados<ProdutoFiltro>(Model.Filtro, NomeFiltro);

            int totalReg = base.RetornaDados<int>(TotalRegistros);
            Model.PaginadorDados.QtdeTotalRegistros = totalReg;

            base.ArmazenarDados<int>(totalReg, TotalRegistros);
            this.PrepararPaginador(ref Model);

            if (totalReg > 0)
            {
                Model.ListaProduto = this.ProdutoBLL.ListarProduto(Model.Filtro, Model.PaginadorDados);
            }
        }

        private void PrepararPaginador(ref ModelProduto Model)
        {
            PaginadorModel clone = Model.PaginadorDados.Clone() as PaginadorModel;
            Paginador.CalcularPaginas(clone, TipoPaginador.Modelo_Linq);
            Model.PaginadorDados = clone;
        }

        private void ZerarFiltro(ModelProduto Model)
        {
            Model.Filtro = new ProdutoFiltro();
            base.ArmazenarDados<ProdutoFiltro>(Model.Filtro, NomeFiltro);
        }

        #endregion

        #region Salvar

        [HttpGet]
        [AjaxOnlyAttribute]
        public ActionResult Salvar(ModelProduto Model)
        {
            Model.TipoValidacao = TiposValidacao.ValidacaoTotal;

            Model.Produto.IDTipoProduto = (Model.Filtro.IDTipoProduto ?? 0);
            var partialView = PartialView("_Edicao", Model);

           
            if (base.ValidarModel(Model, this.ModelState))
            {               
                if (Model.Operacao == OperacoesCRUD.Insert)
                {
                    Model.Resultado = base.ProcessarResultado(this.ProdutoBLL.Adicionar(Model.Produto), OperacoesCRUD.Insert);
                    this.ZerarFiltro(Model);
                    this.AtualizarQtdeRegPaginador(Model);
                    Model.Operacao = OperacoesCRUD.Update;
                }

                if (Model.Operacao == OperacoesCRUD.Editando)
                {
                    Model.Produto.ID = int.Parse(Model.ChavePrimaria);

                    Model.Resultado = base.ProcessarResultado(this.ProdutoBLL.Atualizar(Model.Produto), OperacoesCRUD.Update);
                    Model.Operacao = OperacoesCRUD.Update;
                }
            }
           
          

            partialView.ViewData.Add(new KeyValuePair<string, object>("ListarDados", true));

            var tiposID = TipoProdutoBLL.Listar().Where(tp => tp.Pais == Model.Filtro.Pais).Select(tp => tp.ID).ToList();
            partialView.ViewData.Add(new KeyValuePair<string, object>("IDTipos", tiposID));

            return partialView;
        }

        #endregion

        #region ExcluirRegistro

        [HttpGet]
        [AjaxOnlyAttribute]
        public bool ExcluirRegistro(int Id)
        {
            ModelProduto Model = new ModelProduto();
            try
            {
                Model.Resultado = base.ProcessarResultado(this.ProdutoBLL.Excluir(Id), OperacoesCRUD.Update);
            }
            catch
            {
                return false;
            }
            return true;
        }

        #endregion

    }

}

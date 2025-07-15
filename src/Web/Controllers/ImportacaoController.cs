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
using System.Threading;

namespace Raizen.UniCad.Web.Controllers
{
    public class ImportacaoController : BaseUniCadController
    {
        #region Constantes
        private readonly ImportacaoBusiness ImportacaoBLL = new ImportacaoBusiness();
        private const string NomeFiltro = "Filtro_Importacao";
        private const string NomePaginador = "Paginador_Importacao";
        private const string TotalRegistros = "totalRegistros_Importacao";
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
            ModelImportacao Model = new ModelImportacao();
            Model.Importacao = new Importacao();
            Model.Importacao.Status = 1;
            Model.Resultado = new ResultadoOperacao();
            return PartialView("_Edicao", Model);
        }
        #endregion

        #region Editar
        [HttpGet]
        [AjaxOnlyAttribute]
        public ActionResult Editar(string Id)
        {
            ModelImportacao Model = new ModelImportacao();

            if (!string.IsNullOrEmpty(Id))
            {
                Model.Operacao = OperacoesCRUD.Editando;
                Model.ChavePrimaria = Id;
                Model.Resultado = new ResultadoOperacao();

                Model.Importacao = new Importacao();
                Model.Importacao.ID = int.Parse(Model.ChavePrimaria);
                Model.Importacao = this.ImportacaoBLL.Selecionar(Model.Importacao.ID);

                Model.Importacao.Erros = new ErroImportacaoBusiness().Listar(p => p.IDImportacao == Model.Importacao.ID);
            }

            return PartialView("_Edicao", Model);
        }

        #endregion

        #region CarregarDefault
        private ActionResult CarregarDefault()
        {
            ModelImportacao model = new ModelImportacao();
            model = ImportacaoLayout(model);
            model.Operacao = OperacoesCRUD.List;
            model.Filtro = new ImportacaoFiltro();
            model.Filtro.Status = 1;
            model.Resultado = new ResultadoOperacao();
            model.PaginadorDados = new PaginadorModel();
            model.PaginadorDados.Status = EstadoPaginador.RenovandoConsulta;
            this.ListarPaginador(model);

            return View("Index", model);
        }

        #endregion

        #region ImportacaoLayout
        private ModelImportacao ImportacaoLayout(ModelImportacao model)
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
        private void ListarPaginador(ModelImportacao Model)
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

        private void ListarRenovandoConsulta(ModelImportacao Model)
        {
            base.ArmazenarDados<ImportacaoFiltro>(Model.Filtro, NomeFiltro);

            this.AtualizarQtdeRegPaginador(Model);
            int totalReg = base.RetornaDados<int>(TotalRegistros);

            Model.PaginadorDados = ModelUtils.IniciarPaginador(Model.PaginadorDados, totalReg);
            base.ArmazenarDados<int>(totalReg, TotalRegistros);
            base.ArmazenarDados<PaginadorModel>(Model.PaginadorDados, NomePaginador);

            if (totalReg > 0)
            {
                Model.ListaImportacao = this.ImportacaoBLL.ListarImportacao(Model.Filtro, Model.PaginadorDados);
            }
        }

        private void AtualizarQtdeRegPaginador(ModelImportacao Model)
        {
            Model.Filtro = base.RetornaDados<ImportacaoFiltro>(NomeFiltro);
            base.ArmazenarDados<ImportacaoFiltro>(Model.Filtro, NomeFiltro);

            int totalRegistros = this.ImportacaoBLL.ListarImportacaoCount(Model.Filtro);
            base.ArmazenarDados<int>(totalRegistros, TotalRegistros);
        }

        private void Listar(ModelImportacao Model)
        {
            this.ListarPaginador(Model);
            Model.Resultado = base.ProcessarResultado(!Model.ListaImportacao.IsNullOrEmpty(), Model.Operacao);
        }
        #endregion

        #region Pesquisar
        [HttpGet]
        [AjaxOnlyAttribute]
        public ActionResult Pesquisar(ModelImportacao Model)
        {
            this.PrepararPaginadorOperacoes(Model);
            this.Listar(Model);
            return PartialView("_Pesquisa", Model);
        }

        #endregion

        #region Paginacao

        private void PrepararPaginadorOperacoes(ModelImportacao Model)
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

        private void ListarPaginando(ModelImportacao Model)
        {
            Model.Filtro = base.RetornaDados<ImportacaoFiltro>(NomeFiltro);
            base.ArmazenarDados<ImportacaoFiltro>(Model.Filtro, NomeFiltro);

            int totalReg = base.RetornaDados<int>(TotalRegistros);
            Model.PaginadorDados.QtdeTotalRegistros = totalReg;

            base.ArmazenarDados<int>(totalReg, TotalRegistros);
            this.PrepararPaginador(ref Model);

            if (totalReg > 0)
            {
                Model.ListaImportacao = this.ImportacaoBLL.ListarImportacao(Model.Filtro, Model.PaginadorDados);
            }
        }

        private void PrepararPaginador(ref ModelImportacao Model)
        {
            PaginadorModel clone = Model.PaginadorDados.Clone() as PaginadorModel;
            Paginador.CalcularPaginas(clone, TipoPaginador.Modelo_Linq);
            Model.PaginadorDados = clone;
        }

        private void ZerarFiltro(ModelImportacao Model)
        {
            Model.Filtro = new ImportacaoFiltro();
            base.ArmazenarDados<ImportacaoFiltro>(Model.Filtro, NomeFiltro);
        }

        #endregion

        #region Salvar

        [HttpGet]
        [AjaxOnlyAttribute]
        public ActionResult Salvar(ModelImportacao Model)
        {
            Model.TipoValidacao = TiposValidacao.ValidacaoTotal;
            Model.Importacao.Data = DateTime.Now;
            if (base.ValidarModel(Model, this.ModelState))
            {
                if (Model.Operacao == OperacoesCRUD.Insert)
                {
                    Model.Resultado = base.ProcessarResultado(this.ImportacaoBLL.Adicionar(Model.Importacao), OperacoesCRUD.Insert);
                    this.ZerarFiltro(Model);
                    this.AtualizarQtdeRegPaginador(Model);
                    Model.Operacao = OperacoesCRUD.Update;
                }

                if (Model.Operacao == OperacoesCRUD.Editando)
                {
                    Model.Importacao.ID = int.Parse(Model.ChavePrimaria);

                    Model.Resultado = base.ProcessarResultado(this.ImportacaoBLL.Atualizar(Model.Importacao), OperacoesCRUD.Update);
                    Model.Operacao = OperacoesCRUD.Update;
                }
            }

            return PartialView("_Edicao", Model);
        }

        #endregion

        #region ExcluirRegistro

        [HttpGet]
        [AjaxOnlyAttribute]
        public bool ExcluirRegistro(int Id)
        {
            ModelImportacao Model = new ModelImportacao();
            try
            {
                Model.Resultado = base.ProcessarResultado(this.ImportacaoBLL.ExcluirImportacao(Id), OperacoesCRUD.Update);
            }
            catch
            {
                return false;
            }
            return true;
        }

        #endregion

       
        
        public int Processar(int id)
        {
            this.ImportacaoBLL.ZerarContador(id);
            this.ImportacaoBLL.Processar(Convert.ToInt32(id));
            return id;
        }

        public decimal Verificar(int id)
        {
            return this.ImportacaoBLL.Status(id);
        }

    }

}

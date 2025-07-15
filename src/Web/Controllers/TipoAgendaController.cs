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
    public class TipoAgendaController : BaseUniCadController
    {
        #region Constantes
        private readonly TipoAgendaBusiness TipoAgendaBLL = new TipoAgendaBusiness();
        private const string NomeFiltro = "Filtro_TipoAgenda";
        private const string NomePaginador = "Paginador_TipoAgenda";
        private const string TotalRegistros = "totalRegistros_TipoAgenda";
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
            ModelTipoAgenda Model = new ModelTipoAgenda();
            Model.TipoAgenda = new TipoAgenda();
            Model.TipoAgenda.Status = true;
            Model.Resultado = new ResultadoOperacao();
            return PartialView("_Edicao", Model);
        }
        #endregion

        #region Editar
        [HttpGet]
        [AjaxOnlyAttribute]
        public ActionResult Editar(string Id)
        {
            ModelTipoAgenda Model = new ModelTipoAgenda();

            if (!string.IsNullOrEmpty(Id))
            {
                Model.Operacao = OperacoesCRUD.Editando;
                Model.ChavePrimaria = Id;
                Model.Resultado = new ResultadoOperacao();

                Model.TipoAgenda = new TipoAgenda();
                Model.TipoAgenda.ID = int.Parse(Model.ChavePrimaria);
                Model.TipoAgenda = this.TipoAgendaBLL.Selecionar(Model.TipoAgenda.ID);
            }

            return PartialView("_Edicao", Model);
        }

        #endregion

        #region CarregarDefault
        private ActionResult CarregarDefault()
        {
            ModelTipoAgenda model = new ModelTipoAgenda();
            model = TipoAgendaLayout(model);
            model.Operacao = OperacoesCRUD.List;
            model.Filtro = new TipoAgendaFiltro();
            //model.Filtro.Status = true;
            model.Resultado = new ResultadoOperacao();
            model.PaginadorDados = new PaginadorModel();
            model.PaginadorDados.Status = EstadoPaginador.RenovandoConsulta;
            model.TipoAgenda = new TipoAgenda();
            this.ListarPaginador(model);

            return View("Index", model);
        }

        #endregion

        #region TipoAgendaLayout
        private ModelTipoAgenda TipoAgendaLayout(ModelTipoAgenda model)
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
        private void ListarPaginador(ModelTipoAgenda Model)
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

        private void ListarRenovandoConsulta(ModelTipoAgenda Model)
        {
            base.ArmazenarDados<TipoAgendaFiltro>(Model.Filtro, NomeFiltro);

            this.AtualizarQtdeRegPaginador(Model);
            int totalReg = base.RetornaDados<int>(TotalRegistros);

            Model.PaginadorDados = ModelUtils.IniciarPaginador(Model.PaginadorDados, totalReg);
            base.ArmazenarDados<int>(totalReg, TotalRegistros);
            base.ArmazenarDados<PaginadorModel>(Model.PaginadorDados, NomePaginador);

            if (totalReg > 0)
            {
                Model.ListaTipoAgenda = this.TipoAgendaBLL.ListarTipoAgenda(Model.Filtro, Model.PaginadorDados);
            }
        }

        private void AtualizarQtdeRegPaginador(ModelTipoAgenda Model)
        {
            Model.Filtro = base.RetornaDados<TipoAgendaFiltro>(NomeFiltro);
            base.ArmazenarDados<TipoAgendaFiltro>(Model.Filtro, NomeFiltro);

            int totalRegistros = this.TipoAgendaBLL.ListarTipoAgendaCount(Model.Filtro);
            base.ArmazenarDados<int>(totalRegistros, TotalRegistros);
        }

        private void Listar(ModelTipoAgenda Model)
        {
            this.ListarPaginador(Model);
            Model.Resultado = base.ProcessarResultado(!Model.ListaTipoAgenda.IsNullOrEmpty(), Model.Operacao);
        }
        #endregion

        #region Pesquisar
        [HttpGet]
        [AjaxOnlyAttribute]
        public ActionResult Pesquisar(ModelTipoAgenda Model)
        {
            this.PrepararPaginadorOperacoes(Model);
            this.Listar(Model);
            return PartialView("_Pesquisa", Model);
        }

        #endregion

        #region Paginacao

        private void PrepararPaginadorOperacoes(ModelTipoAgenda Model)
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

        private void ListarPaginando(ModelTipoAgenda Model)
        {
            Model.Filtro = base.RetornaDados<TipoAgendaFiltro>(NomeFiltro);
            base.ArmazenarDados<TipoAgendaFiltro>(Model.Filtro, NomeFiltro);

            int totalReg = base.RetornaDados<int>(TotalRegistros);
            Model.PaginadorDados.QtdeTotalRegistros = totalReg;

            base.ArmazenarDados<int>(totalReg, TotalRegistros);
            this.PrepararPaginador(ref Model);

            if (totalReg > 0)
            {
                Model.ListaTipoAgenda = this.TipoAgendaBLL.ListarTipoAgenda(Model.Filtro, Model.PaginadorDados);
            }
        }

        private void PrepararPaginador(ref ModelTipoAgenda Model)
        {
            PaginadorModel clone = Model.PaginadorDados.Clone() as PaginadorModel;
            Paginador.CalcularPaginas(clone, TipoPaginador.Modelo_Linq);
            Model.PaginadorDados = clone;
        }

        private void ZerarFiltro(ModelTipoAgenda Model)
        {
            Model.Filtro = new TipoAgendaFiltro();
            base.ArmazenarDados<TipoAgendaFiltro>(Model.Filtro, NomeFiltro);
        }

        #endregion

        #region Salvar

        [HttpGet]
        [AjaxOnlyAttribute]
        public ActionResult Salvar(ModelTipoAgenda Model)
        {
            Model.TipoValidacao = TiposValidacao.ValidacaoTotal;

            if (base.ValidarModel(Model, this.ModelState))
            {
                if (Model.Operacao == OperacoesCRUD.Insert)
                {
                    Model.Resultado = base.ProcessarResultado(this.TipoAgendaBLL.Adicionar(Model.TipoAgenda), OperacoesCRUD.Insert);
                    this.ZerarFiltro(Model);
                    this.AtualizarQtdeRegPaginador(Model);
                    Model.Operacao = OperacoesCRUD.Update;
                }

                if (Model.Operacao == OperacoesCRUD.Editando)
                {
                    Model.TipoAgenda.ID = int.Parse(Model.ChavePrimaria);

                    Model.Resultado = base.ProcessarResultado(this.TipoAgendaBLL.Atualizar(Model.TipoAgenda), OperacoesCRUD.Update);
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
            ModelTipoAgenda Model = new ModelTipoAgenda();
            try
            {
                Model.Resultado = base.ProcessarResultado(this.TipoAgendaBLL.Excluir(Id), OperacoesCRUD.Update);
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

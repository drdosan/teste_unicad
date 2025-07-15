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
    public class LogDocumentosController : BaseUniCadController
    {
        #region Constantes
        private readonly LogDocumentosBusiness LogDocumentosBLL = new LogDocumentosBusiness();
        private const string NomeFiltro = "Filtro_LogDocumentos";
        private const string NomePaginador = "Paginador_LogDocumentos";
        private const string TotalRegistros = "totalRegistros_LogDocumentos";
        #endregion

        #region Index
        [HttpGet]
        public ActionResult Index()
        {
            return this.CarregarDefault();
        }

        #endregion

        #region Editar
        [HttpGet]
        [AjaxOnlyAttribute]
        public ActionResult Editar(string Id)
        {
            ModelLogDocumentos Model = new ModelLogDocumentos();

            if (!string.IsNullOrEmpty(Id))
            {
                Model.Operacao = OperacoesCRUD.Editando;
                Model.ChavePrimaria = Id;
                Model.Resultado = new ResultadoOperacao();

                Model.LogDocumentos = new LogDocumentos();
                Model.LogDocumentos.ID = int.Parse(Model.ChavePrimaria);
                Model.LogDocumentos = this.LogDocumentosBLL.Selecionar(Model.LogDocumentos.ID);
            }

            return PartialView("_Edicao", Model);
        }

        #endregion

        #region CarregarDefault
        private ActionResult CarregarDefault()
        {
            ModelLogDocumentos model = new ModelLogDocumentos();
            model = LogDocumentosLayout(model);
            model.Operacao = OperacoesCRUD.List;
            model.Filtro = new LogDocumentosFiltro();
            model.Resultado = new ResultadoOperacao();
            model.PaginadorDados = new PaginadorModel();
            model.PaginadorDados.Status = EstadoPaginador.RenovandoConsulta;
            this.ListarPaginador(model);

            return View("Index", model);
        }

        #endregion

        #region LogDocumentosLayout
        private ModelLogDocumentos LogDocumentosLayout(ModelLogDocumentos model)
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
        private void ListarPaginador(ModelLogDocumentos Model)
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

        private void ListarRenovandoConsulta(ModelLogDocumentos Model)
        {
            base.ArmazenarDados<LogDocumentosFiltro>(Model.Filtro, NomeFiltro);

            this.AtualizarQtdeRegPaginador(Model);
            int totalReg = base.RetornaDados<int>(TotalRegistros);

            Model.PaginadorDados = ModelUtils.IniciarPaginador(Model.PaginadorDados, totalReg);
            base.ArmazenarDados<int>(totalReg, TotalRegistros);
            base.ArmazenarDados<PaginadorModel>(Model.PaginadorDados, NomePaginador);

            if (totalReg > 0)
            {
                Model.ListaLogDocumentos = this.LogDocumentosBLL.ListarLogDocumentos(Model.Filtro, Model.PaginadorDados);
            }
        }

        private void AtualizarQtdeRegPaginador(ModelLogDocumentos Model)
        {
            Model.Filtro = base.RetornaDados<LogDocumentosFiltro>(NomeFiltro);
            base.ArmazenarDados<LogDocumentosFiltro>(Model.Filtro, NomeFiltro);

            int totalRegistros = this.LogDocumentosBLL.ListarLogDocumentosCount(Model.Filtro);
            base.ArmazenarDados<int>(totalRegistros, TotalRegistros);
        }

        private void Listar(ModelLogDocumentos Model)
        {
            this.ListarPaginador(Model);
            Model.Resultado = base.ProcessarResultado(!Model.ListaLogDocumentos.IsNullOrEmpty(), Model.Operacao);
        }
        #endregion

        #region Pesquisar
        [HttpGet]
        [AjaxOnlyAttribute]
        public ActionResult Pesquisar(ModelLogDocumentos Model)
        {
            if (Model.Filtro.DataInicio.HasValue)
            {
                Model.Filtro.DataInicio = new DateTime(Model.Filtro.DataInicio.Value.Year, Model.Filtro.DataInicio.Value.Month, Model.Filtro.DataInicio.Value.Day, 0, 0, 0);
            }
            if (Model.Filtro.DataFim.HasValue)
            {
                Model.Filtro.DataFim = new DateTime(Model.Filtro.DataFim.Value.Year, Model.Filtro.DataFim.Value.Month, Model.Filtro.DataFim.Value.Day, 23, 59, 59);
            } 

            this.PrepararPaginadorOperacoes(Model);
            this.Listar(Model);
            return PartialView("_Pesquisa", Model);
        }

        #endregion

        #region Paginacao

        private void PrepararPaginadorOperacoes(ModelLogDocumentos Model)
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

        private void ListarPaginando(ModelLogDocumentos Model)
        {
            Model.Filtro = base.RetornaDados<LogDocumentosFiltro>(NomeFiltro);
            base.ArmazenarDados<LogDocumentosFiltro>(Model.Filtro, NomeFiltro);

            int totalReg = base.RetornaDados<int>(TotalRegistros);
            Model.PaginadorDados.QtdeTotalRegistros = totalReg;

            base.ArmazenarDados<int>(totalReg, TotalRegistros);
            this.PrepararPaginador(ref Model);

            if (totalReg > 0)
            {
                Model.ListaLogDocumentos = this.LogDocumentosBLL.ListarLogDocumentos(Model.Filtro, Model.PaginadorDados);
            }
        }

        private void PrepararPaginador(ref ModelLogDocumentos Model)
        {
            PaginadorModel clone = Model.PaginadorDados.Clone() as PaginadorModel;
            Paginador.CalcularPaginas(clone, TipoPaginador.Modelo_Linq);
            Model.PaginadorDados = clone;
        }

        private void ZerarFiltro(ModelLogDocumentos Model)
        {
            Model.Filtro = new LogDocumentosFiltro();
            base.ArmazenarDados<LogDocumentosFiltro>(Model.Filtro, NomeFiltro);
        }

        #endregion

        #region Salvar

        [HttpGet]
        [AjaxOnlyAttribute]
        public ActionResult Salvar(ModelLogDocumentos Model)
        {
            Model.TipoValidacao = TiposValidacao.ValidacaoTotal;

            if (base.ValidarModel(Model, this.ModelState))
            {
                if (Model.Operacao == OperacoesCRUD.Insert)
                {
                    Model.Resultado = base.ProcessarResultado(this.LogDocumentosBLL.Adicionar(Model.LogDocumentos), OperacoesCRUD.Insert);
                    this.ZerarFiltro(Model);
                    this.AtualizarQtdeRegPaginador(Model);
                    Model.Operacao = OperacoesCRUD.Update;
                }

                if (Model.Operacao == OperacoesCRUD.Editando)
                {
                    Model.LogDocumentos.ID = int.Parse(Model.ChavePrimaria);

                    Model.Resultado = base.ProcessarResultado(this.LogDocumentosBLL.Atualizar(Model.LogDocumentos), OperacoesCRUD.Update);
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
            ModelLogDocumentos Model = new ModelLogDocumentos();
            try
            {
                Model.Resultado = base.ProcessarResultado(this.LogDocumentosBLL.Excluir(Id), OperacoesCRUD.Update);
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

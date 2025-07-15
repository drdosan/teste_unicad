using Raizen.Framework.Models;
using Raizen.Framework.Web.MVC.Bases;
using Raizen.Framework.Web.MVC.Filters;
using Raizen.Framework.Web.MVC.Models;
using Raizen.Framework.Utils.Extensions;
using Raizen.UniCad.BLL;
using Raizen.UniCad.Model.Filtro;
using Raizen.UniCad.Web.Models;
using System.Web.Mvc;
using Raizen.Framework.Web.MVC.Utils;
using System.Collections.Generic;

namespace Raizen.UniCad.Web.Controllers
{
    public class LogController : BaseUniCadController
    {
        #region [ Constantes ]

        private const string NomeFiltro = "Filtro_Log";
        private const string NomePaginador = "Paginador_Log";
        private const string TotalRegistros = "totalRegistros_Log";

        #endregion

        #region [ Actions ]

        [HttpGet]
        public ActionResult Index()
        {
            return this.CarregarIndex();
        }

        [HttpGet]
        [AjaxOnlyAttribute]
        public ActionResult Pesquisar(ModelLog Model)
        {
            return this.CarregarPartialPesquisa(Model);
        }

        [HttpGet]
        [AjaxOnlyAttribute]
        public ActionResult Detalhar(int IdLog)
        {
            return this.CarregarPartialDetalhe(IdLog);
        }

        #endregion

        #region [ Index ]

        private ActionResult CarregarIndex()
        {
            ModelLog model = new ModelLog();
            model.Operacao = OperacoesCRUD.List;
            model.Filtro = new LogFiltro();
            model.Resultado = new ResultadoOperacao();
            model.PaginadorDados = new PaginadorModel();
            model.PaginadorDados.Status = EstadoPaginador.RenovandoConsulta;
            this.ListarLogs(model);

            return View("Index", model);
        }

        #endregion

        #region [ Pesquisar ]

        private ActionResult CarregarPartialPesquisa(ModelLog Model)
        {
            this.PrepararPaginadorOperacoes(Model);

            Model.TipoValidacao = TiposValidacao.ValidacaoTotal;
            if (base.ValidarModel(Model, ModelState))
            {
                this.ListarLogs(Model);
                Model.Resultado = base.ProcessarResultado(!Model.ListaLogs.IsNullOrEmpty(), Model.Operacao);
            }

            return PartialView("_Pesquisa", Model);
        }

        #endregion

        #region [ Detalhar ]

        private ActionResult CarregarPartialDetalhe(int IdLog)
        {
            ModelLog model = new ModelLog();

            List<Framework.Log.Model.LogErro> logs = Framework.Log.Client.Logger.PesquisarLogErro(x => x.Id == IdLog);

            if (logs != null && logs.Count > 0)
            {
                model.Log = logs[0];
            }

            return PartialView("_Log", model);
        }

        #endregion

        #region [ Listagens ]

        private void ListarLogs(ModelLog Model)
        {
            if (Model.PaginadorDados.Status == EstadoPaginador.RenovandoConsulta)
            {
                this.ListarLogsRenovandoConsulta(Model);
            }
            else
            {
                this.ListarLogsPaginando(Model);
            }
        }

        private void ListarLogsRenovandoConsulta(ModelLog Model)
        {
            base.ArmazenarDados<LogFiltro>(Model.Filtro, NomeFiltro);

            this.AtualizarQtdRegistrosPaginador(Model);
            int totalReg = base.RetornaDados<int>(TotalRegistros);

            Model.PaginadorDados = ModelUtils.IniciarPaginador(Model.PaginadorDados, totalReg);
            base.ArmazenarDados<int>(totalReg, TotalRegistros);
            base.ArmazenarDados<PaginadorModel>(Model.PaginadorDados, NomePaginador);

            if (totalReg > 0)
            {
                Model.ListaLogs = Framework.Log.Client.Logger.PesquisarLogErro(
                    new Framework.Log.Model.Filtro.LogErroFiltro()
                    {
                        DataInicial = Model.Filtro.DataInicial,
                        DataFinal = Model.Filtro.DataFinal,
                        Mensagem = Model.Filtro.Origem
                    },
                Model.PaginadorDados);
            }
        }

        private void ListarLogsPaginando(ModelLog Model)
        {
            Model.Filtro = base.RetornaDados<LogFiltro>(NomeFiltro);
            base.ArmazenarDados<LogFiltro>(Model.Filtro, NomeFiltro);

            int totalReg = base.RetornaDados<int>(TotalRegistros);
            Model.PaginadorDados.QtdeTotalRegistros = totalReg;

            base.ArmazenarDados<int>(totalReg, TotalRegistros);
            this.PrepararPaginador(ref Model);

            if (totalReg > 0)
            {
                Model.ListaLogs = Framework.Log.Client.Logger.PesquisarLogErro(
                   new Framework.Log.Model.Filtro.LogErroFiltro()
                   {
                       DataInicial = Model.Filtro.DataInicial,
                       DataFinal = Model.Filtro.DataFinal,
                       Mensagem = Model.Filtro.Origem
                   },
               Model.PaginadorDados);
            }
        }

        #endregion

        #region [ Paginação ]

        private void PrepararPaginadorOperacoes(ModelLog Model)
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

        private void PrepararPaginador(ref ModelLog Model)
        {
            PaginadorModel clone = Model.PaginadorDados.Clone() as PaginadorModel;
            Paginador.CalcularPaginas(clone, TipoPaginador.Modelo_Linq);
            Model.PaginadorDados = clone;
        }

        private void AtualizarQtdRegistrosPaginador(ModelLog Model)
        {
            Model.Filtro = base.RetornaDados<LogFiltro>(NomeFiltro);
            base.ArmazenarDados<LogFiltro>(Model.Filtro, NomeFiltro);

            int totalRegistros = Framework.Log.Client.Logger.CountPesquisaLogErro(new Framework.Log.Model.Filtro.LogErroFiltro()
            {
                DataInicial = Model.Filtro.DataInicial,
                DataFinal = Model.Filtro.DataFinal
            });

            base.ArmazenarDados<int>(totalRegistros, TotalRegistros);
        }

        #endregion
    }
}
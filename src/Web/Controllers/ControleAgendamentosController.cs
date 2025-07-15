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
using Raizen.UniCad.Model.View;
using System.IO;

namespace Raizen.UniCad.Web.Controllers
{
    public class ControleAgendamentosController : BaseUniCadController
    {
        #region Constantes
        private readonly AgendamentoTerminalBusiness AgendamentlTerminalBLL = new AgendamentoTerminalBusiness();
        private const string NomeFiltro = "Filtro_ControleAgendamentos";
        private const string NomePaginador = "Paginador_ControleAgendamentos";
        private const string TotalRegistros = "totalRegistros_ControleAgendamentos";
        #endregion

        #region Index
        [HttpGet]
        public ActionResult Index()
        {
            return this.CarregarDefault();
        }

        #endregion

        #region CarregarDefault
        private ActionResult CarregarDefault()
        {
            ModelControleAgendamentos model = new ModelControleAgendamentos();
            model = ControleAgendamentosLayout(model);
            model.Operacao = OperacoesCRUD.List;
            model.Filtro = new AgendamentoTerminalFiltro();
            model.Filtro.IdStatus = true;
            model.Resultado = new ResultadoOperacao();
            model.PaginadorDados = new PaginadorModel();
            model.PaginadorDados.Status = EstadoPaginador.RenovandoConsulta;
            this.ListarPaginador(model);

            return View("Index", model);
        }

        #endregion
        #region controle de agendas de treinamento
        public ActionResult EditarControle(DateTime data, int idTerminal, int idTipoAgenda, string vagas, string vagasDisponiveis)
        {
            var model = this.AgendamentlTerminalBLL.SelecionarControleAgendamentos(new AgendamentoTerminalFiltro
            {
                Data = data,
                IdTerminal = idTerminal,
                IdTipoAgenda = idTipoAgenda
            });

            model.listaControles = AgendamentlTerminalBLL.ListarInscritosTreinamento(idTerminal, idTipoAgenda, data);
            return PartialView("_ControlePresenca", model);
        }
        #endregion
        #region pdf
        public ActionResult GerarPdf(DateTime data, int idTerminal, int idTipoAgenda, int vagas, int vagasDisponiveis)
        {
            MemoryStream memStream = AgendamentlTerminalBLL.GerarPdf(data, idTerminal, idTipoAgenda, vagas, vagasDisponiveis );
            string handle = Guid.NewGuid().ToString();
            TempData[handle] = memStream.ToArray();
            string nomeArquivo = "ControleAgendamento" + "_" + DateTime.Now.ToString("yyyyMMdd-hhmmss") + ".pdf";
            return Json(new { FileGuid = handle, FileName = nomeArquivo }, JsonRequestBehavior.AllowGet);
        }



        [HttpGet]
        public virtual ActionResult Download(string fileGuid, string fileName)
        {
            if (TempData[fileGuid] != null)
            {
                byte[] data = TempData[fileGuid] as byte[];
                return File(data, "application/pdf", fileName);
            }
            else
            {
                // Problem - Log the error, generate a blank file,
                //           redirect to another controller action - whatever fits with your application
                return new EmptyResult();
            }
        }
        #endregion
        #region ControleAgendamentosLayout
        private ModelControleAgendamentos ControleAgendamentosLayout(ModelControleAgendamentos model)
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

        #region salvar
        public JsonResult SalvarPresenca(AgendamentoTerminalView Model)
        {
            try
            {
                AgendamentlTerminalBLL.SalvarPresenca(Model);
                return Json(new { retorno = true });
            }
            catch (Exception ex)
            {
                return Json(new { retorno = true, mensagem = "Erro ao salvar: " + ex.Message });
            }
        }
        #endregion

        #region Listar
        private void ListarPaginador(ModelControleAgendamentos Model)
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

        private void ListarRenovandoConsulta(ModelControleAgendamentos Model)
        {
            base.ArmazenarDados<AgendamentoTerminalFiltro>(Model.Filtro, NomeFiltro);

            this.AtualizarQtdeRegPaginador(Model);
            int totalReg = base.RetornaDados<int>(TotalRegistros);

            Model.PaginadorDados = ModelUtils.IniciarPaginador(Model.PaginadorDados, totalReg);
            base.ArmazenarDados<int>(totalReg, TotalRegistros);
            base.ArmazenarDados<PaginadorModel>(Model.PaginadorDados, NomePaginador);

            if (totalReg > 0)
            {
                Model.ListaAgendamentoTerminal = this.AgendamentlTerminalBLL.ListarControleAgendamentos(Model.Filtro, Model.PaginadorDados);
            }
        }

        private void AtualizarQtdeRegPaginador(ModelControleAgendamentos Model)
        {
            Model.Filtro = base.RetornaDados<AgendamentoTerminalFiltro>(NomeFiltro);
            base.ArmazenarDados<AgendamentoTerminalFiltro>(Model.Filtro, NomeFiltro);

            int totalRegistros = this.AgendamentlTerminalBLL.ListarControleAgendamentosCount(Model.Filtro);
            base.ArmazenarDados<int>(totalRegistros, TotalRegistros);
        }

        private void Listar(ModelControleAgendamentos Model)
        {
            this.ListarPaginador(Model);
            Model.Resultado = base.ProcessarResultado(!Model.ListaAgendamentoTerminal.IsNullOrEmpty(), Model.Operacao);
        }
        #endregion

        #region Pesquisar
        [HttpGet]
        [AjaxOnlyAttribute]
        public ActionResult Pesquisar(ModelControleAgendamentos Model)
        {
            this.PrepararPaginadorOperacoes(Model);
            this.Listar(Model);
            return PartialView("_Pesquisa", Model);
        }
        #endregion

        #region Paginacao

        private void PrepararPaginadorOperacoes(ModelControleAgendamentos Model)
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

        private void ListarPaginando(ModelControleAgendamentos Model)
        {
            Model.Filtro = base.RetornaDados<AgendamentoTerminalFiltro>(NomeFiltro);
            base.ArmazenarDados<AgendamentoTerminalFiltro>(Model.Filtro, NomeFiltro);

            int totalReg = base.RetornaDados<int>(TotalRegistros);
            Model.PaginadorDados.QtdeTotalRegistros = totalReg;

            base.ArmazenarDados<int>(totalReg, TotalRegistros);
            this.PrepararPaginador(ref Model);

            if (totalReg > 0)
            {
                Model.ListaAgendamentoTerminal = this.AgendamentlTerminalBLL.ListarControleAgendamentos(Model.Filtro, Model.PaginadorDados);
            }
        }

        private void PrepararPaginador(ref ModelControleAgendamentos Model)
        {
            PaginadorModel clone = Model.PaginadorDados.Clone() as PaginadorModel;
            Paginador.CalcularPaginas(clone, TipoPaginador.Modelo_Linq);
            Model.PaginadorDados = clone;
        }

        private void ZerarFiltro(ModelControleAgendamentos Model)
        {
            Model.Filtro = new AgendamentoTerminalFiltro();
            base.ArmazenarDados<AgendamentoTerminalFiltro>(Model.Filtro, NomeFiltro);
        }

        #endregion

    }

}

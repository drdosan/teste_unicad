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

namespace Raizen.UniCad.Web.Controllers
{
    public class AgendamentoTerminalController : BaseUniCadController
    {
        #region Constantes
        private readonly AgendamentoTerminalBusiness AgendamentoTerminalBLL = new AgendamentoTerminalBusiness();
        private readonly AgendamentoTerminalHorarioBusiness AgendamentoTerminalHorarioBLL = new AgendamentoTerminalHorarioBusiness();
        private readonly AgendamentoChecklistBusiness AgendamentlCheckListBLL = new AgendamentoChecklistBusiness();
        private const string NomeFiltro = "Filtro_AgendamentoTerminal";
        private const string NomePaginador = "Paginador_AgendamentoTerminal";
        private const string TotalRegistros = "totalRegistros_AgendamentoTerminal";
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
            ModelAgendamentoTerminal Model = new ModelAgendamentoTerminal();
            Model.AgendamentoTerminal = new AgendamentoTerminal();
            Model.AgendamentoTerminal.Ativo = true;
            Model.Resultado = new ResultadoOperacao();
            CarregarPermissaoUsuario(Model);
            return PartialView("_Edicao", Model);
        }
        #endregion

        [HttpGet]
        [AjaxOnly]
        public bool VerificarSeEhPool(int idTerminal)
        {
            return new TerminalBusiness().Listar(w => w.ID == idTerminal && w.isPool).Any();
        }
        [HttpGet]
        [AjaxOnlyAttribute]
        public JsonResult ValidarExistemItensRelacionadosHorario(int id)
        {
            var treinamentos = new AgendamentoTreinamentoBusiness().Listar(w => w.IDAgendamentoTerminalHorario == id);
            var checklists = new AgendamentoChecklistBusiness().Listar(w => w.IDAgendamentoTerminalHorario == id);
            return new JsonResult
            {
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                Data = (treinamentos.Any() || checklists.Any())

            };
        }
        #region Excluir
        [HttpGet]
        [AjaxOnlyAttribute]
        public JsonResult ValidarExclusao(int Id)
        {
            var treinamentos = (new AgendamentoTreinamentoBusiness().ListarPorAgendamentoTerminal(Id).Any());
            var horarios = AgendamentoTerminalHorarioBLL.Listar(w => w.IDAgendamentoTerminal == Id);
            var checklists = (AgendamentlCheckListBLL.ListarPorAgendamentoTerminal(Id).Any());

            return new JsonResult
            {
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                Data = horarios != null ? (checklists || treinamentos) : false
            };
        }

        [HttpPost]
        [AjaxOnly]
        public JsonResult Excluir(int Id)
        {
            return new JsonResult { Data = AgendamentoTerminalBLL.ExcluirAgendamento(Id) };
        }

        public JsonResult ValidarExclusaoHorario(int Id)
        {
            var treinamentos = new AgendamentoTreinamentoBusiness().Listar(w => w.IDAgendamentoTerminalHorario == Id).Any();
            var checklists = new AgendamentoChecklistBusiness().Listar(w => w.IDAgendamentoTerminalHorario == Id).Any();
            return new JsonResult
            {
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                Data = checklists || treinamentos
            };
        }

        [HttpPost]
        [AjaxOnly]
        public JsonResult ExcluirHorario(int Id)
        {
            return new JsonResult { Data = AgendamentoTerminalBLL.ExcluirAgendamentoHorario(Id) };
        }

        #endregion
        #region clonar
        [HttpGet]
        [AjaxOnlyAttribute]
        public ActionResult Clonar(int Id)
        {
            return PartialView("_Clone", Id);
        }

        [HttpPost]
        [AjaxOnly]
        public JsonResult Clonar(int id, DateTime[] datas)
        {
            return new JsonResult { Data = AgendamentoTerminalBLL.Clonar(id, datas) };
        }
        #endregion

        #region Editar
        [HttpGet]
        [AjaxOnlyAttribute]
        public ActionResult Editar(string Id)
        {
            ModelAgendamentoTerminal Model = new ModelAgendamentoTerminal();

            if (!string.IsNullOrEmpty(Id))
            {
                Model.Operacao = OperacoesCRUD.Editando;
                Model.ChavePrimaria = Id;
                Model.Resultado = new ResultadoOperacao();

                Model.AgendamentoTerminal = new AgendamentoTerminal();
                Model.AgendamentoTerminal.ID = int.Parse(Model.ChavePrimaria);
                Model.AgendamentoTerminal = this.AgendamentoTerminalBLL.Selecionar(Model.AgendamentoTerminal.ID);
                Model.AgendamentoTerminal.ListaAgendamentoTerminalHorario = AgendamentoTerminalHorarioBLL.ListarPorAgendamentoTerminal(Model.AgendamentoTerminal.ID);
                Model.AgendamentoTerminalHorario = new AgendamentoTerminalHorario();
                CarregarPermissaoUsuario(Model);
            }

            return PartialView("_Edicao", Model);
        }

        #endregion

        #region CarregarDefault
        private ActionResult CarregarDefault()
        {
            ModelAgendamentoTerminal model = new ModelAgendamentoTerminal();
            model = AgendamentoTerminalLayout(model);
            model.Operacao = OperacoesCRUD.List;
            model.Filtro = new AgendamentoTerminalFiltro();
            model.Filtro.IdStatus = true;
            model.Resultado = new ResultadoOperacao();
            model.PaginadorDados = new PaginadorModel();
            model.PaginadorDados.Status = EstadoPaginador.RenovandoConsulta;
            model.AgendamentoTerminal = new AgendamentoTerminal();
            this.ListarPaginador(model);

            return View("Index", model);
        }

        #endregion

        #region AgendamentoTerminalLayout
        private ModelAgendamentoTerminal AgendamentoTerminalLayout(ModelAgendamentoTerminal model)
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
        private void ListarPaginador(ModelAgendamentoTerminal Model)
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

        private void ListarRenovandoConsulta(ModelAgendamentoTerminal Model)
        {
            base.ArmazenarDados<AgendamentoTerminalFiltro>(Model.Filtro, NomeFiltro);

            this.AtualizarQtdeRegPaginador(Model);
            int totalReg = base.RetornaDados<int>(TotalRegistros);

            Model.PaginadorDados = ModelUtils.IniciarPaginador(Model.PaginadorDados, totalReg);
            base.ArmazenarDados<int>(totalReg, TotalRegistros);
            base.ArmazenarDados<PaginadorModel>(Model.PaginadorDados, NomePaginador);

            if (totalReg > 0)
            {
                Model.ListaAgendamentoTerminal = this.AgendamentoTerminalBLL.ListarAgendamentoTerminal(Model.Filtro, Model.PaginadorDados);
            }
        }

        private void AtualizarQtdeRegPaginador(ModelAgendamentoTerminal Model)
        {
            Model.Filtro = base.RetornaDados<AgendamentoTerminalFiltro>(NomeFiltro);
            base.ArmazenarDados<AgendamentoTerminalFiltro>(Model.Filtro, NomeFiltro);

            int totalRegistros = this.AgendamentoTerminalBLL.ListarAgendamentoTerminalCount(Model.Filtro);
            base.ArmazenarDados<int>(totalRegistros, TotalRegistros);
        }

        private void Listar(ModelAgendamentoTerminal Model)
        {
            this.ListarPaginador(Model);
            Model.Resultado = base.ProcessarResultado(!Model.ListaAgendamentoTerminal.IsNullOrEmpty(), Model.Operacao);
        }
        #endregion

        #region Pesquisar
        [HttpGet]
        [AjaxOnlyAttribute]
        public ActionResult Pesquisar(ModelAgendamentoTerminal Model)
        {
            this.PrepararPaginadorOperacoes(Model);
            this.Listar(Model);
            return PartialView("_Pesquisa", Model);
        }

        [HttpGet]
        [AjaxOnlyAttribute]
        public ActionResult PesquisarHorarios(ModelAgendamentoTerminal Model)
        {
            var AgendamentoTerminal = AgendamentoTerminalBLL.Selecionar(w => w.Data == Model.AgendamentoTerminal.Data
                && w.IDTerminal == Model.AgendamentoTerminal.IDTerminal
                && w.IDTipoAgenda == Model.AgendamentoTerminal.IDTipoAgenda);
            if (AgendamentoTerminal?.ID > 0)
                Model.AgendamentoTerminal.ListaAgendamentoTerminalHorario = AgendamentoTerminalHorarioBLL.ListarPorAgendamentoTerminal(AgendamentoTerminal.ID);
            return PartialView("_Horario", Model.AgendamentoTerminal);
        }

        #endregion

        #region Paginacao

        private void PrepararPaginadorOperacoes(ModelAgendamentoTerminal Model)
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

        private void ListarPaginando(ModelAgendamentoTerminal Model)
        {
            Model.Filtro = base.RetornaDados<AgendamentoTerminalFiltro>(NomeFiltro);
            base.ArmazenarDados<AgendamentoTerminalFiltro>(Model.Filtro, NomeFiltro);

            int totalReg = base.RetornaDados<int>(TotalRegistros);
            Model.PaginadorDados.QtdeTotalRegistros = totalReg;

            base.ArmazenarDados<int>(totalReg, TotalRegistros);
            this.PrepararPaginador(ref Model);

            if (totalReg > 0)
            {
                Model.ListaAgendamentoTerminal = this.AgendamentoTerminalBLL.ListarAgendamentoTerminal(Model.Filtro, Model.PaginadorDados);
            }
        }

        private void PrepararPaginador(ref ModelAgendamentoTerminal Model)
        {
            PaginadorModel clone = Model.PaginadorDados.Clone() as PaginadorModel;
            Paginador.CalcularPaginas(clone, TipoPaginador.Modelo_Linq);
            Model.PaginadorDados = clone;
        }

        private void ZerarFiltro(ModelAgendamentoTerminal Model)
        {
            Model.Filtro = new AgendamentoTerminalFiltro();
            base.ArmazenarDados<AgendamentoTerminalFiltro>(Model.Filtro, NomeFiltro);
        }

        #endregion


        private void CarregarPermissaoUsuario(ModelAgendamentoTerminal model)
        {
            var usuario = UsuarioLogado;
            model.AgendamentoTerminalHorario = new AgendamentoTerminalHorario();
            if (usuario != null)
            {
                //se a chamada não vir pela pesquisa o filtro será null
                if (usuario.IDEmpresa != 3)
                {
                    model.AgendamentoTerminalHorario.IDEmpresa = usuario.IDEmpresa;
                    model.AgendamentoTerminalHorario.IDEmpresaUsuario = usuario.IDEmpresa;
                }
                model.AgendamentoTerminalHorario.OperacaoUsuario = model.AgendamentoTerminalHorario.Operacao = usuario.Operacao == "Ambos" ? null : usuario.Operacao;

            }
        }

        #region AdicionarHorario
        [HttpPost]
        [AjaxOnlyAttribute]
        public ActionResult AdicionarHorario(ModelAgendamentoTerminal Model)
        {
            Model.TipoValidacao = TiposValidacao.ValidacaoTotal;

            if (base.ValidarModel(Model, this.ModelState))
            {
                Model.Resultado = base.ProcessarResultado(AgendamentoTerminalBLL.AdicionarHorario(Model.AgendamentoTerminal, Model.AgendamentoTerminalHorario), OperacoesCRUD.Insert);
                Model.AgendamentoTerminalHorario.idHoraAgenda = 0;
            }
            else
            {
                var id = AgendamentoTerminalBLL.BuscarId(Model.AgendamentoTerminal);
                if (id > 0)
                    Model.AgendamentoTerminal.ListaAgendamentoTerminalHorario = AgendamentoTerminalHorarioBLL.ListarPorAgendamentoTerminal(id);
            }
            CarregarPermissaoUsuario(Model);
            return PartialView("_Edicao", Model);
        }
        #endregion

        #region Salvar

        [HttpGet]
        [AjaxOnlyAttribute]
        public ActionResult Salvar(ModelAgendamentoTerminal Model)
        {
            Model.TipoValidacao = TiposValidacao.ValidacaoTotal;
            AgendamentoTerminal agendamentoTerminal;
            if (!String.IsNullOrEmpty(Model.ChavePrimaria))
                agendamentoTerminal = AgendamentoTerminalBLL.Selecionar(Convert.ToInt32(Model.ChavePrimaria));
            else
                agendamentoTerminal = AgendamentoTerminalBLL.Selecionar(w => w.Data == Model.AgendamentoTerminal.Data
                                && w.IDTerminal == Model.AgendamentoTerminal.IDTerminal
                                && w.IDTipoAgenda == Model.AgendamentoTerminal.IDTipoAgenda);

            if (base.ValidarModel(Model, this.ModelState))
            {
                agendamentoTerminal.Ativo = Model.AgendamentoTerminal.Ativo;
                Model.Resultado = base.ProcessarResultado(this.AgendamentoTerminalBLL.Atualizar(agendamentoTerminal), OperacoesCRUD.Update);
                Model.Operacao = OperacoesCRUD.Update;
            }
            else if (agendamentoTerminal != null)
            {
                Model.AgendamentoTerminal.ListaAgendamentoTerminalHorario = AgendamentoTerminalHorarioBLL.ListarPorAgendamentoTerminal(agendamentoTerminal.ID);
            }

            return PartialView("_Edicao", Model);
        }

        #endregion

        #region ExcluirRegistro

        [HttpGet]
        [AjaxOnlyAttribute]
        public bool ExcluirRegistro(int Id)
        {
            ModelAgendamentoTerminal Model = new ModelAgendamentoTerminal();
            try
            {
                Model.Resultado = base.ProcessarResultado(this.AgendamentoTerminalBLL.Excluir(Id), OperacoesCRUD.Update);
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

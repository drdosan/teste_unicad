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
using System.IO;
using Raizen.UniCad.Model.View;

namespace Raizen.UniCad.Web.Controllers
{
    public class AgendamentoChecklistController : BaseUniCadController
    {
        #region Constantes
        private readonly AgendamentoChecklistBusiness AgendamentoChecklistBLL = new AgendamentoChecklistBusiness();
        private const string NomeFiltro = "Filtro_AgendamentoChecklist";
        private const string NomePaginador = "Paginador_AgendamentoChecklist";
        private const string TotalRegistros = "totalRegistros_AgendamentoChecklist";
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
            ModelAgendamentoChecklist Model = new ModelAgendamentoChecklist();
            Model.AgendamentoChecklist = new AgendamentoChecklist();
            Model.Resultado = new ResultadoOperacao();
            CarregarPermissaoUsuario(Model);
            return PartialView("_Edicao", Model);
        }
        #endregion

        [HttpGet]
        [AjaxOnly]
        public JsonResult BuscarEmpresasCongeneres(int idTerminal)
        {
            try
            {
                var retorno = new List<SelectListItem>();
                var lista = new TerminalEmpresaBusiness().Listar(w => w.IDTerminal == idTerminal);
                if (lista.Any())
                    lista.ForEach(x => retorno.Add(new SelectListItem { Value = x.ID.ToString(), Text = x.Nome }));
                return Json(new { result = "sucesso", list = retorno }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { result = "falha", message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        #region Editar
        [HttpGet]
        [AjaxOnlyAttribute]
        public ActionResult Editar(string Id)
        {
            ModelAgendamentoChecklist Model = new ModelAgendamentoChecklist();
            List<AgendamentoTerminalHorarioView> listaDisp = new List<AgendamentoTerminalHorarioView>();
            var bll = new AgendamentoTerminalHorarioBusiness();
            if (!string.IsNullOrEmpty(Id))
            {
                Model.Operacao = OperacoesCRUD.Editando;
                Model.ChavePrimaria = Id;
                Model.Resultado = new ResultadoOperacao();
                var placabll = new PlacaBusiness();
                Model.AgendamentoChecklist = new AgendamentoChecklist();
                Model.AgendamentoChecklist.ID = int.Parse(Model.ChavePrimaria);
                Model.AgendamentoChecklist = this.AgendamentoChecklistBLL.Selecionar(Model.AgendamentoChecklist.ID);
                ViewBag.IdTerminal = Model.AgendamentoChecklist.IDTerminal;
                ViewBag.IdEmpresaCongenere = Model.AgendamentoChecklist.IDEmpresaCongenere;
                if (Model.AgendamentoChecklist.IDEmpresaCongenere ==  0)
                {
                    var comp = new ComposicaoBusiness().Selecionar(Model.AgendamentoChecklist.IDComposicao);
                    if (comp != null)
                    {
                        if (comp.IDEmpresa == (int) EnumEmpresa.EAB ||
                            comp.IDTipoComposicao == (int) EnumTipoComposicao.Truck)
                            Model.AgendamentoChecklist.Placa =
                                new PlacaBusiness().Selecionar(comp.IDPlaca1.Value).PlacaVeiculo;
                        else
                            Model.AgendamentoChecklist.Placa =
                                new PlacaBusiness().Selecionar(comp.IDPlaca2.Value).PlacaVeiculo;

                        Model.AgendamentoChecklist.Placas =
                            placabll.Selecionar(p => p.ID == comp.IDPlaca1).PlacaVeiculo;
                        if (comp.IDPlaca2.HasValue)
                            Model.AgendamentoChecklist.Placas +=
                                " / " + placabll.Selecionar(p => p.ID == comp.IDPlaca2).PlacaVeiculo;
                        if (comp.IDPlaca3.HasValue)
                            Model.AgendamentoChecklist.Placas +=
                                " / " + placabll.Selecionar(p => p.ID == comp.IDPlaca3).PlacaVeiculo;
                        if (comp.IDPlaca4.HasValue)
                            Model.AgendamentoChecklist.Placas +=
                                " / " + placabll.Selecionar(p => p.ID == comp.IDPlaca4).PlacaVeiculo;
                    }

                    
                    listaDisp = bll.ListarAgendamentoTerminalHorarioPorTerminal(comp.IDEmpresa, comp.Operacao, Model.AgendamentoChecklist.IDTerminal, Model.AgendamentoChecklist.Data);
                    
                }
                else
                    listaDisp = bll.ListarAgendamentoTerminalHorarioPorTerminal((int)EnumEmpresa.Combustiveis, "CON", Model.AgendamentoChecklist.IDTerminal, Model.AgendamentoChecklist.Data);

                var lista = new AgendamentoTerminalHorarioBusiness().SelecionarAgendamentoTerminalHorario(Model.AgendamentoChecklist.IDAgendamentoTerminalHorario);
               
                foreach (var item in lista)
                {
                    item.Bloqueado = true;
                }
                foreach (var item in listaDisp)
                {
                    if (!lista.Any(p => p.ID == item.ID))
                    {
                        lista.Add(item);
                    }
                }
                if (Model.Operacao != OperacoesCRUD.Editando)
                    CarregarPermissaoUsuario(Model);
                Model.ListaAgendamentoTerminalHorario = lista.OrderBy(p => p.Horario).ToList();
            }

            return PartialView("_Edicao", Model);
        }

        #endregion

        #region CarregarDefault
        private ActionResult CarregarDefault()
        {
            ModelAgendamentoChecklist model = new ModelAgendamentoChecklist();
            model = AgendamentoChecklistLayout(model);
            model.Operacao = OperacoesCRUD.List;
            model.Filtro = new AgendamentoChecklistFiltro();
            model.Resultado = new ResultadoOperacao();
            model.PaginadorDados = new PaginadorModel();
            model.PaginadorDados.Status = EstadoPaginador.RenovandoConsulta;

            CarregarPermissaoUsuario(model);

            Usuario usCliente = UsuarioCsOnline;
            if (usCliente != null)
            {
                model.Filtro.IDUsuarioCliente = usCliente.ID;
            }

            this.ListarPaginador(model);

            return View("Index", model);
        }

        #endregion

        #region AgendamentoChecklistLayout
        private ModelAgendamentoChecklist AgendamentoChecklistLayout(ModelAgendamentoChecklist model)
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
        [HttpGet]
        [AjaxOnly]
        public JsonResult BuscarTerminal(string Operacao)
        {
            try
            {
                var retorno = new List<SelectListItem>();
                var lista = new TerminalBusiness().ListarHojeDepoisDeHoje(Operacao, true);
                if (lista.Any())
                    lista.ForEach(x => retorno.Add(new SelectListItem { Value = x.ID.ToString(), Text = x.Nome }));
                return Json(new { result = "sucesso", list = retorno }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { result = "falha", message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        private void ListarPaginador(ModelAgendamentoChecklist Model)
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

        private void ListarRenovandoConsulta(ModelAgendamentoChecklist Model)
        {
            base.ArmazenarDados<AgendamentoChecklistFiltro>(Model.Filtro, NomeFiltro);

            this.AtualizarQtdeRegPaginador(Model);
            int totalReg = base.RetornaDados<int>(TotalRegistros);

            Model.PaginadorDados = ModelUtils.IniciarPaginador(Model.PaginadorDados, totalReg);
            base.ArmazenarDados<int>(totalReg, TotalRegistros);
            base.ArmazenarDados<PaginadorModel>(Model.PaginadorDados, NomePaginador);

            if (totalReg > 0)
            {
                Model.ListaAgendamentoChecklist = this.AgendamentoChecklistBLL.ListarAgendamentoChecklist(Model.Filtro, Model.PaginadorDados);
            }
        }

        private void AtualizarQtdeRegPaginador(ModelAgendamentoChecklist Model)
        {
            Model.Filtro = base.RetornaDados<AgendamentoChecklistFiltro>(NomeFiltro);
            base.ArmazenarDados<AgendamentoChecklistFiltro>(Model.Filtro, NomeFiltro);

            int totalRegistros = this.AgendamentoChecklistBLL.ListarAgendamentoChecklistCount(Model.Filtro);
            base.ArmazenarDados<int>(totalRegistros, TotalRegistros);
        }

        private void Listar(ModelAgendamentoChecklist Model)
        {
            this.ListarPaginador(Model);
            Model.Resultado = base.ProcessarResultado(!Model.ListaAgendamentoChecklist.IsNullOrEmpty(), Model.Operacao);
        }
        #endregion

        #region Pesquisar
        [HttpGet]
        [AjaxOnlyAttribute]
        public ActionResult Pesquisar(ModelAgendamentoChecklist Model)
        {
            CarregarPermissaoUsuario(Model, true);
            //Model.Filtro.IDUsuarioTransportadora = GetIdUsuarioTransportadora();


            var usuario = UsuarioLogado;
            if (usuario != null)
            {
                if (usuario.IDEmpresa != 3)
                {
                    Model.Filtro.IDEmpresaUsuario = usuario.IDEmpresa;
                    Model.Filtro.IDEmpresa = usuario.IDEmpresa;
                }

                GetClienteTransportadora(Model, usuario);
            }
            Usuario usCliente = UsuarioCsOnline;
            if (usCliente != null)
            {
                Model.Filtro.IDUsuarioCliente = usCliente.ID;
            }

            this.PrepararPaginadorOperacoes(Model);
            this.Listar(Model);
            return PartialView("_Pesquisa", Model);
        }

        private void CarregarPermissaoUsuario(ModelAgendamentoChecklist model, bool isPesquisa = false)
        {
            var usuario = UsuarioLogado;
            if (usuario != null)
            {
                //se a chamada não vir pela pesquisa o filtro será null
                if (usuario.IDEmpresa != 3)
                {
                    if (model.Filtro != null)
                    {
                        model.Filtro.IDEmpresaUsuario = usuario.IDEmpresa;
                        model.Filtro.IDEmpresa = usuario.IDEmpresa;
                        GetClienteTransportadora(model, usuario);
                    }
                    else
                    {
                        model.AgendamentoChecklist.IDEmpresaUsuario = usuario.IDEmpresa;
                        model.AgendamentoChecklist.IDEmpresa = usuario.IDEmpresa;
                    }

                }
                else if (model.Filtro != null)
                    GetClienteTransportadora(model,usuario);

                //se a chamada não vir pela pesquisa o filtro será null
                if (model.Filtro == null)
                    model.AgendamentoChecklist.OperacaoUsuario = model.AgendamentoChecklist.Operacao = usuario.Operacao == "Ambos" ? null : usuario.Operacao;
                else if (isPesquisa)
                    model.Filtro.OperacaoUsuario = usuario.Operacao == "Ambos" ? null : usuario.Operacao;
                else
                    model.Filtro.Operacao = model.Filtro.OperacaoUsuario = usuario.Operacao == "Ambos" ? null : usuario.Operacao;

            }
        }

        private static void GetClienteTransportadora(ModelAgendamentoChecklist model, Usuario usuario)
        {
            if (usuario.Perfil == "Transportadora")
                model.Filtro.IDUsuarioTransportadora = usuario.ID;
            else if (usuario.Perfil == "Cliente EAB")
                model.Filtro.IDUsuarioCliente = usuario.ID;
        }


        #endregion

        #region Paginacao

        private void PrepararPaginadorOperacoes(ModelAgendamentoChecklist Model)
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

        private void ListarPaginando(ModelAgendamentoChecklist Model)
        {
            Model.Filtro = base.RetornaDados<AgendamentoChecklistFiltro>(NomeFiltro);
            base.ArmazenarDados<AgendamentoChecklistFiltro>(Model.Filtro, NomeFiltro);

            int totalReg = base.RetornaDados<int>(TotalRegistros);
            Model.PaginadorDados.QtdeTotalRegistros = totalReg;

            base.ArmazenarDados<int>(totalReg, TotalRegistros);
            this.PrepararPaginador(ref Model);

            if (totalReg > 0)
            {
                Model.ListaAgendamentoChecklist = this.AgendamentoChecklistBLL.ListarAgendamentoChecklist(Model.Filtro, Model.PaginadorDados);
            }
        }

        private void PrepararPaginador(ref ModelAgendamentoChecklist Model)
        {
            PaginadorModel clone = Model.PaginadorDados.Clone() as PaginadorModel;
            Paginador.CalcularPaginas(clone, TipoPaginador.Modelo_Linq);
            Model.PaginadorDados = clone;
        }

        private void ZerarFiltro(ModelAgendamentoChecklist Model)
        {
            Model.Filtro = new AgendamentoChecklistFiltro();
            base.ArmazenarDados<AgendamentoChecklistFiltro>(Model.Filtro, NomeFiltro);
        }

        #endregion

        #region Salvar

        [HttpGet]
        [AjaxOnlyAttribute]
        public ActionResult Salvar(ModelAgendamentoChecklist Model)
        {
            Model.TipoValidacao = TiposValidacao.ValidacaoTotal;
            Model.AgendamentoChecklist.Usuario = UsuarioLogado.Nome;
            if (base.ValidarModel(Model, this.ModelState))
            {
                if (Model.Operacao == OperacoesCRUD.Insert)
                {
                    Model.Resultado = base.ProcessarResultado(this.AgendamentoChecklistBLL.Adicionar(Model.AgendamentoChecklist), OperacoesCRUD.Insert);
                    this.ZerarFiltro(Model);
                    this.AtualizarQtdeRegPaginador(Model);
                    Model.Operacao = OperacoesCRUD.Update;
                }

                if (Model.Operacao == OperacoesCRUD.Editando)
                {
                    Model.AgendamentoChecklist.ID = int.Parse(Model.ChavePrimaria);

                    Model.Resultado = base.ProcessarResultado(this.AgendamentoChecklistBLL.Atualizar(Model.AgendamentoChecklist), OperacoesCRUD.Update);
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
            ModelAgendamentoChecklist Model = new ModelAgendamentoChecklist();
            try
            {
                Model.Resultado = base.ProcessarResultado(this.AgendamentoChecklistBLL.Excluir(Id), OperacoesCRUD.Update);
            }
            catch
            {
                return false;
            }
            return true;
        }

        #endregion

        public PartialViewResult CarregarHorarios(int IDEmpresa, string Operacao, int IDTerminal, DateTime Data, int? idAgendamentoTerminalHorario)
        {
            var bll = new AgendamentoTerminalHorarioBusiness();
            var listaDisp = bll.ListarAgendamentoTerminalHorarioPorTerminal(IDEmpresa, Operacao, IDTerminal, Data);

            if (idAgendamentoTerminalHorario.HasValue && idAgendamentoTerminalHorario > 0)
            {
                var ag = new AgendamentoTerminalHorarioBusiness().SelecionarAgendamentoTerminalHorario(idAgendamentoTerminalHorario.Value).FirstOrDefault();                
                if (ag != null )
                {
                    ag.Bloqueado = true;
                    if (listaDisp.Any(w => w.ID == ag.ID))
                        listaDisp.Where(w => w.ID == ag.ID).FirstOrDefault().Bloqueado = true;
                    else if (ag.IDEmpresa == IDEmpresa && ag.Operacao == Operacao && ag.Data == Data && ag.IDTerminal == IDTerminal)
                        listaDisp.Add(ag);
                }
                    
                
            }

            return PartialView("_Horarios", listaDisp);
        }

        public PartialViewResult HorariosEditar(int IDAgendamento)
        {
            var bll = new AgendamentoTerminalHorarioBusiness();
            var lista = bll.SelecionarAgendamentoTerminalHorario(IDAgendamento);

            return PartialView("_Horarios", lista);
        }

        public JsonResult ObterComposicao(string placa, int idEmpresa, string operacao)
        {
            JsonResult json = new JsonResult();
            var bll = new ComposicaoBusiness();
            string dados = string.Empty;
            ModelAgendamentoChecklist Model = new ModelAgendamentoChecklist();
            Model.Filtro = new AgendamentoChecklistFiltro();
            CarregarPermissaoUsuario(Model);
            var model = bll.ObterPlacas(placa, idEmpresa, operacao, Model.Filtro.IDUsuarioCliente, Model.Filtro.IDUsuarioTransportadora, out dados);
            if (model != null)
                json.Data = model;
            else
                json.Data = dados;
            return json;
        }

        public JsonResult Inscrever(ModelAgendamentoChecklist model)
        {
            try
            {
                if (model.Operacao == OperacoesCRUD.Editando)
                    model.AgendamentoChecklist.ID = int.Parse(model.ChavePrimaria);

                model.Mensagem = this.AgendamentoChecklistBLL.Validar(model.AgendamentoChecklist);
                string dados = string.Empty;
                var usuario = UsuarioLogado;
                int idUsuario = usuario != null ? usuario.ID : 0;
                model.Filtro = new AgendamentoChecklistFiltro();
                CarregarPermissaoUsuario(model);
                AgendamentoChecklistView modelo;
                if (model.AgendamentoChecklist.Operacao != "CON")
                    modelo = new ComposicaoBusiness().ObterPlacas(model.AgendamentoChecklist.Placa,
                        model.AgendamentoChecklist.IDEmpresa, model.AgendamentoChecklist.Operacao,
                        model.Filtro.IDUsuarioCliente, model.Filtro.IDUsuarioTransportadora, out dados);
                else
                {
                    if (new AgendamentoChecklistBusiness().Listar(w =>
                        w.PlacaCongenere == model.AgendamentoChecklist.PlacaCongenere &&
                        w.Data == model.AgendamentoChecklist.Data).Any())
                    {
                        dados = "Já existe agendamento ativo para essa composição!";
                    }
                }

                

                if (!string.IsNullOrEmpty(dados) && !dados.Contains("possui checklist") && (model.Operacao == OperacoesCRUD.Insert || !dados.Contains("Já existe agendamento")))
                {
                    model.Mensagem = dados;
                }
                else
                {
                    if (string.IsNullOrEmpty(model.Mensagem))
                    {
                        model.AgendamentoChecklist.Usuario = UsuarioLogado.Nome;
                        if (model.Operacao == OperacoesCRUD.Insert)
                            this.AgendamentoChecklistBLL.Adicionar(model.AgendamentoChecklist);
                        else
                            this.AgendamentoChecklistBLL.Atualizar(model.AgendamentoChecklist);
                    }
                }
            }
            catch (Exception ex)
            {
                model.Mensagem = ex.Message;
            }
            JsonResult json = new JsonResult();
            json.Data = model;

            return json;
        }

        [HttpGet]
        public FileResult Exportar(AgendamentoChecklistFiltro filtro)
        {
            var model = new ModelAgendamentoChecklist();
            model.Filtro = filtro;
            CarregarPermissaoUsuario(model);
            var fs = this.AgendamentoChecklistBLL.Exportar(filtro);
            string nomeArquivo = "Agendamento_Checklist_" + DateTime.Now.ToString("ddMMyyyyhhmmss") + ".xlsx";

            return File(fs, System.Net.Mime.MediaTypeNames.Application.Octet, nomeArquivo);


        }

        #region Imprimir
        public ActionResult GerarPdf(int id)
        {
            MemoryStream memStream = this.AgendamentoChecklistBLL.GerarPdf(id);
            string handle = Guid.NewGuid().ToString();
            TempData[handle] = memStream.ToArray();
            string nomeArquivo = "Agendamento_Checklist" + "_" + DateTime.Now.ToString("yyyyMMdd-hhmmss") + ".pdf";
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

    }

}

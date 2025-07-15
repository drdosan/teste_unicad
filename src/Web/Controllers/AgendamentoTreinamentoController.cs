using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Raizen.UniCad.Web.Models;
using Raizen.Framework.Web.MVC.Filters;
using Raizen.UniCad.Model;
using Raizen.UniCad.Model.Filtro;
using Raizen.Framework.Models;
using Raizen.Framework.Web.MVC.Models;
using Raizen.UniCad.BLL;
using Raizen.Framework.Web.MVC.Utils;
using Raizen.Framework.Utils.Extensions;
using System.IO;
using Raizen.UniCad.Extensions;

namespace Raizen.UniCad.Web.Controllers
{
    public class AgendamentoTreinamentoController : BaseUniCadController
    {
        #region Constantes
        private readonly AgendamentoTreinamentoBusiness _agendamentoTreinamentoBll = new AgendamentoTreinamentoBusiness();
        private readonly MotoristaBusiness _motoristaBll = new MotoristaBusiness();
        private readonly AgendamentoTerminalHorarioBusiness _agendamentoTerminalBll = new AgendamentoTerminalHorarioBusiness();
        private const string NomeFiltro = "Filtro_AgendamentoTreinamento";
        private const string NomePaginador = "Paginador_AgendamentoTreinamento";
        private const string TotalRegistros = "totalRegistros_AgendamentoTreinamento";

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
            ModelAgendamentoTreinamento Model = new ModelAgendamentoTreinamento();
            Model.AgendamentoTreinamento = new AgendamentoTreinamento();
            CarregarPermissaoUsuario(Model);
            Model.Resultado = new ResultadoOperacao();
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

        [HttpGet]
        [AjaxOnly]
        public JsonResult BuscarTerminal(string Operacao)
        {
            try
            {
                var retorno = new List<SelectListItem>();
                var lista = new TerminalBusiness().ListarHojeDepoisDeHoje(Operacao,false);
                if (lista.Any())
                    lista.ForEach(x => retorno.Add(new SelectListItem { Value = x.ID.ToString(), Text = x.Nome }));
                return Json(new { result = "sucesso", list = retorno }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { result = "falha", message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        [AjaxOnly]
        public JsonResult BuscarTipoAgenda(int Id)
        {
            try
            {
                var retorno = new List<SelectListItem>();
                var lista = new TipoAgendaBusiness().Listar(w => w.IDTipo == Id && w.Status);
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
            ModelAgendamentoTreinamento Model = new ModelAgendamentoTreinamento();

            if (!string.IsNullOrEmpty(Id))
            {
                Model.Operacao = OperacoesCRUD.Editando;
                Model.ChavePrimaria = Id;
                Model.Resultado = new ResultadoOperacao();
                Model.AgendamentoTreinamento = new AgendamentoTreinamento();
                Model.AgendamentoTreinamento.ID = int.Parse(Model.ChavePrimaria);
                var AgendamentoTreinamento = _agendamentoTreinamentoBll.SelecionarAgendamentoTerminalPorId(Model.AgendamentoTreinamento.ID);
                CarregarPermissaoUsuario(Model);
                if (AgendamentoTreinamento.IDEmpresaCongenere > 0)
                {
                    Model.AgendamentoTreinamento.CPFCongenere = AgendamentoTreinamento.CPF;
                    Model.AgendamentoTreinamento.NomeMotorista = AgendamentoTreinamento.Nome;
                    Model.AgendamentoTreinamento.IDEmpresaCongenere = AgendamentoTreinamento.IDEmpresaCongenere;
                    Model.AgendamentoTreinamento.Operacao = "CON";
                }
                else
                {
                    Model.AgendamentoTreinamento.IDMotorista = AgendamentoTreinamento.IDMotorista;
                    Model.AgendamentoTreinamento.CPF = AgendamentoTreinamento.CPF;
                    Model.AgendamentoTreinamento.Operacao = AgendamentoTreinamento.OperacaoMotorista;
                }
                
                Model.AgendamentoTreinamento.IDAgendamentoTerminalHorario = AgendamentoTreinamento.IDAgendamentoTerminalHorario;
                Model.AgendamentoTreinamento.IDTipoTreinamento = AgendamentoTreinamento.IDTipoTreinamento;
                ViewBag.IdTipo = AgendamentoTreinamento.IdTipo;
                ViewBag.IdTerminal = AgendamentoTreinamento.IDTerminal;
                ViewBag.IdEmpresaCongenere = AgendamentoTreinamento.IDEmpresaCongenere;
                Model.AgendamentoTreinamento.IDEmpresa = AgendamentoTreinamento.IDEmpresaMotorista;
               
                Model.AgendamentoTreinamento.IDTerminal = AgendamentoTreinamento.IDTerminal;
                Model.AgendamentoTreinamento.Data = AgendamentoTreinamento.Data;
                
                Model.NomeMotorista = AgendamentoTreinamento.Nome;
                Model.isEditar = true;
                var bll = new AgendamentoTerminalHorarioBusiness();

                var ag = bll.SelecionarAgendamentoTerminalHorario(Model.AgendamentoTreinamento.IDAgendamentoTerminalHorario).FirstOrDefault();

                var listaDisp = bll.ListarAgendamentoTerminalHorarioPorTerminalPorTipoAgenda(
                    AgendamentoTreinamento.IDEmpresaMotorista,
                    AgendamentoTreinamento.OperacaoMotorista,
                    AgendamentoTreinamento.IDTerminal,
                    AgendamentoTreinamento.IDTipoTreinamento,
                    AgendamentoTreinamento.Data);

                if (listaDisp.Any(w => w.ID == ag.ID))
                    listaDisp.Where(w => w.ID == ag.ID).FirstOrDefault().IsInscrito = true;
                else
                    listaDisp.Add(new Model.View.AgendamentoTreinamentoView
                    {
                        ID = ag.ID,
                        HoraInicio = ag.HoraInicio,
                        HoraFim = ag.HoraFim,
                        IDEmpresa = ag.IDEmpresa,
                        LinhaNegocios = ag.LinhaNegocios,
                        NumVagas = ag.NumVagas,
                        Operacao = ag.Operacao,
                        Cidade = ag.Cidade,
                        Endereco = ag.Endereco,
                        Anexo = ag.Anexo,
                        IDTerminal = ag.IDTerminal,
                        IDTipoTreinamento = ag.IDTipoAgendamento,
                        Data = ag.Data,
                        TipoTreinamento = ag.TipoTreinamento,
                        IsInscrito = true
                    });

                //VerificarSeEstaInscrito(AgendamentoTreinamento.Operacao,
                //    AgendamentoTreinamento.IDTerminal,
                //    AgendamentoTreinamento.IDEmpresa,
                //    AgendamentoTreinamento.IDTipoTreinamento,
                //    AgendamentoTreinamento.HoraInicio,
                //    AgendamentoTreinamento.HoraFim,
                //    AgendamentoTreinamento.Data,
                //    listaDisp);

                Model.ListaAgendamentoTerminalHorario = listaDisp;
            }

            return PartialView("_Edicao", Model);
        }

        #endregion

        #region CarregarDefault
        private ActionResult CarregarDefault()
        {
            ModelAgendamentoTreinamento model = new ModelAgendamentoTreinamento();
            model = AgendamentoTreinamentoLayout(model);
            model.Operacao = OperacoesCRUD.List;
            model.Filtro = new AgendamentoTreinamentoFiltro();
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

        #region AgendamentoTreinamentoLayout
        private ModelAgendamentoTreinamento AgendamentoTreinamentoLayout(ModelAgendamentoTreinamento model)
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

        [HttpGet]
        public ActionResult EditarMotorista(int? id, string cpf, int idEmpresa, string Operacao)
        {
            var model = new ModelMotorista();
            cpf = cpf.RemoveCharacter();
            model.Operacao = OperacoesCRUD.Insert;
            model.Motorista = new Motorista();
            model.Motorista.MotoristaBrasil.CPF = cpf.RemoveCharacter();
            model.Motorista.IDEmpresa = idEmpresa;
            model.Motorista.Operacao = Operacao;
            return PartialView("_EdicaoMotorista", model);
        }

        #region Listar
        public JsonResult VerificarCpfCongenereJaCadastrado(string cpf, int idEmpresa, string data, int? idtipoAgenda, int? idTerminal, bool isEditar)
        {


            ModelAgendamentoTreinamento model = new ModelAgendamentoTreinamento();

            DateTime? dataAgenda = null;
            if (!String.IsNullOrEmpty(data))
                dataAgenda = DateTime.ParseExact(data, "dd/MM/yyyy",
                    System.Globalization.CultureInfo.InvariantCulture);


            GetFiltro(cpf, idEmpresa, "CON", dataAgenda, idtipoAgenda, idTerminal, model);

            var retorno = _agendamentoTreinamentoBll.VerificarCpfCongenereJaCadastrado(model.Filtro, isEditar);
            return Json(new { id = retorno.IdMotorista, nome = retorno.NomeMotorista, situacao = retorno.Situacao, data = retorno.DataValidadeAgendamento }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult BuscarMotorista(string cpf, int idEmpresa, string operacao, string data, int? idtipoAgenda, int? idTerminal, bool isEditar)
        {


            ModelAgendamentoTreinamento model = new ModelAgendamentoTreinamento();

            DateTime? dataAgenda = null;
            if (!String.IsNullOrEmpty(data))
                dataAgenda = DateTime.ParseExact(data, "dd/MM/yyyy",
                                       System.Globalization.CultureInfo.InvariantCulture);


            GetFiltro(cpf, idEmpresa, operacao, dataAgenda, idtipoAgenda, idTerminal, model);

            var retorno = _agendamentoTreinamentoBll.BuscarMotorista(model.Filtro, isEditar);
            return Json(new { id = retorno.IdMotorista, nome = retorno.NomeMotorista, situacao = retorno.Situacao, data = retorno.DataValidadeAgendamento }, JsonRequestBehavior.AllowGet);
        }

        private void GetFiltro(string cpf, int idEmpresa, string operacao, DateTime? data, int? idtipoAgenda, int? idTerminal, ModelAgendamentoTreinamento model)
        {

            model.Filtro = new AgendamentoTreinamentoFiltro

            {
                CPF = cpf,
                IDEmpresa = idEmpresa,
                Operacao = operacao,
                Data = data,
                IDTipoTreinamento = idtipoAgenda,
                IDTerminal = idTerminal
            };

            CarregarPermissaoUsuario(model, true);
        }

        private void ListarPaginador(ModelAgendamentoTreinamento Model)
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

        private void ListarRenovandoConsulta(ModelAgendamentoTreinamento Model)
        {
            base.ArmazenarDados<AgendamentoTreinamentoFiltro>(Model.Filtro, NomeFiltro);

            this.AtualizarQtdeRegPaginador(Model);
            int totalReg = base.RetornaDados<int>(TotalRegistros);

            Model.PaginadorDados = ModelUtils.IniciarPaginador(Model.PaginadorDados, totalReg);
            base.ArmazenarDados<int>(totalReg, TotalRegistros);
            base.ArmazenarDados<PaginadorModel>(Model.PaginadorDados, NomePaginador);

            if (totalReg > 0)
            {
                Model.ListaAgendamentoTreinamento = _agendamentoTreinamentoBll.ListarAgendamentoTreinamento(Model.Filtro, Model.PaginadorDados);
            }
        }

        private void AtualizarQtdeRegPaginador(ModelAgendamentoTreinamento Model)
        {
            Model.Filtro = base.RetornaDados<AgendamentoTreinamentoFiltro>(NomeFiltro);
            base.ArmazenarDados<AgendamentoTreinamentoFiltro>(Model.Filtro, NomeFiltro);

            int totalRegistros = _agendamentoTreinamentoBll.ListarAgendamentoTreinamentoCount(Model.Filtro);
            base.ArmazenarDados<int>(totalRegistros, TotalRegistros);
        }

        private void Listar(ModelAgendamentoTreinamento Model)
        {
            this.ListarPaginador(Model);
            Model.Resultado = base.ProcessarResultado(!Model.ListaAgendamentoTreinamento.IsNullOrEmpty(), Model.Operacao);
        }
        #endregion

        private static void GetClienteTransportadora(ModelAgendamentoTreinamento model, Usuario usuario)
        {
            if (usuario.Perfil == "Transportadora")
                model.Filtro.IDUsuarioTransportadora = usuario.ID;

            //R4) Verificar se o filtro de clientes ou transportadoras deve ser filtrado pelo ID do Usuario logado no cadastro de veiculos/motorista
            else if (usuario.Perfil == "Cliente EAB" || usuario.Perfil == EnumPerfil.CLIENTE_ACS || usuario.Perfil == EnumPerfil.CLIENTE_ACS_ARGENTINA)
                model.Filtro.IDUsuarioCliente = usuario.ID;
        }

        private void CarregarPermissaoUsuario(ModelAgendamentoTreinamento model, bool isPesquisa = false)
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
                        model.AgendamentoTreinamento.IDEmpresaUsuario = usuario.IDEmpresa;
                        model.AgendamentoTreinamento.IDEmpresa = usuario.IDEmpresa;
                    }

                }
                //se a chamada não vir pela pesquisa o filtro será null
                if (model.Filtro == null)
                    model.AgendamentoTreinamento.OperacaoUsuario = model.AgendamentoTreinamento.Operacao = usuario.Operacao == "Ambos" ? null : usuario.Operacao;
                else if (isPesquisa)
                    model.Filtro.OperacaoUsuario = usuario.Operacao == "Ambos" ? null : usuario.Operacao;
                else
                    model.Filtro.Operacao = model.Filtro.OperacaoUsuario = usuario.Operacao == "Ambos" ? null : usuario.Operacao;

            }
        }

        #region Pesquisar
        [HttpGet]
        [AjaxOnlyAttribute]
        public ActionResult Pesquisar(ModelAgendamentoTreinamento Model)
        {
            CarregarPermissaoUsuario(Model, true);
            Usuario usCliente = UsuarioCsOnline;

            if (usCliente != null)
            {
                Model.Filtro.IDUsuarioCliente = usCliente.ID;
            }

            if (!string.IsNullOrEmpty(Model.Filtro.CPF))
                Model.Filtro.CPF = Model.Filtro.CPF.RemoveCharacter();
            this.PrepararPaginadorOperacoes(Model);
            this.Listar(Model);
            return PartialView("_Pesquisa", Model);
        }

        #endregion

        #region Paginacao

        private void PrepararPaginadorOperacoes(ModelAgendamentoTreinamento Model)
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

        private void ListarPaginando(ModelAgendamentoTreinamento Model)
        {
            Model.Filtro = base.RetornaDados<AgendamentoTreinamentoFiltro>(NomeFiltro);
            base.ArmazenarDados<AgendamentoTreinamentoFiltro>(Model.Filtro, NomeFiltro);

            int totalReg = base.RetornaDados<int>(TotalRegistros);
            Model.PaginadorDados.QtdeTotalRegistros = totalReg;

            base.ArmazenarDados<int>(totalReg, TotalRegistros);
            this.PrepararPaginador(ref Model);

            if (totalReg > 0)
            {
                Model.ListaAgendamentoTreinamento = _agendamentoTreinamentoBll.ListarAgendamentoTreinamento(Model.Filtro, Model.PaginadorDados);
            }
        }

        private void PrepararPaginador(ref ModelAgendamentoTreinamento Model)
        {
            PaginadorModel clone = Model.PaginadorDados.Clone() as PaginadorModel;
            Paginador.CalcularPaginas(clone, TipoPaginador.Modelo_Linq);
            Model.PaginadorDados = clone;
        }

        private void ZerarFiltro(ModelAgendamentoTreinamento Model)
        {
            Model.Filtro = new AgendamentoTreinamentoFiltro();
            base.ArmazenarDados<AgendamentoTreinamentoFiltro>(Model.Filtro, NomeFiltro);
        }

        #endregion

        #region Salvar

        [HttpGet]
        [AjaxOnlyAttribute]
        public ActionResult Salvar(ModelAgendamentoTreinamento Model)
        {
            Model.TipoValidacao = TiposValidacao.ValidacaoTotal;
            Model.AgendamentoTreinamento.Usuario = UsuarioLogado.Nome;
            if (base.ValidarModel(Model, this.ModelState))
            {
                if (Model.Operacao == OperacoesCRUD.Insert)
                {
                    Model.Resultado = base.ProcessarResultado(_agendamentoTreinamentoBll.Adicionar(Model.AgendamentoTreinamento), OperacoesCRUD.Insert);
                    this.ZerarFiltro(Model);
                    this.AtualizarQtdeRegPaginador(Model);
                    Model.Operacao = OperacoesCRUD.Update;
                }

                if (Model.Operacao == OperacoesCRUD.Editando)
                {
                    Model.AgendamentoTreinamento.ID = int.Parse(Model.ChavePrimaria);

                    Model.Resultado = base.ProcessarResultado(_agendamentoTreinamentoBll.Atualizar(Model.AgendamentoTreinamento), OperacoesCRUD.Update);
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
            ModelAgendamentoTreinamento Model = new ModelAgendamentoTreinamento();
            try
            {
                Model.Resultado = base.ProcessarResultado(this._agendamentoTreinamentoBll.Excluir(Id), OperacoesCRUD.Update);
            }
            catch
            {
                return false;
            }
            return true;
        }

        #endregion


        public PartialViewResult CarregarHorarios(int IDEmpresa, string Operacao, int IDTerminal, int IDTipoTreinamento, DateTime Data, string ID)
        {
            var bll = new AgendamentoTerminalHorarioBusiness();
            var listaDisp = bll.ListarAgendamentoTerminalHorarioPorTerminalPorTipoAgenda(IDEmpresa, Operacao, IDTerminal, IDTipoTreinamento, Data);

            //se for edição, toda vez que pesquisar os horários precisa verificar se aquele motorista já está inscrito
            if (!string.IsNullOrEmpty(ID) && listaDisp != null && listaDisp.Any())
            {
                var AgendamentoTreinamento = _agendamentoTreinamentoBll.SelecionarAgendamentoTerminalPorId(Convert.ToInt32(ID));
                var idAgendamentoTerminalHorario = AgendamentoTreinamento.IDAgendamentoTerminalHorario;

                var ag = bll.SelecionarAgendamentoTerminalHorario(idAgendamentoTerminalHorario).FirstOrDefault();
                if (ag != null)
                {
                    if (listaDisp.Any(w => w.ID == ag.ID))
                        listaDisp.Where(w => w.ID == ag.ID).FirstOrDefault().IsInscrito = true;
                    else if (ag.IDEmpresa == IDEmpresa && ag.Operacao == Operacao && ag.Data == Data && ag.IDTerminal == IDTerminal)
                        listaDisp.Add(new Model.View.AgendamentoTreinamentoView
                        {
                            ID = ag.ID,
                            HoraInicio = ag.HoraInicio,
                            HoraFim = ag.HoraFim,
                            IDEmpresa = ag.IDEmpresa,
                            LinhaNegocios = ag.LinhaNegocios,
                            NumVagas = ag.NumVagas,
                            Operacao = ag.Operacao,
                            Cidade = ag.Cidade,
                            Endereco = ag.Endereco,
                            Anexo = ag.Anexo,
                            IDTerminal = ag.IDTerminal,
                            IDTipoTreinamento = ag.IDTipoAgendamento,
                            Data = ag.Data,
                            TipoTreinamento = ag.TipoTreinamento,
                            IsInscrito = true
                        });
                }


                //VerificarSeEstaInscrito(AgendamentoTreinamento.Operacao, AgendamentoTreinamento.IDTerminal, AgendamentoTreinamento.IDEmpresa, AgendamentoTreinamento.IDTipoTreinamento, AgendamentoTreinamento.HoraInicio, AgendamentoTreinamento.HoraFim, AgendamentoTreinamento.Data, listaDisp);
            }


            return PartialView("_Horarios", listaDisp);
        }


        [AjaxOnly]
        public JsonResult Inscrever(ModelAgendamentoTreinamento model)
        {
            try
            {
                if (model.Operacao == OperacoesCRUD.Editando)
                    model.AgendamentoTreinamento.ID = int.Parse(model.ChavePrimaria);

                if (model.AgendamentoTreinamento.Operacao != "CON")
                {
                    //validar quantidade de vagas e situação do motorista
                    GetFiltro(model.AgendamentoTreinamento.CPF,
                        model.AgendamentoTreinamento.IDEmpresa,
                        model.AgendamentoTreinamento.Operacao,
                        model.AgendamentoTreinamento.Data,
                        model.AgendamentoTreinamento.IDTipoTreinamento,
                        model.AgendamentoTreinamento.IDTerminal,
                        model);

                    model.Mensagem =
                        _agendamentoTreinamentoBll.Validar(model.AgendamentoTreinamento, model.Filtro, model.isEditar);
                }
                else
                    model.AgendamentoTreinamento.CPFCongenere =
                        model.AgendamentoTreinamento.CPFCongenere.RemoveSpecialCharacters();

                if (string.IsNullOrEmpty(model.Mensagem))
                {
                    model.AgendamentoTreinamento.Usuario = UsuarioLogado.Login;
                    if (model.Operacao == OperacoesCRUD.Editando)
                        _agendamentoTreinamentoBll.Atualizar(model.AgendamentoTreinamento);
                    else
                        _agendamentoTreinamentoBll.Adicionar(model.AgendamentoTreinamento);
                }
            }
            catch (Exception ex)
            {
                model.Mensagem = "Problemas ao salvar o agendamento " + ex.Message;
            }
            JsonResult json = new JsonResult();
            json.Data = model;

            return json;
        }

        [HttpGet]
        public FileResult Exportar(AgendamentoTreinamentoFiltro filtro)
        {

            var fs = _agendamentoTreinamentoBll.Exportar(filtro);
            string nomeArquivo = "Agendamento_Treinamento_" + DateTime.Now.ToString("ddMMyyyyhhmmss") + ".xlsx";

            return File(fs, System.Net.Mime.MediaTypeNames.Application.Octet, nomeArquivo);


        }

        #region Imprimir
        public ActionResult GerarPdf(int id)
        {
            MemoryStream memStream = _agendamentoTreinamentoBll.GerarPdf(id);
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

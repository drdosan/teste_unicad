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
using Raizen.UniCad.BLL.Util;
using Raizen.UniCad.BLL.Log;
using Raizen.Framework.Log.Bases;

namespace Raizen.UniCad.Web.Controllers
{
    public class ConfiguracaoController : BaseUniCadController
    {
        #region Constantes
        private readonly ConfiguracaoBusiness ConfiguracaoBLL = new ConfiguracaoBusiness();        
        private readonly LogSincronizacaoBusiness SincronizacaoBLL = new LogSincronizacaoBusiness();        
        private readonly JobBusiness jobBLL = new JobBusiness();
        private const string NomeFiltro = "Filtro_Configuracao";
        private const string NomePaginador = "Paginador_Configuracao";
        private const string TotalRegistros = "totalRegistros_Configuracao";
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
            ModelConfiguracao Model = new ModelConfiguracao();
            Model.Configuracao = new Configuracao();
            Model.Resultado = new ResultadoOperacao();
            ConfiguraDropDownList();
            return PartialView("_Edicao", Model);
        }
        #endregion

        #region Editar
        [HttpGet]
        [AjaxOnlyAttribute]
        public ActionResult Editar(string Id)
        {
            ModelConfiguracao Model = new ModelConfiguracao();

            if (!string.IsNullOrEmpty(Id))
            {
                Model.Operacao = OperacoesCRUD.Editando;
                Model.ChavePrimaria = Id;
                Model.Resultado = new ResultadoOperacao();

                Model.Configuracao = new Configuracao();
                Model.Configuracao.ID = int.Parse(Model.ChavePrimaria);
                Model.Configuracao = this.ConfiguracaoBLL.Selecionar(Model.Configuracao.ID);
            }
            ConfiguraDropDownList();
            return PartialView("_Edicao", Model);
        }

        public JsonResult CarregarJobs()
        {
            ModelJob Model = new ModelJob();
            var Jobs = this.jobBLL.Listar();
            var jsonData = new { rows = Jobs };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        #endregion

        
        #region CarregarDefault
        private ActionResult CarregarDefault()
        {
            ModelConfiguracao model = new ModelConfiguracao();
            model = ConfiguracaoLayout(model);
            model.Operacao = OperacoesCRUD.List;
            model.Filtro = new ConfiguracaoFiltro();
            model.Resultado = new ResultadoOperacao();
            model.PaginadorDados = new PaginadorModel();
            model.PaginadorDados.Status = EstadoPaginador.RenovandoConsulta;
            model.UsuarioLogado = UsuarioLogado;
            this.ListarRenovandoConsulta(model);
            ConfiguraDropDownList();

            return View("Index", model);
        }

        private void ConfiguraDropDownList()
        {
            using (PaisBusiness paisBusiness = new PaisBusiness())
            {
                List<Pais> paises = paisBusiness.Listar().OrderBy(p => p.ID).ToList();
                ViewBag.ddlPaises = new SelectList(paises, "ID", "Nome");
            }
        }

        #endregion

        #region ConfiguracaoLayout

        private ModelConfiguracao ConfiguracaoLayout(ModelConfiguracao model)
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

        private void ListarPaginador(ModelConfiguracao Model)
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

      

        private void ListarRenovandoConsulta(ModelConfiguracao Model)
        {
            base.ArmazenarDados<ConfiguracaoFiltro>(Model.Filtro, NomeFiltro);

            this.AtualizarQtdeRegPaginador(Model);
            int totalReg = base.RetornaDados<int>(TotalRegistros);

            Model.PaginadorDados = ModelUtils.IniciarPaginador(Model.PaginadorDados, totalReg);
            base.ArmazenarDados<int>(totalReg, TotalRegistros);
            base.ArmazenarDados<PaginadorModel>(Model.PaginadorDados, NomePaginador);

            if (totalReg > 0)
            {
                Model.ListaConfiguracao = this.ConfiguracaoBLL.ListarConfiguracao(Model.Filtro, Model.PaginadorDados);
            }
        }
     

        private void AtualizarQtdeRegPaginador(ModelConfiguracao Model)
        {
            Model.Filtro = base.RetornaDados<ConfiguracaoFiltro>(NomeFiltro);
            base.ArmazenarDados<ConfiguracaoFiltro>(Model.Filtro, NomeFiltro);

            int totalRegistros = this.ConfiguracaoBLL.ListarConfiguracaoCount(Model.Filtro);
            base.ArmazenarDados<int>(totalRegistros, TotalRegistros);
        }

        private void Listar(ModelConfiguracao Model)
        {
            this.ListarPaginador(Model);
            Model.Resultado = base.ProcessarResultado(!Model.ListaConfiguracao.IsNullOrEmpty(), Model.Operacao);
        }
        #endregion

        #region Pesquisar
        [HttpGet]
        [AjaxOnlyAttribute]
        public ActionResult Pesquisar(ModelConfiguracao Model)
        {
            this.PrepararPaginadorOperacoes(Model);
            this.Listar(Model);
            ConfiguraDropDownList();
            Model.UsuarioLogado = UsuarioLogado;
            return PartialView("_Pesquisa", Model);
        }

        #endregion

        #region Paginacao

        private void PrepararPaginadorOperacoes(ModelConfiguracao Model)
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

        private void ListarPaginando(ModelConfiguracao Model)
        {
            Model.Filtro = base.RetornaDados<ConfiguracaoFiltro>(NomeFiltro);
            base.ArmazenarDados<ConfiguracaoFiltro>(Model.Filtro, NomeFiltro);

            int totalReg = base.RetornaDados<int>(TotalRegistros);
            Model.PaginadorDados.QtdeTotalRegistros = totalReg;

            base.ArmazenarDados<int>(totalReg, TotalRegistros);
            this.PrepararPaginador(ref Model);

            if (totalReg > 0)
            {
                Model.ListaConfiguracao = this.ConfiguracaoBLL.ListarConfiguracao(Model.Filtro, Model.PaginadorDados);
            }
        }

        private void PrepararPaginador(ref ModelConfiguracao Model)
        {
            PaginadorModel clone = Model.PaginadorDados.Clone() as PaginadorModel;
            Paginador.CalcularPaginas(clone, TipoPaginador.Modelo_Linq);
            Model.PaginadorDados = clone;
        }

        private void ZerarFiltro(ModelConfiguracao Model)
        {
            Model.Filtro = new ConfiguracaoFiltro();
            base.ArmazenarDados<ConfiguracaoFiltro>(Model.Filtro, NomeFiltro);
        }

        #endregion

        #region Salvar

        public string SalvarJob(Job job)
        {
            try
            {
                jobBLL.CorrigirID(job);
                jobBLL.Atualizar(job);
                return "1";
            }
            catch(Exception ex)
            {
                return ex.Message;
            }
        }

        [AjaxOnlyAttribute]
        [ValidateInput(false)]
        public ActionResult Salvar(ModelConfiguracao Model)
        {
            Model.TipoValidacao = TiposValidacao.ValidacaoTotal;
            Model.UsuarioLogado = UsuarioLogado;
            if (base.ValidarModel(Model, this.ModelState))
            {
                Model.Configuracao.DtAtualizacao = DateTime.Now;
                Model.Configuracao.Valor = string.IsNullOrEmpty(Model.Configuracao.Valor) ? "" : Model.Configuracao.Valor;
                if (Model.Operacao == OperacoesCRUD.Insert)
                {
                    Model.Configuracao.DtCriacao = DateTime.Now;
                    Model.Resultado = base.ProcessarResultado(this.ConfiguracaoBLL.Adicionar(Model.Configuracao), OperacoesCRUD.Insert);
                    this.ZerarFiltro(Model);
                    this.AtualizarQtdeRegPaginador(Model);
                    Model.Operacao = OperacoesCRUD.Update;
                }

                if (Model.Operacao == OperacoesCRUD.Editando)
                {
                    Model.Configuracao.ID = int.Parse(Model.ChavePrimaria);

                    Model.Resultado = base.ProcessarResultado(this.ConfiguracaoBLL.Atualizar(Model.Configuracao), OperacoesCRUD.Update);
                    Model.Operacao = OperacoesCRUD.Update;
                }
            }
            ConfiguraDropDownList();
            return PartialView("_Edicao", Model);
        }


        #endregion

        #region ExcluirRegistro

        [HttpGet]
        [AjaxOnlyAttribute]
        public bool ExcluirRegistro(int Id)
        {
            ModelConfiguracao Model = new ModelConfiguracao();
            try
            {
                Model.Resultado = base.ProcessarResultado(this.ConfiguracaoBLL.Excluir(Id), OperacoesCRUD.Update);
            }
            catch
            {
                return false;
            }
            return true;
        }

        #endregion        
        [AjaxOnlyAttribute]
        public string ExecutarJob(int id)
        {
            try
            {
                ILogExecucao logger;
                switch (id)
                {
                    case (int)EnumJob.Cliente:
                        //COMBUSTÍVEIS
                        Configuracao dataInicialCliente = ConfiguracaoBLL.Selecionar(w => w.NmVariavel == "dataInicialConsultaCliente");
                        DateTime? dtInicialCliente = null;
                        dtInicialCliente = DateTime.ParseExact(dataInicialCliente.Valor, "yyyy-MM-dd HH:mm",
                                               System.Globalization.CultureInfo.InvariantCulture);
                        new ClienteBusiness().Importar(dtInicialCliente, EnumEmpresa.Combustiveis);

                        //EAB
                        Configuracao dataInicialClienteEAB = ConfiguracaoBLL.Selecionar(w => w.NmVariavel == "dataInicialConsultaClienteEAB");
                        DateTime? dtInicialClienteEAB = null;
                        dtInicialClienteEAB = DateTime.ParseExact(dataInicialClienteEAB.Valor, "yyyy-MM-dd HH:mm",
                                               System.Globalization.CultureInfo.InvariantCulture);
                        new ClienteBusiness().Importar(dtInicialClienteEAB, EnumEmpresa.EAB);
                        break;                    
                    case (int)EnumJob.Transportadora:
                        //COMBUSTÍVEIS
                        Configuracao dataInicialTransportadora = ConfiguracaoBLL.Selecionar(w => w.NmVariavel == "dataInicialConsultaTransportador");
                        DateTime? dtInicialTransportadora = null;
                        dtInicialTransportadora = DateTime.ParseExact(dataInicialTransportadora.Valor, "yyyy-MM-dd HH:mm",
                                               System.Globalization.CultureInfo.InvariantCulture);
                        new TransportadoraBusiness().Importar(dtInicialTransportadora, EnumEmpresa.Combustiveis);

                        //EAB
                        Configuracao dataInicialTransportadoraEAB = ConfiguracaoBLL.Selecionar(w => w.NmVariavel == "dataInicialConsultaTransportadorEAB");
                        DateTime? dtInicialTransportadoraEAB = null;
                        dtInicialTransportadoraEAB = DateTime.ParseExact(dataInicialTransportadoraEAB.Valor, "yyyy-MM-dd HH:mm",
                                               System.Globalization.CultureInfo.InvariantCulture);
                        new TransportadoraBusiness().Importar(dtInicialTransportadoraEAB, EnumEmpresa.EAB);
                        break;
                    case (int)EnumJob.IntegrarComposicaoPendenteSAP:
                        new ComposicaoBusiness().IntegrarComposicaoPendenteSap();
                        new ComposicaoBusiness(EnumPais.Argentina).IntegrarComposicaoPendenteSap();
                        break;
                    case (int)EnumJob.ExcluirArquivos:
                        new ArquivoBusiness().ProcessarArquivos();
                        break;
                    case (int)EnumJob.SincronizarMotoritas:
                        return new LogSincronizacaoBusiness().Sincronizar();
                    case (int)EnumJob.EnviarEmailTeste:
                        var email = Config.GetConfig(EnumConfig.emailDebug, (int)EnumPais.Padrao);

                        if(!string.IsNullOrEmpty(email))
                        {
                            Email.Enviar("email", "Teste de envio de e-mail UNICAD", "Teste de envio de e-mail.");
                        }
                        else
                        {
                            throw new RaizenException("E-mail debug não configurado. Informe um e-mail debug e teste novamente.");
                        }

                        break;
                    case (int)EnumJob.ComposicaoDocumentoAlerta:
                        logger = new LogExecucao("ProcessarAlertaDocumentosComposicao", true);
                        string dataInicialDocumentoAlerta = Config.GetConfig(EnumConfig.DataInicialDocumento, (int)EnumPais.Padrao);
                        string habilitarLogDetalhadoJobAlerta = Config.GetConfig(EnumConfig.HabilitarLogDetalhadoJob, (int)EnumPais.Padrao);

                        var pdBusiness = new PlacaDocumentoBusiness();

                        bool definirLogAlerta = false;
                        if (bool.TryParse(habilitarLogDetalhadoJobAlerta, out definirLogAlerta) && definirLogAlerta)
                        {
                            pdBusiness.DefinirLogger(logger);
                        }

                        DateTime dtDataInicial;
                        var okParse = DateTime.TryParseExact(dataInicialDocumentoAlerta, "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out dtDataInicial);

                        if (okParse)
                            pdBusiness.ProcessarAlertaDocumentosComposicao(dtDataInicial);
                        else
                            throw new RaizenException("Data inicial inválida");
                        break;
                    case (int)EnumJob.ComposicaoDocumento:
                        logger = new LogExecucao("ProcessarDocumentosVencidosComposicao", true);
                        string dataInicialComposicaoDocumento = Config.GetConfig(EnumConfig.DataInicialDocumento, (int)EnumPais.Padrao);
                        string habilitarLogDetalhadoJobComposicaoDocumentoVencido = Config.GetConfig(EnumConfig.HabilitarLogDetalhadoJob, (int)EnumPais.Padrao);

                        var placaDoctoBusiness = new PlacaDocumentoBusiness();

                        bool definirLogComposicaoDocumento = false;
                        if (bool.TryParse(habilitarLogDetalhadoJobComposicaoDocumentoVencido, out definirLogComposicaoDocumento) && definirLogComposicaoDocumento)
                        {
                            placaDoctoBusiness.DefinirLogger(logger);
                        }

                        DateTime dtInicial;
                        var okParseData = DateTime.TryParseExact(dataInicialComposicaoDocumento, "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out dtInicial);

                        if (okParseData)
                            placaDoctoBusiness.ProcessarDocumentosVencidos(dtInicial);
                        else
                            throw new RaizenException("Data inicial inválida");
                        break;
                    case (int)EnumJob.MotoristaDocumentoAlerta:
                        logger = new LogExecucao("ProcessarAlertaDocumentosMotorista", true);
                        string dataInicialMotoristaDocumento = Config.GetConfig(EnumConfig.DataInicialDocumento, (int)EnumPais.Padrao);
                        string habilitarLogDetalhadoJobMotoristaDocumentoVencido = Config.GetConfig(EnumConfig.HabilitarLogDetalhadoJob, (int)EnumPais.Padrao);

                        var motoristaDocBusiness = new MotoristaDocumentoBusiness();

                        bool definirLogMotoristaDocumento = false;
                        if (bool.TryParse(habilitarLogDetalhadoJobMotoristaDocumentoVencido, out definirLogMotoristaDocumento) && definirLogMotoristaDocumento)
                        {
                            motoristaDocBusiness.DefinirLogger(logger);
                        }

                        DateTime dataInicial;
                        var okParseDataa = DateTime.TryParseExact(dataInicialMotoristaDocumento, "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out dataInicial);

                        if (okParseDataa)
                            motoristaDocBusiness.ProcessarAlertaDocumentosMotorista(dataInicial);
                        else
                            throw new RaizenException("Data inicial inválida");
                        break;
                    case (int)EnumJob.MotoristaDocumentoVencido:
                        logger = new LogExecucao("ProcessarDocumentosVencidosMotorista", true);
                        string dataInicialMotoristaDocumentoVencido = Config.GetConfig(EnumConfig.DataInicialDocumento, (int)EnumPais.Padrao);
                        string habilitarLogDetalhadoJobMotoristaDocumentosVencidos = Config.GetConfig(EnumConfig.HabilitarLogDetalhadoJob, (int)EnumPais.Padrao);

                        var motoristaDoctoBusiness = new MotoristaDocumentoBusiness();

                        bool definirLogMotoritaDocumento = false;
                        if (bool.TryParse(habilitarLogDetalhadoJobMotoristaDocumentosVencidos, out definirLogMotoritaDocumento) && definirLogMotoritaDocumento)
                        {
                            motoristaDoctoBusiness.DefinirLogger(logger);
                        }

                        DateTime dataInicialProcessento;
                        var okParseDataInicialProcessamento = DateTime.TryParseExact(dataInicialMotoristaDocumentoVencido, "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out dataInicialProcessento);

                        if (okParseDataInicialProcessamento)
                            motoristaDoctoBusiness.ProcessarDocumentosVencidos(dataInicialProcessento);
                        else
                            throw new RaizenException("Data inicial inválida");
                        break;
                }
                return "1";
            }
            catch(Exception ex)
            {
                return ex.Message;
            }
        }
    }
}

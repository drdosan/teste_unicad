using Raizen.Framework.Models;
using Raizen.Framework.UserSystem.Client;
using Raizen.Framework.Utils.Extensions;
using Raizen.Framework.Web.MVC.Filters;
using Raizen.Framework.Web.MVC.Models;
using Raizen.Framework.Web.MVC.Utils;
using Raizen.UniCad.BLL;
using Raizen.UniCad.BLL.Util;
using Raizen.UniCad.Extensions;
using Raizen.UniCad.Model;
using Raizen.UniCad.Model.Filtro;
using Raizen.UniCad.Model.View;
using Raizen.UniCad.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Raizen.UniCad.Web.Controllers
{
    public class PlacaController : BaseUniCadController
    {
        #region Constantes

        private readonly PlacaBusiness PlacaBLL = new PlacaBusiness();
        private const string NomeFiltro = "Filtro_Placa";
        private const string NomePaginador = "Paginador_Placa";
        private const string TotalRegistros = "totalRegistros_Placa";

        #endregion Constantes

        public PlacaController() : base(BaseControllerOptions.NaoValidarAcesso)
        {
        }


        #region Index

        [HttpGet]
        public ActionResult Index()
        {
            return this.CarregarDefault();
        }

        #endregion Index

        #region Novo

        [HttpGet]
        [AjaxOnly]
        public ActionResult Novo(int? tipoVeiculo, int idTipoParteVeiculo, int idTipoComposicao, string operacaoFrete, int linhaNegocio, int idPais)
        {
            ModelPlaca Model = new ModelPlaca();
            Model.Novo = true;
            Model.Placa = new Placa();
            Model.Placa.Setas = new List<PlacaSeta>();
            if (linhaNegocio == (int)EnumEmpresa.Combustiveis)
                Model.Placa.Setas.Add(new PlacaSeta() { Colunas = 1, Sequencial = 1, VolumeCompartimento1 = 0 });
            else
                Model.Placa.Setas.Add(new PlacaSeta() { Colunas = 1, Sequencial = 1, VolumeCompartimento1 = 0, LacreCompartimento1 = 0 });
            if (tipoVeiculo.HasValue)
                Model.Placa.IDTipoVeiculo = tipoVeiculo.Value;
            Model.Placa.Operacao = operacaoFrete;
            if (linhaNegocio != (int)EnumEmpresa.Combustiveis)
                Model.Placa.IDTipoProduto = (int)EnumTipoProduto.Claros;
            Model.Placa.LinhaNegocio = linhaNegocio;
            Model.Placa.idTipoParteVeiculo = idTipoParteVeiculo;
            Model.Placa.idTipoComposicao = idTipoComposicao;
            Model.Resultado = new ResultadoOperacao();
            Model.PlacaOficial = new PlacaView();
            Model.Placa.PlacaBrasil = new PlacaBrasil();

            var loginUsuario = UsuarioCsOnline != null && UsuarioCsOnline.Login != null ? UsuarioCsOnline.Login : UserSession.GetCurrentInfoUserSystem().InformacoesUsuario.Login;
            var usuario = new UsuarioBusiness().Selecionar(w => w.Login == loginUsuario);
            if (usuario != null)
            {
                Model.Usuario = usuario;
                Model.Placa.idUsuario = usuario.ID;
                usuario.Transportadoras = new UsuarioTransportadoraBusiness().ListarTransportadorasPorUsuario(usuario.ID, linhaNegocio);
                usuario.Clientes = new UsuarioClienteBusiness().ListarClientesPorUsuario(usuario.ID, linhaNegocio);

                if (usuario.Externo)
                {

                    if ((Model.Placa.Operacao == "CIF") && usuario.Transportadoras != null && usuario.Transportadoras.Count == 1)
                    {
                        Model.Placa.IDTransportadora = usuario.Transportadoras.First().IDTransportadora;
                    }

                    if (Model.Placa.Operacao == "FOB" && linhaNegocio == (int)EnumEmpresa.EAB && usuario.Clientes != null && usuario.Clientes.Any())
                    {
                        Model.Placa.Clientes = new List<PlacaClienteView>();
                        foreach (var item in usuario.Clientes)
                        {
                            Model.Placa.Clientes.Add(new PlacaClienteView() { IDCliente = item.IDCliente, RazaoSocial = item.IBM + " - " + item.CPF_CNPJ + " - " + item.RazaoSocial });
                        }
                    }
                }
            }

            return PartialView("_Edicao", Model);
        }

        #endregion Novo

        #region Editar

        [HttpGet]
        public ActionResult Editar(string Id, string IdplacaOficial, bool Aprovar, string operacaoComposicao, string idComposicao, int linhaNegocio, int numero, int idTipoComposicao)
        {
            ModelPlaca Model = new ModelPlaca();
            var loginUsuario = UsuarioCsOnline != null && UsuarioCsOnline.Login != null ? UsuarioCsOnline.Login : UserSession.GetCurrentInfoUserSystem().InformacoesUsuario.Login;
            var usuario = new UsuarioBusiness().Selecionar(w => w.Login == loginUsuario);
            if (usuario == null)
                throw new Exception("Usuário não cadastrado no UNICAD, entre em contato com o responsável");
            if (!string.IsNullOrEmpty(Id))
            {
                Model.Operacao = OperacoesCRUD.Editando;
                Model.ChavePrimaria = Id;
                Model.Resultado = new ResultadoOperacao();
                int IDUsuarioCliente = 0;

                //R5) Verificar se deve associar o ID do usuário logado ao salvar a placa
                if (usuario.Perfil == "Cliente EAB" || usuario.Perfil == EnumPerfil.CLIENTE_ACS || usuario.Perfil == EnumPerfil.CLIENTE_ACS_ARGENTINA)
                    IDUsuarioCliente = usuario.ID;

                Model.Placa = new Placa();
                Model.Placa.ID = int.Parse(Model.ChavePrimaria);
                Model.Placa = this.PlacaBLL.Selecionar(Model.Placa.ID);
                Model.Placa.LinhaNegocio = linhaNegocio;
                Model.Placa.Numero = numero;
                Model.Placa.idTipoParteVeiculo = numero;
                Model.Placa.idTipoComposicao = idTipoComposicao;

                Model.Aprovar = Aprovar;
                Model.Placa.Clientes = new PlacaClienteBusiness().ListarClientesPorPlaca(Model.Placa.ID, IDUsuarioCliente).OrderByDescending(o => o.DataAprovacao).ToList();
                Model.Placa.Setas = new PlacaSetaBusiness().Listar(p => p.IDPlaca == Model.Placa.ID);
                Model.Placa.Documentos = new PlacaDocumentoBusiness().ListarPlacaDocumentoPorPlaca(Model.Placa.ID);
                if (!String.IsNullOrEmpty(IdplacaOficial) && IdplacaOficial != "0")
                {
                    Model.Placa.idPlacaOficial = Convert.ToInt32(IdplacaOficial);
                    CarregarAlteracoesDePlacas(Model.Placa.idPlacaOficial.Value, Model);
                }
                if (Model.Placa.Setas == null || !Model.Placa.Setas.Any())
                {
                    Model.Placa.Setas = new List<PlacaSeta>();
                    Model.Placa.Setas.Add(new PlacaSeta() { Colunas = 1, Sequencial = 1 });
                }

                CalcularColunas(Model);
            }
            else
            {
                Model.Placa = new Placa();
                Model.Placa.Setas = new List<PlacaSeta>();
                Model.Placa.Setas.Add(new PlacaSeta() { Colunas = 1, Sequencial = 1, VolumeCompartimento1 = 0.00m });
            }
            Model.Placa.idUsuario = usuario.ID;
            //var login = UserSession.GetCurrentInfoUserSystem().InformacoesUsuario.Login;
            //var tipo = new UsuarioBusiness().Selecionar(p => p.Login == login);
            //if (tipo != null)
            //{
            //    Model.Placa.Operacao = tipo.Operacao;
            //}

            //validar o acesso somente da placa aprovada, pois se ele modificar a placa e já salvar, logo terá acesso aquela placa
            Placa placaAprovada = null;
            if (Model.Placa != null && Model.Placa.ID > 0)
                placaAprovada = new PlacaBusiness().SelecionarPlacaComposicaoAprovada(Model.Placa);
            var acesso = new PlacaBusiness().ValidarAcesso(UserSession.GetCurrentInfoUserSystem().InformacoesUsuario.Login, UsuarioCsOnline, placaAprovada);

            Model.Placa.SomenteLiberacaoAcesso = !acesso;


            if (usuario != null)
                Model.Usuario = usuario;

            var composicao = string.IsNullOrEmpty(idComposicao) ? null : new ComposicaoBusiness().Selecionar(Convert.ToInt32(idComposicao));

            var composicaoAprovado = composicao == null ? false : composicao.IDStatus == (int)EnumStatusComposicao.Aprovado;
            Model.Placa.StatusComposicao = composicao == null ? 0 : composicao.IDStatus;
            if (Model.Placa.Operacao != "Ambos")
                Model.Placa.Operacao = operacaoComposicao;

            if ((composicaoAprovado || Model.Placa.StatusComposicao == 0) && (!Model.Placa.SomenteLiberacaoAcesso || Model.Placa.Operacao == "FOB"))
            {
                //Model.Placa.ID = 0;
                Model.ChavePrimaria = "0";
                Model.Operacao = OperacoesCRUD.Insert;


                if (Model.Placa.Setas != null)
                    Model.Placa.Setas.ForEach(x => x.ID = 0);
            }

            if (!acesso)
            {
                if (Model.Placa.Operacao == "CIF")
                {
                    return PartialView("Permissao", Model);
                }
                else
                {
                    Model.Placa.SomenteLiberacaoAcesso = false;
                }
            }
            return PartialView("_Edicao", Model);

        }

        private void CarregarAlteracoesDePlacas(int IdplacaOficial, ModelPlaca Model)
        {
            Model.PlacaOficial = PlacaBLL.SelecionarPlacaCompleta(new ComposicaoFiltro { IdPlacaOficial = Model.Placa.idPlacaOficial.Value });
            //se a placaOficial estiver nula, significa que o status dela não é mais de aprovada
            if (Model.PlacaOficial != null)
                Model.Placa.PlacaAlteracoes = PlacaBLL.ListarAlteracoes(Model.Placa.ID, Model.Placa.idPlacaOficial.Value);

        }

        private static void CalcularColunas(ModelPlaca Model)
        {
            new PlacaBusiness().CalcularColunas(Model.Placa);
        }

        #endregion Editar

        #region CarregarDefault

        private ActionResult CarregarDefault()
        {
            ModelPlaca model = new ModelPlaca();
            model = PlacaLayout(model);
            model.Operacao = OperacoesCRUD.List;
            model.Filtro = new PlacaFiltro();
            model.Filtro.Status = true;
            model.Resultado = new ResultadoOperacao();
            model.PaginadorDados = new PaginadorModel();
            model.PaginadorDados.Status = EstadoPaginador.RenovandoConsulta;
            this.ListarPaginador(model);

            return View("Index", model);
        }

        #endregion CarregarDefault

        #region PlacaLayout

        private ModelPlaca PlacaLayout(ModelPlaca model)
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

        #endregion PlacaLayout

        #region Listar
        public PartialViewResult AbrirPopupQuestion(string idControle)
        {
            var model = new PopupQuestionModelView();

            switch (idControle)
            {
                case "Placa_Tara_Help":
                    model.imagem = Config.GetConfigAnexo(EnumConfig.ArquivoAjudaTara);
                    break;
                case "Placa_NumeroEixos":
                    model.imagem = Config.GetConfigAnexo(EnumConfig.ArquivoAjudaNumEixos);
                    break;
                case "Placa_IDCategoriaVeiculo":
                    model.imagem = Config.GetConfigAnexo(EnumConfig.ArquivoAjudaCategoria);
                    break;
                default:
                    break;
            }

            model.ExibirEmHex64 = true;
            model.ImagemEmHex64 = base.ImageInByteArray(model.imagem);

            return PartialView("_PopupAjuda", model);
        }
        private void ListarPaginador(ModelPlaca Model)
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

        private void ListarRenovandoConsulta(ModelPlaca Model)
        {
            base.ArmazenarDados<PlacaFiltro>(Model.Filtro, NomeFiltro);

            this.AtualizarQtdeRegPaginador(Model);
            int totalReg = base.RetornaDados<int>(TotalRegistros);

            Model.PaginadorDados = ModelUtils.IniciarPaginador(Model.PaginadorDados, totalReg);
            base.ArmazenarDados<int>(totalReg, TotalRegistros);
            base.ArmazenarDados<PaginadorModel>(Model.PaginadorDados, NomePaginador);

            if (totalReg > 0)
            {
                Model.ListaPlaca = this.PlacaBLL.ListarPlacaSemComposicao(Model.Filtro, Model.PaginadorDados);
            }
        }

        private void AtualizarQtdeRegPaginador(ModelPlaca Model)
        {
            Model.Filtro = base.RetornaDados<PlacaFiltro>(NomeFiltro);
            base.ArmazenarDados<PlacaFiltro>(Model.Filtro, NomeFiltro);

            int totalRegistros = this.PlacaBLL.ListarPlacaSemComposicaoCount(Model.Filtro);
            base.ArmazenarDados<int>(totalRegistros, TotalRegistros);
        }

        private void Listar(ModelPlaca Model)
        {
            this.ListarPaginador(Model);
            Model.Resultado = base.ProcessarResultado(!Model.ListaPlaca.IsNullOrEmpty(), Model.Operacao);
        }

        #endregion Listar

        #region Pesquisar

        [HttpGet]
        [AjaxOnlyAttribute]
        public ActionResult Pesquisar(ModelPlaca Model)
        {
            this.PrepararPaginadorOperacoes(Model);
            this.Listar(Model);
            return PartialView("_Pesquisa", Model);
        }

        #endregion Pesquisar

        #region Paginacao

        private void PrepararPaginadorOperacoes(ModelPlaca Model)
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

        private void ListarPaginando(ModelPlaca Model)
        {
            Model.Filtro = base.RetornaDados<PlacaFiltro>(NomeFiltro);
            base.ArmazenarDados<PlacaFiltro>(Model.Filtro, NomeFiltro);

            int totalReg = base.RetornaDados<int>(TotalRegistros);
            Model.PaginadorDados.QtdeTotalRegistros = totalReg;

            base.ArmazenarDados<int>(totalReg, TotalRegistros);
            this.PrepararPaginador(ref Model);

            if (totalReg > 0)
            {
                Model.ListaPlaca = this.PlacaBLL.ListarPlacaSemComposicao(Model.Filtro, Model.PaginadorDados);
            }
        }

        private void PrepararPaginador(ref ModelPlaca Model)
        {
            PaginadorModel clone = Model.PaginadorDados.Clone() as PaginadorModel;
            Paginador.CalcularPaginas(clone, TipoPaginador.Modelo_Linq);
            Model.PaginadorDados = clone;
        }

        private void ZerarFiltro(ModelPlaca Model)
        {
            Model.Filtro = new PlacaFiltro();
            base.ArmazenarDados<PlacaFiltro>(Model.Filtro, NomeFiltro);
        }

        #endregion Paginacao

        #region Salvar

        [AjaxOnlyAttribute]
        public ActionResult Salvar(ModelPlaca Model)
        {
            //TODO: remover a linha abaixo pois ela será preenchida diretamente pela VIEW
            Model.Placa.IDPais = EnumPais.Brasil;

            Model.TipoValidacao = TiposValidacao.ValidacaoTotal;

            var loginUsuario = UsuarioCsOnline != null && UsuarioCsOnline.Login != null ? UsuarioCsOnline.Login : UserSession.GetCurrentInfoUserSystem().InformacoesUsuario.Login;
            var usuario = new UsuarioBusiness().Selecionar(w => w.Login == loginUsuario);
            if (usuario != null)
            {
                //R5) Verificar se deve associar o ID do usuário logado ao salvar a placa
                if (usuario.Perfil == "Cliente EAB" || usuario.Perfil == EnumPerfil.CLIENTE_ACS || usuario.Perfil == EnumPerfil.CLIENTE_ACS_ARGENTINA)
                    Model.Placa.idUsuario = usuario.ID;
            }

            if (Model.Placa.SomenteLiberacaoAcesso)
            {
                Model.Placa.ID = int.Parse(Model.ChavePrimaria);
                Placa placaNew;
                Model.Resultado = base.ProcessarResultado(this.PlacaBLL.AtualizarPlacaPermissao(Model.Placa, out placaNew), OperacoesCRUD.Insert);
                Model.Placa = placaNew;
                Model.Placa.SomenteLiberacaoAcesso = true;
                Model.Operacao = OperacoesCRUD.Update;
            }
            else
            {
                if (!base.ValidarModel(Model, this.ModelState) && Model.Placa.Operacao == "FOB" && Model.ValidacoesModelo.Any(vm => vm.IdControle.Contains("Renavam")))
                {
                    Model.Placa.PlacaBrasil.Renavam = "0";
                    Model.ValidacoesModelo.RemoveAll(vm => vm.IdControle.Contains("Renavam"));
                }

                if (Model.ValidacoesModelo.Count == 0)
                {
                    Model.ContemErrosModel = "N";
                    if (Model.Operacao == OperacoesCRUD.Insert)
                    {
                        if (Model.Placa.Clientes == null) Model.Placa.Clientes = new List<PlacaClienteView>();
                        Model.Resultado = base.ProcessarResultado(this.PlacaBLL.AdicionarPlaca(Model.Placa), OperacoesCRUD.Insert);
                        this.ZerarFiltro(Model);
                        this.AtualizarQtdeRegPaginador(Model);
                        Model.Operacao = OperacoesCRUD.Update;
                    }

                    if (Model.Operacao == OperacoesCRUD.Editando)
                    {
                        Model.Placa.ID = int.Parse(Model.ChavePrimaria);

                        Model.Resultado = base.ProcessarResultado(this.PlacaBLL.AtualizarPlaca(Model.Placa), OperacoesCRUD.Update);
                        Model.Operacao = OperacoesCRUD.Update;
                    }
                }
                else
                {
                    Model.ValidacoesModelo.ForEach(vm =>
                    {
                        if (vm.IdControle.Contains("Placa_PlacaBrasil"))
                            vm.IdControle = vm.IdControle.Replace("Placa_", "");

                        if (vm.MensagemValidacao.Contains("Preenchimento Obrigatório!") && Model.Placa.IDPais == EnumPais.Argentina)
                            vm.MensagemValidacao = "Por favor completar los campos obligatorios";
                    });

                    if (Model.Placa.idPlacaOficial.HasValue)
                    {
                        Model.Placa.ID = int.Parse(Model.ChavePrimaria);
                        Model.Placa.Setas = new PlacaSetaBusiness().Listar(p => p.IDPlaca == Model.Placa.ID);
                        CarregarAlteracoesDePlacas(Model.Placa.idPlacaOficial.Value, Model);
                    }
                }
                CalcularColunas(Model);
            }

            if (Model.Placa?.PlacaBrasil.CPFCNPJ != null)
            {
                Model.Placa.PlacaBrasil.CPFCNPJ = Model.Placa.PlacaBrasil.CPFCNPJ.RemoveCharacter();
            }

            if (Model.IdPais == (int)EnumPais.Brasil)
            {
                if (Model.Placa.SomenteLiberacaoAcesso)
                {
                    return PartialView("Permissao", Model);
                }
                else
                {
                    return PartialView("_Edicao", Model);
                }
            }
            else
            {
                if (Model.Placa.SomenteLiberacaoAcesso)
                {
                    return PartialView("PermissaoArgentina", Model);
                }
                else
                {
                    return PartialView("_EdicaoArgentina", Model);
                }
            }
        }

        [AjaxOnlyAttribute]
        public ActionResult SalvarPermissao(Placa Model)
        {
            return PartialView("Permissao", Model);
        }

        #endregion Salvar

        #region ExcluirRegistro

        [HttpGet]
        [AjaxOnlyAttribute]
        public bool ExcluirRegistro(int Id)
        {
            ModelPlaca Model = new ModelPlaca();
            try
            {
                Model.Resultado = base.ProcessarResultado(this.PlacaBLL.ExcluirPlaca(Id), OperacoesCRUD.Update);
            }
            catch
            {
                return false;
            }
            return true;
        }

        #endregion ExcluirRegistro

        #region WebMethods

        public PartialViewResult AdicionarCliente(int idCliente)
        {
            var model = new PlacaClienteView();
            var cliente = new ClienteBusiness().Selecionar(idCliente);
            model.IDCliente = cliente.ID;
            model.RazaoSocial = cliente.IBM + " - " + cliente.CNPJCPF + " - " + cliente.RazaoSocial;
            return PartialView("_ItemCliente", model);
        }

        public PartialViewResult InativarCompartimento(int idPlaca, int seq)
        {
            var placa = new PlacaBusiness().Selecionar(w => w.ID == idPlaca);
            placa.Setas = new PlacaSetaBusiness().Listar(p => p.IDPlaca == idPlaca);
            foreach (var comp in placa.Setas)
            {
                switch (seq)
                {
                    case 2:
                        comp.Compartimento2IsInativo = comp.Compartimento2IsInativo.HasValue ? !comp.Compartimento2IsInativo : true;
                        break;
                    case 3:
                        comp.Compartimento2IsInativo = comp.Compartimento3IsInativo.HasValue ? !comp.Compartimento3IsInativo : true;
                        break;
                    case 4:
                        comp.Compartimento2IsInativo = comp.Compartimento4IsInativo.HasValue ? !comp.Compartimento4IsInativo : true;
                        break;
                    case 5:
                        comp.Compartimento2IsInativo = comp.Compartimento5IsInativo.HasValue ? !comp.Compartimento5IsInativo : true;
                        break;
                    case 6:
                        comp.Compartimento2IsInativo = comp.Compartimento6IsInativo.HasValue ? !comp.Compartimento6IsInativo : true;
                        break;
                    case 7:
                        comp.Compartimento2IsInativo = comp.Compartimento7IsInativo.HasValue ? !comp.Compartimento7IsInativo : true;
                        break;
                    case 8:
                        comp.Compartimento2IsInativo = comp.Compartimento8IsInativo.HasValue ? !comp.Compartimento8IsInativo : true;
                        break;
                    case 9:
                        comp.Compartimento2IsInativo = comp.Compartimento9IsInativo.HasValue ? !comp.Compartimento9IsInativo : true;
                        break;
                    case 10:
                        comp.Compartimento2IsInativo = comp.Compartimento10IsInativo.HasValue ? !comp.Compartimento10IsInativo : true;
                        break;
                    default:
                        break;
                }
            }
            if (placa.LinhaNegocio == (int)EnumEmpresa.Combustiveis)
                return PartialView("_ItemSeta", placa);
            else
                return PartialView("_ItemSetaComLacre", placa);
        }

        public PartialViewResult AdicionarSeta(int colunas, int sequencial, bool multiseta, int linhaNegocio)
        {
            var model = new PlacaSeta();
            model.Colunas = colunas;
            model.Sequencial = sequencial;
            model.Multiseta = multiseta;
            if (linhaNegocio == (int)EnumEmpresa.Combustiveis)
                return PartialView("_ItemSeta", model);
            else
                return PartialView("_ItemSetaComLacre", model);
        }

        public PartialViewResult ListarDocumentos(int? IDTipoVeiculo, int? IDCategoriaVeiculo, string Operacao, int? LinhaNegocio, int? IDTipoProduto, bool? Aprovar, int idPais, int? idPlaca)
        {
            List<PlacaDocumentoView> DocsAtuais = new List<PlacaDocumentoView>();
            /*Buscar os documentos da composição atual*/
            if (idPlaca != null)
            {
                DocsAtuais = new PlacaDocumentoBusiness().ListarPlacaDocumentoPorPlaca(Convert.ToInt32(idPlaca));
            }

            /*Fazendo a mesclagem das informações*/



            var model = new PlacaDocumentoBusiness().ListarPlacaDocumento(IDTipoVeiculo, IDCategoriaVeiculo, Operacao, LinhaNegocio, IDTipoProduto);
            if (model != null && model.Any())
                foreach (var item in model)
                {
                    if (DocsAtuais != null && DocsAtuais.Count > 0)
                    {
                        var i = DocsAtuais.Where(d => d.IDTipoDocumento == item.IDTipoDocumento).FirstOrDefault();

                        if (i != null)
                        {
                            item.DataVencimento = i.DataVencimento;
                            item.Anexo = i.Anexo;
                           
                        }
                    }

                    if (Aprovar.HasValue)
                        item.Aprovar = Aprovar.Value;
                }

            if (idPais == (int)EnumPais.Brasil)
                return PartialView("_Documentos", model);

            return PartialView("_DocumentosArgentina", model);
        }

        #endregion WebMethods
    }
}
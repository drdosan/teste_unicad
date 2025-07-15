using Infraestructure.Extensions;
using Raizen.Framework.Models;
using Raizen.Framework.UserSystem.Client;
using Raizen.Framework.Utils.Extensions;
using Raizen.Framework.Web.MVC.Filters;
using Raizen.Framework.Web.MVC.Models;
using Raizen.Framework.Web.MVC.Utils;
using Raizen.UniCad.BLL;
using Raizen.UniCad.Extensions;
using Raizen.UniCad.Model;
using Raizen.UniCad.Model.Filtro;
using Raizen.UniCad.Model.View;
using Raizen.UniCad.Utils;
using Raizen.UniCad.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Reflection;

namespace Raizen.UniCad.Web.Controllers
{
    public class MotoristaController : MotoristaBaseController
    {
        /// <summary>
		/// Nao validar acesso.
		/// </summary>
        public MotoristaController() : base(BaseControllerOptions.NaoValidarAcesso)
        {
        }

        #region Constantes
        private readonly MotoristaBusiness _motoristaBll = new MotoristaBusiness();
        private const string NomeFiltro = "Filtro_Motorista";
        private const string NomePaginador = "Paginador_Motorista";
        private const string TotalRegistros = "totalRegistros_Motorista";
        #endregion

        #region Index
        [HttpGet]
        public ActionResult Index()
        {
            return CarregarDefault();
        }

        #endregion

        #region Novo
        [HttpGet]
        [AjaxOnlyAttribute]
        public ActionResult Novo()
        {
            ModelMotorista model = new ModelMotorista();
            model.Motorista = new Motorista();
            model.Resultado = new ResultadoOperacao();
            return PartialView("_Edicao", model);
        }
        #endregion

        #region Editar
        [HttpGet]
        public ActionResult Editar(string id, bool aprovar = false)
        {
            ModelMotorista model = CarregarDadosMotorista(id, aprovar);
            var usuario = UsuarioLogado;
            if (usuario != null)
            {
                model.Motorista.IDEmpresa = usuario.IDEmpresa == 3 ? 0 : usuario.IDEmpresa;
            }

            return PartialView("Edicao", model);
        }

        [HttpGet]
        public PartialViewResult UploadDocumentos(int id)
        {
            var model = new ModelMotorista();
            model.Motorista = _motoristaBll.Selecionar(id);
            model.Aprovar = true;
            model.Motorista.Documentos = new MotoristaDocumentoBusiness().ListarMotoristaDocumentoPorMotorista(id);
            model.Motorista.Documentos.ForEach(w => w.Aprovar = true);
            return PartialView("_UploadDocumentos", model);
        }

        [HttpGet]
        public ActionResult Aprovar(string id)
        {
            ModelMotorista model = CarregarDadosMotorista(id, true);
            return PartialView("_EdicaoMotorista", model);
        }

        public ActionResult Aprovacao(string id, int status, bool comRessalvas)
        {
            var model = CarregarDadosMotorista(id, true);

            model.Motorista.ID = int.Parse(model.ChavePrimaria);
            model.Motorista.IDStatus = status;
            model.Resultado = ProcessarResultado(_motoristaBll.AtualizarMotorista(model.Motorista, comRessalvas, bloqueio: false), OperacoesCRUD.Update);
            if (!string.IsNullOrEmpty(model.Motorista.Mensagem))
            {
                model.Resultado.Mensagem = model.Motorista.Mensagem;
                model.ContemErrosModel = "S";
            }
            model.Operacao = OperacoesCRUD.Update;

            return PartialView("_Edicao", model);
        }

        private ModelMotorista CarregarDadosMotorista(string id, bool aprovar)
        {
            var model = new ModelMotorista
            {
                Motorista = new Motorista(),
                Aprovar = aprovar
            };
            if (!string.IsNullOrEmpty(id))
            {
                model.Operacao = OperacoesCRUD.Editando;
                model.ChavePrimaria = id;
                model.Resultado = new ResultadoOperacao();

                model.Motorista = _motoristaBll.Selecionar(int.Parse(model.ChavePrimaria));
                if (!aprovar && model.Motorista.IDStatus != (int)EnumStatusMotorista.Reprovado)
                {
                    model.Motorista.IDMotorista = model.Motorista.ID;
                    model.Motorista.ID = 0;
                    model.ChavePrimaria = "0";
                }


            }
            return model;
        }

        private void CarregarAlteracoes(ModelMotorista model)
        {
            if (model.Motorista.IDMotorista.HasValue)
            {
                var motoristaAnterior = new MotoristaPesquisaBusiness().Selecionar(model.Motorista.IDMotorista.Value).Mapear();

                if (motoristaAnterior != null)
                {
                    model.Alteracoes = _motoristaBll.CarregarAlteracoes(model.Motorista, motoristaAnterior);
                }
            }
        }

        [HttpGet]
        public ActionResult EditarMotorista(int? id, string cpf, int? idEmpresa, int? acao, int? idPais, bool aprovar = false, int naoAprovado = 0)
        {
            var model = new ModelMotorista();
            cpf = cpf.RemoveCharacter();
            bool acesso;
            var usuario = UsuarioLogado;
            if (usuario == null)
            {
                var login = UserSession.GetCurrentInfoUserSystem().InformacoesUsuario.Login;
                usuario = new UsuarioBusiness().Selecionar(w => w.Login == login);
            }

            //edição que vem da tela de pesquisa
            if (id.HasValue && acao != (int)EnumAcao.Clonar)
            {
                acesso = new MotoristaBusiness().ValidarAcesso(UserSession.GetCurrentInfoUserSystem().InformacoesUsuario.Login, model.Motorista);
                model.Aprovar = aprovar;
                model.Acao = (int)EnumAcao.Editar;
                model.Operacao = OperacoesCRUD.Editando;
                model.Resultado = new ResultadoOperacao();
                model.Motorista = _motoristaBll.Selecionar(moto => moto.ID == id.Value);
                model.ChavePrimaria = model.Motorista.ID.ToString();
                model.Motorista.Documentos = new MotoristaDocumentoBusiness().ListarMotoristaDocumentoPorMotorista(model.Motorista.ID, model.Motorista.Operacao);

                if (aprovar)
                    CarregarAlteracoes(model);

                //se não for aprovado deverá apenas visualizar os dados e também não deverá ter a ação de anexar 
                model.Motorista.Documentos?.ForEach(x => x.Aprovar = aprovar);
                if (!aprovar && model.Motorista.IDStatus != (int)EnumStatusMotorista.Reprovado)
                {
                    model.Motorista.IDMotorista = model.Motorista.ID;
                    model.Motorista.ID = 0;
                    model.ChavePrimaria = "0";
                }
                CarregarPermissaoUsuario(model);

            }
            //clonar
            else if ((id.HasValue || !string.IsNullOrEmpty(cpf)) && acao == (int)EnumAcao.Clonar)
            {
                acesso = true;
                model.Operacao = OperacoesCRUD.Editando;
                model.Resultado = new ResultadoOperacao();

                model.Motorista = id.HasValue ? _motoristaBll.Selecionar(id.Value) : _motoristaBll.Selecionar(w => w.MotoristaBrasil.CPF == cpf && w.IDStatus == (int)EnumStatusMotorista.Aprovado);
                if (model.Motorista != null)
                {
                    if (usuario?.Operacao != "Ambos")
                    {
                        if (usuario != null)
                            model.Motorista.Operacao = model.Motorista.OperacaoUsuario = usuario.Operacao;
                    }

                    model.Motorista.IDEmpresa = model.Motorista.IDEmpresa == (int)EnumEmpresa.EAB ? (int)EnumEmpresa.Combustiveis : (int)EnumEmpresa.EAB;

                    model.Motorista.Justificativa = null;
                    model.Motorista.Documentos = new MotoristaDocumentoBusiness().ListarMotoristaDocumento(model.Motorista.IDEmpresa, model.Motorista.Operacao);
                    var documentosOrigem = new MotoristaDocumentoBusiness().ListarMotoristaDocumentoPorMotorista(model.Motorista.ID);

                    foreach (var doc in model.Motorista.Documentos)
                    {
                        if (documentosOrigem.Any(w => w.IDTipoDocumento == doc.IDTipoDocumento))
                        {
                            var docOrigem = documentosOrigem.FirstOrDefault(w => w.IDTipoDocumento == doc.IDTipoDocumento);
                            if (docOrigem != null)
                            {
                                doc.DataVencimento = docOrigem.DataVencimento;
                                doc.Anexo = docOrigem.Anexo;
                            }
                        }
                    }
                }
                else
                {
                    return Json(new
                    {
                        status = "e",
                        result = "O status desse motorista não foi alterado, por favor, tente novamente"
                    }, JsonRequestBehavior.AllowGet);
                }

                if (model.Motorista.Operacao == "CIF")
                {
                    if (idEmpresa != null) CarregarPermissaoCif(null, idEmpresa.Value, null);
                }
                else
                {
                    if (idEmpresa != null) CarregarPermissaoFob(model.Motorista, null, idEmpresa.Value);
                }

                CarregarPermissaoUsuario(model);
            }
            //editar
            else if (!string.IsNullOrEmpty(cpf) && acao == (int)EnumAcao.Editar)
            {
                model.Acao = acao.Value;
                model.Operacao = OperacoesCRUD.Editando;
                model.Resultado = new ResultadoOperacao();

                model.Motorista = new Motorista();

                //Tratamento para recuperar Motorista Reprovado.
                var idStatusMotorista = naoAprovado == 0 ? (int)EnumStatusMotorista.Aprovado : (int)EnumStatusMotorista.Reprovado;

                model.Motorista = new MotoristaPesquisaBusiness().Selecionar(w => w.CPF == cpf && w.IDEmpresa == idEmpresa && w.IDStatus == idStatusMotorista).Mapear();
                if (model.Motorista != null)
                {
                    acesso = new MotoristaBusiness().ValidarAcesso(UserSession.GetCurrentInfoUserSystem().InformacoesUsuario.Login, model.Motorista);
                    model.Motorista.naoAprovado = naoAprovado;
                    model.ChavePrimaria = model.Motorista.ID.ToString();
                    model.Motorista.Documentos = new MotoristaDocumentoBusiness().ListarMotoristaDocumentoPorMotorista(model.Motorista.ID);
                    if (aprovar)
                        CarregarAlteracoes(model);
                    //se não for aprovado deverá apenas visualizar os dados e também não deverá ter a ação de anexar 
                    model.Motorista.Documentos?.ForEach(x => x.naoAprovado = naoAprovado != 0);

                    if (!aprovar && model.Motorista.IDStatus != (int)EnumStatusMotorista.Reprovado)
                    {
                        model.Motorista.IDMotorista = model.Motorista.ID;
                        //Model.Motorista.ID = 0;
                        //Model.ChavePrimaria = "0";
                    }

                    CarregarPermissaoUsuario(model);
                }
                else
                //novo
                //Caso o usuário aperte o tab e não dê tempo do botão mudar de edit para insert
                {
                    acesso = true;
                    model.Operacao = OperacoesCRUD.Insert;
                    model.Acao = acao.Value;
                    model.Motorista = new Motorista();
                    if (!string.IsNullOrEmpty(cpf))
                        model.Motorista.MotoristaBrasil.CPF = cpf.RemoveCharacter();
                }
            }
            else
            {
                model.Novo = true;
                acesso = true;
                model.Operacao = OperacoesCRUD.Insert;
                model.Motorista = new Motorista();
                if (!string.IsNullOrEmpty(cpf))
                {
                    model.Motorista.MotoristaBrasil = new MotoristaBrasil();
                    model.Motorista.MotoristaBrasil.CPF = cpf.RemoveCharacter();
                }
                if (idEmpresa != null) model.Motorista.IDEmpresa = idEmpresa.Value;
                CarregarPermissaoUsuario(model);
            }


            if (usuario != null)
            {
                if (string.IsNullOrEmpty(model.Motorista.Operacao))
                {
                    model.Motorista.Operacao = usuario.Operacao;
                }
                //if (Model.Operacao != OperacoesCRUD.Editando)
                //    Model.Motorista.IDEmpresa = usuario.IDEmpresa;

                model.UsuarioPerfil = usuario.Perfil.ToLowerInvariant();
            }

            var dataVencimentoEditavel = (model.UsuarioPerfil.ToLowerInvariant() == "administrador" ||
                 model.UsuarioPerfil.ToLowerInvariant() == "analista de cadastro" ||
                 model.UsuarioPerfil.ToLowerInvariant() == "central atendimento");

            //Tratamento para a Data de Vencimento na grid de documentos.
            //Permitir edição para os perfis:
            // Administrador 
            // Analista de Cadastro
            // Central de Atendimento
            model.Motorista.Documentos?.ForEach(x => x.DataVencimentoEditavel = dataVencimentoEditavel);

            if (usuario != null && !acesso)
            {
                if (model.Motorista.Operacao == "FOB")
                {
                    return PartialView("_EdicaoMotorista", model);
                }

                if (model.Motorista.Operacao != usuario.Operacao)
                    model.Motorista.Operacao = usuario.Operacao;
                model.LinhaNegocio = model.Motorista.IDEmpresa;
                return PartialView("Permissao", model);
            }
            else
            {
                return PartialView("_EdicaoMotorista", model);
            }
        }

        #endregion

        #region CarregarDefault
        private ActionResult CarregarDefault()
        {
            ModelMotorista model = new ModelMotorista();
            model = MotoristaLayout(model);
            model.Operacao = OperacoesCRUD.List;
            model.Filtro = new MotoristaFiltro();
            model.Filtro.Ativo = true;
            model.Filtro.IdPais = (int)EnumPais.Brasil;
            CarregarPermissaoUsuario(model);

            Usuario usCliente = UsuarioCsOnline;
            if (usCliente != null)
            {
                model.Filtro.IDUsuarioCliente = usCliente.ID;
            }

            if (UserSession.GetCurrentInfoUserSystem() != null)
            {
                if (UserSession.GetCurrentInfoUserSystem().InformacoesPermissao.FirstOrDefault(p => p.MVCController == "Motorista" && p.NomeAcao == "Aprovar") != null)
                    model.Filtro.IDStatus = (int)EnumStatusMotorista.EmAprovacao;
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }

            bool usuarioQuality;
            model.Resultado = new ResultadoOperacao();
            model.PaginadorDados = new PaginadorModel { Status = EstadoPaginador.RenovandoConsulta };
            ListarPaginador(model);
            model.Usuario = UsuarioLogado;
            if (model.Usuario != null)
            {
                model.Filtro.UsuarioExterno = model.Usuario.Externo;
                usuarioQuality = model?.Usuario.Perfil.Contains("Quality") ?? false;
                model.Filtro.UsuarioExterno = usuarioQuality ? false : model.Filtro.UsuarioExterno;
                return PartialView("Index", model);
            }

            var login = UserSession.GetCurrentInfoUserSystem().InformacoesUsuario.Login;
            model.Usuario = new UsuarioBusiness().Selecionar(w => w.Login == login);
            model.Filtro.UsuarioExterno = model.Usuario?.Externo ?? true;
            usuarioQuality = model?.Usuario.Perfil.Contains("Quality") ?? false;
            model.Filtro.UsuarioExterno = usuarioQuality ? false : model.Filtro.UsuarioExterno;
            if (Session["MotoristaFiltro"] != null)
                model.Filtro = (MotoristaFiltro)Session["MotoristaFiltro"];

            return View("Index", model);
        }

        private void CarregarPermissaoUsuario(ModelMotorista model, bool isPesquisa = false)
        {
            var usuario = UsuarioLogado;
            if (usuario != null)
            {
                //se a chamada não vir pela pesquisa o filtro será null
                if (usuario.IDEmpresa != 3 && model.Filtro != null)
                {
                    model.Filtro.IDEmpresaUsuario = usuario.IDEmpresa;
                    model.Filtro.IDEmpresa = usuario.IDEmpresa;
                    GetClienteTransportadora(model, usuario);
                }
                //se a chamada não vir pela pesquisa o filtro será null
                if (model.Filtro == null)
                {
                    model.Motorista.OperacaoUsuario = usuario.Operacao == "Ambos" ? null : usuario.Operacao;
                }
                else if (isPesquisa)
                {
                    model.Filtro.OperacaoUsuario = usuario.Operacao == "Ambos" ? null : usuario.Operacao;
                }
                else
                {
                    model.Filtro.Operacao = model.Filtro.OperacaoUsuario = usuario.Operacao == "Ambos" ? null : usuario.Operacao;
                }
            }
        }

        private static void GetClienteTransportadora(ModelMotorista model, Usuario usuario)
        {
            if (usuario.Perfil == "Transportadora")
                model.Filtro.IDUsuarioTransportadora = usuario.ID;
            else if (usuario.Perfil == "Cliente EAB")
                model.Filtro.IDUsuarioCliente = usuario.ID;
        }

        #endregion

        #region MotoristaLayout
        private ModelMotorista MotoristaLayout(ModelMotorista model)
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
        private void ListarPaginador(ModelMotorista model)
        {
            if (model.PaginadorDados.Status == EstadoPaginador.RenovandoConsulta && model.Operacao != OperacoesCRUD.Editando)
            {
                ListarRenovandoConsulta(model);
            }
            else
            {
                ListarPaginando(model);
            }
        }

        private void ListarRenovandoConsulta(ModelMotorista model)
        {
            ArmazenarDados(model.Filtro, NomeFiltro);

            AtualizarQtdeRegPaginador(model);
            var totalReg = RetornaDados<int>(TotalRegistros);

            model.PaginadorDados = ModelUtils.IniciarPaginador(model.PaginadorDados, totalReg);
            ArmazenarDados(totalReg, TotalRegistros);
            ArmazenarDados(model.PaginadorDados, NomePaginador);

            if (totalReg > 0)
            {
                model.ListaMotorista = _motoristaBll.ListarMotorista(model.Filtro, model.PaginadorDados);
            }
        }

        private void AtualizarQtdeRegPaginador(ModelMotorista model)
        {
            model.Filtro = RetornaDados<MotoristaFiltro>(NomeFiltro);
            ArmazenarDados(model.Filtro, NomeFiltro);

            int totalRegistros = _motoristaBll.ListarMotoristaCount(model.Filtro);
            ArmazenarDados(totalRegistros, TotalRegistros);
        }

        private void Listar(ModelMotorista model)
        {
            ListarPaginador(model);
            model.Resultado = ProcessarResultado(!model.ListaMotorista.IsNullOrEmpty(), model.Operacao);
        }
        #endregion

        #region Pesquisar
        [HttpGet]
        [AjaxOnlyAttribute]
        public ActionResult Pesquisar(ModelMotorista model)
        {
            CarregarPermissaoUsuario(model, true);

            Session["MotoristaFiltro"] = model.Filtro;

            model.Filtro.IdPais = (int)EnumPais.Brasil;

            Usuario usCliente = UsuarioCsOnline;

            if (usCliente != null)
            {
                model.Filtro.IDUsuarioCliente = usCliente.ID;
            }

            if (!string.IsNullOrEmpty(model.Filtro.CPF))
                model.Filtro.CPF = model.Filtro.CPF.RemoveCharacter();
            PrepararPaginadorOperacoes(model);
            Listar(model);
            bool usuarioQuality;
            model.Usuario = UsuarioLogado;
            if (model.Usuario != null)
            {
                model.Filtro.UsuarioExterno = model.Usuario.Externo;
                usuarioQuality = model?.Usuario.Perfil.Contains("Quality") ?? false;
                model.Filtro.UsuarioExterno = usuarioQuality ? false : model.Filtro.UsuarioExterno;
                return PartialView("_Pesquisa", model);
            }

            var login = UserSession.GetCurrentInfoUserSystem().InformacoesUsuario.Login;
            model.Usuario = new UsuarioBusiness().Selecionar(w => w.Login == login);
            model.Filtro.UsuarioExterno = model.Usuario?.Externo ?? true;
            usuarioQuality = model?.Usuario.Perfil.Contains("Quality") ?? false;
            model.Filtro.UsuarioExterno = usuarioQuality ? false : model.Filtro.UsuarioExterno;
            return PartialView("_Pesquisa", model);
        }

        public JsonResult ValidarCpf(string cpf)
        {
            return Json(new { retorno = ValidacoesUtil.ValidaCPF(cpf) }, JsonRequestBehavior.AllowGet);
        }

        //TODO: Refatorar para novo padrão Brasil/Argentina
        public JsonResult VerificarCpfExiste(string cpf, int idEmpresa)
        {
            return BuscarCpfDni(cpf, idEmpresa, EnumPais.Brasil);
        }

        public PartialViewResult CarregarPermissaoFob(Motorista motorista, int? idMotorista, int idEmpresa)
        {
            var loginUsuario = UsuarioCsOnline != null && UsuarioCsOnline.Login != null ? UsuarioCsOnline.Login : UserSession.GetCurrentInfoUserSystem().InformacoesUsuario.Login;
            var usuario = new UsuarioBusiness().Selecionar(w => w.Login == loginUsuario);

            ModelMotorista model = new ModelMotorista();

            Motorista motoristaPermissao;

            motoristaPermissao = CarregaPermissaoFobBase(motorista, idMotorista, idEmpresa, usuario);

            model.Motorista = motoristaPermissao;
            model.LinhaNegocio = idEmpresa;
            SelecionarTranspCliente(model, "FOB");

            return PartialView("_Cliente", model);
        }


        private void SelecionarTranspCliente(ModelMotorista model, string operacao)
        {
            var loginUsuario = UsuarioCsOnline != null && UsuarioCsOnline.Login != null ? UsuarioCsOnline.Login : UserSession.GetCurrentInfoUserSystem().InformacoesUsuario.Login;
            var usuario = new UsuarioBusiness().Selecionar(w => w.Login == loginUsuario);

            if (usuario != null)
            {
                model.Usuario = usuario;

                usuario.Transportadoras = new UsuarioTransportadoraBusiness().ListarTransportadorasPorUsuario(usuario.ID, model.LinhaNegocio);
                usuario.Clientes = new UsuarioClienteBusiness().ListarClientesPorUsuario(usuario.ID, model.LinhaNegocio);

                if (usuario.Externo)
                {

                    if ((operacao == "CIF") && usuario.Transportadoras != null && usuario.Transportadoras.Count == 1)
                    {
                        model.Motorista.IDTransportadora = usuario.Transportadoras.First().IDTransportadora;
                    }

                    if (operacao == "FOB" && model.LinhaNegocio == (int)EnumEmpresa.EAB && usuario.Clientes != null && usuario.Clientes.Any())
                    {
                        model.Motorista.Clientes = new List<MotoristaClienteView>();
                        foreach (var item in usuario.Clientes)
                        {
                            model.Motorista.Clientes.Add(new MotoristaClienteView() { IDCliente = item.IDCliente, RazaoSocial = item.IBM + " - " + item.RazaoSocial });
                        }
                    }
                }
            }
        }

        public PartialViewResult CarregarPermissaoCif(int? idMotorista, int idEmpresa, int? idTransportadora, bool? novo = null)
        {
            return CarregarPermissaoCifBase(idMotorista, idEmpresa, idTransportadora, novo, new ModelMotorista());
        }

        public PartialViewResult ListarDocumentos(int? idEmpresa, string operacao, bool aprovar, int? idMotorista, string acao)
        {
            if (!idEmpresa.HasValue || string.IsNullOrEmpty(operacao))
                return PartialView("_Documentos", null);
            List<MotoristaDocumentoView> model = null;
            if (idMotorista.HasValue && idMotorista != 0)
                model = new MotoristaDocumentoBusiness().ListarMotoristaDocumentoPorMotorista(idMotorista.Value, operacao);
            if (model == null || model.Count == 0)
                model = new MotoristaDocumentoBusiness().ListarMotoristaDocumento(idEmpresa, operacao);
            model.ForEach(x => x.Aprovar = aprovar);

            TempData["Operacao"] = acao;

            return PartialView("_Documentos", model);
        }

        #endregion

        #region Paginacao

        private void PrepararPaginadorOperacoes(ModelMotorista model)
        {
            if (model.PaginadorDados == null)
            {
                model.PaginadorDados = RetornaDados<PaginadorModel>(NomePaginador);
                ArmazenarDados(model.PaginadorDados, NomePaginador);
            }
            else
            {
                ArmazenarDados(model.PaginadorDados, NomePaginador);
            }

            //Sifnifica que não tem registro ainda
            if (model.PaginadorDados == null)
            {
                model.PaginadorDados = new PaginadorModel();
            }

            model.PaginadorDados.ConjuntoPaginas = ModelUtils.ListarConjuntoPaginas();
        }

        private void ListarPaginando(ModelMotorista model)
        {
            model.Filtro = RetornaDados<MotoristaFiltro>(NomeFiltro);
            ArmazenarDados(model.Filtro, NomeFiltro);

            int totalReg = RetornaDados<int>(TotalRegistros);
            model.PaginadorDados.QtdeTotalRegistros = totalReg;

            ArmazenarDados(totalReg, TotalRegistros);
            PrepararPaginador(ref model);

            if (totalReg > 0)
            {
                model.ListaMotorista = _motoristaBll.ListarMotorista(model.Filtro, model.PaginadorDados);
            }
        }

        private void PrepararPaginador(ref ModelMotorista model)
        {
            PaginadorModel clone = model.PaginadorDados.Clone() as PaginadorModel;
            Paginador.CalcularPaginas(clone, TipoPaginador.Modelo_Linq);
            model.PaginadorDados = clone;
        }

        private void ZerarFiltro(ModelMotorista model)
        {
            model.Filtro = new MotoristaFiltro();
            ArmazenarDados(model.Filtro, NomeFiltro);
        }

        #endregion

        #region Salvar
        [AjaxOnly]
        public ActionResult SalvarPermissoes(ModelMotorista Model)
        {
            var model = Model.Motorista;
            var motorista = new MotoristaBusiness().Selecionar(Convert.ToInt32(Model.ChavePrimaria));
            motorista.Documentos = new MotoristaDocumentoBusiness().ListarMotoristaDocumentoPorMotorista(motorista.ID);
            Model.Motorista = motorista;

            SomarPermissoes(Model, model);

            Model.Motorista.ID = 0;
            Model.Motorista.IDMotorista = model.IDMotorista;
            Model.Motorista.IDStatus = (int)EnumStatusMotorista.EmAprovacao;

            Model.Motorista.LoginUsuario = UsuarioCsOnline != null && UsuarioCsOnline.Login != null ? UsuarioCsOnline.Login : UserSession.GetCurrentInfoUserSystem().InformacoesUsuario.Login;
            Model.Motorista.MotoristaBrasil.CPF = Model.Motorista.MotoristaBrasil.CPF.RemoveCharacter();
            Model.Motorista.DataAtualizazao = DateTime.Now;
            Model.Motorista.Ativo = true;
            Model.Motorista.EmailSolicitante = UsuarioCsOnline != null && UsuarioCsOnline.Email != null ? UsuarioCsOnline.Email : UserSession.GetCurrentInfoUserSystem().InformacoesUsuario.Email;
            Model.Motorista.Operacao = model.Operacao;
            Model.Motorista.IdPais = EnumPais.Brasil; //Todo : deve vir na model

            Model.Resultado = ProcessarResultado(_motoristaBll.AdicionarMotorista(Model.Motorista), OperacoesCRUD.Insert);
            ZerarFiltro(Model);
            AtualizarQtdeRegPaginador(Model);
            Model.Operacao = OperacoesCRUD.Update;
            return PartialView("Permissao", Model);
        }

        [AjaxOnly]
        public ActionResult Salvar(ModelMotorista model, int status, bool comRessalvas, int idPais)
        {
            string result = string.Empty;
            model.comRessalvas = comRessalvas;
            model.TipoValidacao = TiposValidacao.ValidacaoTotal;
            bool primeiroCadastro = !model.Motorista.IDMotorista.HasValue;
            bool jaEnviadoQuickTas = false;
            bool dadosAlterados = false;
            // Utilizado para corrigir o bug CSCUNI-1623 - 1ª parte correção
            var statusAtual = model.Motorista.IDStatus;

            model.Motorista.IDStatus = (int)EnumStatusMotorista.EmAprovacao;
            if (ValidarModel(model, ModelState))
            {
                if (model.Motorista.MotoristaBrasil != null)
                {
                    dadosAlterados = _motoristaBll.VerificarAlteracoesApenasTelefoneEmail(model.Motorista);
                }
                if (!dadosAlterados && model.Motorista.IDMotorista.HasValue)
                {
                    status = (int)EnumStatusMotorista.Aprovado;
                    model.Operacao = OperacoesCRUD.Editando;

                    if (idPais == (int)EnumPais.Brasil && statusAtual != 0 && model.Motorista.IDEmpresa == (int)EnumEmpresa.Combustiveis)
                    {
                        result = _motoristaBll.EnviarDadosQuickTAS(model.Motorista);
                        jaEnviadoQuickTas = true;
                    }

                    if (result.ToUpper().Contains("ERRO"))
                    {
                        model.Resultado = new ResultadoOperacao
                        {
                            Mensagem = result
                        };
                        model.ContemErrosModel = "S";
                    }
                    else
                    {
                        var motoristaAnterior = new MotoristaPesquisaBusiness().Selecionar(model.Motorista.IDMotorista.Value).Mapear();
                        motoristaAnterior.Telefone = model.Motorista.Telefone.RemoveCharacter();
                        motoristaAnterior.Email = model.Motorista.Email;
                        motoristaAnterior.DataAtualizazao = DateTime.Now;

                        this._motoristaBll.AlterarTelefoneEmailMotorista(motoristaAnterior);

                        model.Motorista.IDStatus = motoristaAnterior.IDStatus;

                        model.Resultado = new ResultadoOperacao();
                        model.Resultado.Mensagem = "ALTERACAO_EMAIL_TELEFONE";
                        model.ContemErrosModel = "N";
                    }

                    return PartialView("_EdicaoMotorista", model);
                }


                if (status == (int)EnumStatusMotorista.EmAprovacao)
                    model.Motorista.LoginUsuario = UsuarioCsOnline?.Login ?? UserSession.GetCurrentInfoUserSystem().InformacoesUsuario.Login;

                model.Motorista.MotoristaBrasil.CPF = model.Motorista.MotoristaBrasil.CPF.RemoveCharacter();
                model.Motorista.DataAtualizazao = DateTime.Now;
                model.Motorista.Telefone = model.Motorista.Telefone.RemoveCharacter();
                model.Motorista.Ativo = true;
                model.Motorista.EmailSolicitante = UsuarioCsOnline?.Email ?? UserSession.GetCurrentInfoUserSystem().InformacoesUsuario.Email;
                model.Motorista.IDStatus = status;

                if (model.Motorista.IdPais == 0 && model.Motorista.MotoristaBrasil != null)
                    model.Motorista.IdPais = EnumPais.Brasil;


                if (!jaEnviadoQuickTas)
                {
                    if (primeiroCadastro)
                    {
                        if (model.Motorista.IDStatus == (int)EnumStatusMotorista.Aprovado && idPais == (int)EnumPais.Brasil && statusAtual != 0 && model.Motorista.IDEmpresa == (int)EnumEmpresa.Combustiveis)
                            result = _motoristaBll.EnviarDadosQuickTAS(model.Motorista);
                    }
                    else
                    if (idPais == (int)EnumPais.Brasil && statusAtual != 0 && model.Motorista.IDEmpresa == (int)EnumEmpresa.Combustiveis)
                        result = _motoristaBll.EnviarDadosQuickTAS(model.Motorista);

                    if (result.ToUpper().Contains("ERRO"))
                    {
                        model.Resultado = new ResultadoOperacao
                        {
                            Mensagem = result
                        };
                        model.ContemErrosModel = "S";
                        return PartialView("_EdicaoMotorista", model);
                    }
                }

                if (model.Operacao == OperacoesCRUD.Insert)
                {
                    model.Motorista.UsuarioAlterouStatus = UsuarioCsOnline != null && UsuarioCsOnline.Login != null ? UsuarioCsOnline.Login : UserSession.GetCurrentInfoUserSystem().InformacoesUsuario.Login;

                    //caso o usuário esteja incluindo um motorista existente, irá carregar apenas os clientes daquela transportadora/cliente na tela
                    //deverá pegar a lista antiga + as que foram adicionadas/excluídas
                    if (model.Motorista.IDMotorista != null && model.Motorista.Operacao == "FOB")
                    {
                        var usuario = new UsuarioBusiness().Selecionar(x => x.Login == model.Motorista.LoginUsuario);
                        var clientes = new MotoristaClienteBusiness().ListarMotoristaClientePorMotorista(model.Motorista.ID, model.Motorista.IDEmpresa, usuario.ID, true);
                        clientes.AddRange(model.Motorista.Clientes);
                        model.Motorista.Clientes = clientes.Distinct(c => c.IDCliente).ToList();
                    }

                    model.Resultado = ProcessarResultado(_motoristaBll.AdicionarMotorista(model.Motorista), OperacoesCRUD.Insert);

                    if (!string.IsNullOrEmpty(model.Motorista.Mensagem))
                        model.Resultado.Mensagem = model.Motorista.Mensagem;

					ZerarFiltro(model);
                    AtualizarQtdeRegPaginador(model);
                    model.Operacao = OperacoesCRUD.Update;
                }

                if (model.Operacao == OperacoesCRUD.Editando)
                {
                    model.Motorista.ID = int.Parse(model.ChavePrimaria);

                    Motorista motoristaAntigo = _motoristaBll.Selecionar(model.Motorista.ID);
                    if (motoristaAntigo != null)
                    {
                        string loginUsuarioLogado = UsuarioCsOnline?.Login ?? UserSession.GetCurrentInfoUserSystem().InformacoesUsuario.Login;
                        model.Motorista.UsuarioAlterouStatus = motoristaAntigo.IDStatus != status ? loginUsuarioLogado : motoristaAntigo.UsuarioAlterouStatus;
                    }

                    model.Resultado = base.ProcessarResultado(this._motoristaBll.AtualizarMotorista(model.Motorista, comRessalvas, bloqueio: false), OperacoesCRUD.Update);
                    if (!string.IsNullOrEmpty(model.Motorista.Mensagem))
                    {
                        model.Resultado.Mensagem = model.Motorista.Mensagem;
                        model.ContemErrosModel = "S";
                    }
                    model.Operacao = OperacoesCRUD.Update;
                }

            }
            else
            {
                //Renomeando IdControles para que sejam corretamente identificados nas mensagens de erro
                model.ValidacoesModelo.ForEach(vm =>
                {
                    if (vm.IdControle.Contains("Motorista_MotoristaBrasil"))
                        vm.IdControle = vm.IdControle.Replace("Motorista_", "");

                    if (vm.MensagemValidacao.Contains("Preenchimento Obrigatório!") && idPais == (int)EnumPais.Argentina)
                        vm.MensagemValidacao = "Por favor completar los campos obligatorios";
                });
                CarregarPermissaoUsuario(model);

                // Utilizado para corrigir o bug CSCUNI-1623 2ª parte correção
                model.Motorista.IDStatus = statusAtual;
            }

            return PartialView("_EdicaoMotorista", model);
        }


        #endregion

        #region carteirinha
        public ActionResult GerarPdf(int id)
        {
            return GerarPdfBase(id, EnumPais.Brasil);
        }

        [HttpGet]
        public virtual ActionResult Download(string fileGuid, string fileName)
        {
            return DownloadBase(fileGuid, fileName, EnumPais.Brasil);
        }
        #endregion

        #region In/Ativar/Bloqueio
        public PartialViewResult Ativar(int id)
        {
            ModelMotorista model = new ModelMotorista();
            model.ID = id;
            model.Motorista = _motoristaBll.Selecionar(id);
            model.ListaHistoricoAtivar = new HistorioAtivarMotoristaBusiness().Listar(p => p.IDMotorista == id).OrderByDescending(o => o.Data).ToList();
            return PartialView("_Ativar", model);
        }

        public PartialViewResult Bloquear(int id)
        {
            ModelMotorista model = new ModelMotorista();
            model.ID = id;
            model.Motorista = _motoristaBll.Selecionar(id);
            model.ListaHistorico = new HistorioBloqueioMotoristaBusiness().Listar(p => p.IDMotorista == id).OrderByDescending(o => o.Data).ToList();
            return PartialView("_Bloquear", model);
        }

        public JsonResult SalvarAtivar(int id, string justificativa, bool ativo)
        {
            return SalvarAtivarBase(id, justificativa, ativo, EnumPais.Brasil);
        }

        public JsonResult SalvarBloquear(int id, string justificativa, bool bloqueio)
        {
            return SalvarBloquearBase(id, justificativa, bloqueio, EnumPais.Brasil);
        }

        public JsonResult SalvarDocumentos(ModelMotorista model)
        {
            var json = new JsonResult();
            try
            {
                new MotoristaDocumentoBusiness().AtualizarDocumentos(model.Motorista.Documentos);
                json.Data = "S";
                return json;
            }
            catch (Exception ex)
            {
                json.Data = ex.Message;
                return json;
            }
        }

        public ActionResult SalvarTreinamento(ModelMotorista model)
        {
            model.TipoValidacao = TiposValidacao.ComExcecaoObrigatorios;
            if (ValidarModel(model, this.ModelState))
            {
                model.TreinamentoView.IDMotorista = model.ID;
                model.MotoristaTreinamentoTerminal.IDMotorista = model.ID;
                model.Resultado = base.ProcessarResultado(new MotoristaBusiness().SalvarTreinamento(model.ListaTerminais, model.TreinamentoView), OperacoesCRUD.Insert);
            }
            else
            {
                model.ContemErrosModel = "S";
                model.ListaTreinamento = new HistoricoTreinamentoTeoricoMotoristaBusiness().Listar(w => w.IDMotorista == model.ID);
            }
            return PartialView("_Treinamento", model);
        }
        #endregion

        public PartialViewResult Treinamento(int id)
        {
            ModelMotorista model = new ModelMotorista();
            model.TreinamentoView = new TreinamentoView();
            model.ListaTerminais = new TerminalBusiness().ListarPorMotorista(id);
            model.ListaTreinamento = new HistoricoTreinamentoTeoricoMotoristaBusiness().Listar(w => w.IDMotorista == id);
            return PartialView("_Treinamento", model);
        }

        #region ExcluirRegistro

        [HttpGet]
        [AjaxOnlyAttribute]
        public JsonResult Excluir(int id, int status)
        {
            try
            {
                var mensagem = _motoristaBll.ExcluirRegistro(id, status);
                return Json(new { retorno = mensagem }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { retorno = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        [HttpGet]
        public FileResult Exportar(MotoristaFiltro filtro)
        {
            Usuario usCliente = UsuarioCsOnline;

            if (usCliente != null)
            {
                filtro.IDUsuarioCliente = usCliente.ID;
                filtro.UsuarioExterno = usCliente.Externo;
            }
            else
            {
                usCliente = UsuarioLogado;
                if (usCliente != null)
                {
                    var usuario = new UsuarioBusiness().Selecionar(w => w.Login == usCliente.Login);
                    filtro.UsuarioExterno = usuario.Externo;

                    if (usuario.Perfil == "Transportadora")
                        filtro.IDUsuarioTransportadora = usuario.ID;
                    else if (usuario.Perfil == "Cliente EAB")
                        filtro.IDUsuarioCliente = usuario.ID;
                }
                else
                {
                    filtro.UsuarioExterno = true;
                }
            }

            if (!string.IsNullOrEmpty(filtro.CPF))
            {
                filtro.CPF = filtro.CPF.RemoveCharacter();
            }

            var fs = _motoristaBll.Exportar(filtro);
            string nomeArquivo = "Motorista_" + DateTime.Now.ToString("ddMMyyyyhhmmss") + ".xlsx";
            return File(fs, System.Net.Mime.MediaTypeNames.Application.Octet, nomeArquivo);
        }
    }
}

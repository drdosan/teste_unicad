using Infraestructure.Extensions;
using Raizen.Framework.Models;
using Raizen.Framework.UserSystem.Client;
using Raizen.Framework.Utils.Extensions;
using Raizen.Framework.Web.MVC.Filters;
using Raizen.Framework.Web.MVC.Models;
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

namespace Raizen.UniCad.Web.Controllers
{
    public class MotoristaArgentinaController : MotoristaBaseController
	{
		/// <summary>
		/// Nao validar acesso.
		/// </summary>
		public MotoristaArgentinaController() : base(BaseControllerOptions.NaoValidarAcesso)
		{
		}

		#region Novo

		[HttpGet]
		[AjaxOnlyAttribute]
		public ActionResult Novo()
		{
			ModelMotoristaArgentina model = new ModelMotoristaArgentina();
			model.Motorista = new Motorista();
			model.Resultado = new ResultadoOperacao();
			return PartialView("_Edicao", model);
		}

        #endregion

        #region Index

        [HttpGet]
		public ActionResult Index()
		{
			return CarregarDefault();
		}

		#endregion

		#region Editar

		[HttpGet]
		public ActionResult Editar(string id, bool? aprovar)
		{
			ModelMotoristaArgentina model = base.CarregarDadosMotorista(id, aprovar ?? false);
			var usuario = UsuarioLogado;
			if (usuario != null)
			{
				model.Motorista.IDEmpresa = usuario.IDEmpresa == 3 ? 0 : usuario.IDEmpresa;
			}

			return PartialView("Edicao", model);
		}

		public ActionResult Aprovar(string id)
		{
			ModelMotoristaArgentina model = base.CarregarDadosMotorista(id, true);
			return PartialView("_EdicaoMotorista", model);
		}

		public ActionResult Aprovacao(string id, int status, bool comRessalvas)
		{
			var model = CarregarDadosMotorista(id, true);

			model.Motorista.ID = int.Parse(model.ChavePrimaria);
			model.Motorista.IDStatus = status;
			model.Resultado = base.ProcessarResultado(this._motoristaBll.AtualizarMotorista(model.Motorista, comRessalvas, false, false, true), OperacoesCRUD.Update);
			if (!string.IsNullOrEmpty(model.Motorista.Mensagem))
			{
				model.Resultado.Mensagem = model.Motorista.Mensagem;
				model.ContemErrosModel = "S";
			}
			model.Operacao = OperacoesCRUD.Update;

			return PartialView("_Edicao", model);
		}

		private void CarregarAlteracoes(ModelMotoristaArgentina model)
		{
			if (model.Motorista.IDMotorista.HasValue)
			{
				var motoristaAnterior = _motoristaPesquisaBll.Selecionar(model.Motorista.IDMotorista.Value).Mapear();

				if (motoristaAnterior != null)
				{
					model.Alteracoes = _motoristaBll.CarregarAlteracoes(model.Motorista, motoristaAnterior);
				}
			}
		}
		
        #region EditarMotorista

		[HttpGet]
		public ActionResult EditarMotorista(int? id, string dni, int? idEmpresa, int? acao, bool aprovar = false, int naoAprovado = 0)
		{
			var model = new ModelMotoristaArgentina();
			dni = dni.RemoveCharacter();
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
				model.Motorista = _motoristaPesquisaBll.Selecionar(w => w.ID == id).Mapear();
				acesso = new MotoristaBusiness().ValidarAcesso(UserSession.GetCurrentInfoUserSystem().InformacoesUsuario.Login, model.Motorista);
				model.Aprovar = aprovar;
				model.Acao = (int)EnumAcao.Editar;
				model.Operacao = OperacoesCRUD.Editando;
				model.Resultado = new ResultadoOperacao();
				model.Motorista = _motoristaPesquisaBll.Selecionar(id.Value).Mapear();
				model.ChavePrimaria = model.Motorista.ID.ToString();
				model.Motorista.Documentos = new MotoristaDocumentoBusiness(EnumPais.Argentina).ListarMotoristaDocumentoPorMotorista(model.Motorista.ID, model.Motorista.Operacao);

				CarregarAlteracoes(model);
				model.ListaTipoProduto = new MotoristaTipoProdutoBusiness().ListarTipoProdutoPorMotorista(int.Parse(model.ChavePrimaria));
				model.ListaTipoComposicao = new MotoristaTipoComposicaoBusiness().ListarTipoComposicaoPorMotorista(int.Parse(model.ChavePrimaria));


				//se não for aprovado deverá apenas visualizar os dados e também não deverá ter a ação de anexar 
				model.Motorista.Documentos?.ForEach(x => x.Aprovar = aprovar);
				if (!aprovar && model.Motorista.IDStatus != (int)EnumStatusMotorista.Reprovado)
				{
					model.Motorista.IDMotorista = model.Motorista.ID;
					model.Motorista.ID = 0;
					model.ChavePrimaria = "0";
				}
				base.CarregarPermissaoUsuario(model);

			}
			//clonar
			else if ((id.HasValue || !string.IsNullOrEmpty(dni)) && acao == (int)EnumAcao.Clonar)
			{
				acesso = true;
				model.Operacao = OperacoesCRUD.Editando;
				model.Resultado = new ResultadoOperacao();

				model.Motorista = id.HasValue ? _motoristaPesquisaBll.Selecionar(id.Value).Mapear() : _motoristaPesquisaBll.Selecionar(w => w.DNI == dni && w.IDStatus == (int)EnumStatusMotorista.Aprovado).Mapear();
				if (model.Motorista != null)
				{
					if (usuario?.Operacao != "Ambos" && usuario != null)
						model.Motorista.Operacao = model.Motorista.OperacaoUsuario = usuario.Operacao;

					model.Motorista.IDEmpresa = model.Motorista.IDEmpresa == (int)EnumEmpresa.EAB ? (int)EnumEmpresa.Combustiveis : (int)EnumEmpresa.EAB;

					model.Motorista.Justificativa = null;
					model.Motorista.Documentos = new MotoristaDocumentoBusiness(EnumPais.Argentina).ListarMotoristaDocumento(model.Motorista.IDEmpresa, model.Motorista.Operacao);
					var documentosOrigem = new MotoristaDocumentoBusiness(EnumPais.Argentina).ListarMotoristaDocumentoPorMotorista(model.Motorista.ID);
					PreencheDocumentos(model, documentosOrigem);
				}
				else
				{
					return Json(new
					{
						status = "e",
						result = "El estatus de este conductor no fue modificado, por favor inténtelo de nuevo"
					}, JsonRequestBehavior.AllowGet);
				}

				if (model.Motorista.Operacao == "CIF")
				{
					if (idEmpresa != null) CarregarPermissaoCif(null, idEmpresa.Value, null, null);
				}
				else
				{
					if (idEmpresa != null) CarregarPermissaoFob(model.Motorista, null, idEmpresa.Value);
				}

				CarregarPermissaoUsuario(model);
			}
			//editar
			else if (!string.IsNullOrEmpty(dni) && acao == (int)EnumAcao.Editar)
			{
				model.Acao = acao.Value;
				model.Operacao = OperacoesCRUD.Editando;
				model.Resultado = new ResultadoOperacao();

				model.Motorista = new Motorista();

				//Tratamento para recuperar Motorista Reprovado.
				var idStatusMotorista = naoAprovado == 0 ? (int)EnumStatusMotorista.Aprovado : (int)EnumStatusMotorista.Reprovado;

				model.Motorista = _motoristaPesquisaBll.Selecionar(w => w.DNI == dni && w.IDEmpresa == idEmpresa && w.IDStatus == idStatusMotorista).Mapear();
				if (model.Motorista != null)
				{
					acesso = new MotoristaBusiness().ValidarAcesso(UserSession.GetCurrentInfoUserSystem().InformacoesUsuario.Login, model.Motorista);
					model.Motorista.naoAprovado = naoAprovado;
					model.ChavePrimaria = model.Motorista.ID.ToString();
					model.Motorista.Documentos = new MotoristaDocumentoBusiness(EnumPais.Argentina).ListarMotoristaDocumentoPorMotorista(model.Motorista.ID);
					model.ListaTipoComposicao = new MotoristaTipoComposicaoBusiness().ListarTipoComposicaoPorMotorista(model.Motorista.ID);
					model.ListaTipoProduto = new MotoristaTipoProdutoBusiness().ListarTipoProdutoPorMotorista(model.Motorista.ID);

					if (aprovar)
						CarregarAlteracoes(model);
					//se não for aprovado deverá apenas visualizar os dados e também não deverá ter a ação de anexar 
					model.Motorista.Documentos?.ForEach(x => x.naoAprovado = naoAprovado != 0);

					if (!aprovar && model.Motorista.IDStatus != (int)EnumStatusMotorista.Reprovado)
					{
						model.Motorista.IDMotorista = model.Motorista.ID;
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
					if (!string.IsNullOrEmpty(dni))
						model.Motorista.MotoristaArgentina.DNI = dni.RemoveCharacter();
				}
			}
			else
			{
				model.Novo = true;
				acesso = true;
				model.Operacao = OperacoesCRUD.Insert;
				model.Motorista = new Motorista();
				if (!string.IsNullOrEmpty(dni))
				{
					model.Motorista.MotoristaArgentina = new MotoristaArgentina();
					model.Motorista.MotoristaArgentina.DNI = dni.RemoveCharacter();
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

				model.UsuarioPerfil = usuario.Perfil.ToLowerInvariant();
			}

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

		private static void PreencheDocumentos(ModelMotoristaArgentina model, List<MotoristaDocumentoView> documentosOrigem)
		{
			MotoristaDocumentoView docOrigem = null;
			foreach (var doc in model.Motorista.Documentos)
			{
				if (documentosOrigem.Any(w => w.IDTipoDocumento == doc.IDTipoDocumento) && (docOrigem = documentosOrigem.FirstOrDefault(w => w.IDTipoDocumento == doc.IDTipoDocumento)) != null)
				{
					doc.DataVencimento = docOrigem.DataVencimento;
					doc.Anexo = docOrigem.Anexo;
				}
			}
		}

		public PartialViewResult CarregarPermissaoFob(Motorista motorista, int? idMotorista, int idEmpresa)
		{
			var loginUsuario = UsuarioCsOnline != null && UsuarioCsOnline.Login != null ? UsuarioCsOnline.Login : UserSession.GetCurrentInfoUserSystem().InformacoesUsuario.Login;
			var usuario = new UsuarioBusiness().Selecionar(w => w.Login == loginUsuario);

			ModelMotoristaArgentina model = new ModelMotoristaArgentina();
			Motorista motoristaPermissao;

			motoristaPermissao = CarregaPermissaoFobBase(motorista, idMotorista, idEmpresa, usuario);

			model.Motorista = motoristaPermissao;
			model.LinhaNegocio = idEmpresa;
			SelecionarTranspCliente(model, "FOB");

			return PartialView("_Cliente", model);
		}

		public PartialViewResult CarregarPermissaoCif(int? idMotorista, int idEmpresa, int? idTransportadora, bool? novo)
		{
			return CarregarPermissaoCifBase(idMotorista, idEmpresa, idTransportadora, novo, new ModelMotoristaArgentina());
		}

		#endregion
		
        #endregion

		#region MotoristaLayout

		private ModelMotoristaArgentina MotoristaLayout(ModelMotoristaArgentina model)
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

        #region CarregarDefault

        private ActionResult CarregarDefault()
		{
			ModelMotoristaArgentina model = new ModelMotoristaArgentina();
			model = MotoristaLayout(model);
			model.Operacao = OperacoesCRUD.List;
			model.Filtro = new MotoristaFiltro();
			model.Filtro.Ativo = true;
			model.Filtro.IdPais = (int)EnumPais.Argentina;
			CarregarPermissaoUsuario(model);

			Usuario usCliente = UsuarioCsOnline;
			if (usCliente != null)
			{
				model.Filtro.IDUsuarioCliente = usCliente.ID;
			}

			if (UserSession.GetCurrentInfoUserSystem() != null)
			{
				if (UserSession.GetCurrentInfoUserSystem().InformacoesPermissao.FirstOrDefault(p => p.MVCController == "MotoristaArgentina" && p.NomeAcao == "Aprovar") != null)
					model.Filtro.IDStatus = (int)EnumStatusMotorista.EmAprovacao;
			}
			else
			{
				return RedirectToAction("Index", "Home");
			}

			bool usuarioQuality;

			model.Resultado = new ResultadoOperacao();
			model.PaginadorDados = new PaginadorModel { Status = EstadoPaginador.RenovandoConsulta };
			base.ListarPaginador(model);
			model.Usuario = UsuarioLogado;
			if (model.Usuario != null)
			{
				model.Filtro.UsuarioExterno = (model.Usuario.Externo) || model.Usuario.Perfil.Contains("Transportadora") || model.Usuario.Perfil.Contains("Cliente");
				
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

		#endregion

		#region Pesquisar
		[HttpGet]
		[AjaxOnlyAttribute]
		public ActionResult Pesquisar(ModelMotoristaArgentina model)
		{
			CarregarPermissaoUsuario(model, true);

			Session["MotoristaFiltro"] = model.Filtro;

			model.Filtro.IdPais = (int)EnumPais.Argentina;

			Usuario usCliente = UsuarioCsOnline;

			if (usCliente != null)
			{
				model.Filtro.IDUsuarioCliente = usCliente.ID;
			}

			if (!string.IsNullOrEmpty(model.Filtro.DNI))
				model.Filtro.DNI = model.Filtro.DNI.RemoveCharacter();
			base.PrepararPaginadorOperacoes(model);
			Listar(model);

			bool usuarioQuality;
			model.Usuario = UsuarioLogado;
			if (model.Usuario != null)
			{
				model.Filtro.UsuarioExterno = (model.Usuario.Externo) || model.Usuario.Perfil.Contains("Transportadora") || model.Usuario.Perfil.Contains("Cliente");

				usuarioQuality = model?.Usuario.Perfil.Contains("Quality") ?? false;
				model.Filtro.UsuarioExterno = usuarioQuality ? false : model.Filtro.UsuarioExterno;

				return PartialView("_Pesquisa", model);
			}

			var login = UserSession.GetCurrentInfoUserSystem().InformacoesUsuario.Login;
			model.Usuario = new UsuarioBusiness().Selecionar(w => w.Login == login);
			bool usuarioTransportadora = model.Usuario?.Perfil.Contains("Transportadora") ?? true;
			bool usuarioCliente = model.Usuario?.Perfil.Contains("Cliente") ?? true;
			model.Filtro.UsuarioExterno = (model.Usuario?.Externo ?? true) || usuarioTransportadora || usuarioCliente;

			usuarioQuality = model?.Usuario.Perfil.Contains("Quality") ?? false;
			model.Filtro.UsuarioExterno = usuarioQuality ? false : model.Filtro.UsuarioExterno;

			return PartialView("_Pesquisa", model);
		}

		public JsonResult ValidarCpf(string cpf)
		{
			return Json(new { retorno = ValidacoesUtil.ValidaCPF(cpf) }, JsonRequestBehavior.AllowGet);
		}

		public JsonResult VerificarDniExiste(string dni, int idEmpresa)
		{
			return BuscarCpfDni(dni, idEmpresa, EnumPais.Argentina);
		}

		public PartialViewResult ListarComposicoes(int? idEmpresa, string operacao, int? idMotorista, int? composicaoUtilizada)
		{
			return PartialView("_Composicoes", null);
		}

        public PartialViewResult ListarProdutos(int? idEmpresa, string operacao, int? idMotorista, int? produtoCarregado)
		{
			return PartialView("_Produtos", null);
		}

		#region ListarDocumentos

		public PartialViewResult ListarDocumentos(int? idEmpresa, string operacao, bool aprovar, int? idMotorista, List<int> tipoProdutoList, List<int> tipoComposicaoList)
		{
			if (!idEmpresa.HasValue || string.IsNullOrEmpty(operacao) || tipoProdutoList == null || tipoComposicaoList == null)
				return PartialView("_Documentos", null);

			List<MotoristaDocumentoView> model = null;

			if (idMotorista.HasValue && idMotorista != 0)
				model = new MotoristaDocumentoBusiness(EnumPais.Argentina).ListarMotoristaDocumentoPorMotorista(idMotorista.Value, operacao, tipoProdutoList, tipoComposicaoList).GroupBy(mdv => mdv.IDTipoDocumento).Select(mdv => mdv.First()).ToList();
			if (model == null || model.Count == 0)
				model = new MotoristaDocumentoBusiness(EnumPais.Argentina).ListarMotoristaDocumento(idEmpresa, operacao, tipoProdutoList, tipoComposicaoList).GroupBy(mdv => mdv.IDTipoDocumento).Select(mdv => mdv.First()).ToList();

			model.ForEach(x =>
			{
				x.Aprovar = aprovar;
			});

			return PartialView("_Documentos", model);
		}
		
        #endregion

		#endregion

		#region Salvar

		[AjaxOnlyAttribute]
		public override ActionResult SalvarPermissoes(ModelMotoristaArgentina Model)
		{
			return base.SalvarPermissoes(Model);
		}

		[AjaxOnlyAttribute]
		public ActionResult Salvar(ModelMotoristaArgentina model, int status, bool comRessalvas, Dictionary<string, string> tipoProdutoList, Dictionary<string, string> tipoComposicaoList)
		{
			model.comRessalvas = comRessalvas;
			model.TipoValidacao = TiposValidacao.ValidacaoTotal;

			// Utilizado para corrigir o bug CSCUNI-1623 - 1ª parte correção
			var statusAtual = model.Motorista.IDStatus;

			model.Motorista.IDStatus = (int)EnumStatusMotorista.EmAprovacao;

			//Remove validação de combos de filtro
			if (!ValidarModel(model, ModelState))
				model.ValidacoesModelo.RemoveAll(vm => vm.IdControle == "lbl_Filtro_IdTipoProduto" || vm.IdControle == "lbl_Filtro_IdTipoComposicao"
                 || vm.IdControle == "lbl_Motorista_Telefone" || vm.IdControle == "lbl_Motorista_Email");

			if (model.ValidacoesModelo.Count == 0)
			{
				int result = 0;
				model.ListaTipoProduto = new List<TipoProduto>();
				if (!tipoProdutoList.Any(w => !int.TryParse(w.Key, out result)))
					foreach (var item in tipoProdutoList)
					{
						model.ListaTipoProduto.Add(new TipoProduto
						{
							ID = int.Parse(item.Key),
							Nome = item.Value
						});
					}

				model.ListaTipoComposicao = new List<TipoComposicao>();
				if (!tipoComposicaoList.Any(w => !int.TryParse(w.Key, out result)))
					foreach (var item in tipoComposicaoList)
					{
						model.ListaTipoComposicao.Add(new TipoComposicao
						{
							ID = int.Parse(item.Key),
							Nome = item.Value
						});
					}

				model.ContemErrosModel = "N";

				if (status == (int)EnumStatusMotorista.EmAprovacao)
					model.Motorista.LoginUsuario = UsuarioCsOnline?.Login ?? UserSession.GetCurrentInfoUserSystem().InformacoesUsuario.Login;

				model.Motorista.MotoristaArgentina.DNI = model.Motorista.MotoristaArgentina.DNI.RemoveCharacter();
				model.Motorista.DataAtualizazao = DateTime.Now;
				model.Motorista.Telefone = model.Motorista.Telefone.RemoveCharacter();
				model.Motorista.Ativo = true;
				model.Motorista.EmailSolicitante = UsuarioCsOnline?.Email ?? UserSession.GetCurrentInfoUserSystem().InformacoesUsuario.Email;
				model.Motorista.IDStatus = status;
				model.Motorista.IdPais = EnumPais.Argentina; //Todo : deve vir na model

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
					var motoristaArgentinaBLL = new MotoristaBusiness(EnumPais.Argentina);
					model.Resultado = ProcessarResultado(motoristaArgentinaBLL.AdicionarMotorista(model.Motorista, model.ListaTipoProduto.Select(w => w.ID).ToList(), model.ListaTipoComposicao.Select(w => w.ID).ToList()), OperacoesCRUD.Insert);

					if (model.Resultado.CodigoResultado == TipoResultado.Sucess)
						model.Resultado.Mensagem = "¡Registro incluido con éxito!";

					ZerarFiltro(model);
					AtualizarQtdeRegPaginador(model);
					model.Operacao = OperacoesCRUD.Update;
				}

				if (model.Operacao == OperacoesCRUD.Editando)
				{
					model.Motorista.ID = int.Parse(model.ChavePrimaria);

					if (model.Motorista.ID == 0)
						model.Motorista.ID = (int)model.Motorista.IDMotorista;

					Motorista motoristaAntigo = _motoristaPesquisaBll.Selecionar(model.Motorista.ID).Mapear();

					if (motoristaAntigo != null)
					{
						string loginUsuarioLogado = UsuarioCsOnline?.Login ?? UserSession.GetCurrentInfoUserSystem().InformacoesUsuario.Login;
						model.Motorista.UsuarioAlterouStatus = motoristaAntigo.IDStatus != status ? loginUsuarioLogado : motoristaAntigo.UsuarioAlterouStatus;
					}

					model.Resultado = base.ProcessarResultado(new MotoristaBusiness(EnumPais.Argentina).AtualizarMotorista(model.Motorista, comRessalvas, false, false, true, model.ListaTipoProduto.ToList(), model.ListaTipoComposicao.ToList()), OperacoesCRUD.Update);

					if (model.Resultado.CodigoResultado == TipoResultado.Sucess)
						model.Resultado.Mensagem = "¡Registro actualizado con éxito!";

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
					if (vm.IdControle.Contains("Motorista_MotoristaArgentina"))
						vm.IdControle = vm.IdControle.Replace("Motorista_", "");

					if (vm.MensagemValidacao.Contains("Preenchimento Obrigatório!"))
						vm.MensagemValidacao = "Por favor completar los campos obligatorios";
				});

				CarregarPermissaoUsuario(model);

				// Utilizado para corrigir o bug CSCUNI-1623 2ª parte correção
				model.Motorista.IDStatus = statusAtual;

				int result = 0;
				model.ListaTipoProduto = new List<TipoProduto>();
				if (!tipoProdutoList.Any(w => !int.TryParse(w.Key, out result)))
					foreach (var item in tipoProdutoList)
					{
						model.ListaTipoProduto.Add(new TipoProduto
						{
							ID = int.Parse(item.Key),
							Nome = item.Value
						});
					}

				model.ListaTipoComposicao = new List<TipoComposicao>();
				if (!tipoComposicaoList.Any(w => !int.TryParse(w.Key, out result)))
					foreach (var item in tipoComposicaoList)
					{
						model.ListaTipoComposicao.Add(new TipoComposicao
						{
							ID = int.Parse(item.Key),
							Nome = item.Value
						});
					}
			}

			return PartialView("_EdicaoMotorista", model);
		}

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

					if (usuario.Perfil == "Transportadora" || usuario.Perfil == "Transportadora Argentina")
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

			using (var motoristaArgentinaBLL = new MotoristaBusiness(EnumPais.Argentina))
			{
				var fs = motoristaArgentinaBLL.Exportar(filtro);
				string nomeArquivo = "Choferes_" + DateTime.Now.ToString("ddMMyyyyhhmmss") + ".xlsx";
				return File(fs, System.Net.Mime.MediaTypeNames.Application.Octet, nomeArquivo);
			}
		}

		public PartialViewResult AdicionarTipoProduto(ModelMotoristaArgentina modelMotorista, int idTipoProduto, int idPais)
		{
			var model = new TipoDocumentoTipoProdutoView();
			var tipo = new TipoProdutoBusiness().Selecionar(idTipoProduto);
			model.IDTipoProduto = tipo.ID;
			model.Nome = tipo.Nome;

			if (modelMotorista.ListaTipoProduto == null)
				modelMotorista.ListaTipoProduto = new List<TipoProduto>();

			modelMotorista.ListaTipoProduto.Add(tipo);

			return PartialView("~/Views/MotoristaArgentina/_Produtos.cshtml", modelMotorista);
		}

		public PartialViewResult AdicionarComposicaoMotorista(ModelMotoristaArgentina modelMotorista, int idTipoComposicao, int idPais)
		{
			var model = new TipoDocumentoTipoComposicaoPlacaView();
			var tipo = new TipoComposicaoBusiness().Selecionar(idTipoComposicao);
			model.IdComposicao = tipo.ID;
			model.NomeComposicao = tipo.Nome;

			if (modelMotorista.ListaTipoComposicao == null)
				modelMotorista.ListaTipoComposicao = new List<TipoComposicao>();

			modelMotorista.ListaTipoComposicao.Add(tipo);

			return PartialView("~/Views/MotoristaArgentina/_Composicoes.cshtml", modelMotorista);
		}

		#endregion

		#region Carteirinha / Treinamento

		public ActionResult GerarPdf(int id)
		{
			return GerarPdfBase(id, EnumPais.Argentina);
		}

		[HttpGet]
		public virtual ActionResult Download(string fileGuid, string fileName)
		{
			return DownloadBase(fileGuid, fileName, EnumPais.Argentina);
		}

		public PartialViewResult Treinamento(int id)
		{
			ModelMotoristaArgentina model = new ModelMotoristaArgentina();
			model.TreinamentoView = new TreinamentoView();
			model.ListaTerminais = new TerminalBusiness().ListarPorMotorista(id);
			model.ListaTreinamento = new HistoricoTreinamentoTeoricoMotoristaBusiness().Listar(w => w.IDMotorista == id);
			return PartialView("_Treinamento", model);
		}

        #endregion

        #region In/Ativar/Bloqueio

        public PartialViewResult Ativar(int id)
		{
			ModelMotoristaArgentina model = new ModelMotoristaArgentina();
			model.ID = id;
			model.Motorista = _motoristaPesquisaBll.Selecionar(id).Mapear();
			model.ListaHistoricoAtivar = new HistorioAtivarMotoristaBusiness().Listar(p => p.IDMotorista == id).OrderByDescending(o => o.Data).ToList();
			return PartialView("_Ativar", model);
		}

		public PartialViewResult Bloquear(int id)
		{
			ModelMotoristaArgentina model = new ModelMotoristaArgentina();
			model.ID = id;
			model.Motorista = _motoristaPesquisaBll.Selecionar(id).Mapear();
			model.ListaHistorico = new HistorioBloqueioMotoristaBusiness().Listar(p => p.IDMotorista == id).OrderByDescending(o => o.Data).ToList();
			return PartialView("_Bloquear", model);
		}

		public JsonResult SalvarAtivar(int id, string justificativa, bool ativo)
		{
			return SalvarAtivarBase(id, justificativa, ativo, EnumPais.Argentina);
		}

		public JsonResult SalvarBloquear(int id, string justificativa, bool bloqueio)
		{
			return SalvarBloquearBase(id, justificativa, bloqueio, EnumPais.Argentina);
		}

		public JsonResult SalvarDocumentos(ModelMotoristaArgentina model)
		{
			var json = new JsonResult();
			try
			{
				new MotoristaDocumentoBusiness(EnumPais.Argentina).AtualizarDocumentos(model.Motorista.Documentos);
				json.Data = "S";
				return json;
			}
			catch (Exception ex)
			{
				json.Data = ex.Message;
				return json;
			}
		}

		public ActionResult SalvarTreinamento(ModelMotoristaArgentina model)
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

		#region ExcluirRegistro

		[HttpGet]
		[AjaxOnlyAttribute]
		public JsonResult Excluir(int id, int status)
		{
			try
			{
				using (var motoristaBusiness = new MotoristaBusiness(EnumPais.Argentina))
				{
					var mensagem = motoristaBusiness.ExcluirRegistro(id, status);
					return Json(new { retorno = mensagem }, JsonRequestBehavior.AllowGet);
				}
			}
			catch (Exception ex)
			{
				return Json(new { retorno = ex.Message }, JsonRequestBehavior.AllowGet);
			}
		}

		#endregion
	}
}
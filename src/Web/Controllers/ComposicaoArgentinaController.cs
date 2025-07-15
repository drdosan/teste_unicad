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
using Raizen.UniCad.Web.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;

namespace Raizen.UniCad.Web.Controllers
{
	public class ComposicaoArgentinaController : ComposicaoBaseController
	{
		public ComposicaoArgentinaController() : base(BaseControllerOptions.NaoValidarAcesso)
		{
		}
		private readonly PlacaBusiness placaBll = new PlacaBusiness(EnumPais.Argentina);

		#region Views

		[HttpGet]
		public ActionResult Index()
		{
			return this.CarregarDefault();
		}

		[HttpGet]
		public ActionResult Editar(string Id)
		{
			ModelComposicao Model = this.CarregarDadosComposicao(Id, false, EnumPais.Argentina);

			return PartialView("Edicao", Model);
		}

		#endregion


		#region Public methods

		public override JsonResult VerificarAlteracoes(ModelComposicao model)
		{
			bool isOutrosDadosAlterados = false;

			var composicaoAtual = _composicaoBLL.Selecionar(model.Composicao.IDComposicao.Value);
			if (composicaoAtual != null)
			{
				if (model.Composicao.IDPlaca1.HasValue && model.Composicao.IDPlaca1 > 0 && composicaoAtual.IDPlaca1.HasValue && composicaoAtual.IDPlaca1 > 0)
					model.Composicao.isPlaca1Alterada = placaBll.isPlacaAlterada(model.Composicao.IDPlaca1.Value, composicaoAtual.IDPlaca1.Value, true, true);
				if (model.Composicao.IDPlaca2.HasValue && model.Composicao.IDPlaca2 > 0 && composicaoAtual.IDPlaca2.HasValue && composicaoAtual.IDPlaca2 > 0)
					model.Composicao.isPlaca2Alterada = placaBll.isPlacaAlterada(model.Composicao.IDPlaca2.Value, composicaoAtual.IDPlaca2.Value, true, true);
				if (model.Composicao.IDPlaca3.HasValue && model.Composicao.IDPlaca3 > 0 && composicaoAtual.IDPlaca3.HasValue && composicaoAtual.IDPlaca3 > 0)
					model.Composicao.isPlaca3Alterada = placaBll.isPlacaAlterada(model.Composicao.IDPlaca3.Value, composicaoAtual.IDPlaca3.Value, true, true);
			}

			isOutrosDadosAlterados = model.Composicao.isPlaca1Alterada
					   || model.Composicao.isPlaca2Alterada
					   || model.Composicao.isPlaca3Alterada;

			return Json(new
			{
				Data = new
				{
					isOutrosDadosAlterados
				}
			}, JsonRequestBehavior.AllowGet);

		}

		public override ActionResult Salvar(ModelComposicao Model, int status, bool comRessalvas, bool forcar, bool comAlteracoes = false)
		{
			Model.comRessalvas = comRessalvas;
			Model.TipoValidacao = TiposValidacao.ValidacaoTotal;
			Model.Composicao.IgnorarLeci = forcar;

			if (Model.Composicao.Operacao == "FOB")
				EqualizarPlacasClientes(Model);

			if (base.ValidarModel(Model, this.ModelState))
			{
				Model.Composicao.IsUsuarioCsOnline = UsuarioCsOnline != null;

				if (Model.Operacao == OperacoesCRUD.Insert)
				{
					Model.Composicao.IDStatus = status;
					Model.Composicao.LoginUsuario = Model.Composicao.LoginUsuarioCorrente = UsuarioCsOnline != null && UsuarioCsOnline.Login != null ? UsuarioCsOnline.Login : UserSession.GetCurrentInfoUserSystem().InformacoesUsuario.Login;
					Model.Composicao.EmailSolicitante = UsuarioCsOnline != null && UsuarioCsOnline.Email != null ? UsuarioCsOnline.Email.Trim() : UserSession.GetCurrentInfoUserSystem().InformacoesUsuario.Email.Trim();
					Model.Composicao.UsuarioAlterouStatus = UsuarioCsOnline?.Login ?? UserSession.GetCurrentInfoUserSystem().InformacoesUsuario.Login;
					Model.Resultado = base.ProcessarResultado(new ComposicaoBusiness(EnumPais.Argentina).AdicionarComposicao(composicao: Model.Composicao), OperacoesCRUD.Insert);

					if (Model.Resultado.CodigoResultado == TipoResultado.Sucess)
						Model.Resultado.Mensagem = "¡Registro incluido con éxito!";

					ZerarFiltro(Model);
					AtualizarQtdeRegPaginador(Model);
					Model.Operacao = OperacoesCRUD.Update;
				}

				if (Model.Operacao == OperacoesCRUD.Editando)
				{
					Model.Composicao.ID = int.Parse(Model.ChavePrimaria);
					Model.Composicao.LoginUsuarioCorrente = UsuarioCsOnline != null && UsuarioCsOnline.Login != null ? UsuarioCsOnline.Login : UserSession.GetCurrentInfoUserSystem().InformacoesUsuario.Login;
					Model.Composicao.EmailSolicitante = UsuarioCsOnline != null && UsuarioCsOnline.Email != null ? UsuarioCsOnline.Email.Trim() : UserSession.GetCurrentInfoUserSystem().InformacoesUsuario.Email.Trim();

					Composicao composicaoAntiga = new ComposicaoBusiness(EnumPais.Argentina).Selecionar(Model.Composicao.ID);
					if (composicaoAntiga != null)
					{
						string loginUsuarioLogado = UsuarioCsOnline?.Login ?? UserSession.GetCurrentInfoUserSystem().InformacoesUsuario.Login;
						Model.Composicao.UsuarioAlterouStatus = composicaoAntiga.IDStatus != status ? loginUsuarioLogado : composicaoAntiga.UsuarioAlterouStatus;
					}
					using (var composicaoBll = new ComposicaoBusiness(EnumPais.Argentina))
						Model.Resultado = base.ProcessarResultado(composicaoBll.AtualizarComposicao(Model.Composicao, comRessalvas, idStatus: status), OperacoesCRUD.Update);

					if (Model.Resultado.CodigoResultado == TipoResultado.Sucess)
						Model.Resultado.Mensagem = "¡Registro actualizado con éxito!";

					if (!string.IsNullOrEmpty(Model.Composicao.Mensagem))
					{
						Model.Resultado.Mensagem = Model.Composicao.Mensagem;
						Model.ContemErrosModel = "S";
					}
					Model.Operacao = OperacoesCRUD.Update;
				}
			}
			if (Model.Composicao != null)
			{
				if (Model.Composicao.CPFCNPJ != null)
					Model.Composicao.CPFCNPJ = Model.Composicao.CPFCNPJ.RemoveCharacter();

				if (Model.Composicao.CPFCNPJArrendamento != null)
					Model.Composicao.CPFCNPJArrendamento = Model.Composicao.CPFCNPJArrendamento.RemoveCharacter();

				if (Model.Composicao != null)
					Model.Composicao.ComposicaoEixo = CarregarEixos(Model.Composicao.IDTipoComposicao);
			}

			return PartialView("~/Views/ComposicaoArgentina/Partial/_Edicao.cshtml", Model);
		}

        public override ModelComposicao CarregarDadosComposicao(string Id, bool aprovar, EnumPais pais)
		{
			ModelComposicao model = new ModelComposicao { Composicao = new Composicao(), IdPais = (int)pais };
			if (!aprovar)
				model.Composicao.LoginUsuario = UsuarioCsOnline?.Login ?? UserSession.GetCurrentInfoUserSystem().InformacoesUsuario.Login;
			model.Aprovar = aprovar;
			if (!string.IsNullOrEmpty(Id))
			{
				model.Operacao = OperacoesCRUD.Editando;
				model.ChavePrimaria = Id;
				model.Resultado = new ResultadoOperacao();
				Composicao composicaoAtual = null;

				using (var placaArgentinaBll = new PlacaBusiness(EnumPais.Argentina))
				{
					model.Composicao = this._composicaoBLL.Selecionar(int.Parse(model.ChavePrimaria));
					if (!aprovar)
						model.Composicao.isUtilizaPlacaChave = false;

					model.isArrendamento = String.IsNullOrEmpty(model.Composicao.CPFCNPJArrendamento) ? "FALSE" : "TRUE";
					if (!aprovar && model.Composicao.IDStatus != (int)EnumStatusComposicao.Reprovado)
					{
						model.Composicao.IDComposicao = model.Composicao.ID;
						model.Composicao.ID = 0;
						model.ChavePrimaria = "0";
					}
					else if (model.Composicao.IDComposicao.HasValue && (composicaoAtual = _composicaoBLL.Selecionar(model.Composicao.IDComposicao.Value)) != null)
					{
						if (model.Composicao.IDPlaca1.HasValue && model.Composicao.IDPlaca1 > 0 && composicaoAtual.IDPlaca1.HasValue && composicaoAtual.IDPlaca1 > 0)
						{
							model.Composicao.isPlaca1Alterada = placaArgentinaBll.isPlacaAlterada(model.Composicao.IDPlaca1.Value, composicaoAtual.IDPlaca1.Value);
							model.Composicao.PlacaOficial1 = composicaoAtual.IDPlaca1.Value;
						}
						if (model.Composicao.IDPlaca2.HasValue && model.Composicao.IDPlaca2 > 0 && composicaoAtual.IDPlaca2.HasValue && composicaoAtual.IDPlaca2 > 0)
						{
							model.Composicao.isPlaca2Alterada = placaArgentinaBll.isPlacaAlterada(model.Composicao.IDPlaca2.Value, composicaoAtual.IDPlaca2.Value);
							model.Composicao.PlacaOficial2 = composicaoAtual.IDPlaca2.Value;
						}
						if (model.Composicao.IDPlaca3.HasValue && model.Composicao.IDPlaca3 > 0 && composicaoAtual.IDPlaca3.HasValue && composicaoAtual.IDPlaca3 > 0)
						{
							model.Composicao.isPlaca3Alterada = placaArgentinaBll.isPlacaAlterada(model.Composicao.IDPlaca3.Value, composicaoAtual.IDPlaca3.Value);
							model.Composicao.PlacaOficial3 = composicaoAtual.IDPlaca3.Value;
						}
						if (model.Composicao.IDPlaca4.HasValue && model.Composicao.IDPlaca4 > 0 && composicaoAtual.IDPlaca4.HasValue && composicaoAtual.IDPlaca4 > 0)
						{
							model.Composicao.isPlaca4Alterada = placaArgentinaBll.isPlacaAlterada(model.Composicao.IDPlaca4.Value, composicaoAtual.IDPlaca4.Value);
							model.Composicao.PlacaOficial4 = composicaoAtual.IDPlaca4.Value;
						}
					}

					if (model.Composicao.IDPlaca1.HasValue && model.Composicao.IDPlaca1 > 0)
					{
						model.Composicao.isPlaca1Pendente = placaArgentinaBll.isPlacaPendente(model.Composicao.IDPlaca1.Value);
						model.Composicao.Placa1 = placaArgentinaBll.Selecionar(model.Composicao.IDPlaca1.Value).PlacaVeiculo;
					}
					if (model.Composicao.IDPlaca2.HasValue && model.Composicao.IDPlaca2 > 0)
					{
						model.Composicao.isPlaca2Pendente = placaArgentinaBll.isPlacaPendente(model.Composicao.IDPlaca2.Value);
						model.Composicao.Placa2 = placaArgentinaBll.Selecionar(model.Composicao.IDPlaca2.Value).PlacaVeiculo;
					}
					if (model.Composicao.IDPlaca3.HasValue && model.Composicao.IDPlaca3 > 0)
					{
						model.Composicao.isPlaca3Pendente = placaArgentinaBll.isPlacaPendente(model.Composicao.IDPlaca3.Value);
						model.Composicao.Placa3 = placaArgentinaBll.Selecionar(model.Composicao.IDPlaca3.Value).PlacaVeiculo;
					}
					if (model.Composicao.IDPlaca4.HasValue && model.Composicao.IDPlaca4 > 0)
					{
						model.Composicao.isPlaca4Pendente = placaArgentinaBll.isPlacaPendente(model.Composicao.IDPlaca4.Value);
						model.Composicao.Placa4 = placaArgentinaBll.Selecionar(model.Composicao.IDPlaca4.Value).PlacaVeiculo;
					}
					model.Composicao.Documentos = new List<PlacaDocumentoView>();

					var retorno = ObterPlacas(model.Composicao.IDPlaca1, model.Composicao.IDPlaca2, model.Composicao.IDPlaca3, model.Composicao.IDPlaca4);
					model.Composicao.Documentos = retorno;
					var eixos = ObterDadosPlaca(model.Composicao.IDPlaca1, model.Composicao.IDPlaca2, model.Composicao.IDPlaca3, model.Composicao.IDPlaca4, model.Composicao.IDEmpresa);
					model.Composicao.EixosComposicao = eixos.NumeroEixos;
					model.Composicao.EixosDistanciados = eixos.NumeroEixosDistanciados;
					model.Composicao.EixosPneusDuplos = eixos.NumeroEixosPneusDuplos;
					model.Composicao.TaraComposicao = eixos.Tara;
				}
			}

			var usuario = UsuarioLogado;
			if (usuario != null)
			{
				if (String.IsNullOrEmpty(model.Composicao.Operacao))
				{
					model.Composicao.Operacao = usuario.Operacao;
				}

				if (model.Operacao != OperacoesCRUD.Editando && usuario.IDEmpresa != 3)
				{
					model.Composicao.IDEmpresa = usuario.IDEmpresa;
				}

				model.UsuarioPerfil = usuario.Perfil.ToLowerInvariant();
			}

			return model;
		}

		public override PartialViewResult ListarDocumentos(int? placa1, int? placa2, int? placa3, int? placa4)
		{
			var model = ObterPlacas(placa1, placa2, placa3, placa4);

			return PartialView("~/Views/ComposicaoArgentina/Partial/_Documentos.cshtml", model);
		}

        public override JsonResult ObterDados(string placa1, string placa2, string placa3, string placa4, int IDEmpresa)
		{
			int? p1 = null;
			int? p2 = null;
			int? p3 = null;
			int? p4 = null;

			if (!string.IsNullOrEmpty(placa1))
				p1 = Convert.ToInt32(placa1);

			if (!string.IsNullOrEmpty(placa2))
				p2 = Convert.ToInt32(placa2);

			if (!string.IsNullOrEmpty(placa3))
				p3 = Convert.ToInt32(placa3);

			if (!string.IsNullOrEmpty(placa4))
				p4 = Convert.ToInt32(placa4);

			PlacaView retorno = ObterDadosPlaca(p1, p2, p3, p4, IDEmpresa);
			JsonResult json = new JsonResult { Data = retorno };

			return json;
		}

        public override PlacaView ObterDadosPlaca(int? placa1, int? placa2, int? placa3, int? placa4, int IDEmpresa)
		{
			PlacaView retorno = new PlacaView(0, 0, 0, EnumCategoriaVeiculo.Particular, EnumPais.Argentina);

			CarregarPlaca(placa1, retorno, 1, IDEmpresa);
			CarregarPlaca(placa2, retorno, 2, IDEmpresa);
			CarregarPlaca(placa3, retorno, 3, IDEmpresa);
			CarregarPlaca(placa4, retorno, 4, IDEmpresa);

			return retorno;
		}

        public override Placa CarregarPlaca(int? placaVeiculo, PlacaView retorno, int placaId, int IDEmpresa)
		{

			Placa placa = null;
			if (placaVeiculo.HasValue)
			{
				placa = _placaBll.Selecionar(p => p.ID == placaVeiculo);
				placa.PlacaArgentina = placaArgentinaBll.Selecionar(p => p.IDPlaca == placaVeiculo);

				if (placa != null)
				{
					if (placaId <= 2 && (placaId == 1 || IDEmpresa == (int)EnumEmpresa.Combustiveis))
					{
						retorno.Cuit = placa.PlacaArgentina.CUIT;
						retorno.RazaoSocial = placa.RazaoSocial;
						retorno.Datas = placa.DataNascimento.HasValue ? placa.DataNascimento.Value.ToShortDateString() : "";
					}

					retorno.PBTC += placa.PlacaArgentina.PBTC;
					retorno.NumeroEixos += placa.NumeroEixos;
					retorno.NumeroEixosDistanciados += placa.NumeroEixosDistanciados.HasValue ? placa.NumeroEixosDistanciados : 0;
					retorno.NumeroEixosPneusDuplos += placa.NumeroEixosPneusDuplos.HasValue ? placa.NumeroEixosPneusDuplos : 0;
					retorno.Tara += placa.Tara;
					retorno.Tara = Math.Round(retorno.Tara, 2);
					if (placa.IDCategoriaVeiculo == (int)EnumCategoriaVeiculo.Aluguel)
						retorno.IDCategoriaVeiculo = (int)EnumCategoriaVeiculo.Aluguel;
				}
			}

			return placa;
		}

        public ActionResult CarregarDefault()
		{
			return CarregarDefaultBase(EnumPais.Argentina);
		}

		public override ActionResult Aprovar(string Id)
		{
			ModelComposicao Model = CarregarDadosComposicao(Id, true, EnumPais.Argentina);

			return PartialView("Edicao", Model);
		}

        public PartialViewResult Bloquear(int idComposicao)
		{
			ModelPlaca model = new ModelPlaca();
			var comp = new ComposicaoBusiness().Selecionar(idComposicao);
			model.ListaPlaca = new PlacaBusiness().ListarPorComposicao(comp);
			model.IDComposicao = idComposicao;
			model.ListaHistorico = new HistorioBloqueioComposicaoBusiness().Listar(p => p.IDComposicao == idComposicao);
			return PartialView("~/Views/ComposicaoArgentina/Partial/_Bloquear.cshtml", model);
		}

		public JsonResult SalvarBloquear(int idComposicao, string justificativa, bool bloqueado)
		{
			var bloqueio = new HistorioBloqueioComposicao();
			bloqueio.Bloqueado = bloqueado;
			bloqueio.IDComposicao = idComposicao;
			bloqueio.Justificativa = justificativa;
			bloqueio.Data = DateTime.Now;
			bloqueio.Usuario = UserSession.GetCurrentInfoUserSystem().InformacoesUsuario.Nome;
			bloqueio.CodigoUsuario = UserSession.GetCurrentInfoUserSystem().InformacoesUsuario.Login;
			var msg = new HistorioBloqueioComposicaoBusiness().AdicionarBloqueio(bloqueio, EnumPais.Argentina);

			var json = new JsonResult();
			json.Data = msg;
			return json;
		}

		public override void AtualizarQtdeRegPaginador(ModelComposicao Model)
		{
			Model.Filtro = base.RetornaDados<ComposicaoFiltro>(NomeFiltro);
			base.ArmazenarDados<ComposicaoFiltro>(Model.Filtro, NomeFiltro);

			int totalRegistros = new ComposicaoPesquisaBusiness(EnumPais.Argentina).ListarComposicaoCount(Model.Filtro);
			base.ArmazenarDados<int>(totalRegistros, TotalRegistros);
		}

        public override ActionResult Pesquisar(ModelComposicao Model)
		{
			Model.IdPais = (int)EnumPais.Argentina;

			CarregarPermissaoUsuario(Model, true);

			Session["ComposicaoFiltro"] = Model.Filtro;

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
				Model.Filtro.UsuarioExterno = usCliente.Externo;
				Model.UsuarioPerfil = usCliente.Perfil;
				bool usuarioQuality;
				usuarioQuality = usCliente?.Perfil.Contains("Quality") ?? false;
				Model.Filtro.UsuarioExterno = usuarioQuality ? false : Model.Filtro.UsuarioExterno;
			}
			else
			{
				Usuario usuarioLogado = UsuarioLogado;
				bool usuarioTransportadora = usuarioLogado?.Perfil.Contains("Transportadora") ?? true;
				bool usuarioCliente = usuarioLogado?.Perfil.Contains("Cliente") ?? true;
				Model.Filtro.UsuarioExterno = (usuarioLogado?.Externo ?? true) || usuarioTransportadora || usuarioCliente;
				Model.UsuarioPerfil = usuarioLogado?.Perfil;
				bool usuarioQuality;
				usuarioQuality = usuarioLogado?.Perfil.Contains("Quality") ?? false;
				Model.Filtro.UsuarioExterno = usuarioQuality ? false : Model.Filtro.UsuarioExterno;
			}

			PrepararPaginadorOperacoes(Model);
			this.Listar(Model);

			return PartialView("~/Views/ComposicaoArgentina/Partial/_Pesquisa.cshtml", Model);
		}

		public FileResult Exportar(ComposicaoFiltro filtro)
		{
			return ExportarBase(filtro, EnumPais.Argentina);
		}

		public override JsonResult VerificarPlaca(string placa, int numero, int? tipoComposicao, string composicao, string operacao, string idEmpresa)
		{
			ModelPlaca model = new ModelPlaca();
			var json = new JsonResult();

			int empresa = string.IsNullOrEmpty(idEmpresa) ? 0 : Convert.ToInt32(idEmpresa);
			model.Placa = new PlacaBusiness(EnumPais.Argentina).ListarPlacaEmAprovacao(placa, operacao, empresa);

			if (model.Placa == null)
			{
				model.Placa = new Placa();
				json.Data = model;
				return json;
			}

			model.Mensagem = "¡Este vehículo ya está registrado! Si deseas verlo, haz clic en el icono 'Lápiz'";
			model.MensagemId = EnumMensagemPlaca.VeiculoJaCadastrado;

			var composicaoAprovado = _placaBll.ListarPorStatus(model.Placa.ID, composicao, null, EnumStatusComposicao.EmAprovacao, operacao);

			bool composicaoAguardandoSAP = false;
			if (model.Placa.IDTipoVeiculo == (int)EnumTipoVeiculo.Truck
			|| (empresa == (int)EnumEmpresa.EAB && numero == 1)
			|| (empresa == (int)EnumEmpresa.Combustiveis && numero == 2)
			|| (empresa == (int)EnumEmpresa.Ambos && (numero == 1 || numero == 2)))
				composicaoAguardandoSAP = _placaBll.ListarPorStatus(model.Placa.ID, composicao, null, EnumStatusComposicao.AguardandoAtualizacaoSAP, operacao);

			if (composicaoAprovado)
			{
				model.Mensagem = "Patente en espera de aprobación de otra solicitud.";
				model.MensagemId = EnumMensagemPlaca.SemMensagemTratadaNoJs;
			}

			else if (composicaoAguardandoSAP)
			{
				model.Mensagem = "Patente en espera de actualización SAP de otra solicitud.";
				model.MensagemId = EnumMensagemPlaca.SemMensagemTratadaNoJs;
			}
			else if (tipoComposicao.HasValue)
			{
				 ValidaTipoComposicao(numero, tipoComposicao, model);
			}

			json.Data = model;
			return json;
		}

		private void ValidaTipoComposicao(int numeroPlaca, int? tipoComposicao, ModelPlaca model)
		{
			switch (tipoComposicao)
			{
				case (int)EnumTipoComposicao.SemirremolqueChico:
				case (int)EnumTipoComposicao.SemirremolqueGrande:
				case (int)EnumTipoComposicao.Escalado:
					{
						if (numeroPlaca == 1 && model.Placa.IDTipoVeiculo != (int)EnumTipoVeiculo.Tractor)
							DefineMensagemErroVeiculoIncompativel(model);
						else if (numeroPlaca == 2 && model.Placa.IDTipoVeiculo != (int)EnumTipoVeiculo.Semiremolque)
							DefineMensagemErroVeiculoIncompativel(model);

						break;
					}
				case (int)EnumTipoComposicao.BitrenChico:
				case (int)EnumTipoComposicao.BitrenGrande:
					{
						if (numeroPlaca == 1 && model.Placa.IDTipoVeiculo != (int)EnumTipoVeiculo.Tractor)
							DefineMensagemErroVeiculoIncompativel(model);
						else if (numeroPlaca == 2 && model.Placa.IDTipoVeiculo != (int)EnumTipoVeiculo.Semiremolque)
							DefineMensagemErroVeiculoIncompativel(model);
						else if (numeroPlaca == 3 && model.Placa.IDTipoVeiculo != (int)EnumTipoVeiculo.Semiremolque)
							DefineMensagemErroVeiculoIncompativel(model);
						break;
					}
				default:
					break;
			}
		}

		private void DefineMensagemErroVeiculoIncompativel(ModelPlaca model)
		{
			model.Mensagem = "El tipo de vehículo no coincide con la posición de la placa en la composición!";
			model.MensagemId = EnumMensagemPlaca.SemMensagemTratadaNoJs;
		}

		public override PartialViewResult VisualizarCapacidade(int idComposicao)
		{
			VisualizarCapacidadeSetaView model = new VisualizarCapacidadeSetaView();
			Composicao comp = new ComposicaoBusiness(EnumPais.Argentina).Selecionar(x => x.ID == idComposicao && x.IdPais == (int)EnumPais.Argentina);
			model.placasCapacidadeSeta = new PlacaBusiness(EnumPais.Argentina).ListarPorComposicaoCapacidade(comp);
			model.placasCompartimentos = new PlacaBusiness(EnumPais.Argentina).ListarPorComposicao(comp);

			VerificarSeTemInativo(model.placasCompartimentos);

			model.placasCompartimentos.ForEach(w => { w.somenteVisualizacao = true; w.Setas.ForEach(x => x.SomenteVisualizacao = true); });

			return PartialView("~/Views/ComposicaoArgentina/Partial/_VisualizarCapacidadeSeta.cshtml", model);
		}

        public override string ExcluirComposicao(int Id, bool somenteComposicao)
		{
			try
			{
				return new ComposicaoBusiness(EnumPais.Argentina).ExcluirComposicao(Id, somenteComposicao);
			}
			catch (Exception ex)
			{
				return ex.Message;
			}
		}

		public PartialViewResult AlterarSeta(int idComposicao)
		{
			var comp = new ComposicaoBusiness(EnumPais.Argentina).Selecionar(idComposicao);
			var model = new PlacaBusiness(EnumPais.Argentina).ListarPorComposicao(comp);

			if (model != null && model.Any())
				model = model.Where(p => p.MultiSeta).ToList();



			return PartialView("~/Views/ComposicaoArgentina/Partial/_AlterarSeta.cshtml", model);
		}

		public JsonResult AlterarSetaSalvar(Placa placa, int idComposicao)
		{
			return AlterarSetaSalvarBase(placa, idComposicao, EnumPais.Argentina);
		}

		public PartialViewResult InativarCompartimento(int idComposicao)
		{
			var comp = new ComposicaoBusiness(EnumPais.Argentina).Selecionar(idComposicao);
			var model = new PlacaBusiness(EnumPais.Argentina).ListarPorComposicao(comp);
			VerificarSeTemInativo(model);
			return PartialView("_InativarCompartimento", model);
		}

		public JsonResult InativarCompartimentoSalvar(Placa placa, int idComposicao)
		{
			PlacaSetaBusiness bll = new PlacaSetaBusiness();
			var json = new JsonResult();

			foreach (var item in placa.Setas)
			{
				var seta = bll.Selecionar(item.ID);
				seta.Compartimento1IsInativo = item.Compartimento1IsInativo;
				seta.Compartimento2IsInativo = item.Compartimento2IsInativo;
				seta.Compartimento3IsInativo = item.Compartimento3IsInativo;
				seta.Compartimento4IsInativo = item.Compartimento4IsInativo;
				seta.Compartimento5IsInativo = item.Compartimento5IsInativo;
				seta.Compartimento6IsInativo = item.Compartimento6IsInativo;
				seta.Compartimento7IsInativo = item.Compartimento7IsInativo;
				seta.Compartimento8IsInativo = item.Compartimento8IsInativo;
				seta.Compartimento9IsInativo = item.Compartimento9IsInativo;
				seta.Compartimento10IsInativo = item.Compartimento10IsInativo;
				bll.Atualizar(seta);
			}

			var comp = new ComposicaoBusiness(EnumPais.Argentina).Selecionar(idComposicao);

			if (!string.IsNullOrEmpty(comp.Mensagem))
			{
				json.Data = comp.Mensagem;
			}
			else
			{
				json.Data = "Compartimento alterado com sucesso!";
			}
			return json;
		}

		public JsonResult VerificarClientePermissao(int id)
		{
			return VerificarClientePermissaoBase(id);
		}

		[HttpPost]
		[AjaxOnlyAttribute]
		public string ExcluirPlaca(int idComposicao, int idPlaca, int[] placaClientes)
		{
			return ExcluirPlacaBase(idComposicao, idPlaca, placaClientes);
		}

		#endregion

		#region Private methods

		private void Listar(ModelComposicao Model)
		{
			this.ListarPaginador(Model);
			Model.Resultado = base.ProcessarResultado(!Model.ListaComposicao.IsNullOrEmpty(), Model.Operacao);
		}

		#endregion
	}
}
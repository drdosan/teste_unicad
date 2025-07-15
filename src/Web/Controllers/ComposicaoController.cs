using Raizen.Framework.Models;
using Raizen.Framework.UserSystem.Client;
using Raizen.Framework.Utils.Extensions;
using Raizen.Framework.Web.MVC.Filters;
using Raizen.Framework.Web.MVC.Models;
using Raizen.Framework.Web.MVC.Utils;
using Raizen.UniCad.BLL;
using Raizen.UniCad.Extensions;
using Raizen.UniCad.Model;
using Raizen.UniCad.Model.Base;
using Raizen.UniCad.Model.Filtro;
using Raizen.UniCad.Model.View;
using Raizen.UniCad.Web.Models;
using Raizen.UniCad.Web.Util;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Helpers;
using System.Web.Mvc;

namespace Raizen.UniCad.Web.Controllers
{
	public class ComposicaoController : ComposicaoBaseController
	{
		public ComposicaoController() : base(BaseControllerOptions.NaoValidarAcesso)
		{
		}

		#region Constantes

		private readonly ComposicaoBusiness _composicaoBLL = new ComposicaoBusiness();
		private readonly ComposicaoPesquisaBusiness ComposicaoPesquisaBLL = new ComposicaoPesquisaBusiness();
		private readonly PlacaBusiness placaBll = new PlacaBusiness();
		private readonly PlacaClienteBusiness placaClienteBll = new PlacaClienteBusiness();
		private const string NomeFiltro = "Filtro_Composicao";
		private const string NomePaginador = "Paginador_Composicao";
		private const string TotalRegistros = "totalRegistros_Composicao";

		#endregion Constantes

		#region Index

		[HttpGet]
		public ActionResult Index()
		{
			return this.CarregarDefault();
		}

		#endregion Index

		#region Novo

		[HttpGet]
		[AjaxOnlyAttribute]
		public ActionResult Novo()
		{
			ModelComposicao Model = new ModelComposicao();
			Model.Composicao = new Composicao();
			Model.Resultado = new ResultadoOperacao();
			return PartialView("_Edicao", Model);
		}

		#endregion Novo

		#region Editar

		[HttpGet]
		public ActionResult Editar(string Id)
		{
			ModelComposicao Model = CarregarDadosComposicao(Id, false, EnumPais.Brasil);

			return PartialView("Edicao", Model);
		}

		[HttpGet]
		public override ActionResult Aprovar(string Id)
		{
			ModelComposicao Model = CarregarDadosComposicao(Id, true, EnumPais.Brasil);

			return PartialView("Edicao", Model);
		}

		public override ModelComposicao CarregarDadosComposicao(string Id, bool aprovar, EnumPais pais)
		{
			ModelComposicao model = new ModelComposicao { Composicao = new Composicao() };
			if (!aprovar)
				model.Composicao.LoginUsuario = UsuarioCsOnline?.Login ?? UserSession.GetCurrentInfoUserSystem().InformacoesUsuario.Login;
			model.Aprovar = aprovar;
			if (!string.IsNullOrEmpty(Id))
			{
				model.Operacao = OperacoesCRUD.Editando;
				model.ChavePrimaria = Id;
				model.Resultado = new ResultadoOperacao();

				PlacaBusiness placaBll = new PlacaBusiness();
				model.Composicao = this._composicaoBLL.Selecionar(int.Parse(model.ChavePrimaria));
				if (!aprovar)
					model.Composicao.isUtilizaPlacaChave = false;
				model.Composicao.ufCRLV = _composicaoBLL.SelecionarUfCRLV(model.Composicao);
				model.isArrendamento = String.IsNullOrEmpty(model.Composicao.CPFCNPJArrendamento) ? "FALSE" : "TRUE";
				if (!aprovar && model.Composicao.IDStatus != (int)EnumStatusComposicao.Reprovado)
				{
					model.Composicao.IDComposicao = model.Composicao.ID;
					model.Composicao.ID = 0;
					model.ChavePrimaria = "0";
				}
				else
				{
					if (model.Composicao.IDComposicao.HasValue)
					{
						var composicaoAtual = _composicaoBLL.Selecionar(model.Composicao.IDComposicao.Value);
						if (composicaoAtual != null)
						{
							if (model.Composicao.IDPlaca1.HasValue && model.Composicao.IDPlaca1 > 0 && composicaoAtual.IDPlaca1.HasValue && composicaoAtual.IDPlaca1 > 0)
							{
								model.Composicao.isPlaca1Alterada = placaBll.isPlacaAlterada(model.Composicao.IDPlaca1.Value, composicaoAtual.IDPlaca1.Value);
								model.Composicao.PlacaOficial1 = composicaoAtual.IDPlaca1.Value;
							}
							if (model.Composicao.IDPlaca2.HasValue && model.Composicao.IDPlaca2 > 0 && composicaoAtual.IDPlaca2.HasValue && composicaoAtual.IDPlaca2 > 0)
							{
								model.Composicao.isPlaca2Alterada = placaBll.isPlacaAlterada(model.Composicao.IDPlaca2.Value, composicaoAtual.IDPlaca2.Value);
								model.Composicao.PlacaOficial2 = composicaoAtual.IDPlaca2.Value;
							}
							if (model.Composicao.IDPlaca3.HasValue && model.Composicao.IDPlaca3 > 0 && composicaoAtual.IDPlaca3.HasValue && composicaoAtual.IDPlaca3 > 0)
							{
								model.Composicao.isPlaca3Alterada = placaBll.isPlacaAlterada(model.Composicao.IDPlaca3.Value, composicaoAtual.IDPlaca3.Value);
								model.Composicao.PlacaOficial3 = composicaoAtual.IDPlaca3.Value;
							}
							if (model.Composicao.IDPlaca4.HasValue && model.Composicao.IDPlaca4 > 0 && composicaoAtual.IDPlaca4.HasValue && composicaoAtual.IDPlaca4 > 0)
							{
								model.Composicao.isPlaca4Alterada = placaBll.isPlacaAlterada(model.Composicao.IDPlaca4.Value, composicaoAtual.IDPlaca4.Value);
								model.Composicao.PlacaOficial4 = composicaoAtual.IDPlaca4.Value;
							}
						}
					}
				}

				if (model.Composicao.IDPlaca1.HasValue && model.Composicao.IDPlaca1 > 0)
				{
					model.Composicao.isPlaca1Pendente = placaBll.isPlacaPendente(model.Composicao.IDPlaca1.Value);
					model.Composicao.Placa1 = placaBll.Selecionar(model.Composicao.IDPlaca1.Value).PlacaVeiculo;
				}
				if (model.Composicao.IDPlaca2.HasValue && model.Composicao.IDPlaca2 > 0)
				{
					model.Composicao.isPlaca2Pendente = placaBll.isPlacaPendente(model.Composicao.IDPlaca2.Value);
					model.Composicao.Placa2 = placaBll.Selecionar(model.Composicao.IDPlaca2.Value).PlacaVeiculo;
				}
				if (model.Composicao.IDPlaca3.HasValue && model.Composicao.IDPlaca3 > 0)
				{
					model.Composicao.isPlaca3Pendente = placaBll.isPlacaPendente(model.Composicao.IDPlaca3.Value);
					model.Composicao.Placa3 = placaBll.Selecionar(model.Composicao.IDPlaca3.Value).PlacaVeiculo;
				}
				if (model.Composicao.IDPlaca4.HasValue && model.Composicao.IDPlaca4 > 0)
				{
					model.Composicao.isPlaca4Pendente = placaBll.isPlacaPendente(model.Composicao.IDPlaca4.Value);
					model.Composicao.Placa4 = placaBll.Selecionar(model.Composicao.IDPlaca4.Value).PlacaVeiculo;
				}
				model.Composicao.Documentos = new List<PlacaDocumentoView>();

				var retorno = ObterPlacas(model.Composicao.IDPlaca1, model.Composicao.IDPlaca2, model.Composicao.IDPlaca3, model.Composicao.IDPlaca4);
				model.Composicao.Documentos = retorno;
				var eixos = ObterDadosPlaca(model.Composicao.IDPlaca1, model.Composicao.IDPlaca2, model.Composicao.IDPlaca3, model.Composicao.IDPlaca4, model.Composicao.IDEmpresa);
				model.Composicao.EixosComposicao = eixos.NumeroEixos;
				model.Composicao.EixosDistanciados = eixos.NumeroEixosDistanciados;
				model.Composicao.EixosPneusDuplos = eixos.NumeroEixosPneusDuplos;
				model.Composicao.TaraComposicao = eixos.Tara;
				model.Composicao.ufCRLV = (int)eixos.IDEstado;
			}

			var usuario = UsuarioLogado;
			if (usuario != null)
			{
				if (String.IsNullOrEmpty(model.Composicao.Operacao))
				{
					model.Composicao.Operacao = usuario.Operacao;
				}

				if (model.Operacao != OperacoesCRUD.Editando)
				{
					if (usuario.IDEmpresa != 3)
					{
						model.Composicao.IDEmpresa = usuario.IDEmpresa;
					}
				}


				model.UsuarioPerfil = usuario.Perfil.ToLowerInvariant();
			}

			return model;
		}

		#endregion Editar

		#region CarregarDefault

		public ActionResult CarregarDefault()
		{
			return CarregarDefaultBase(EnumPais.Brasil);
		}

		#endregion CarregarDefault

		#region ComposicaoLayout

		private ModelComposicao ComposicaoLayout(ModelComposicao model)
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

		#endregion ComposicaoLayout

		#region Listar

		public override void AtualizarQtdeRegPaginador(ModelComposicao Model)
		{
			Model.Filtro = base.RetornaDados<ComposicaoFiltro>(NomeFiltro);
			base.ArmazenarDados<ComposicaoFiltro>(Model.Filtro, NomeFiltro);

			int totalRegistros = this.ComposicaoPesquisaBLL.ListarComposicaoCount(Model.Filtro);
			base.ArmazenarDados<int>(totalRegistros, TotalRegistros);
		}

		private void Listar(ModelComposicao Model)
		{
			this.ListarPaginador(Model);
			Model.Resultado = base.ProcessarResultado(!Model.ListaComposicao.IsNullOrEmpty(), Model.Operacao);
		}

		#endregion Listar

		#region Pesquisar
		private static void GetClienteTransportadora(ModelComposicao model, Usuario usuario)
		{
			if (usuario.Perfil == "Transportadora")
				model.Filtro.IDUsuarioTransportadora = usuario.ID;
			else if (usuario.Perfil == "Cliente EAB")
				model.Filtro.IDUsuarioCliente = usuario.ID;
		}

		[HttpGet]
		[AjaxOnlyAttribute]
		public override ActionResult Pesquisar(ModelComposicao Model)
		{
			Model.IdPais = (int)EnumPais.Brasil;

			CarregarPermissaoUsuario(Model, true);
			//Model.Filtro.IDUsuarioTransportadora = GetIdUsuarioTransportadora();

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
				Model.Filtro.UsuarioExterno = usuarioLogado?.Externo ?? true;
                Model.UsuarioPerfil = usuarioLogado?.Perfil;
				bool usuarioQuality;
				usuarioQuality = usuarioLogado?.Perfil.Contains("Quality") ?? false;
				Model.Filtro.UsuarioExterno = usuarioQuality ? false : Model.Filtro.UsuarioExterno;
			}

			this.PrepararPaginadorOperacoes(Model);
			this.Listar(Model);
			return PartialView("_Pesquisa", Model);
		}

		#endregion Pesquisar

		#region Paginacao

		private void PrepararPaginadorOperacoes(ModelComposicao Model)
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

		private void PrepararPaginador(ref ModelComposicao Model)
		{
			PaginadorModel clone = Model.PaginadorDados.Clone() as PaginadorModel;
			Paginador.CalcularPaginas(clone, TipoPaginador.Modelo_Linq);
			Model.PaginadorDados = clone;
		}

		private void ZerarFiltro(ModelComposicao Model)
		{
			Model.Filtro = new ComposicaoFiltro();
			base.ArmazenarDados<ComposicaoFiltro>(Model.Filtro, NomeFiltro);
		}

		#endregion Paginacao

		#region aprovacao


		#endregion

		#region Salvar

		public override ActionResult Salvar(ModelComposicao Model, int status, bool comRessalvas, bool forcar, bool comAlteracoes = false)
		{
            bool editandoReprovada = false;
            Model.comRessalvas = comRessalvas;
			Model.TipoValidacao = TiposValidacao.ValidacaoTotal;
			Model.Composicao.IgnorarLeci = forcar;

			if (Model.Composicao.Operacao == "FOB")
				this.EqualizarPlacasClientes(Model);

			if (base.ValidarModel(Model, this.ModelState))
			{
				Model.Composicao.IsUsuarioCsOnline = UsuarioCsOnline != null;

                if (Model.ChavePrimaria != null)
                {
                    var composicaoAnterior = this._composicaoBLL.Selecionar(int.Parse(Model.ChavePrimaria));

                    if (composicaoAnterior != null && composicaoAnterior.IDStatus == (int)EnumStatusComposicao.Reprovado)
                    {
                        editandoReprovada = true;
                    }

                }


                if (Model.Operacao == OperacoesCRUD.Insert || editandoReprovada)
				{
					Model.Composicao.IDStatus = status;
					Model.Composicao.LoginUsuario = Model.Composicao.LoginUsuarioCorrente = UsuarioCsOnline != null && UsuarioCsOnline.Login != null ? UsuarioCsOnline.Login : UserSession.GetCurrentInfoUserSystem().InformacoesUsuario.Login;
					Model.Composicao.EmailSolicitante = UsuarioCsOnline != null && UsuarioCsOnline.Email != null ? UsuarioCsOnline.Email.Trim() : UserSession.GetCurrentInfoUserSystem().InformacoesUsuario.Email.Trim();
					Model.Composicao.UsuarioAlterouStatus = UsuarioCsOnline?.Login ?? UserSession.GetCurrentInfoUserSystem().InformacoesUsuario.Login;
					Model.Resultado = base.ProcessarResultado(this._composicaoBLL.AdicionarComposicao(Model.Composicao, true, comAlteracoes), OperacoesCRUD.Insert);
					if (!string.IsNullOrEmpty(Model.Composicao.Mensagem))
					{
						Model.Resultado.Mensagem = Model.Composicao.Mensagem;
					}
					this.ZerarFiltro(Model);
					this.AtualizarQtdeRegPaginador(Model);
					Model.Operacao = OperacoesCRUD.Update;
				}
                else
				if (Model.Operacao == OperacoesCRUD.Editando)
				{
					Model.Composicao.ID = int.Parse(Model.ChavePrimaria);
					Model.Composicao.LoginUsuarioCorrente = UsuarioCsOnline != null && UsuarioCsOnline.Login != null ? UsuarioCsOnline.Login : UserSession.GetCurrentInfoUserSystem().InformacoesUsuario.Login;
					Model.Composicao.EmailSolicitante = UsuarioCsOnline != null && UsuarioCsOnline.Email != null ? UsuarioCsOnline.Email.Trim() : UserSession.GetCurrentInfoUserSystem().InformacoesUsuario.Email.Trim();

					Composicao composicaoAntiga = _composicaoBLL.Selecionar(Model.Composicao.ID);
					if (composicaoAntiga != null)
					{
						string loginUsuarioLogado = UsuarioCsOnline?.Login ?? UserSession.GetCurrentInfoUserSystem().InformacoesUsuario.Login;
						Model.Composicao.UsuarioAlterouStatus = composicaoAntiga.IDStatus != status ? loginUsuarioLogado : composicaoAntiga.UsuarioAlterouStatus;
					}

					Model.Resultado = base.ProcessarResultado(this._composicaoBLL.AtualizarComposicao(Model.Composicao, comRessalvas, idStatus: status), OperacoesCRUD.Update);
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

			return PartialView("_Edicao", Model);
		}

		public void EqualizarPlacasClientes(ModelComposicao Model)
		{
			int idPlacaPrincipal = _composicaoBLL.SelecionaPlacaPrincipalFOB(Model.Composicao).ID;
			var listIdClientes = this.placaBll.ListarClientesPorPlaca(idPlacaPrincipal);

			if (Model.Composicao.IDPlaca1.HasValue && Model.Composicao.IDPlaca1.Value != idPlacaPrincipal)
				AdicionarClientesPlaca(Model.Composicao.IDPlaca1.Value, listIdClientes);
			if (Model.Composicao.IDPlaca2.HasValue && Model.Composicao.IDPlaca2.Value != idPlacaPrincipal)
				AdicionarClientesPlaca(Model.Composicao.IDPlaca2.Value, listIdClientes);
			if (Model.Composicao.IDPlaca3.HasValue && Model.Composicao.IDPlaca3.Value != idPlacaPrincipal)
				AdicionarClientesPlaca(Model.Composicao.IDPlaca3.Value, listIdClientes);
			if (Model.Composicao.IDPlaca4.HasValue && Model.Composicao.IDPlaca4.Value != idPlacaPrincipal)
				AdicionarClientesPlaca(Model.Composicao.IDPlaca4.Value, listIdClientes);
		}

		private void AdicionarClientesPlaca(int idPlaca, List<int> listIdClientesPlacaChave)
		{
			var listIdClientes = this.placaBll.ListarClientesPorPlaca(idPlaca);
			bool adicionar = false;

			if (listIdClientesPlacaChave != null)
			{
				foreach (var idClientePlacaChave in listIdClientesPlacaChave)
				{
					if (listIdClientes == null)
					{
						adicionar = true;
					}
					else if (!listIdClientes.Contains(idClientePlacaChave))
						adicionar = true;

					if (adicionar)
					{
						PlacaCliente placaCliente = new PlacaCliente();
						placaCliente.IDPlaca = idPlaca;
						placaCliente.IDCliente = idClientePlacaChave;
						placaClienteBll.Adicionar(placaCliente);
					}
				}
			}
		}
		
		#endregion Salvar

		#region ExcluirRegistro

		[HttpGet]
		[AjaxOnlyAttribute]
		public string ExcluirRegistro(int Id)
		{
			ModelComposicao Model = new ModelComposicao();
			try
			{
				return this._composicaoBLL.ExcluirComposicao(Id, true);
			}
			catch (Exception ex)
			{
				return ex.Message;
			}
		}

		#endregion ExcluirRegistro

		#region ExcluirComposicao

		[HttpPost]
		[AjaxOnlyAttribute]
		public override string ExcluirComposicao(int Id, bool somenteComposicao)
		{
			ModelComposicao Model = new ModelComposicao();
			try
			{
				return this._composicaoBLL.ExcluirComposicao(Id, somenteComposicao);
			}
			catch (Exception ex)
			{
				return ex.Message;
			}
		}

		#endregion ExcluirComposicao

		#region WebMethods
		public override JsonResult VerificarAlteracoes(ModelComposicao model)
		{
			bool isClientesAlterados = false;
			bool isOutrosDadosAlterados = false;

			var composicaoAtual = _composicaoBLL.Selecionar(model.Composicao.IDComposicao.Value);
			if (composicaoAtual != null)
			{
				if (model.Composicao.RazaoSocialArrendamento != composicaoAtual.RazaoSocialArrendamento ||
					model.Composicao.CPFCNPJArrendamento != composicaoAtual.CPFCNPJArrendamento)
					isOutrosDadosAlterados = true;

				if (model.Composicao.IDPlaca1.HasValue && model.Composicao.IDPlaca1 > 0 && composicaoAtual.IDPlaca1.HasValue && composicaoAtual.IDPlaca1 > 0)
				{
					var alteracoesPlaca1 = placaBll.isPlacaClientesAlterados(model.Composicao.IDPlaca1.Value, composicaoAtual.IDPlaca1.Value, true, true);
					isClientesAlterados = isClientesAlterados || alteracoesPlaca1.IsClientesAlterados;
					isOutrosDadosAlterados = isOutrosDadosAlterados || alteracoesPlaca1.IsOutrosDadosAlterados;
				}
				if (model.Composicao.IDPlaca2.HasValue && model.Composicao.IDPlaca2 > 0 && composicaoAtual.IDPlaca2.HasValue && composicaoAtual.IDPlaca2 > 0)
				{
					var alteracoesPlaca2 = placaBll.isPlacaClientesAlterados(model.Composicao.IDPlaca2.Value, composicaoAtual.IDPlaca2.Value, true, true);
					isClientesAlterados = isClientesAlterados || alteracoesPlaca2.IsClientesAlterados;
					isOutrosDadosAlterados = isOutrosDadosAlterados || alteracoesPlaca2.IsOutrosDadosAlterados;
				}
				if (model.Composicao.IDPlaca3.HasValue && model.Composicao.IDPlaca3 > 0 && composicaoAtual.IDPlaca3.HasValue && composicaoAtual.IDPlaca3 > 0)
				{
					var alteracoesPlaca3 = placaBll.isPlacaClientesAlterados(model.Composicao.IDPlaca3.Value, composicaoAtual.IDPlaca3.Value, true, true);
					isClientesAlterados = isClientesAlterados || alteracoesPlaca3.IsClientesAlterados;
					isOutrosDadosAlterados = isOutrosDadosAlterados || alteracoesPlaca3.IsOutrosDadosAlterados;
				}
				if (model.Composicao.IDPlaca4.HasValue && model.Composicao.IDPlaca4 > 0 && composicaoAtual.IDPlaca4.HasValue && composicaoAtual.IDPlaca4 > 0)
				{
					var alteracoesPlaca4 = placaBll.isPlacaClientesAlterados(model.Composicao.IDPlaca4.Value, composicaoAtual.IDPlaca4.Value, true, true);
					isClientesAlterados = isClientesAlterados || alteracoesPlaca4.IsClientesAlterados;
					isOutrosDadosAlterados = isOutrosDadosAlterados || alteracoesPlaca4.IsOutrosDadosAlterados;
				}
			}

			return Json(new
			{
				Data = new
				{
					isClientesAlterados,
					isOutrosDadosAlterados
				}
			}, JsonRequestBehavior.AllowGet);
		}


		public override PartialViewResult ListarDocumentos(int? placa1, int? placa2, int? placa3, int? placa4)
		{
			var model = ObterPlacas(placa1, placa2, placa3, placa4);

			return PartialView("_Documentos", model);
		}

		public PartialViewResult ObterEixos(int? tipoComposicao)
		{
			List<ComposicaoEixo> model = CarregarEixos(tipoComposicao);
			return PartialView("_Eixos", model);
		}

		private static List<ComposicaoEixo> CarregarEixos(int? tipoComposicao)
		{
			var composicaoBll = new ComposicaoEixoBusiness();
			var model = new List<ComposicaoEixo>();
			if (tipoComposicao.HasValue)
			{
				model = composicaoBll.Listar(p => p.IDTipoComposicao == tipoComposicao.Value && p.Ativo);
			}

			return model;
		}

		public override JsonResult ObterDados(string placa1, string placa2, string placa3, string placa4, int IDEmpresa)
		{
			int? p1 = null;
			int? p2 = null;
			int? p3 = null;
			int? p4 = null;
			if (!String.IsNullOrEmpty(placa1))
			{
				p1 = Convert.ToInt32(placa1);
			}
			if (!String.IsNullOrEmpty(placa2))
			{
				p2 = Convert.ToInt32(placa2);
			}
			if (!String.IsNullOrEmpty(placa3))
			{
				p3 = Convert.ToInt32(placa3);
			}
			if (!String.IsNullOrEmpty(placa4))
			{
				p4 = Convert.ToInt32(placa4);
			}
			var retorno = ObterDadosPlaca(p1, p2, p3, p4, IDEmpresa);
			var json = new JsonResult { Data = retorno };
			return json;
		}

		public PartialViewResult InativarCompartimento(int idComposicao)
		{
			var comp = new ComposicaoBusiness().Selecionar(idComposicao);
			var model = new PlacaBusiness().ListarPorComposicao(comp);
			VerificarSeTemInativo(model);
			return PartialView("_InativarCompartimento", model);
		}

		public PartialViewResult AlterarSeta(int idComposicao)
		{
			var comp = new ComposicaoBusiness().Selecionar(idComposicao);
			var model = new PlacaBusiness().ListarPorComposicao(comp);

			if (model != null && model.Any())
				model = model.Where(p => p.MultiSeta).ToList();



			return PartialView("_AlterarSeta", model);
		}

		public PartialViewResult Bloquear(int idComposicao)
		{
			ModelPlaca model = new ModelPlaca();
			var comp = new ComposicaoBusiness().Selecionar(idComposicao);
			model.ListaPlaca = new PlacaBusiness().ListarPorComposicao(comp);
			model.IDComposicao = idComposicao;
			model.ListaHistorico = new HistorioBloqueioComposicaoBusiness().Listar(p => p.IDComposicao == idComposicao);
			return PartialView("_Bloquear", model);
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
			var msg = new HistorioBloqueioComposicaoBusiness().AdicionarBloqueio(bloqueio, EnumPais.Brasil);

			var json = new JsonResult();
			json.Data = msg;
			return json;
		}

		public PartialViewResult Checklist(int idComposicao)
		{
			ModelPlaca model = new ModelPlaca();
			var comp = new ComposicaoBusiness().Selecionar(idComposicao);
			model.ListaPlaca = new PlacaBusiness().ListarPorComposicao(comp);
			model.IDComposicao = idComposicao;
			model.IDEmpresa = comp.IDEmpresa;
			model.ListaHistoricoCheck = new ChecklistComposicaoBusiness().Listar(p => p.IDComposicao == idComposicao);
			return PartialView("_Checklist", model);
		}

		public JsonResult ValidarAlteracoes(ModelComposicao Model)
		{

			return new JsonResult { Data = true };
		}

		public JsonResult SalvarChecklist(int idComposicao, string justificativa, DateTime? data, string anexo, bool aprovado, bool isReplicarEab)
		{
			var checklist = new ChecklistComposicao();
			if (data.HasValue)
				checklist.Data = data.Value;
			else
				checklist.Data = DateTime.Now;
			checklist.DataCadastro = DateTime.Now;
			checklist.IDComposicao = idComposicao;
			checklist.Justificativa = justificativa;
			checklist.Aprovado = aprovado;
			checklist.Anexo = anexo;
			checklist.Usuario = UserSession.GetCurrentInfoUserSystem().InformacoesUsuario.Nome;
			checklist.CodigoUsuario = UserSession.GetCurrentInfoUserSystem().InformacoesUsuario.Login;
			checklist.isReplicarEab = isReplicarEab;
			var msg = new ChecklistComposicaoBusiness().AdicionarChecklist(checklist);


			var json = new JsonResult();
			json.Data = msg;
			return json;
		}

		public override PartialViewResult VisualizarCapacidade(int idComposicao)
		{
			var model = new VisualizarCapacidadeSetaView();
			var comp = new ComposicaoBusiness().Selecionar(idComposicao);
			model.placasCapacidadeSeta = new PlacaBusiness().ListarPorComposicaoCapacidade(comp);
			model.placasCompartimentos = new PlacaBusiness().ListarPorComposicao(comp);
			VerificarSeTemInativo(model.placasCompartimentos);
			model.placasCompartimentos.ForEach(w => { w.somenteVisualizacao = true; w.Setas.ForEach(x => x.SomenteVisualizacao = true); });
			return PartialView("_VisualizarCapacidadeSeta", model);
		}

		public JsonResult AlterarSetaSalvar(Placa placa, int idComposicao)
		{
			return AlterarSetaSalvarBase(placa, idComposicao, EnumPais.Brasil);
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

			var comp = new ComposicaoBusiness().Selecionar(idComposicao);
			var msg = new ComposicaoBusiness().AtualizarComposicao(comp, false, enviaEmail: false, idStatus: comp.IDStatus);


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

		public override JsonResult VerificarPlaca(string placa, int numero, int? tipoComposicao, string composicao, string operacao, string idEmpresa)
		{
			ModelPlaca model = new ModelPlaca();
			var json = new JsonResult();
			Regex regex = new Regex(@"^\w+$");
			int empresa = string.IsNullOrEmpty(idEmpresa) ? 0 : Convert.ToInt32(idEmpresa);
			model.Placa = new PlacaBusiness().ListarPlacaEmAprovacao(placa, operacao, empresa);
			if (model.Placa == null)
			{
				model.Placa = new Placa();
			}
			else
			{
				model.Mensagem = "Este veículo já está cadastrado! Caso queria conferir, clicar no ícone 'Lápis'";
				model.MensagemId = EnumMensagemPlaca.VeiculoJaCadastrado;
				//if (placa != null && !regex.Match(placa).Success)
				//{
				//    model.Mensagem = "O formato da Placa está inválido.";
				//}
				//else
				//{

				var composicaoAprovado = placaBll.ListarPorStatus(model.Placa.ID, composicao, null, EnumStatusComposicao.EmAprovacao, operacao);

				bool composicaoAguardandoSAP = false;
				if (model.Placa.IDTipoVeiculo == (int)EnumTipoVeiculo.Truck
				|| (empresa == (int)EnumEmpresa.EAB && numero == 1)
				|| (empresa == (int)EnumEmpresa.Combustiveis && numero == 2)
				|| (empresa == (int)EnumEmpresa.Ambos && (numero == 1 || numero == 2)))
					composicaoAguardandoSAP = placaBll.ListarPorStatus(model.Placa.ID, composicao, null, EnumStatusComposicao.AguardandoAtualizacaoSAP, operacao);

				if (composicaoAprovado)
				{
					model.Mensagem = "Placa aguardando aprovação de outra solicitação.";
					model.MensagemId = EnumMensagemPlaca.SemMensagemTratadaNoJs;
				}

				else if (composicaoAguardandoSAP)
				{
					model.Mensagem = "Placa Aguardando Atualização SAP de outra solicitação.";
					model.MensagemId = EnumMensagemPlaca.SemMensagemTratadaNoJs;
				}
				else
				{
					if (tipoComposicao.HasValue)
					{
						switch (tipoComposicao)
						{
							case (int)EnumTipoComposicao.Truck:
								{
									if (numero == 1)
									{
										if (model.Placa.IDTipoVeiculo != (int)EnumTipoVeiculo.Truck)
										{
											model.Mensagem = "A placa de uma composição do tipo Truck, deve ser um Truck";
											model.MensagemId = EnumMensagemPlaca.SemMensagemTratadaNoJs;
										}
									}
									break;
								}
							case (int)EnumTipoComposicao.Carreta:
							case (int)EnumTipoComposicao.Bitrem:
							case (int)EnumTipoComposicao.BitremDolly:
								{
									if (numero == 1)
									{
										if (model.Placa.IDTipoVeiculo != (int)EnumTipoVeiculo.Cavalo)
										{
											model.Mensagem = "Tipo de veículo não corresponde à posição da placa na composição!";
											model.MensagemId = EnumMensagemPlaca.SemMensagemTratadaNoJs;
										}
									}
									else if (numero == 2)
									{
										if (model.Placa.IDTipoVeiculo != (int)EnumTipoVeiculo.Carreta)
										{
											model.Mensagem = "Tipo de veículo não corresponde à posição da placa na composição!";
											model.MensagemId = EnumMensagemPlaca.SemMensagemTratadaNoJs;
										}
									}
									else if (numero == 3 && tipoComposicao == (int)EnumTipoComposicao.BitremDolly)
									{
										if (model.Placa.IDTipoVeiculo != (int)EnumTipoVeiculo.Dolly)
										{
											model.Mensagem = "Tipo de veículo não corresponde à posição da placa na composição!";
											model.MensagemId = EnumMensagemPlaca.SemMensagemTratadaNoJs;
										}
									}
									else if (numero == 3)
									{
										if (model.Placa.IDTipoVeiculo != (int)EnumTipoVeiculo.Carreta)
										{
											model.Mensagem = "Tipo de veículo não corresponde à posição da placa na composição!";
											model.MensagemId = EnumMensagemPlaca.SemMensagemTratadaNoJs;
										}
									}
									else if (numero == 4)
									{
										if (model.Placa.IDTipoVeiculo != (int)EnumTipoVeiculo.Carreta)
										{
											model.Mensagem = "Tipo de veículo não corresponde à posição da placa na composição!";
											model.MensagemId = EnumMensagemPlaca.SemMensagemTratadaNoJs;
										}
									}
									break;
								}
							default:
								break;
						}
					}
				}
				//}
			}
			json.Data = model;
			return json;
		}

		public override PlacaView ObterDadosPlaca(int? placa1, int? placa2, int? placa3, int? placa4, int IDEmpresa)
		{
			var retorno = new PlacaView();
			retorno.NumeroEixos = 0;
			retorno.NumeroEixosDistanciados = 0;
			retorno.NumeroEixosPneusDuplos = 0;
			retorno.IDCategoriaVeiculo = (int)EnumCategoriaVeiculo.Particular;

			var model = new List<Placa>();

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
				placa = placaBll.Selecionar(p => p.ID == placaVeiculo);
				if (placa != null)
				{
					if (placaId <= 2)
					{
						if (placaId == 1 || IDEmpresa == (int)EnumEmpresa.Combustiveis)
						{
							retorno.IDEstado = placa.PlacaBrasil.IDEstado;
							retorno.CPFCNPJ = placa.PlacaBrasil.CPFCNPJ;
							retorno.RazaoSocial = placa.RazaoSocial;
							if (placa.DataNascimento.HasValue)
								retorno.Datas = placa.DataNascimento.Value.ToShortDateString();
						}
					}
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

		[HttpGet]
		public FileResult Exportar(ComposicaoFiltro filtro)
		{
			return ExportarBase(filtro, EnumPais.Brasil);
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

		#endregion WebMethods
	}
}
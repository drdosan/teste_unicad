using Raizen.Framework.Models;
using Raizen.Framework.UserSystem.Client;
using Raizen.Framework.Web.MVC.Filters;
using Raizen.Framework.Web.MVC.Models;
using Raizen.Framework.Web.MVC.Utils;
using Raizen.UniCad.BLL;
using Raizen.UniCad.BLL.Util;
using Raizen.UniCad.Model;
using Raizen.UniCad.Model.Filtro;
using Raizen.UniCad.Model.View;
using Raizen.UniCad.Utils;
using Raizen.UniCad.Web.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace Raizen.UniCad.Web.Controllers
{
    public abstract class ComposicaoBaseController : BaseUniCadController
	{
		/// <summary>
		/// Nao validar acesso.
		/// </summary>
		public ComposicaoBaseController(BaseControllerOptions options) : base(options)
		{

		}

		/// <summary>
		/// Nao validar acesso.
		/// </summary>
		public ComposicaoBaseController() : base(BaseControllerOptions.NaoValidarAcesso)
		{
		}

		#region Constantes

		protected readonly ComposicaoPesquisaBusiness ComposicaoPesquisaBLL = new ComposicaoPesquisaBusiness();
		protected readonly ComposicaoBusiness _composicaoBLL = new ComposicaoBusiness();
		protected readonly PlacaBusiness _placaBll = new PlacaBusiness();
		protected readonly PlacaArgentinaBusiness placaArgentinaBll = new PlacaArgentinaBusiness();
		protected readonly PlacaClienteBusiness placaClienteBll = new PlacaClienteBusiness();
		protected const string NomeFiltro = "Filtro_Composicao";
		protected const string TotalRegistros = "totalRegistros_Composicao";
		protected const string NomePaginador = "Paginador_Composicao";

		#endregion

		#region Public methods

		public abstract ModelComposicao CarregarDadosComposicao(string Id, bool aprovar, EnumPais pais);

        public abstract PartialViewResult ListarDocumentos(int? placa1, int? placa2, int? placa3, int? placa4);

        public abstract JsonResult ObterDados(string placa1, string placa2, string placa3, string placa4, int IDEmpresa);

        public abstract PlacaView ObterDadosPlaca(int? placa1, int? placa2, int? placa3, int? placa4, int IDEmpresa);

        public abstract Placa CarregarPlaca(int? placaVeiculo, PlacaView retorno, int placaId, int IDEmpresa);

        public abstract ActionResult Salvar(ModelComposicao Model, int status, bool comRessalvas, bool forcar, bool comAlteracoes);

        public abstract ActionResult Aprovar(string Id);
		
        public abstract void AtualizarQtdeRegPaginador(ModelComposicao Model);

        public abstract ActionResult Pesquisar(ModelComposicao Model);

        public abstract JsonResult VerificarPlaca(string placa, int numero, int? tipoComposicao, string composicao, string operacao, string idEmpresa);

        public abstract PartialViewResult VisualizarCapacidade(int idComposicao);

        public abstract string ExcluirComposicao(int Id, bool somenteComposicao);

		public abstract JsonResult VerificarAlteracoes(ModelComposicao model);

		#endregion

		#region Protected methods

		protected static List<PlacaDocumentoView> ObterPlacas(int? placa1, int? placa2, int? placa3, int? placa4)
		{
			var placaDocBll = new PlacaDocumentoBusiness();
			var placaBll = new PlacaBusiness();
			var model = new List<PlacaDocumentoView>();
			if (placa1.HasValue)
			{
				model.AddRange(placaDocBll.ListarPlacaDocumentoPorPlaca(placa1.Value));
				model.Add(new PlacaDocumentoView() { Descricao = "ANEXO GERAL", Anexo = placaBll.Selecionar(placa1.Value).Anexo });
			}
			if (placa2.HasValue)
			{
				model.AddRange(placaDocBll.ListarPlacaDocumentoPorPlaca(placa2.Value));
				model.Add(new PlacaDocumentoView() { Descricao = "ANEXO GERAL", Anexo = placaBll.Selecionar(placa2.Value).Anexo });
			}
			if (placa3.HasValue)
			{
				model.AddRange(placaDocBll.ListarPlacaDocumentoPorPlaca(placa3.Value));
				model.Add(new PlacaDocumentoView() { Descricao = "ANEXO GERAL", Anexo = placaBll.Selecionar(placa3.Value).Anexo });
			}
			if (placa4.HasValue)
			{
				model.AddRange(placaDocBll.ListarPlacaDocumentoPorPlaca(placa4.Value));
				model.Add(new PlacaDocumentoView() { Descricao = "ANEXO GERAL", Anexo = placaBll.Selecionar(placa4.Value).Anexo });
			}
			return model;
		}

		protected ModelComposicao ComposicaoLayout(ModelComposicao model)
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

        #region CarregarPermissaoUsuario

        protected void CarregarPermissaoUsuario(ModelComposicao model)
		{
			CarregarPermissaoUsuario(model, false);
		}

        protected void CarregarPermissaoUsuario(ModelComposicao model, bool isPesquisa)
		{
			var usuario = UsuarioLogado;
			if (usuario != null)
			{
				//se a chamada não vir pela pesquisa o filtro será null
				if (usuario.IDEmpresa != (int)EnumEmpresa.Ambos && model.Filtro != null)
				{
					model.Filtro.IDEmpresaUsuario = usuario.IDEmpresa;
					model.Filtro.IDEmpresa = usuario.IDEmpresa;
					GetClienteTransportadora(model, usuario);
				}
				//se a chamada não vir pela pesquisa o filtro será null
				if (model.Filtro == null)
					model.Composicao.OperacaoUsuario = usuario.Operacao == "Ambos" ? null : usuario.Operacao;
				else if (isPesquisa)
					model.Filtro.OperacaoUsuario = usuario.Operacao == "Ambos" ? null : usuario.Operacao;
				else
					model.Filtro.Operacao = model.Filtro.OperacaoUsuario = usuario.Operacao == "Ambos" ? null : usuario.Operacao;
			}
		}
		
        #endregion

		protected void ListarPaginador(ModelComposicao Model)
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

        protected static void GetClienteTransportadora(ModelComposicao model, Usuario usuario)
		{
			if (usuario.Perfil.IsTransportadora())
				model.Filtro.IDUsuarioTransportadora = usuario.ID;
			else if (usuario.Perfil.IsClienteEAB())
				model.Filtro.IDUsuarioCliente = usuario.ID;
		}

		protected void ListarRenovandoConsulta(ModelComposicao Model)
		{
			base.ArmazenarDados<ComposicaoFiltro>(Model.Filtro, NomeFiltro);

			this.AtualizarQtdeRegPaginador(Model);
			int totalReg = base.RetornaDados<int>(TotalRegistros);

			Model.PaginadorDados = ModelUtils.IniciarPaginador(Model.PaginadorDados, totalReg);
			base.ArmazenarDados<int>(totalReg, TotalRegistros);
			base.ArmazenarDados<PaginadorModel>(Model.PaginadorDados, NomePaginador);

			if (totalReg > 0)
			{
				Model.ListaComposicao = new ComposicaoPesquisaBusiness((EnumPais)Model.IdPais).ListarComposicao(Model.Filtro, Model.PaginadorDados);
			}
		}

		protected void ListarPaginando(ModelComposicao Model)
		{
			Model.Filtro = base.RetornaDados<ComposicaoFiltro>(NomeFiltro);
			base.ArmazenarDados<ComposicaoFiltro>(Model.Filtro, NomeFiltro);

			int totalReg = base.RetornaDados<int>(TotalRegistros);
			Model.PaginadorDados.QtdeTotalRegistros = totalReg;

			base.ArmazenarDados<int>(totalReg, TotalRegistros);
			this.PrepararPaginador(ref Model);

			if (totalReg > 0)
			{
				Model.ListaComposicao = new ComposicaoPesquisaBusiness((EnumPais)Model.IdPais).ListarComposicao(Model.Filtro, Model.PaginadorDados);
			}
		}

		protected void PrepararPaginador(ref ModelComposicao Model)
		{
			PaginadorModel clone = Model.PaginadorDados.Clone() as PaginadorModel;
			Paginador.CalcularPaginas(clone, TipoPaginador.Modelo_Linq);
			Model.PaginadorDados = clone;
		}

        protected void EqualizarPlacasClientes(ModelComposicao Model)
		{
			int idPlacaPrincipal = _composicaoBLL.SelecionaPlacaPrincipalFOB(Model.Composicao).ID;
			var listIdClientes = this._placaBll.ListarClientesPorPlaca(idPlacaPrincipal);

			if (Model.Composicao.IDPlaca1.HasValue && Model.Composicao.IDPlaca1.Value != idPlacaPrincipal)
				AdicionarClientesPlaca(Model.Composicao.IDPlaca1.Value, listIdClientes);
			if (Model.Composicao.IDPlaca2.HasValue && Model.Composicao.IDPlaca2.Value != idPlacaPrincipal)
				AdicionarClientesPlaca(Model.Composicao.IDPlaca2.Value, listIdClientes);
			if (Model.Composicao.IDPlaca3.HasValue && Model.Composicao.IDPlaca3.Value != idPlacaPrincipal)
				AdicionarClientesPlaca(Model.Composicao.IDPlaca3.Value, listIdClientes);
			if (Model.Composicao.IDPlaca4.HasValue && Model.Composicao.IDPlaca4.Value != idPlacaPrincipal)
				AdicionarClientesPlaca(Model.Composicao.IDPlaca4.Value, listIdClientes);
		}

        protected void ZerarFiltro(ModelComposicao Model)
		{
			Model.Filtro = new ComposicaoFiltro();
			base.ArmazenarDados<ComposicaoFiltro>(Model.Filtro, NomeFiltro);
		}

        protected List<ComposicaoEixo> CarregarEixos(int? tipoComposicao)
		{
			ComposicaoEixoBusiness composicaoBll = new ComposicaoEixoBusiness();
			List<ComposicaoEixo> model = new List<ComposicaoEixo>();

			if (tipoComposicao.HasValue)
				model = composicaoBll.Listar(p => p.IDTipoComposicao == tipoComposicao.Value && p.Ativo);

			return model;
		}

		protected void VerificarSeTemInativo(List<Placa> model)
		{
			foreach (Placa item in model)
			{
				item.IsInativar = true;
				if (item.Setas.Any(w => (w.Compartimento1IsInativo.HasValue && w.Compartimento1IsInativo.Value)
									 || (w.Compartimento2IsInativo.HasValue && w.Compartimento2IsInativo.Value)
									 || (w.Compartimento3IsInativo.HasValue && w.Compartimento3IsInativo.Value)
									 || (w.Compartimento4IsInativo.HasValue && w.Compartimento4IsInativo.Value)
									 || (w.Compartimento5IsInativo.HasValue && w.Compartimento5IsInativo.Value)
									 || (w.Compartimento6IsInativo.HasValue && w.Compartimento6IsInativo.Value)
									 || (w.Compartimento7IsInativo.HasValue && w.Compartimento7IsInativo.Value)
									 || (w.Compartimento8IsInativo.HasValue && w.Compartimento8IsInativo.Value)
									 || (w.Compartimento9IsInativo.HasValue && w.Compartimento9IsInativo.Value)
									 || (w.Compartimento10IsInativo.HasValue && w.Compartimento10IsInativo.Value)))
				{
					item.IsInativo = true;
				}
			}
		}

		protected void PrepararPaginadorOperacoes(ModelComposicao Model)
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

		public JsonResult AlterarSetaSalvarBase(Placa placa, int idComposicao, EnumPais idPais)
		{
			PlacaSetaBusiness bll = new PlacaSetaBusiness();
			var json = new JsonResult();

			foreach (var item in placa.Setas)
			{
				var msgs = string.Empty;

				if (item.VolumeCompartimento1.HasValue && item.VolumeCompartimento1.Value == 0 && item.CompartimentoPrincipal1.HasValue && item.CompartimentoPrincipal1.Value)
				{
					msgs = Traducao.GetTextoPorLingua("Seta principal do compartimento 1 é obrigatória e deve conter volume.", "La flecha principal del compartimento 1 es obligatoria y debe contener volumen.", idPais);
				}
				else if (item.VolumeCompartimento2.HasValue && item.VolumeCompartimento2.Value == 0 && item.CompartimentoPrincipal2.HasValue && item.CompartimentoPrincipal2.Value)
				{
					msgs = Traducao.GetTextoPorLingua("Seta principal do compartimento 2 é obrigatória e deve conter volume.", "La flecha principal del compartimento 2 es obligatoria y debe contener volumen.", idPais);
				}
				else if (item.VolumeCompartimento3.HasValue && item.VolumeCompartimento3.Value == 0 && item.CompartimentoPrincipal3.HasValue && item.CompartimentoPrincipal3.Value)
				{
					msgs = Traducao.GetTextoPorLingua("Seta principal do compartimento 3 é obrigatória e deve conter volume.", "La flecha principal del compartimento 3 es obligatoria y debe contener volumen.", idPais);
				}
				else if (item.VolumeCompartimento4.HasValue && item.VolumeCompartimento4.Value == 0 && item.CompartimentoPrincipal4.HasValue && item.CompartimentoPrincipal4.Value)
				{
					msgs = Traducao.GetTextoPorLingua("Seta principal do compartimento 4 é obrigatória e deve conter volume.", "La flecha principal del compartimento 4 es obligatoria y debe contener volumen.", idPais);
				}
				else if (item.VolumeCompartimento5.HasValue && item.VolumeCompartimento5.Value == 0 && item.CompartimentoPrincipal5.HasValue && item.CompartimentoPrincipal5.Value)
				{
					msgs = Traducao.GetTextoPorLingua("Seta principal do compartimento 5 é obrigatória e deve conter volume.", "La flecha principal del compartimento 5 es obligatoria y debe contener volumen.", idPais);
				}
				else if (item.VolumeCompartimento6.HasValue && item.VolumeCompartimento6.Value == 0 && item.CompartimentoPrincipal6.HasValue && item.CompartimentoPrincipal6.Value)
				{
					msgs = Traducao.GetTextoPorLingua("Seta principal do compartimento 6 é obrigatória e deve conter volume.", "La flecha principal del compartimento 6 es obligatoria y debe contener volumen.", idPais);
				}
				else if (item.VolumeCompartimento7.HasValue && item.VolumeCompartimento7.Value == 0 && item.CompartimentoPrincipal7.HasValue && item.CompartimentoPrincipal7.Value)
				{
					msgs = Traducao.GetTextoPorLingua("Seta principal do compartimento 7 é obrigatória e deve conter volume.", "La flecha principal del compartimento 7 es obligatoria y debe contener volumen.", idPais);
				}
				else if (item.VolumeCompartimento8.HasValue && item.VolumeCompartimento8.Value == 0 && item.CompartimentoPrincipal8.HasValue && item.CompartimentoPrincipal8.Value)
				{
					msgs = Traducao.GetTextoPorLingua("Seta principal do compartimento 8 é obrigatória e deve conter volume.", "La flecha principal del compartimento 8 es obligatoria y debe contener volumen.", idPais);
				}
				else if (item.VolumeCompartimento9.HasValue && item.VolumeCompartimento9.Value == 0 && item.CompartimentoPrincipal9.HasValue && item.CompartimentoPrincipal9.Value)
				{
					msgs = Traducao.GetTextoPorLingua("Seta principal do compartimento 9 é obrigatória e deve conter volume.", "La flecha principal del compartimento 9 es obligatoria y debe contener volumen.", idPais);
				}
				else if (item.VolumeCompartimento10.HasValue && item.VolumeCompartimento10.Value == 0 && item.CompartimentoPrincipal10.HasValue && item.CompartimentoPrincipal10.Value)
				{
					msgs = Traducao.GetTextoPorLingua("Seta principal do compartimento 10 é obrigatória e deve conter volume.", "La flecha principal del compartimento 10 es obligatoria y debe contener volumen.", idPais);
				}

				if (!string.IsNullOrEmpty(msgs))
				{
					json.Data = msgs;
					return json;
				}

				var seta = bll.Selecionar(item.ID);
				seta.CompartimentoPrincipal1 = item.CompartimentoPrincipal1;
				seta.CompartimentoPrincipal2 = item.CompartimentoPrincipal2;
				seta.CompartimentoPrincipal3 = item.CompartimentoPrincipal3;
				seta.CompartimentoPrincipal4 = item.CompartimentoPrincipal4;
				seta.CompartimentoPrincipal5 = item.CompartimentoPrincipal5;
				seta.CompartimentoPrincipal6 = item.CompartimentoPrincipal6;
				seta.CompartimentoPrincipal7 = item.CompartimentoPrincipal7;
				seta.CompartimentoPrincipal8 = item.CompartimentoPrincipal8;
				seta.CompartimentoPrincipal9 = item.CompartimentoPrincipal9;
				seta.CompartimentoPrincipal10 = item.CompartimentoPrincipal10;
				bll.Atualizar(seta);
			}

			var comp = new ComposicaoBusiness(idPais).Selecionar(idComposicao);

			if (idPais == EnumPais.Brasil)
				new ComposicaoBusiness(idPais).AtualizarComposicao(comp, false, enviaEmail: false, idStatus: comp.IDStatus);


			if (!string.IsNullOrEmpty(comp.Mensagem))
			{
				json.Data = comp.Mensagem;
			}
			else
			{
				json.Data = Traducao.GetTextoPorLingua("Seta alterada com sucesso!", "Flecha cambiada!", idPais);
			}
			return json;
		}

		#endregion

		public JsonResult VerificarClientePermissaoBase(int id)
		{
			var composicao = _composicaoBLL.Selecionar(id);
			int idPlacaPrincipal = _composicaoBLL.SelecionaPlacaPrincipalFOB(composicao).ID;

			var loginUsuario = UsuarioCsOnline != null && UsuarioCsOnline.Login != null ? UsuarioCsOnline.Login : UserSession.GetCurrentInfoUserSystem().InformacoesUsuario.Login;
			var usuario = new UsuarioBusiness().Selecionar(w => w.Login == loginUsuario);

			var placaClientes = new PlacaClienteBusiness().ListarClientesPorPlaca(idPlacaPrincipal, usuario.ID).ToList();

			var retorno = new
			{
				idPlaca = idPlacaPrincipal,
				quantidade = placaClientes.Count,
				placasUsuario = placaClientes.Where(w => w.Ibm == loginUsuario).Select(s => s.ID).ToArray(),
				placasRede = placaClientes.Select(s => s.ID).ToArray()
			};

			return Json(new
			{
				Data = retorno
			}, JsonRequestBehavior.AllowGet);
		}

		[HttpPost]
		[AjaxOnlyAttribute]
		public string ExcluirPlacaBase(int idComposicao, int idPlaca, int[] placaClientes)
		{
			try
			{
				return this.placaClienteBll.ExcluirPlacaCliente(idComposicao, idPlaca, placaClientes);
			}
			catch (Exception ex)
			{
				return ex.Message;
			}
		}

		public FileResult ExportarBase(ComposicaoFiltro filtro, EnumPais pais)
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
					Usuario usuario = new UsuarioBusiness().Selecionar(w => w.Login == usCliente.Login);
					filtro.UsuarioExterno = usuario.Externo;

					if (usuario.Perfil.IsTransportadora())
						filtro.IDUsuarioTransportadora = usuario.ID;
					else if (usuario.Perfil.IsClienteEAB())
						filtro.IDUsuarioCliente = usuario.ID;
				}
				else
				{
					filtro.UsuarioExterno = true;
				}
			}

			Stream fs = new ComposicaoBusiness(pais).Exportar(filtro);

            var nomeArquivo = $"Composicao_Placas_{DateTime.Now.ToString("ddMMyyyyhhmmss")}.xlsx";

            if (pais == EnumPais.Argentina)
                nomeArquivo = $"Composicion_Patente_{DateTime.Now.ToString("ddMMyyyyhhmmss")}.xlsx";

            return File(fs, System.Net.Mime.MediaTypeNames.Application.Octet, nomeArquivo);
		}

		public virtual PartialViewResult VisualizarDocumentos(int idComposicao)
		{
			var comp = new ComposicaoBusiness().Selecionar(idComposicao);
			var model = new PlacaBusiness().ListarPorComposicao(comp);
			
			if (comp.IdPais == (int)EnumPais.Argentina)
				return PartialView("~/Views/ComposicaoArgentina/Partial/_VisualizarDocumentosPlaca.cshtml", model);

			return PartialView("_VisualizarDocumentosPlaca", model);
		}

		public ActionResult CarregarDefaultBase(EnumPais pais)
		{
			ModelComposicao model = new ModelComposicao();
			model = ComposicaoLayout(model);
			model.Operacao = OperacoesCRUD.List;
			model.Filtro = new ComposicaoFiltro();
			model.Filtro.Status = true;
			model.IdPais = (int)pais;

			CarregarPermissaoUsuario(model);

			Usuario usCliente = UsuarioCsOnline;
			if (usCliente != null)
			{
				bool usuarioQuality;
				model.Filtro.IDUsuarioCliente = usCliente.ID;
				model.Filtro.UsuarioExterno = usCliente.Externo;
				model.UsuarioPerfil = usCliente.Perfil;
				usuarioQuality = usCliente?.Perfil.Contains("Quality") ?? false;
				model.Filtro.UsuarioExterno = usuarioQuality ? false : model.Filtro.UsuarioExterno;
			}
			else
			{
				Usuario usuarioLogado = UsuarioLogado;

				bool usuarioTransportadora, usuarioCliente, usuarioQuality;

				if (pais == EnumPais.Argentina)
				{
					usuarioTransportadora = usuarioLogado?.Perfil.Contains("Transportadora") ?? true;
					usuarioCliente = usuarioLogado?.Perfil.Contains("Cliente") ?? true;
					usuarioQuality = usuarioLogado?.Perfil.Contains("Quality") ?? false;
					model.Filtro.UsuarioExterno = (usuarioLogado?.Externo ?? true) || usuarioTransportadora || usuarioCliente;
					model.Filtro.UsuarioExterno = usuarioQuality ? false : model.Filtro.UsuarioExterno;
				}
				else if (pais == EnumPais.Brasil)
				{
					usuarioQuality = usuarioLogado?.Perfil.Contains("Quality") ?? false;
					model.Filtro.UsuarioExterno = usuarioLogado?.Externo ?? true;
					model.Filtro.UsuarioExterno = usuarioQuality ? false : model.Filtro.UsuarioExterno;
				}


				model.UsuarioPerfil = usuarioLogado?.Perfil;
			}

			if (UserSession.GetCurrentInfoUserSystem() != null)
            {
				if (UserSession.GetCurrentInfoUserSystem().InformacoesPermissao.FirstOrDefault(p => p.MVCController == "Composicao" && p.NomeAcao == "Aprovar") != null)
					model.Filtro.IDStatus = (int)EnumStatusComposicao.EmAprovacao;
            }
            else
            {
				return RedirectToAction("Index", "Home");
            }


			model.Resultado = new ResultadoOperacao();
			model.PaginadorDados = new PaginadorModel();
			model.PaginadorDados.Status = EstadoPaginador.RenovandoConsulta;

			if (Session["ComposicaoFiltro"] != null)
				model.Filtro = (ComposicaoFiltro)Session["ComposicaoFiltro"];

			this.ListarPaginador(model);

			return View("Index", model);
		}



		#region Private methods

		private void AdicionarClientesPlaca(int idPlaca, List<int> listIdClientesPlacaChave)
		{
			var listIdClientes = this._placaBll.ListarClientesPorPlaca(idPlaca);
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

		#endregion
	}
}
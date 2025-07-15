using Raizen.Framework.UserSystem.Client;
using Raizen.Framework.Web.MVC.Models;
using Raizen.UniCad.BLL;
using Raizen.UniCad.Model;
using Raizen.UniCad.Model.Filtro;
using Raizen.UniCad.Model.View;
using Raizen.UniCad.Web.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Raizen.UniCad.Web.Controllers
{
	public abstract class PlacaBaseController : BaseUniCadController
	{
		#region Constantes

		protected readonly PlacaBusiness PlacaBLL = new PlacaBusiness();
		protected readonly PlacaArgentinaBusiness PlacaArgentinaBll = new PlacaArgentinaBusiness();
		protected const string NomeFiltro = "Filtro_Placa";
		protected const string TotalRegistros = "totalRegistros_Placa";

		#endregion

		#region Public methods

		public virtual ActionResult Novo(int? tipoVeiculo, int idTipoParteVeiculo, int idTipoComposicao, string operacaoFrete, int linhaNegocio, int idPais)
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

			var loginUsuario = UsuarioCsOnline != null && UsuarioCsOnline.Login != null ? UsuarioCsOnline.Login : UserSession.GetCurrentInfoUserSystem().InformacoesUsuario.Login;
			var usuario = new UsuarioBusiness().Selecionar(w => w.Login == loginUsuario);
			if (usuario != null)
			{
				Model.Usuario = usuario;
				Model.Placa.idUsuario = usuario.ID;
				usuario.Transportadoras = new UsuarioTransportadoraBusiness().ListarTransportadorasPorUsuario(usuario.ID, linhaNegocio);
				usuario.Clientes = new UsuarioClienteBusiness().ListarClientesPorUsuario(usuario.ID, linhaNegocio);

				if (usuario.Externo && Model.Placa.Operacao == "CIF" && usuario.Transportadoras != null && usuario.Transportadoras.Count == 1)
				{
					Model.Placa.IDTransportadora = usuario.Transportadoras.First().IDTransportadora;
				}

				if (usuario.Externo && Model.Placa.Operacao == "FOB" && linhaNegocio == (int)EnumEmpresa.EAB && usuario.Clientes != null && usuario.Clientes.Any())
				{
					Model.Placa.Clientes = new List<PlacaClienteView>();
					foreach (var item in usuario.Clientes)
					{
						Model.Placa.Clientes.Add(new PlacaClienteView() { IDCliente = item.IDCliente, RazaoSocial = item.IBM + " - " + item.CPF_CNPJ + " - " + item.RazaoSocial });
					}
				}
			}

			return PartialView("~/Views/PlacaArgentina/Partial/_Edicao.cshtml", Model);
		}
		#region ListarDocumentos
		public virtual PartialViewResult ListarDocumentos(int? IDTipoVeiculo, int? IDCategoriaVeiculo, string Operacao, int? LinhaNegocio, int? IDTipoProduto, bool? Aprovar, int idPais, int? idTipoComposicao, int? numero)
		{
			var model = new PlacaDocumentoBusiness((EnumPais)idPais).ListarPlacaDocumento(IDTipoVeiculo, IDCategoriaVeiculo, Operacao, LinhaNegocio, IDTipoProduto, idTipoComposicao, numero);
			if (model != null && model.Any())
				foreach (var item in model)
				{
					if (Aprovar.HasValue)
						item.Aprovar = Aprovar.Value;
				}

			return PartialView("~/Views/PlacaArgentina/Partial/_Documentos.cshtml", model);
		}
		#endregion
		public abstract PartialViewResult AdicionarSeta(int colunas, int sequencial, bool multiseta, int linhaNegocio);
		public abstract ActionResult Salvar(ModelPlaca model);
		public abstract ActionResult Editar(string Id, string IdplacaOficial, bool Aprovar, string operacaoComposicao, string idComposicao, int linhaNegocio, int numero, int idTipoComposicao);
		public abstract PartialViewResult AdicionarCliente(int idCliente);

		#endregion

		#region Protected methods

		protected Usuario GetUsuarioLogado()
		{
			string loginUsuario = GetLoginUsuario();

			return new UsuarioBusiness().Selecionar(w => w.Login.Equals(loginUsuario));
		}
		protected void ZerarFiltro(ModelPlaca Model)
		{
			Model.Filtro = new PlacaFiltro();
			base.ArmazenarDados<PlacaFiltro>(Model.Filtro, NomeFiltro);
		}
		protected void AtualizarQtdeRegPaginador(ModelPlaca Model)
		{
			Model.Filtro = base.RetornaDados<PlacaFiltro>(NomeFiltro);
			base.ArmazenarDados<PlacaFiltro>(Model.Filtro, NomeFiltro);

			int totalRegistros = this.PlacaBLL.ListarPlacaSemComposicaoCount(Model.Filtro);
			base.ArmazenarDados<int>(totalRegistros, TotalRegistros);
		}
		protected void CarregarAlteracoesDePlacas(int IdplacaOficial, ModelPlaca Model)
		{
			using (var placaBll = new PlacaBusiness(Model.Placa.IDPais))
			{
				Model.PlacaOficial = placaBll.SelecionarPlacaCompleta(new ComposicaoFiltro { IdPlacaOficial = Model.Placa.idPlacaOficial.Value });
				//se a placaOficial estiver nula, significa que o status dela não é mais de aprovada
				if (Model.PlacaOficial != null)
					Model.Placa.PlacaAlteracoes = placaBll.ListarAlteracoes(Model.Placa.ID, Model.Placa.idPlacaOficial.Value);
			}
		}
		protected static void CalcularColunas(ModelPlaca Model)
		{
			new PlacaBusiness().CalcularColunas(Model.Placa);
		}

		#endregion

		#region Private methods

		private string GetLoginUsuario()
		{
			return UsuarioCsOnline != null && UsuarioCsOnline.Login != null ? UsuarioCsOnline.Login : UserSession.GetCurrentInfoUserSystem().InformacoesUsuario.Login;
		}

		#endregion
	}
}
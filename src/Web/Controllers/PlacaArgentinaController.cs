using Raizen.Framework.Models;
using Raizen.Framework.UserSystem.Client;
using Raizen.Framework.Web.MVC.Filters;
using Raizen.Framework.Web.MVC.Models;
using Raizen.UniCad.BLL;
using Raizen.UniCad.Model;
using Raizen.UniCad.Model.View;
using Raizen.UniCad.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Raizen.UniCad.Web.Controllers
{
	public class PlacaArgentinaController : PlacaBaseController
	{
		[HttpGet]
		[AjaxOnly]
		public override ActionResult Novo(int? tipoVeiculo, int idTipoParteVeiculo, int idTipoComposicao, string operacaoFrete, int linhaNegocio, int idPais)
		{
			return base.Novo(tipoVeiculo, idTipoParteVeiculo, idTipoComposicao, operacaoFrete, linhaNegocio, idPais);
		}

        public override PartialViewResult AdicionarSeta(int colunas, int sequencial, bool multiseta, int linhaNegocio)
		{
			PlacaSeta model = new PlacaSeta(colunas, sequencial, multiseta);

			if (linhaNegocio == (int)EnumEmpresa.Combustiveis)
				return PartialView("~/Views/PlacaArgentina/Partial/_ItemSeta.cshtml", model);
			else
				return PartialView("~/Views/PlacaArgentina/Partial/_ItemSetaComLacre.cshtml", model);
		}

        [AjaxOnly]
		public override ActionResult Salvar(ModelPlaca model)
		{
			Usuario usuario = GetUsuarioLogado();

			var compBll = new ComposicaoBusiness(EnumPais.Argentina);

			model.TipoValidacao = TiposValidacao.ValidacaoTotal;

			//R5) Verificar se deve associar o ID do usuário logado ao salvar a placa
			if (usuario != null && (usuario.Perfil.Equals("Cliente EAB") || usuario.Perfil.Equals(EnumPerfil.CLIENTE_ACS) || usuario.Perfil.Equals(EnumPerfil.CLIENTE_ACS_ARGENTINA)))
				model.Placa.idUsuario = usuario.ID;

			if (model.Placa.SomenteLiberacaoAcesso)
			{
				model.Placa.ID = int.Parse(model.ChavePrimaria);
				Placa placaNew;
				model.Resultado = base.ProcessarResultado(new PlacaBusiness(EnumPais.Argentina).AtualizarPlacaPermissao(model.Placa, out placaNew), OperacoesCRUD.Insert);
				model.Placa = placaNew;
				model.Placa.SomenteLiberacaoAcesso = true;
				model.Operacao = OperacoesCRUD.Update;
			}
			else
			{
				ModelState.Remove("Placa.IDCategoriaVeiculo");
				if (base.ValidarModel(model, this.ModelState))
				{
					if (model.Placa.Clientes == null)
						model.Placa.Clientes = new List<PlacaClienteView>();

					if (model.Operacao.Equals(OperacoesCRUD.Insert))
					{
						model.Resultado = base.ProcessarResultado(new PlacaBusiness(EnumPais.Argentina).AdicionarPlaca(model.Placa), OperacoesCRUD.Insert);
						ZerarFiltro(model);
						AtualizarQtdeRegPaginador(model);
						model.Operacao = OperacoesCRUD.Update;
					}

					if (model.Operacao.Equals(OperacoesCRUD.Editando))
					{
						model.Placa.ID = int.Parse(model.ChavePrimaria);

						model.Resultado = base.ProcessarResultado(new PlacaBusiness(EnumPais.Argentina).AtualizarPlaca(model.Placa, EnumPais.Argentina), OperacoesCRUD.Update);
						model.Operacao = OperacoesCRUD.Update;
					}
				}
				else
				{

					if (model.Placa.idPlacaOficial.HasValue)
					{
						model.Placa.ID = int.Parse(model.ChavePrimaria);
						model.Placa.Setas = new PlacaSetaBusiness().Listar(p => p.IDPlaca == model.Placa.ID);
						CarregarAlteracoesDePlacas(model.Placa.idPlacaOficial.Value, model);
					}
				}

				CalcularColunas(model);
			}

			if (model.IDComposicao > 0)
			{
				var composicao = compBll.Selecionar(model.IDComposicao.Value);
				
				if (composicao.IDPlaca1 == model.Placa.ID && composicao.CPFCNPJ != model.Placa.PlacaArgentina.CUIT)
				{
					composicao.CPFCNPJ = model.Placa.PlacaArgentina.CUIT;
					composicao.RazaoSocial = model.Placa.RazaoSocial;
					compBll.Atualizar(composicao);
				}
			}

			if (model.Placa.SomenteLiberacaoAcesso)
				return PartialView("~/Views/PlacaArgentina/Permissao.cshtml", model);

			return PartialView("~/Views/PlacaArgentina/Partial/_Edicao.cshtml", model);
		}

        public override ActionResult Editar(string Id, string IdplacaOficial, bool Aprovar, string operacaoComposicao, string idComposicao, int linhaNegocio, int numero, int idTipoComposicao)
		{
			Usuario usuario = GetUsuarioLogado();

			ModelPlaca Model = new ModelPlaca();

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
				Model.Placa = new PlacaBusiness(EnumPais.Argentina).Selecionar(Model.Placa.ID);
				Model.Placa.PlacaArgentina = this.PlacaArgentinaBll.Selecionar(Model.Placa.ID);
				Model.Placa.LinhaNegocio = linhaNegocio;
				Model.Placa.Numero = numero;
				Model.Placa.idTipoParteVeiculo = numero;
				Model.Placa.idTipoComposicao = idTipoComposicao;

				if (!string.IsNullOrEmpty(idComposicao))
					Model.IDComposicao = int.Parse(idComposicao);

				Model.Aprovar = Aprovar;
				Model.Placa.Clientes = new PlacaClienteBusiness().ListarClientesPorPlaca(Model.Placa.ID, IDUsuarioCliente).OrderByDescending(o => o.DataAprovacao).ToList();
				Model.Placa.Setas = new PlacaSetaBusiness().Listar(p => p.IDPlaca == Model.Placa.ID);

				using (var placaDocumentoBll = new PlacaDocumentoBusiness(EnumPais.Argentina))
					Model.Placa.Documentos = placaDocumentoBll.ListarPlacaDocumentoPorPlaca(Model.Placa.ID, numero);

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

			Placa placaAprovada = null;
			if (Model.Placa != null && Model.Placa.ID > 0)
				placaAprovada = new PlacaBusiness().SelecionarPlacaComposicaoAprovada(Model.Placa);
			var acesso = new PlacaBusiness().ValidarAcesso(UserSession.GetCurrentInfoUserSystem().InformacoesUsuario.Login, UsuarioCsOnline, placaAprovada);

			Model.Placa.SomenteLiberacaoAcesso = !acesso;


			if (usuario != null)
				Model.Usuario = usuario;

			var composicao = string.IsNullOrEmpty(idComposicao) ? null : new ComposicaoBusiness().Selecionar(Convert.ToInt32(idComposicao));

			var composicaoAprovado = composicao != null && composicao.IDStatus == (int)EnumStatusComposicao.Aprovado;
			Model.Placa.StatusComposicao = composicao == null ? 0 : composicao.IDStatus;
			if (Model.Placa.Operacao != "Ambos")
				Model.Placa.Operacao = operacaoComposicao;

			if ((composicaoAprovado || Model.Placa.StatusComposicao == 0) && (!Model.Placa.SomenteLiberacaoAcesso || Model.Placa.Operacao == "FOB"))
			{
				Model.Placa.ID = 0;
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
			return PartialView("~/Views/PlacaArgentina/Partial/_Edicao.cshtml", Model);
		}

        public override PartialViewResult AdicionarCliente(int idCliente)
		{
			Cliente cliente = new ClienteBusiness().Selecionar(idCliente);
			PlacaClienteView model = new PlacaClienteView(cliente);

			return PartialView("~/Views/PlacaArgentina/Partial/_ItemCliente.cshtml", model);
		}
	}
}
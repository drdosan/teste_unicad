using Infraestructure.Extensions;
using Raizen.Framework.Models;
using Raizen.Framework.UserSystem.Client;
using Raizen.Framework.Utils.Extensions;
using Raizen.Framework.Web.MVC.Models;
using Raizen.Framework.Web.MVC.Utils;
using Raizen.UniCad.BLL;
using Raizen.UniCad.BLL.Util;
using Raizen.UniCad.Extensions;
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
    public abstract class MotoristaBaseController : BaseUniCadController
	{
		public MotoristaBaseController(BaseControllerOptions options) : base(options)
		{

		}

		public MotoristaBaseController() : base(BaseControllerOptions.NaoValidarAcesso)
		{
		}

		#region Constantes

		public readonly MotoristaBusiness _motoristaBll = new MotoristaBusiness();
		public readonly MotoristaPesquisaBusiness _motoristaPesquisaBll = new MotoristaPesquisaBusiness();

		protected const string NomeFiltro = "Filtro_Motorista";
		protected const string NomePaginador = "Paginador_Motorista";
		protected const string TotalRegistros = "totalRegistros_Motorista";
		
        #endregion

		#region Documentos

		public virtual PartialViewResult VisualizarDocumentos(int id, int idPais)
		{
			string descricao = Traducao.GetTextoPorLingua("Anexo Geral", "Apego General", (EnumPais)idPais);
			var model = _motoristaBll.Selecionar(id);
			model.Documentos = new MotoristaDocumentoBusiness().ListarMotoristaDocumentoPorMotorista(id);
			model.Documentos.ForEach(w => w.VisualizarDoc = true);

			model.Documentos.Add(new MotoristaDocumentoView() { Descricao = descricao, Anexo = model.Anexo, VisualizarDoc = true });

			model.TreinamentoView = new TreinamentoView();
			model.ListaTerminais = new TerminalBusiness().ListarPorMotorista(id);
			model.ListaTreinamento = new HistoricoTreinamentoTeoricoMotoristaBusiness().Listar(w => w.IDMotorista == id);

			return PartialView("_VisualizarDocumentosMotorista", model);
		}

		#endregion

		#region Editar

		public virtual ModelMotoristaArgentina CarregarDadosMotorista(string id, bool aprovar)
		{
			var model = new ModelMotoristaArgentina
			{
				Motorista = new Motorista(),
				Aprovar = aprovar
			};
			if (!string.IsNullOrEmpty(id))
			{
				model.Operacao = OperacoesCRUD.Editando;
				model.ChavePrimaria = id;
				model.Resultado = new ResultadoOperacao();

				model.Motorista = new MotoristaPesquisaBusiness().Selecionar(int.Parse(model.ChavePrimaria)).Mapear();

				if (!aprovar && model.Motorista.IDStatus != (int)EnumStatusMotorista.Reprovado)
				{
					model.Motorista.IDMotorista = model.Motorista.ID;
					model.Motorista.ID = 0;
					model.ChavePrimaria = "0";
				}

			}
			return model;
		}

        public virtual PartialViewResult UploadDocumentos(int id)
		{
			var model = new ModelMotoristaArgentina();
			model.Motorista = _motoristaBll.Selecionar(id);
			model.Aprovar = true;
			model.Motorista.Documentos = new MotoristaDocumentoBusiness().ListarMotoristaDocumentoPorMotorista(id);
			model.Motorista.Documentos.ForEach(w => w.Aprovar = true);
			return PartialView("_UploadDocumentos", model);
		}

		public virtual void CarregarAlteracoes(ModelMotoristaArgentina model)
		{
			if (model.Motorista.IDMotorista.HasValue)
			{
				using (var motoristaBll = new MotoristaBusiness(model.Motorista.IdPais))
				{
					var motoristaAnterior = _motoristaPesquisaBll.Selecionar(model.Motorista.IDMotorista.Value).Mapear();

					if (motoristaAnterior != null)
					{
						model.Alteracoes = motoristaBll.CarregarAlteracoes(model.Motorista, motoristaAnterior);
					}
				}
			}
		}

		public virtual void CarregarAlteracoes(ModelMotorista model)
		{
			if (!model.Motorista.IDMotorista.HasValue)
			{
				return;
			}
			using (var motoristaBll = new MotoristaBusiness(model.Motorista.IdPais))
			{
				var motoristaAnterior = _motoristaPesquisaBll.Selecionar(model.Motorista.IDMotorista.Value).Mapear();

				if (motoristaAnterior != null)
				{
					model.Alteracoes = motoristaBll.CarregarAlteracoes(model.Motorista, motoristaAnterior);
				}
			}
		}

		public ActionResult Aprovar(string id)
		{
			ModelMotoristaArgentina model = CarregarDadosMotorista(id, true);
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
		
        #endregion

		#region Listar

		public virtual void ListarRenovandoConsulta(ModelMotoristaArgentina model)
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

        public virtual void ListarPaginador(ModelMotoristaArgentina model)
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

        protected void ListarPaginando(ModelMotoristaArgentina model)
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

        public virtual void Listar(ModelMotoristaArgentina model)
		{
			ListarPaginador(model);
			model.Resultado = ProcessarResultado(!model.ListaMotorista.IsNullOrEmpty(), model.Operacao);
		}
 
        #endregion

        #region Pesquisar
        
        #region CarregarPermissaoUsuario

        public virtual void CarregarPermissaoUsuario(ModelMotoristaArgentina model)
		{
			CarregarPermissaoUsuario(model, false);
		}

		public virtual void CarregarPermissaoUsuario(ModelMotoristaArgentina model, bool isPesquisa)
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
					model.Motorista.OperacaoUsuario = usuario.Operacao == "Ambos" ? null : usuario.Operacao;
				else if (isPesquisa)
					model.Filtro.OperacaoUsuario = usuario.Operacao == "Ambos" ? null : usuario.Operacao;
				else
					model.Filtro.Operacao = model.Filtro.OperacaoUsuario = usuario.Operacao == "Ambos" ? null : usuario.Operacao;

			}
		}
		
        #endregion

		public virtual void PrepararPaginadorOperacoes(ModelMotoristaArgentina model)
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

		#region CarregarPermissaoCif

		protected PartialViewResult CarregarPermissaoCifBase(int? idMotorista, int idEmpresa, int? idTransportadora, bool? novo, ModelMotoristaArgentina model)
		{
			if (idTransportadora.HasValue)
			{
				model.Motorista = new Motorista { IDTransportadora = idTransportadora };
			}
			else if (idMotorista != 0 && idMotorista != null)
				model.Motorista = _motoristaPesquisaBll.Selecionar(idMotorista.Value).Mapear();
			else
				model.Motorista = new Motorista();

			model.LinhaNegocio = idEmpresa;

			SelecionarTranspCliente(model, "CIF");

			if (model.Motorista.IDStatus == (int)EnumStatusMotorista.EmAprovacao)
				CarregarAlteracoes(model);

			var loginUsuario = UsuarioCsOnline != null && UsuarioCsOnline.Login != null ? UsuarioCsOnline.Login : UserSession.GetCurrentInfoUserSystem().InformacoesUsuario.Login;
			var usuario = new UsuarioBusiness().Selecionar(w => w.Login == loginUsuario);

			if (usuario != null && usuario.Externo && novo.HasValue)
				model.Novo = novo.Value;

			return PartialView("_Transportadora", model);
		}
		
        #endregion

		protected PartialViewResult CarregarPermissaoCifBase(int? idMotorista, int idEmpresa, int? idTransportadora, bool? novo, ModelMotorista model)
		{
			if (!idTransportadora.HasValue)
			{
				if (idMotorista != 0 && idMotorista != null)
					model.Motorista = _motoristaPesquisaBll.Selecionar(idMotorista.Value).Mapear();
				else
					model.Motorista = new Motorista();
			}
			else
			{
				model.Motorista = new Motorista { IDTransportadora = idTransportadora };
			}

			model.LinhaNegocio = idEmpresa;

			SelecionarTranspCliente(model, "CIF");

			if (model.Motorista.IDStatus == (int)EnumStatusMotorista.EmAprovacao)
				CarregarAlteracoes(model);

			var loginUsuario = UsuarioCsOnline != null && UsuarioCsOnline.Login != null ? UsuarioCsOnline.Login : UserSession.GetCurrentInfoUserSystem().InformacoesUsuario.Login;
			var usuario = new UsuarioBusiness().Selecionar(w => w.Login == loginUsuario);

			if (usuario != null && usuario.Externo && novo.HasValue)
				model.Novo = novo.Value;

			return PartialView("_Transportadora", model);
		}

		protected Motorista CarregaPermissaoFobBase(Motorista motorista, int? idMotorista, int idEmpresa, Usuario usuario)
		{
			Motorista newMotorista;
			if (motorista?.Clientes != null && motorista.Clientes.Any())
			{
				newMotorista = new Motorista { Clientes = motorista.Clientes };
			}
			else if (idMotorista != 0 && idMotorista != null)
			{
				newMotorista = _motoristaBll.Selecionar(idMotorista.Value);
				List<MotoristaClienteView> clientes;

                if (!usuario.Externo || usuario.Perfil.IsQuality())
					clientes = new MotoristaClienteBusiness().ListarMotoristaClientePorMotorista(idMotorista.Value, idEmpresa);
				else
					clientes = new MotoristaClienteBusiness().ListarMotoristaClientePorMotorista(idMotorista.Value, idEmpresa, usuario.ID);

				if (clientes != null && clientes.Any())
				{
					newMotorista.Clientes = new List<MotoristaClienteView>();
					foreach (var item in clientes)
					{
						newMotorista.Clientes.Add(new MotoristaClienteView
						{
							ID = item.IDCliente,
							IDCliente = item.IDCliente,
							RazaoSocial = item.IBM + " - " + item.CNPJCPF + " - " + item.RazaoSocial,
							DataAprovacao = item.DataAprovacao
						});
					}
				}
			}
			else
				newMotorista = new Motorista();

			return newMotorista;
		}

		protected static void GetClienteTransportadora(ModelMotoristaArgentina model, Usuario usuario)
		{
			if (usuario.Perfil == "Transportadora")
				model.Filtro.IDUsuarioTransportadora = usuario.ID;
			else if (usuario.Perfil == "Cliente EAB")
				model.Filtro.IDUsuarioCliente = usuario.ID;
		}

        protected void SelecionarTranspCliente(ModelMotoristaArgentina model, string operacao)
		{
			var loginUsuario = UsuarioCsOnline != null && UsuarioCsOnline.Login != null ? UsuarioCsOnline.Login : UserSession.GetCurrentInfoUserSystem().InformacoesUsuario.Login;
			var usuario = new UsuarioBusiness().Selecionar(w => w.Login == loginUsuario);

			if (usuario != null)
			{
				model.Usuario = usuario;

				usuario.Transportadoras = new UsuarioTransportadoraBusiness().ListarTransportadorasPorUsuario(usuario.ID, model.LinhaNegocio);
				usuario.Clientes = new UsuarioClienteBusiness().ListarClientesPorUsuario(usuario.ID, model.LinhaNegocio);

				if (usuario.Externo && operacao == "CIF" && usuario.Transportadoras != null && usuario.Transportadoras.Count == 1)
				{
					model.Motorista.IDTransportadora = usuario.Transportadoras.First().IDTransportadora;
				}

				if (usuario.Externo && operacao == "FOB" && model.LinhaNegocio == (int)EnumEmpresa.EAB && usuario.Clientes != null && usuario.Clientes.Any())
				{
					model.Motorista.Clientes = new List<MotoristaClienteView>();
					foreach (var item in usuario.Clientes)
					{
						model.Motorista.Clientes.Add(new MotoristaClienteView() { IDCliente = item.IDCliente, RazaoSocial = item.IBM + " - " + item.RazaoSocial });
					}
				}

			}
		}

		protected void SelecionarTranspCliente(ModelMotorista model, string operacao)
		{
			var loginUsuario = UsuarioCsOnline != null && UsuarioCsOnline.Login != null ? UsuarioCsOnline.Login : UserSession.GetCurrentInfoUserSystem().InformacoesUsuario.Login;
			var usuario = new UsuarioBusiness().Selecionar(w => w.Login == loginUsuario);

			if (usuario != null)
			{
				model.Usuario = usuario;

				usuario.Transportadoras = new UsuarioTransportadoraBusiness().ListarTransportadorasPorUsuario(usuario.ID, model.LinhaNegocio);
				usuario.Clientes = new UsuarioClienteBusiness().ListarClientesPorUsuario(usuario.ID, model.LinhaNegocio);

				if (usuario.Externo && operacao == "CIF" && usuario.Transportadoras != null && usuario.Transportadoras.Count == 1)
				{
					model.Motorista.IDTransportadora = usuario.Transportadoras.First().IDTransportadora;
				}

				if (usuario.Externo && operacao == "FOB" && model.LinhaNegocio == (int)EnumEmpresa.EAB && usuario.Clientes != null && usuario.Clientes.Any())
				{
					model.Motorista.Clientes = new List<MotoristaClienteView>();
					foreach (var item in usuario.Clientes)
					{
						model.Motorista.Clientes.Add(new MotoristaClienteView() { IDCliente = item.IDCliente, RazaoSocial = item.IBM + " - " + item.RazaoSocial });
					}
				}

			}
		}

		#endregion

		#region Paginacao

		public virtual void ZerarFiltro(ModelMotoristaArgentina model)
		{
			model.Filtro = new MotoristaFiltro();
			ArmazenarDados(model.Filtro, NomeFiltro);
		}

		protected void AtualizarQtdeRegPaginador(ModelMotoristaArgentina model)
		{
			model.Filtro = RetornaDados<MotoristaFiltro>(NomeFiltro);
			ArmazenarDados(model.Filtro, NomeFiltro);

			int totalRegistros = _motoristaBll.ListarMotoristaCount(model.Filtro);
			ArmazenarDados(totalRegistros, TotalRegistros);
		}

		protected void PrepararPaginador(ref ModelMotoristaArgentina model)
		{
			PaginadorModel clone = model.PaginadorDados.Clone() as PaginadorModel;
			Paginador.CalcularPaginas(clone, TipoPaginador.Modelo_Linq);
			model.PaginadorDados = clone;
		}

		#endregion

		#region Salvar

		public virtual ActionResult SalvarPermissoes(ModelMotoristaArgentina Model)
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

			Model.Resultado = ProcessarResultado(_motoristaBll.AdicionarMotorista(Model.Motorista), OperacoesCRUD.Insert);
			ZerarFiltro(Model);
			AtualizarQtdeRegPaginador(Model);
			Model.Operacao = OperacoesCRUD.Update;
			return PartialView("Permissao", Model);
		}

		private static void SomarPermissoes(ModelMotoristaArgentina Model, Motorista model)
		{
			if (model.Operacao == "CIF")
			{
				Model.Motorista.IDTransportadora = model.IDTransportadora;
			}
			else if (model.Operacao == "FOB")
			{
				Model.Motorista.Clientes = new MotoristaClienteBusiness().ListarMotoristaClientePorMotorista(Model.Motorista.ID, Model.Motorista.IDEmpresa);
				foreach (var item in Model.Motorista.Clientes)
					model.Clientes.Add(new MotoristaClienteView { IDCliente = item.IDCliente, RazaoSocial = item.RazaoSocial });
				//SOMAR AS PERMISSÕES ANTIGAS COM AS NOVAS
				Model.Motorista.Clientes = model.Clientes.Distinct().ToList();
			}
		}

		#endregion

		public PartialViewResult AdicionarCliente(int idCliente)
		{
			var model = new MotoristaClienteView();
			var cliente = new ClienteBusiness().Selecionar(idCliente);
			model.IDCliente = cliente.ID;
			model.RazaoSocial = cliente.IBM + " - " + cliente.CNPJCPF + " - " + cliente.RazaoSocial;
			return PartialView("_ItemCliente", model);
		}

		public PartialViewResult AdicionarTerminal(int idTerminal)
		{
			var terminal = new TerminalBusiness().Selecionar(idTerminal);
			string usuario = string.Empty;
			string codigoUsuario = string.Empty;
			if (UsuarioLogado != null)
			{
				usuario = UsuarioLogado.Nome;
				codigoUsuario = UsuarioLogado.Login;
			}

			var item = new TerminalTreinamentoView { IDTerminal = terminal.ID, Nome = terminal.Nome, Usuario = usuario, CodigoUsuario = codigoUsuario };
			return PartialView("_ItemTerminal", item);
		}

		#region In/Ativar/Bloqueio

		public JsonResult SalvarAtivarBase(int id, string justificativa, bool ativo, EnumPais pais)
		{
			var ativar = new HistorioAtivarMotorista();
			ativar.Ativo = ativo;
			ativar.IDMotorista = id;
			ativar.Justificativa = justificativa;
			ativar.Data = DateTime.Now;
			ativar.Usuario = UserSession.GetCurrentInfoUserSystem().InformacoesUsuario.Nome;
			ativar.CodigoUsuario = UserSession.GetCurrentInfoUserSystem().InformacoesUsuario.Login;
			string emailSolicitante = UsuarioCsOnline != null && UsuarioCsOnline.Email != null ? UsuarioCsOnline.Email : UserSession.GetCurrentInfoUserSystem().InformacoesUsuario.Email;
			var msg = new HistorioAtivarMotoristaBusiness().AdicionarAtivar(ativar, emailSolicitante);


			var json = new JsonResult();
			json.Data = msg;
			return json;
		}

		public JsonResult SalvarBloquearBase(int id, string justificativa, bool bloqueio, EnumPais pais)
		{
			var bloq = new HistorioBloqueioMotorista();
			bloq.Bloqueado = bloqueio;
			bloq.IDMotorista = id;
			bloq.Justificativa = justificativa;
			bloq.Data = DateTime.Now;
			bloq.Usuario = UserSession.GetCurrentInfoUserSystem().InformacoesUsuario.Nome;
			bloq.CodigoUsuario = UserSession.GetCurrentInfoUserSystem().InformacoesUsuario.Login;
			string emailSolicitante = UsuarioCsOnline != null && UsuarioCsOnline.Email != null ? UsuarioCsOnline.Email : UserSession.GetCurrentInfoUserSystem().InformacoesUsuario.Email;
			var msg = new HistorioBloqueioMotoristaBusiness().AdicionarBloqueio(bloq, emailSolicitante, pais);


			var json = new JsonResult();
			json.Data = msg;
			return json;
		}

		#endregion

		public ActionResult GerarPdfBase(int id, EnumPais pais)
		{
			MemoryStream memStream = _motoristaBll.GerarPdfCarteirinha(id);
			string handle = Guid.NewGuid().ToString();
			TempData[handle] = memStream.ToArray();
			string nomeArquivo = "Carteirinha" + "_" + DateTime.Now.ToString("yyyyMMdd-hhmmss") + ".pdf";
			return Json(new { FileGuid = handle, FileName = nomeArquivo }, JsonRequestBehavior.AllowGet);
		}

		[HttpGet]
		public virtual ActionResult DownloadBase(string fileGuid, string fileName, EnumPais pais)
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

		protected static void SomarPermissoes(ModelMotorista Model, Motorista model)
		{
			if (model.Operacao == "CIF")
			{
				Model.Motorista.IDTransportadora = model.IDTransportadora;
			}
			else if (model.Operacao == "FOB")
			{
				Model.Motorista.Clientes = new MotoristaClienteBusiness().ListarMotoristaClientePorMotorista(Model.Motorista.ID, Model.Motorista.IDEmpresa);
				foreach (var item in Model.Motorista.Clientes)
					model.Clientes.Add(new MotoristaClienteView { IDCliente = item.IDCliente, RazaoSocial = item.RazaoSocial });
				//SOMAR AS PERMISSÕES ANTIGAS COM AS NOVAS
				Model.Motorista.Clientes = model.Clientes.Distinct().ToList();
			}
		}

		protected JsonResult BuscarCpfDni(string documento, int idEmpresa, EnumPais pais)
		{
			string retorno = string.Empty;

			if (!string.IsNullOrEmpty(documento))
			{
				documento = documento.RemoveCharacter();

				var motoristaBll = new MotoristaPesquisaBusiness();
				var motorista = motoristaBll.Listar(w => (pais == EnumPais.Brasil ? w.CPF : w.DNI) == documento && w.IDEmpresa == idEmpresa).Mapear();
				bool motoristaFob = false;

				if (motorista != null && motorista.Any())
				{
					if (motorista.Any(w => w.IDStatus == (int)EnumStatusMotorista.EmAprovacao))
					{
						retorno = "naoAprovado";
					}
					else if (motorista.Any(w => w.IDStatus == (int)EnumStatusMotorista.Reprovado))
					{
						retorno = "reprovado";
						// Se for reprovado, retornar um atributo indicando se é FOB, pois será possível editar.
						motoristaFob = motorista.Any(w => w.IDStatus == (int)EnumStatusMotorista.Reprovado && w.Operacao.ToUpperInvariant().Trim() == "FOB");
					}
					else if (motorista.Any(w => w.IDStatus == (int)EnumStatusMotorista.Bloqueado))
					{
						retorno = "bloqueado";
					}
					else if (motorista.Any(w => w.IDStatus == (int)EnumStatusMotorista.Aprovado))
					{
						retorno = "aprovado";
					}

					return Json(new { retorno, nome = motorista.FirstOrDefault()?.Nome, isFob = motoristaFob }, JsonRequestBehavior.AllowGet);
				}
				else
				{
					//verificar se existe em outra linha de negócio fazendo com que a opção de clonar seja habilitada                    
					if (motoristaBll.Listar(w => (pais == EnumPais.Brasil ? w.CPF : w.DNI) == documento && w.IDStatus == (int)EnumStatusMotorista.Aprovado && w.IDEmpresa != idEmpresa).Any())
						retorno = "jaExisteOutraEmpresa";
					else
						retorno = "naoExiste";
				}
			}
			else
				retorno = "naoExiste";

			return Json(new { retorno, nome = string.Empty }, JsonRequestBehavior.AllowGet);
		}
	}
}
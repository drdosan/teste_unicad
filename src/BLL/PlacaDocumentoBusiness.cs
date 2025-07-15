using Infraestructure.Utils;
using Raizen.Framework.Utils.Extensions;
using Raizen.UniCad.BLL.Interfaces;
using Raizen.UniCad.BLL.Log;
using Raizen.UniCad.BLL.Util;
using Raizen.UniCad.DAL;
using Raizen.UniCad.DAL.CodeFirst;
using Raizen.UniCad.DAL.CodeFirst.ConfiguracaoModelo;
using Raizen.UniCad.DAL.Interfaces;
using Raizen.UniCad.DAL.Repositories;
using Raizen.UniCad.Model;
using Raizen.UniCad.Model.View;
using Raizen.UniCad.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;

namespace Raizen.UniCad.BLL
{
	public class PlacaDocumentoBusiness : UniCadBusinessBase<PlacaDocumento>
	{
		private readonly IPlacaDocumentoRepository _placaDocumentoRepository;
		private readonly IComposicaoBusiness _composicaoBusiness;
		private readonly IComposicaoBusiness _composicaoArgentinaBusiness;
		private readonly IConfigBusiness _configBusiness;
		private readonly IPlacaBusiness _placaBusiness;
		private readonly EnumPais _pais;

		#region Constructors

		public PlacaDocumentoBusiness()
		{
			_placaDocumentoRepository = new PlacaDocumentoRepository(getContext());
			_composicaoBusiness = new ComposicaoBusiness();
			_composicaoArgentinaBusiness = new ComposicaoBusiness(EnumPais.Argentina);
			_configBusiness = new ConfigBusiness();
			_placaBusiness = new PlacaBusiness();
			_pais = EnumPais.Brasil;
		}

		public PlacaDocumentoBusiness(EnumPais pais)
		{
			_placaDocumentoRepository = new PlacaDocumentoRepository(getContext());
			_composicaoBusiness = new ComposicaoBusiness();
			_composicaoArgentinaBusiness = new ComposicaoBusiness(EnumPais.Argentina);
			_configBusiness = new ConfigBusiness();
			_placaBusiness = new PlacaBusiness();
			_pais = pais;
		}

		public PlacaDocumentoBusiness(IPlacaDocumentoRepository placaDocumentoRepository,
									  IComposicaoBusiness composicaoBusiness,
									  IComposicaoBusiness composicaoArgentinaBusiness,
									  IConfigBusiness configBusiness,
									  IPlacaBusiness placaBusiness,
									  EnumPais pais)
		{
			_placaDocumentoRepository = placaDocumentoRepository;
			_composicaoBusiness = composicaoBusiness;
			_composicaoArgentinaBusiness = composicaoArgentinaBusiness;
			_configBusiness = configBusiness;
			_placaBusiness = placaBusiness;
			_pais = pais;
		}

		UniCadContexto getContext()
		{
			var contexto = new UniCadContexto(ConfigBuilder.GetConnection(), ConfigBuilder.GetModeloCompilado());
#if DEBUG
			contexto.Database.Log = s => System.Diagnostics.Debug.WriteLine(s);
#endif
			return contexto;
		}

		#endregion

		#region Variáveis privadas 

		private Action<string, string> _logar;

		#endregion

		/// <summary>
		/// Definir um logger, que será utilizado para registrar passo a passo de execução de Job.
		/// </summary>
		/// <param name="logar">Action de Log</param>
		public void DefinirLogger(ILogExecucao logar)
		{
			_logar = new Action<string, string>((titulo, descricao) =>
			{
				logar.Log(titulo, descricao, CodigoExecucao.Descricao);
			});
		}
		public List<PlacaDocumentoView> ListarPlacaDocumento(int? idTipoVeiculo, int? idCategoriaVeiculo, string operacao, int? linhaNegocio, int? idTipoProduto)
		{
			return ListarPlacaDocumento(idTipoVeiculo, idCategoriaVeiculo, operacao, linhaNegocio, idTipoProduto, null);
		}
		public List<PlacaDocumentoView> ListarPlacaDocumento(int? idTipoVeiculo, int? idCategoriaVeiculo, string operacao, int? linhaNegocio, int? idTipoProduto, int? idTipoComposicao)
		{
			using (var repositorio = new UniCadDalRepositorio<PlacaDocumento>())
			{
				var query = GetQuery(repositorio, idTipoVeiculo, idCategoriaVeiculo, operacao, linhaNegocio, idTipoProduto, idTipoComposicao);

				return query.ToList();
			}
		}

		public List<PlacaDocumentoView> ListarPlacaDocumento(int? idTipoVeiculo, int? idCategoriaVeiculo, string operacao, int? linhaNegocio, int? idTipoProduto, int? idTipoComposicao, int? numero)
		{
			using (var repositorio = new UniCadDalRepositorio<PlacaDocumento>())
			{
				var query = GetQuery(repositorio, idTipoVeiculo, idCategoriaVeiculo, operacao, linhaNegocio, idTipoProduto, idTipoComposicao, numero);

				return query.ToList();
			}
		}

		private IQueryable<PlacaDocumentoView> GetPlacaDocumentView(UniCadDalRepositorio<PlacaDocumento> repositorio, int? idCategoriaVeiculo, string operacao, int? linhaNegocio, int? idTipoProduto, int? ordemPlaca)
		{
			var tipoDocProd = new TipoDocumentoTipoProdutoBusiness().Listar(p => !idTipoProduto.HasValue || p.IDTipoProduto == idTipoProduto).Select(p => p.IDTipoDocumento);

			switch (_pais)
			{
				default:
					return (from tipoDocumento in repositorio.ListComplex<TipoDocumento>().AsNoTracking()
							join tipoDocumentoTipoVeiculo in repositorio.ListComplex<TipoDocumentoTipoVeiculo>().AsNoTracking() on tipoDocumento.ID equals tipoDocumentoTipoVeiculo.IDTipoDocumento
							where ((!idCategoriaVeiculo.HasValue) || tipoDocumento.IDCategoriaVeiculo == idCategoriaVeiculo || idCategoriaVeiculo == (int)EnumCategoriaVeiculo.Todos || tipoDocumento.IDCategoriaVeiculo == (int)EnumCategoriaVeiculo.Todos)
							&& (string.IsNullOrEmpty(operacao) || tipoDocumento.Operacao == operacao || (operacao == "Ambos") || tipoDocumento.Operacao == "Ambos")
							&& (!linhaNegocio.HasValue || tipoDocumento.IDEmpresa == linhaNegocio || (linhaNegocio == (int)EnumEmpresa.Ambos) || tipoDocumento.IDEmpresa == (int)EnumEmpresa.Ambos)
							&& (!idTipoProduto.HasValue || tipoDocProd.Contains(tipoDocumento.ID))
							&& (tipoDocumento.Status)

							select new PlacaDocumentoView { Descricao = tipoDocumento.Descricao, Sigla = tipoDocumento.Sigla, IDTipoDocumento = tipoDocumento.ID, Obrigatorio = tipoDocumento.Obrigatorio, DocumentoPossuiVencimento = tipoDocumento.DocumentoPossuiVencimento }
						   ).Distinct();

				case EnumPais.Argentina:
					ordemPlaca = ordemPlaca ?? 0;

					return (from tipoDocumento in repositorio.ListComplex<TipoDocumento>().AsNoTracking()
							join tipoDocumentoTipoComposicao in repositorio.ListComplex<TipoDocumentoTipoComposicao>().AsNoTracking() on tipoDocumento.ID equals tipoDocumentoTipoComposicao.IDTipoDocumento
							where ((!idCategoriaVeiculo.HasValue) || tipoDocumento.IDCategoriaVeiculo == idCategoriaVeiculo || idCategoriaVeiculo == (int)EnumCategoriaVeiculo.Todos || tipoDocumento.IDCategoriaVeiculo == (int)EnumCategoriaVeiculo.Todos)
							&& (string.IsNullOrEmpty(operacao) || tipoDocumento.Operacao == operacao || (operacao == "Ambos") || tipoDocumento.Operacao == "Ambos")
							&& (!linhaNegocio.HasValue || tipoDocumento.IDEmpresa == linhaNegocio || (linhaNegocio == (int)EnumEmpresa.Ambos) || tipoDocumento.IDEmpresa == (int)EnumEmpresa.Ambos)
							&& (!idTipoProduto.HasValue || tipoDocProd.Contains(tipoDocumento.ID))
							&& (tipoDocumento.Status)
							&& ((ordemPlaca == 1 ? tipoDocumentoTipoComposicao.Placa1 :
								 ordemPlaca == 2 ? tipoDocumentoTipoComposicao.Placa2 :
								 ordemPlaca == 3 ? tipoDocumentoTipoComposicao.Placa3 :
								 ordemPlaca == 4 ? tipoDocumentoTipoComposicao.Placa4 : true)
								 ?? false)

							select new PlacaDocumentoView { Descricao = tipoDocumento.Descricao, Sigla = tipoDocumento.Sigla, IDTipoDocumento = tipoDocumento.ID, Obrigatorio = tipoDocumento.Obrigatorio, DocumentoPossuiVencimento = tipoDocumento.DocumentoPossuiVencimento }
						   ).Distinct();
			}
		}

		private IQueryable<PlacaDocumentoView> GetQuery(UniCadDalRepositorio<PlacaDocumento> repositorio, int? idTipoVeiculo, int? idCategoriaVeiculo, string operacao, int? linhaNegocio, int? idTipoProduto, int? idTipoComposicao)
		{
			return GetQuery(repositorio, idTipoVeiculo, idCategoriaVeiculo, operacao, linhaNegocio, idTipoProduto, idTipoComposicao, null);
		}

		private IQueryable<PlacaDocumentoView> GetQuery(UniCadDalRepositorio<PlacaDocumento> repositorio, int? idTipoVeiculo, int? idCategoriaVeiculo, string operacao, int? linhaNegocio, int? idTipoProduto, int? idTipoComposicao, int? numero)
		{
			IQueryable<PlacaDocumentoView> placaDocumentos = null;

			placaDocumentos = GetPlacaDocumentView(repositorio, idCategoriaVeiculo, operacao, linhaNegocio, idTipoProduto, numero);

			if (Equals(_pais, EnumPais.Brasil))
			{
				var tipoDocVeiculo = new TipoDocumentoTipoVeiculoBusiness()
					.Listar(p => !idTipoVeiculo.HasValue || p.IDTipoVeiculo == idTipoVeiculo)
					.Select(p => p.IDTipoDocumento);

				placaDocumentos = placaDocumentos.Where(p => !idTipoVeiculo.HasValue || tipoDocVeiculo.Contains(p.IDTipoDocumento));
			}
			else /* Argentina */
			{
				var tipoDocComposicao = new TipoDocumentoTipoComposicaoBusiness()
					.Listar(p => !idTipoComposicao.HasValue || p.IDTipoComposicao == idTipoComposicao)
					.Select(p => p.IDTipoDocumento);

				placaDocumentos = placaDocumentos.Where(p => !idTipoComposicao.HasValue || tipoDocComposicao.Contains(p.IDTipoDocumento));
			}

			return placaDocumentos;
		}
		
		public List<PlacaDocumentoServicoView> ListarPlacaDocumentoPorPlacaServico(int idPlaca)
		{
			using (UniCadDalRepositorio<PlacaDocumento> repositorio = new UniCadDalRepositorio<PlacaDocumento>())
			{
				var query = GetQueryServico(repositorio, idPlaca);

				return query.ToList();
			}
		}

		private IQueryable<PlacaDocumentoServicoView> GetQueryServico(UniCadDalRepositorio<PlacaDocumento> repositorio,
			int? idPlaca)
		{
			var placaDocumentos = from placaDocumento in repositorio.ListComplex<PlacaDocumento>().AsNoTracking()
								  join tipoDocumento in repositorio.ListComplex<TipoDocumento>().AsNoTracking() on placaDocumento.IDTipoDocumento equals tipoDocumento.ID
								  join placas in repositorio.ListComplex<Placa>().AsNoTracking() on placaDocumento.IDPlaca equals placas.ID
								  where (placaDocumento.IDPlaca == idPlaca || !idPlaca.HasValue)
								  select new PlacaDocumentoServicoView
								  {
									  Descricao = tipoDocumento.Descricao,
									  Sigla = tipoDocumento.Sigla,
									  DataVencimento = placaDocumento.DataVencimento
								  };

			return placaDocumentos;
		}

		public List<PlacaDocumentoView> ListarPlacaDocumentoPorPlaca(int idPlaca)
		{
			using (UniCadDalRepositorio<PlacaDocumento> repositorio = new UniCadDalRepositorio<PlacaDocumento>())
			{
				var query = GetQuery(repositorio, idPlaca, null);

				return query.ToList();
			}
		}

		private IQueryable<PlacaDocumentoView> GetQuery(UniCadDalRepositorio<PlacaDocumento> repositorio, int? idPlaca, string placa)
		{
			var placaDocumentos = from placaDocumento in repositorio.ListComplex<PlacaDocumento>().AsNoTracking()
								  join tipoDocumento in repositorio.ListComplex<TipoDocumento>().AsNoTracking() on placaDocumento.IDTipoDocumento equals tipoDocumento.ID
								  join placas in repositorio.ListComplex<Placa>().AsNoTracking() on placaDocumento.IDPlaca equals placas.ID
								  where (placaDocumento.IDPlaca == idPlaca || !idPlaca.HasValue)
								  && ((placas.PlacaVeiculo == placa && placas != null && placaDocumento != null) || string.IsNullOrEmpty(placa))
								  orderby placaDocumento.ID
                                  select new PlacaDocumentoView
								  {
									  ID = placaDocumento.ID,
									  IDTipoDocumento = tipoDocumento.ID,
									  Descricao = tipoDocumento.Descricao,
									  Sigla = tipoDocumento.Sigla,
									  Anexo = placaDocumento.Anexo,
									  Placa = placas.PlacaVeiculo,
									  Obrigatorio = tipoDocumento.Obrigatorio,
									  DocumentoPossuiVencimento = tipoDocumento.DocumentoPossuiVencimento,
									  DataVencimento = placaDocumento.DataVencimento,
									  Pendente = (placaDocumento.Bloqueado || placaDocumento.Vencido),
									  Processado = placaDocumento.Processado
								  };

			return placaDocumentos;

		}

		public List<PlacaDocumentoView> ListarPlacaDocumentoPorPlaca(int idPlaca, int ordemPlaca)
		{
			using (UniCadDalRepositorio<PlacaDocumento> repositorio = new UniCadDalRepositorio<PlacaDocumento>())
			{
				var query = GetQuery(repositorio, idPlaca, null, ordemPlaca);

				return query.ToList();
			}
		}

		private IQueryable<PlacaDocumentoView> GetQuery(UniCadDalRepositorio<PlacaDocumento> repositorio, int? idPlaca, string placa, int ordemPlaca)
		{
			var placaDocumentos = (from placaDocumento in repositorio.ListComplex<PlacaDocumento>().AsNoTracking()
								   join tipoDocumento in repositorio.ListComplex<TipoDocumento>().AsNoTracking() on placaDocumento.IDTipoDocumento equals tipoDocumento.ID
								   join placas in repositorio.ListComplex<Placa>().AsNoTracking() on placaDocumento.IDPlaca equals placas.ID
								   join tipoDocumentoTipoComposicao in repositorio.ListComplex<TipoDocumentoTipoComposicao>().AsNoTracking() on tipoDocumento.ID equals tipoDocumentoTipoComposicao.IDTipoDocumento
								   where (placaDocumento.IDPlaca == idPlaca || !idPlaca.HasValue)
								   && tipoDocumento.Status
								   && ((placas.PlacaVeiculo == placa && placas != null && placaDocumento != null) || string.IsNullOrEmpty(placa))
								   && ((ordemPlaca == 1 ? tipoDocumentoTipoComposicao.Placa1 :
									   ordemPlaca == 2 ? tipoDocumentoTipoComposicao.Placa2 :
									   ordemPlaca == 3 ? tipoDocumentoTipoComposicao.Placa3 :
									   ordemPlaca == 4 ? tipoDocumentoTipoComposicao.Placa4 : true)
									   ?? false)

								   orderby tipoDocumento.Sigla
								   select new PlacaDocumentoView
								   {
									   ID = placaDocumento.ID,
									   IDTipoDocumento = tipoDocumento.ID,
									   Descricao = tipoDocumento.Descricao,
									   Sigla = tipoDocumento.Sigla,
									   Anexo = placaDocumento.Anexo,
									   Placa = placas.PlacaVeiculo,
									   Obrigatorio = tipoDocumento.Obrigatorio,
									   DocumentoPossuiVencimento = tipoDocumento.DocumentoPossuiVencimento,
									   DataVencimento = placaDocumento.DataVencimento,
									   Pendente = (placaDocumento.Bloqueado || placaDocumento.Vencido)
								   }).Distinct();

			return placaDocumentos;

		}

		public List<PlacaDocumentoView> ListarAlteracoes(List<PlacaDocumentoView> documentoNovo, List<PlacaDocumentoView> documentoOficial)
		{
			List<PlacaDocumentoView> docs = new List<PlacaDocumentoView>();
			foreach (var doc in documentoNovo)
			{
				var docOficial = documentoOficial.FirstOrDefault(w => w.IDTipoDocumento == doc.IDTipoDocumento);
				if (docOficial != null)
				{
					doc.isDataVencimentoAlterada = doc.DataVencimento != docOficial.DataVencimento;
					doc.isAnexoAlterado = doc.Anexo != docOficial.Anexo;
				}

				docs.Add(doc);
			}
			return docs;
		}

		public void ExcluirListaDoc(int id)
		{
			var docs = Listar(p => p.IDPlaca == id).ToList();
			foreach (var item in docs)
			{
				ExcluirDoc(item.ID);
			}
		}

		public void ExcluirDoc(int id)
		{
			var arquivo = Selecionar(p => p.ID == id);
			ArquivoUtil.ExcluirArquivo(arquivo.Anexo, Config.GetConfig(EnumConfig.CaminhoAnexos, (int)EnumPais.Padrao));
			Excluir(id);
		}

		private void EnviarEmailDocumentosBloqueados(IEnumerable<PlacaDocumentoView> documento, string email, int idPais)
		{
			_logar?.Invoke("Escrevendo e-mail", "Escrevendo e-mail");

			var assunto = Config.GetConfig(EnumConfig.TituloEmailBloqueioAutomatico, idPais);
			var corpo = Config.GetConfig(EnumConfig.CorpoEmailBloqueioAutomatico, idPais);

			var resourceManager = new ResourceManager("Raizen.Unicad.BLL.Resources.Resource", Assembly.GetExecutingAssembly());
			var cultureInfo = CultureUtil.CultureInfoPorPais(idPais);
			var veiculoText = resourceManager.GetString(EnumResource.CorpoVeiculoEmailBloqueioReprovaAutomaticaComposicao.ToString(), cultureInfo);
			var veiculoSingularText = resourceManager.GetString(EnumResource.VeiculoSingular.ToString(), cultureInfo);
			var veiculoPluralText = resourceManager.GetString(EnumResource.VeiculoPlural.ToString(), cultureInfo);

			var corpoEmail = new StringBuilder();
			var veiculoDocumentos = new StringBuilder();

			var placaDocumentoViews = documento as PlacaDocumentoView[] ?? documento.ToArray();
			foreach (var placa in placaDocumentoViews.Select(x => new { x.Placa, x.TipoPlaca }).Distinct())
			{
				veiculoDocumentos.AppendFormat(String.Format(veiculoText, placa.Placa, placa.TipoPlaca.GetDescription()));
				foreach (var doc in placaDocumentoViews.Where(w => w.Placa == placa.Placa).Select(x => x.Documento).Distinct())
				{
					veiculoDocumentos.AppendLine("- " + doc + "<br/>");
				}
				veiculoDocumentos.Append("<br/>");
			}

			corpoEmail.AppendFormat(corpo, veiculoPluralText, veiculoDocumentos, veiculoSingularText);

			_logar?.Invoke("Enviando e-mail", "Chamada de Raizen.UniCad.BLL.Email.Enviar");
			Email.Enviar(email, assunto, corpoEmail.ToString(), null, true, _logar);

			//Se chegou até aqui significa que todos os documentos tiveram seu email enviado
			documento.ToList().ForEach(doc => doc.Enviado = true);
		}

		private void EnviarEmailDocumentosReprovados(IEnumerable<PlacaDocumentoView> documento, string email, int idPais)
		{
			_logar?.Invoke("Escrevendo e-mail", "Escrevendo e-mail");

			var assunto = Config.GetConfig(EnumConfig.TituloEmailReprovaAutomatica, idPais);
			var corpo = Config.GetConfig(EnumConfig.CorpoEmailReprovaAutomatica, idPais);

			var resourceManager = new ResourceManager("Raizen.Unicad.BLL.Resources.Resource", Assembly.GetExecutingAssembly());
			var cultureInfo = CultureUtil.CultureInfoPorPais(idPais);
			var veiculoText = resourceManager.GetString(EnumResource.CorpoVeiculoEmailBloqueioReprovaAutomaticaComposicao.ToString(), cultureInfo);
			var veiculoSingularText = resourceManager.GetString(EnumResource.VeiculoSingular.ToString(), cultureInfo);
			var veiculoPluralText = resourceManager.GetString(EnumResource.VeiculoPlural.ToString(), cultureInfo);

			var corpoEmail = new StringBuilder();
			var veiculoDocumentos = new StringBuilder();

			var placaDocumentoViews = documento as PlacaDocumentoView[] ?? documento.ToArray();
			foreach (var placa in placaDocumentoViews.Select(x => new { x.Placa, x.TipoPlaca }).Distinct())
			{
				veiculoDocumentos.AppendFormat(String.Format(veiculoText, placa.Placa, placa.TipoPlaca.GetDescription()));
				foreach (var doc in placaDocumentoViews.Where(w => w.Placa == placa.Placa).Select(x => x.Documento).Distinct())
				{
					veiculoDocumentos.AppendLine("- " + doc + "<br/>");
				}
				veiculoDocumentos.Append("<br/>");
			}

			corpoEmail.AppendFormat(corpo, veiculoPluralText, veiculoDocumentos, veiculoSingularText);

			_logar?.Invoke("Enviando e-mail", "Chamada de Raizen.UniCad.BLL.Email.Enviar");
			Email.Enviar(email, assunto, corpoEmail.ToString(), null, true, _logar);

			//Se chegou até aqui significa que todos os documentos tiveram seu email enviado
			documento.ToList().ForEach(doc => doc.Enviado = true);
		}

		public void EnviarEmailDeDocumentosVencidos(IEnumerable<PlacaDocumentoView> documento, string ibm, string razaoSocial, string email, int idPais)
		{
			_logar?.Invoke("Escrevendo e-mail", "Escrevendo e-mail");

			var assunto = Config.GetConfig(EnumConfig.TituloAlertaDocumentoComposicao, idPais);
			var corpoEmail = Config.GetConfig(EnumConfig.CorpoAlertaDocumentoComposicao, idPais);

			var resourceManager = new ResourceManager("Raizen.Unicad.BLL.Resources.Resource", Assembly.GetExecutingAssembly());
			var cultureInfo = CultureUtil.CultureInfoPorPais(idPais);
			var placaText = resourceManager.GetString(EnumResource.CorpoPlacaAlertaDocumentoComposicao.ToString(), cultureInfo);
			var documentoText = resourceManager.GetString(EnumResource.CorpoDocumentoAlertaDocumentoComposicao.ToString(), cultureInfo);
			var diasVencimentoText = resourceManager.GetString(EnumResource.CorpoDiasAcaoAlertaDocumentoComposicao.ToString(), cultureInfo);

			var corpoEmailDocs = new StringBuilder();
			var placaDocumentoViews = documento as PlacaDocumentoView[] ?? documento.ToArray();
			foreach (var placa in placaDocumentoViews.Select(x => x.Placa).Distinct())
			{
				corpoEmailDocs.AppendLine(String.Format(placaText, placa) + "<br/>");
				foreach (var doc in placaDocumentoViews.Where(w => w.Placa == placa).Select(x => new { x.Documento, x.DiasVencimento }).Distinct())
				{
					corpoEmailDocs.AppendLine(String.Format(documentoText, doc.Documento) + "<br/>");
					if (doc.DiasVencimento > 0)
						corpoEmailDocs.AppendLine(String.Format(diasVencimentoText, doc.DiasVencimento) + "<br/>");
				}
				corpoEmailDocs.AppendLine("<br/>");
			}

			var body = String.Format(corpoEmail, corpoEmailDocs);
			_logar?.Invoke("Enviando e-mail", "Chamada de Raizen.UniCad.BLL.Email.Enviar");
			Email.Enviar(email, assunto, body, null, true, _logar);

			//Se chegou até aqui significa que todos os documentos tiveram seu email enviado
			documento.ToList().ForEach(doc => doc.Enviado = true);
		}

		public void EnviarEmailAlerta(IEnumerable<PlacaDocumentoView> documento, string email, int idPais, DateTime? dataExecucao = null)
		{
			_logar?.Invoke("Escrevendo e-mail", "Escrevendo e-mail");

			var assunto = Config.GetConfig(EnumConfig.TituloVeiculoAlertaVencimento, idPais);
			var corpo = Config.GetConfig(EnumConfig.CorpoVeiculoAlertaVencimento, idPais);

			var resourceManager = new ResourceManager("Raizen.Unicad.BLL.Resources.Resource", Assembly.GetExecutingAssembly());
			var cultureInfo = CultureUtil.CultureInfoPorPais(idPais);
			var placaDiasVencimentoText = resourceManager.GetString(EnumResource.CorpoPlacaDiasVencimentoVeiculoAlertaVencimento.ToString(), cultureInfo);

			var corpoEmail = new StringBuilder();
			var veiculosDocumentos = new StringBuilder();

			var placaDocumentoViews = documento as PlacaDocumentoView[] ?? documento.ToArray();
			foreach (var placa in placaDocumentoViews.Select(x => new { x.Placa, x.TipoPlaca, x.DiasVencimento, x.DataVencimento, x.DiasVencimentoA2 }).OrderBy(o => o.Placa).ThenBy(t => t.DiasVencimento).Distinct())
			{
				bool useDiasVencimentoAlerta2 = dataExecucao != null && placa.DiasVencimentoA2 > 0 && placa.DataVencimento == dataExecucao.Value.AddDays(placa.DiasVencimentoA2);

				veiculosDocumentos.AppendFormat(String.Format(placaDiasVencimentoText, placa.Placa, placa.TipoPlaca.GetDescription(), useDiasVencimentoAlerta2 ? placa.DiasVencimentoA2 : placa.DiasVencimento));

				foreach (var doc in placaDocumentoViews.Where(w => w.Placa == placa.Placa &&
																(
																	(useDiasVencimentoAlerta2 && w.DiasVencimentoA2 == placa.DiasVencimentoA2) ||
																	(!useDiasVencimentoAlerta2 && w.DiasVencimento == placa.DiasVencimento))

																)
																.Select(x => new { x.Documento }).Distinct())
				{
					veiculosDocumentos.AppendLine("- " + doc.Documento + "<br/>");
				}

				veiculosDocumentos.Append("<br/>");
			}

			corpoEmail.AppendFormat(corpo, veiculosDocumentos);

			_logar?.Invoke("Enviando e-mail", "Chamada de Raizen.UniCad.BLL.Email.Enviar");
			Email.Enviar(email, assunto, corpoEmail.ToString(), null, true, _logar);

			//Se chegou até aqui significa que todos os documentos tiveram seu email enviado
			documento.ToList().ForEach(doc => doc.Enviado = true);
		}

		public int ProcessarAlertaDocumentosComposicao(DateTime dtInicioExecucao)
		{
			int erros = 0;
			var listaDocumentos = _placaDocumentoRepository.GetDocumentosAVencer(dtInicioExecucao);

			_logar?.Invoke(string.Format("ProcessarAlertaDocumentosComposicao"),
						   string.Format("Método recebe dados em GetDocumentosAVencer - count: {0}", listaDocumentos.Count()));

			foreach (var documentoEmail in listaDocumentos.Where(w => !String.IsNullOrEmpty(w.EmailEnviar) && w.QtdeAlertas > 0).Select(s => new { s.EmailEnviar, s.IdPais }).Distinct())
			{
				try
				{
					_logar?.Invoke("Iterando lista Emails", string.Format("EMAIL:{0}, PAÍS:{1}", documentoEmail.EmailEnviar, documentoEmail.IdPais));

					var documentos = listaDocumentos.Where(w => w.EmailEnviar == documentoEmail.EmailEnviar && w.IdPais == documentoEmail.IdPais);

					_logar?.Invoke("Enviar e-mail de alerta", string.Format("EMAIL:{0} - PAÍS {1} - QTDE DOCS: {2}",
								documentoEmail.EmailEnviar, documentoEmail.IdPais, documentos.Count()));

					EnviarEmailAlerta(documentos, documentoEmail.EmailEnviar, (int)documentoEmail.IdPais, dtInicioExecucao);

					foreach (var documento in documentos)
					{
						var placa = _placaDocumentoRepository.Selecionar(documento.ID);
						_logar?.Invoke(string.Format("ID {0}", documento.ID), "Placa selecionada.");

						if (documento.Alerta1Enviado == false && documento.DiasVencimento > 0 && documento.DataVencimento == dtInicioExecucao.AddDays(documento.DiasVencimento))
						{
							placa.Alerta1Enviado = true;
						}
						else if (documento.Alerta2Enviado == false && documento.DiasVencimentoA2 > 0 && documento.DataVencimento == dtInicioExecucao.AddDays(documento.DiasVencimentoA2))
						{
							placa.Alerta2Enviado = true;
						}

						_placaDocumentoRepository.Atualizar(placa);
						_logar?.Invoke(string.Format("ID {0}", documento.ID), string.Format("Placa marcada. Alerta1: {0} - Alerta2: {1}", placa.Alerta1Enviado, placa.Alerta2Enviado));
					}
				}
				catch
				{
					erros++;
				}
			}

			return erros;
		}

		public int ProcessarDocumentosVencidos(DateTime dtInicioExecucao)
		{
			_logar?.Invoke("PLACA DOCUMENTOS - INICIO", "-----");

			_logar?.Invoke(
				"Consulta - Documentos vencidos/bloqueados",
				$"Listando documentos vencidos/bloqueados, data de busca: {dtInicioExecucao}");

			var listaDocumentosBloqueadosVencidos = _placaDocumentoRepository.GetDocumentosBloqueados(dtInicioExecucao);

			_logar?.Invoke(
				"Consulta - Documentos vencidos/bloqueados",
				$"Documentos vencidos/bloqueados consultados, count: {listaDocumentosBloqueadosVencidos.Count()}");

			if (listaDocumentosBloqueadosVencidos.Any())
			{
				listaDocumentosBloqueadosVencidos.ForEach(tipoDocumento =>
				{
					tipoDocumento.TipoAlerta = 2;

					if (tipoDocumento.TipoBloqueioImediato == EnumTipoBloqueioImediato.Sim)
					{
						tipoDocumento.TipoAlerta = 1;
					}
					else if (tipoDocumento.TipoBloqueioImediato == EnumTipoBloqueioImediato.Nao &&
							 tipoDocumento.TipoAcaoVencimento != EnumTipoAcaoVencimento.SemAcao &&
							 tipoDocumento.DataVencimento.Value.AddDays(tipoDocumento.QuantidadeDiasBloqueio.Value) <= dtInicioExecucao)
					{
						tipoDocumento.TipoAlerta = 1;
					}

				});

				// ENVIAR OS E-MAILS AVISANDO QUE A PLACA SERÁ BLOQUEADA/REPROVADA.
				_logar?.Invoke("Processar documentos bloqueados/vencidos NV1 - Chamada", "Chamada de EnviarEmailDocumentosBloqueadosVencidos - Nível 1");
				EnviarEmailDocumentosBloqueadosVencidos(listaDocumentosBloqueadosVencidos, 1);

				// ENVIAR OS E-MAILS AVISANDO QUE O DOCUMENTO ESTÁ VENCIDO
				_logar?.Invoke("Processar documentos bloqueados/vencidos NV2 - Chamada", "Chamada de EnviarEmailDocumentosBloqueadosVencidos - Nível 2");
				EnviarEmailDocumentosBloqueadosVencidos(listaDocumentosBloqueadosVencidos, 2);

				// BLOQUEAR E/OU REPROVAR DOCUMENTOS
				_logar?.Invoke("Marcar documentos bloqueados e vencidos(sem ação)", "Após envio de e-mails, marcar documentos de placas bloqueadas e vencidos(sem ação).");

				MarcarDocumentosBloqueadosReprovados(listaDocumentosBloqueadosVencidos, EnumTipoAcaoVencimento.Bloquear, EnumConfig.habilitarBloqueioDocPlaca, true);

				_logar?.Invoke("Marcar documentos reprovados", "Após envio de e-mails, marcar documentos de placas reprovadas.");

				MarcarDocumentosBloqueadosReprovados(listaDocumentosBloqueadosVencidos, EnumTipoAcaoVencimento.Reprovar, EnumConfig.habilitarReprovaDocPlacaAutomatica, false);

				//BLOQUEAR COMPOSICAO
				MarcarComposicaoBloqueadosReprovados(
					listaDocumentosBloqueadosVencidos,
					EnumConfig.habilitarBloqueioPlaca,
					EnumTipoAcaoVencimento.Bloquear,
					EnumTipoIntegracaoSAP.Bloqueio,
					EnumStatusComposicao.Bloqueado,
					EnumConfig.JustificativaBloqueioAutomatico);

				// REPROVAR COMPOSIÇÃO
				MarcarComposicaoBloqueadosReprovados(
					listaDocumentosBloqueadosVencidos,
					EnumConfig.habilitarReprovaDocPlacaAutomatica,
					EnumTipoAcaoVencimento.Reprovar,
					EnumTipoIntegracaoSAP.Nenhum,
					EnumStatusComposicao.Reprovado,
					EnumConfig.JustificativaReprovaAutomatica);

			}

			_logar?.Invoke("PLACA DOCUMENTOS - FIM", "-----");

			return 0;
		}

		private void MarcarComposicaoBloqueadosReprovados(
			List<PlacaDocumentoView> listaDocumentosBloqueadosVencidos,
			EnumConfig habilitar,
			EnumTipoAcaoVencimento tipoAcaoVencimento,
			EnumTipoIntegracaoSAP tipoIntegracaoSap,
			EnumStatusComposicao statusComposicao,
			EnumConfig justificativaAutomatica)
		{
			if (_configBusiness.GetConfigInt(habilitar, (int)EnumPais.Padrao) != 0)
			{
				const string usuario = "JOB";

				string textoLog1 = null;
				string textoLog2 = null;
				string textoLog3 = null;
				string textoLog4 = null;

				switch (tipoAcaoVencimento)
				{
					case EnumTipoAcaoVencimento.Bloquear:
						textoLog1 = "Bloquear";
						textoLog2 = "bloqueada";
						textoLog3 = "Bloqueio";
						textoLog4 = "Bloqueando";
						break;
					case EnumTipoAcaoVencimento.Reprovar:
						textoLog1 = "Reprovar";
						textoLog2 = "reprovada";
						textoLog3 = "Reprovação";
						textoLog4 = "Reprovando";
						break;
					default:
						break;
				}

				var lista = listaDocumentosBloqueadosVencidos
					.Where(w => w.TipoAlerta == 1 && w.TipoAcaoVencimento == tipoAcaoVencimento)
					.Select(x => x.IDComposicao)
					.Distinct();

				_logar?.Invoke($"{textoLog1} composições", $"{textoLog1} composições - COUNT:{lista.Count()}");

				foreach (var item in lista)
				{
					var comp = _composicaoBusiness.Selecionar(item);

					_logar?.Invoke($"Marcar composições {textoLog2}s", $"Após envio de e-mails, marcar composição {textoLog2}.");

					if (comp != null)
					{
						_logar?.Invoke($"{textoLog3} - Composição {comp.ID}", $"{textoLog4}...");

						var placa = _placaBusiness.Selecionar((int)comp.IDPlaca1);

						if (tipoIntegracaoSap != EnumTipoIntegracaoSAP.Nenhum)
						{
							comp.tipoIntegracao = tipoIntegracaoSap;
						}

						comp.IDStatus = (int)statusComposicao;
						comp.UsuarioAlterouStatus = usuario;
						comp.Justificativa = _configBusiness.GetConfig(justificativaAutomatica, (int)placa.IDPais);
						comp.DataAtualizacao = DateTime.Now;

						(comp.IdPais == (int)EnumPais.Argentina ? _composicaoArgentinaBusiness : _composicaoBusiness)
							.AtualizarComposicao(
							comp,
							false,
							statusComposicao == EnumStatusComposicao.Bloqueado,
							idStatus: (int)statusComposicao);

						_logar?.Invoke($"{textoLog3} - Composição {comp.ID}", $"{textoLog2}.");

					}

				}

			}

		}

		private void MarcarDocumentosBloqueadosReprovados(List<PlacaDocumentoView> listaDocumentosBloqueadosVencidos, EnumTipoAcaoVencimento tipoAcaoVencimento, EnumConfig habilitarBloqueio, bool incluirSemAcao)
		{
			const string usuario = "JOB";

			var documentos = listaDocumentosBloqueadosVencidos
				.Where(w => w.TipoAcaoVencimento == tipoAcaoVencimento || (incluirSemAcao && w.TipoAcaoVencimento == EnumTipoAcaoVencimento.SemAcao))
				.Select(x => new { x.ID, x.TipoAlerta, x.TipoAcaoVencimento })
				.Distinct()
				.ToList();

			foreach (var item in documentos)
			{
				var placa = _placaDocumentoRepository.Selecionar(item.ID);

				if (_configBusiness.GetConfigInt(EnumConfig.habilitarVectoPlaca, (int)EnumPais.Padrao) != 0)
				{
					placa.Vencido = true;
				}

				if (item.TipoAcaoVencimento == (int)EnumTipoAcaoVencimento.SemAcao)
				{
					placa.Processado = true;
				}

				if (_configBusiness.GetConfigInt(habilitarBloqueio, (int)EnumPais.Padrao) != 0 && item.TipoAlerta == 1)
				{
					placa.Bloqueado = true;
					placa.TipoAcaoVencimento = (int)tipoAcaoVencimento;
					placa.UsuarioAlterouStatus = usuario;
					placa.Processado = true;
				}

				_placaDocumentoRepository.Atualizar(placa);

				_logar?.Invoke(
					string.Format("ID {0}", item),
					$"Placa marcada. Vencido: {placa.Vencido} - {tipoAcaoVencimento}: {placa.Bloqueado}");
			}
		}

		private void EnviarEmailDocumentosBloqueadosVencidos(IEnumerable<PlacaDocumentoView> listaDocumentosBloqueadosVencidos, int nivelAlerta)
		{
			_logar?.Invoke(
				$"EnviarEmailDocumentosBloqueadosVencidos - NV {nivelAlerta}",
				$"Método recebe dados em listaDocumentosBloqueadosVencidos - count: {listaDocumentosBloqueadosVencidos.Count()}");

			var listaDocumentoEmail = listaDocumentosBloqueadosVencidos
				.Where(w => !String.IsNullOrEmpty(w.EmailEnviar) && w.TipoAlerta == nivelAlerta && w.QtdeAlertas > 0)
				.Select(s => new { s.EmailEnviar, s.IdPais, s.TipoAcaoVencimento })
				.Distinct()
				.ToList();

			foreach (var documentoEmail in listaDocumentoEmail)
			{
				var documentos = listaDocumentosBloqueadosVencidos
					.Where(w => w.TipoAlerta == nivelAlerta
								&& w.EmailEnviar == documentoEmail.EmailEnviar
								&& w.IdPais == documentoEmail.IdPais
								&& w.TipoAcaoVencimento == documentoEmail.TipoAcaoVencimento
								&& w.QtdeAlertas > 0)
					.ToList();

				var msgLog = $"EMAIL:{documentoEmail.EmailEnviar} - QTDE DOCS: {documentos.Count}";

				switch (nivelAlerta)
				{
					case 2:
						_logar?.Invoke("Enviar e-mail de alerta Vencido", msgLog);
						EnviarEmailDeDocumentosVencidos(documentos, null, null, documentoEmail.EmailEnviar, (int)documentoEmail.IdPais);
						break;
					default:
						switch (documentoEmail.TipoAcaoVencimento)
						{
							case EnumTipoAcaoVencimento.Bloquear:
								_logar?.Invoke("Enviar e-mail de alerta Bloqueado", msgLog);
								EnviarEmailDocumentosBloqueados(documentos, documentoEmail.EmailEnviar, (int)documentoEmail.IdPais);
								break;
							case EnumTipoAcaoVencimento.Reprovar:
								_logar?.Invoke("Enviar e-mail de alerta Reprovação", msgLog);
								EnviarEmailDocumentosReprovados(documentos, documentoEmail.EmailEnviar, (int)documentoEmail.IdPais);
								break;
							default:
								break;
						}
						break;
				}
			}
		}
	}
}

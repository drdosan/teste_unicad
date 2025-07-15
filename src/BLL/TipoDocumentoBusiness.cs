using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using Raizen.Framework.Log.Bases;
using Raizen.Framework.Models;
using Raizen.Framework.Utils.Transacao;
using Raizen.UniCad.DAL;
using Raizen.UniCad.Model;
using Raizen.UniCad.Model.Filtro;
using Raizen.UniCad.Model.View;

namespace Raizen.UniCad.BLL
{
	public class TipoDocumentoBusiness : UniCadBusinessBase<TipoDocumento>
	{

		public List<TipoDocumentoView> ListarTipoDocumento(TipoDocumentoFiltro filtro, PaginadorModel paginador)
		{

			using (UniCadDalRepositorio<TipoDocumento> repositorio = new UniCadDalRepositorio<TipoDocumento>())
			{
				IQueryable<TipoDocumentoView> query = GetQueryTipoDocumento(filtro, repositorio)
														.Take(paginador.QtdeItensPagina * paginador.PaginaAtual)
														.OrderBy(i => i.Sigla)
														.Skip(unchecked((int)paginador.InicioPaginacao));
				return query.ToList();
			}

		}

		public int ListarTipoDocumentoCount(TipoDocumentoFiltro filtro)
		{

			using (UniCadDalRepositorio<TipoDocumento> repositorio = new UniCadDalRepositorio<TipoDocumento>())
			{
				IQueryable<TipoDocumentoView> query = GetQueryTipoDocumento(filtro, repositorio);
				return query.Count();
			}

		}


		private IQueryable<TipoDocumentoView> GetQueryTipoDocumento(TipoDocumentoFiltro filtro, IUniCadDalRepositorio<TipoDocumento> repositorio)
		{
			IQueryable<TipoDocumentoView> query = from app in repositorio.ListComplex<TipoDocumento>().AsNoTracking().OrderBy(i => i.Sigla)
												  where (app.Operacao.Contains(string.IsNullOrEmpty(filtro.Operacao) ? app.Operacao : filtro.Operacao))
												  && (app.Sigla.Contains(string.IsNullOrEmpty(filtro.Sigla) ? app.Sigla : filtro.Sigla))
												  && (app.Descricao.Contains(string.IsNullOrEmpty(filtro.Descricao) ? app.Descricao : filtro.Descricao))
												  && (app.Status == filtro.Status || !filtro.Status.HasValue)
												  && (app.IDEmpresa == filtro.IDEmpresa || !filtro.IDEmpresa.HasValue)
												  && (app.tipoCadastro == filtro.tipoCadastro || !filtro.tipoCadastro.HasValue)
												  && (app.IDPais == (int)(filtro.IDPais ?? 0) || !filtro.IDPais.HasValue)

												  select new TipoDocumentoView { ID = app.ID, Sigla = app.Sigla, Descricao = app.Descricao, Operacao = app.Operacao, Status = app.Status, DataAtualizacao = app.DataAtualizacao, IDPais = app.IDPais, Pais = (EnumPais)app.IDPais };
			return query;
		}



		public bool AdicionarTipoDocumento(TipoDocumento TipoDocumento)
		{
			using (TransactionScope transactionScope = Transactions.CreateTransactionScope(1, IsolationLevel.ReadCommitted))
			{
				using (var TipoDocumentoRepositorio = new UniCadDalRepositorio<TipoDocumento>())
				{
					TipoDocumento.DataAtualizacao = DateTime.Now;

					TipoDocumentoRepositorio.Add(TipoDocumento);

					var TipoProdutoBusiness = new TipoDocumentoTipoProdutoBusiness();

					if (TipoDocumento != null && TipoDocumento.TiposProduto != null)
						foreach (var TipoProduto in TipoDocumento.TiposProduto)
						{
							var tipoDocumentoTipoProduto = new TipoDocumentoTipoProduto
							{
								IDTipoProduto = TipoProduto.IDTipoProduto,
								IDTipoDocumento = TipoDocumento.ID
							};
							TipoProdutoBusiness.Adicionar(tipoDocumentoTipoProduto);
							TipoProduto.IDTipoDocumento = TipoDocumento.ID;
							TipoProduto.ID = tipoDocumentoTipoProduto.ID;
						}

					var TipoVeiculoBusiness = new TipoDocumentoTipoVeiculoBusiness();

					if (TipoDocumento != null && TipoDocumento.TiposVeiculo != null)
						foreach (var TipoVeiculo in TipoDocumento.TiposVeiculo)
						{
							var tipoDocumentoTipoVeiculo = new TipoDocumentoTipoVeiculo
							{
								IDTipoVeiculo = TipoVeiculo.IDTipoVeiculo,
								IDTipoDocumento = TipoDocumento.ID
							};

							TipoVeiculo.IDTipoDocumento = TipoDocumento.ID;
							TipoVeiculoBusiness.Adicionar(tipoDocumentoTipoVeiculo);
							TipoVeiculo.ID = tipoDocumentoTipoVeiculo.ID;
						}

					if (TipoDocumento != null && TipoDocumento.ComposicaoPlaca != null && TipoDocumento.ComposicaoPlaca.Any())
					{
						IEnumerable<IGrouping<int, TipoDocumentoTipoComposicaoPlacaView>> docsAgrupados = TipoDocumento.ComposicaoPlaca.GroupBy(x => x.IdComposicao);
						TipoDocumentoTipoComposicaoBusiness tipoDocumentoTipoComposicaoBusiness = new TipoDocumentoTipoComposicaoBusiness();
						foreach (IGrouping<int, TipoDocumentoTipoComposicaoPlacaView> doc in docsAgrupados)
						{
							TipoDocumentoTipoComposicao composicao = new TipoDocumentoTipoComposicao()
							{
								IDTipoComposicao = doc.Key,
								IDTipoDocumento = TipoDocumento.ID,
								Placa1 = doc.Any(x => x.IdPlaca == "Placa1"),
								Placa2 = doc.Any(x => x.IdPlaca == "Placa2"),
								Placa3 = doc.Any(x => x.IdPlaca == "Placa3"),
								Placa4 = doc.Any(x => x.IdPlaca == "Placa4")
							};
							tipoDocumentoTipoComposicaoBusiness.Adicionar(composicao);
						}
					}

					if (TipoDocumento != null && TipoDocumento.ComposicaoMotorista != null && TipoDocumento.ComposicaoMotorista.Any())
					{
						TipoDocumentoTipoComposicaoBusiness tipoDocumentoTipoComposicaoBusiness = new TipoDocumentoTipoComposicaoBusiness();
						foreach (TipoDocumentoTipoComposicaoPlacaView doc in TipoDocumento.ComposicaoMotorista)
						{
							TipoDocumentoTipoComposicao composicao = new TipoDocumentoTipoComposicao()
							{
								IDTipoComposicao = doc.IdComposicao,
								IDTipoDocumento = TipoDocumento.ID,
								Placa1 = false,
								Placa2 = false,
								Placa3 = false,
								Placa4 = false
							};
							tipoDocumentoTipoComposicaoBusiness.Adicionar(composicao);
						}
					}

					transactionScope.Complete();
					return true;
				}
			}
		}

		public bool AtualizarTipoDocumento(TipoDocumento TipoDocumento)
		{
			using (TransactionScope transactionScope = Transactions.CreateTransactionScope(1, IsolationLevel.ReadCommitted))
			{
				TipoDocumento.DataAtualizacao = DateTime.Now;

				Atualizar(TipoDocumento);

				var TipoDocumentoTipoProdutoBusiness = new TipoDocumentoTipoProdutoBusiness();
				var TipoDocumentoTipoProdutos = TipoDocumentoTipoProdutoBusiness.Listar(p => p.IDTipoDocumento == TipoDocumento.ID);

				foreach (var item in TipoDocumentoTipoProdutos)
				{
					if (TipoDocumento.TiposProduto == null || !TipoDocumento.TiposProduto.Any(p => p.ID == item.ID))
					{

						TipoDocumentoTipoProdutoBusiness.Excluir(item.ID);
					}
				}

				if (TipoDocumento.TiposProduto != null)
					foreach (var item in TipoDocumento.TiposProduto)
					{
						item.IDTipoDocumento = TipoDocumento.ID;

						if (item.ID == 0)
						{
							TipoDocumentoTipoProdutoBusiness.Adicionar(new TipoDocumentoTipoProduto { IDTipoProduto = item.IDTipoProduto, IDTipoDocumento = TipoDocumento.ID });
						}
						else
						{
							TipoDocumentoTipoProdutoBusiness.Atualizar(new TipoDocumentoTipoProduto { IDTipoProduto = item.IDTipoProduto, IDTipoDocumento = TipoDocumento.ID, ID = item.ID });
						}
					}

				var TipoDocumentoTipoVeiculoBusiness = new TipoDocumentoTipoVeiculoBusiness();
				var TipoDocumentoTipoVeiculos = TipoDocumentoTipoVeiculoBusiness.Listar(p => p.IDTipoDocumento == TipoDocumento.ID);

				foreach (var item in TipoDocumentoTipoVeiculos)
				{
					if (TipoDocumento.TiposVeiculo == null || !TipoDocumento.TiposVeiculo.Any(p => p.ID == item.ID))
					{

						TipoDocumentoTipoVeiculoBusiness.Excluir(item.ID);
					}
				}

				if (TipoDocumento.TiposVeiculo != null)
					foreach (var item in TipoDocumento.TiposVeiculo)
					{
						item.IDTipoDocumento = TipoDocumento.ID;

						if (item.ID == 0)
						{
							TipoDocumentoTipoVeiculoBusiness.Adicionar(new TipoDocumentoTipoVeiculo { IDTipoVeiculo = item.IDTipoVeiculo, IDTipoDocumento = TipoDocumento.ID });
						}
						else
						{
							TipoDocumentoTipoVeiculoBusiness.Atualizar(new TipoDocumentoTipoVeiculo { IDTipoVeiculo = item.IDTipoVeiculo, IDTipoDocumento = TipoDocumento.ID, ID = item.ID });
						}
					}
				if (TipoDocumento.ComposicaoPlaca == null)
					TipoDocumento.ComposicaoPlaca = new List<TipoDocumentoTipoComposicaoPlacaView>();

				if (TipoDocumento != null && TipoDocumento.ComposicaoPlaca != null)
				{
					IEnumerable<IGrouping<int, TipoDocumentoTipoComposicaoPlacaView>> docsAgrupados = TipoDocumento.ComposicaoPlaca.GroupBy(x => x.IdComposicao);
					TipoDocumentoTipoComposicaoBusiness tipoDocumentoTipoComposicaoBusiness = new TipoDocumentoTipoComposicaoBusiness();

					IList<TipoDocumentoTipoComposicao> original = tipoDocumentoTipoComposicaoBusiness.Listar(x => x.IDTipoDocumento == TipoDocumento.ID);

					foreach (IGrouping<int, TipoDocumentoTipoComposicaoPlacaView> doc in docsAgrupados)
					{
						TipoDocumentoTipoComposicao composicao = original.FirstOrDefault(x => x.IDTipoComposicao == doc.Key);
						if (composicao == null)
						{
							composicao = new TipoDocumentoTipoComposicao();
							composicao.IDTipoComposicao = doc.Key;
							composicao.IDTipoDocumento = TipoDocumento.ID;
							composicao.Placa1 = doc.Any(x => x.IdPlaca == "Placa1");
							composicao.Placa2 = doc.Any(x => x.IdPlaca == "Placa2");
							composicao.Placa3 = doc.Any(x => x.IdPlaca == "Placa3");
							composicao.Placa4 = doc.Any(x => x.IdPlaca == "Placa4");
							tipoDocumentoTipoComposicaoBusiness.Adicionar(composicao);
						}
						else
						{
							composicao.Placa1 = doc.Any(x => x.IdPlaca == "Placa1");
							composicao.Placa2 = doc.Any(x => x.IdPlaca == "Placa2");
							composicao.Placa3 = doc.Any(x => x.IdPlaca == "Placa3");
							composicao.Placa4 = doc.Any(x => x.IdPlaca == "Placa4");
							tipoDocumentoTipoComposicaoBusiness.Atualizar(composicao);
						}
					}

					IList<int> toDelete = original.Where(x => !TipoDocumento.ComposicaoPlaca.Select(c => c.IdComposicao).Contains(x.IDTipoComposicao)).Select(x => x.IDTipoComposicao).ToList();

					tipoDocumentoTipoComposicaoBusiness.ExcluirLista(x => x.IDTipoDocumento == TipoDocumento.ID && toDelete.Contains(x.IDTipoComposicao));
				}

				if (TipoDocumento != null && TipoDocumento.ComposicaoMotorista != null && TipoDocumento.ComposicaoMotorista.Any())
				{
					TipoDocumentoTipoComposicaoBusiness tipoDocumentoTipoComposicaoBusiness = new TipoDocumentoTipoComposicaoBusiness();

					IList<TipoDocumentoTipoComposicao> original = tipoDocumentoTipoComposicaoBusiness.Listar(x => x.IDTipoDocumento == TipoDocumento.ID);

					foreach (TipoDocumentoTipoComposicaoPlacaView doc in TipoDocumento.ComposicaoMotorista)
					{
						TipoDocumentoTipoComposicao composicao = original.FirstOrDefault(x => x.IDTipoComposicao == doc.IdComposicao);
						if (composicao == null)
						{
							composicao = new TipoDocumentoTipoComposicao();
							composicao.IDTipoComposicao = doc.IdComposicao;
							composicao.IDTipoDocumento = TipoDocumento.ID;
							composicao.Placa1 = false;
							composicao.Placa2 = false;
							composicao.Placa3 = false;
							composicao.Placa4 = false;
							tipoDocumentoTipoComposicaoBusiness.Adicionar(composicao);
						}
						else
						{
							composicao.Placa1 = false;
							composicao.Placa2 = false;
							composicao.Placa3 = false;
							composicao.Placa4 = false;
							tipoDocumentoTipoComposicaoBusiness.Atualizar(composicao);
						}
					}

					IList<int> toDelete = original.Where(x => !TipoDocumento.ComposicaoMotorista.Select(c => c.IdComposicao).Contains(x.IDTipoComposicao)).Select(x => x.IDTipoComposicao).ToList();

					tipoDocumentoTipoComposicaoBusiness.ExcluirLista(x => x.IDTipoDocumento == TipoDocumento.ID && toDelete.Contains(x.IDTipoComposicao));
				}

				transactionScope.Complete();
			}
			return true;
		}
	}
}


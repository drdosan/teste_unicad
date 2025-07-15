using System;
using System.Collections.Generic;
using System.Linq;
using Raizen.Framework.Log.Bases;
using Raizen.Framework.Models;
using Raizen.UniCad.DAL;
using Raizen.UniCad.Model;
using Raizen.UniCad.Model.Filtro;
using Raizen.UniCad.Model.View;

namespace Raizen.UniCad.BLL
{
    public class ProdutoBusiness : UniCadBusinessBase<Produto>
    {

        public List<ProdutoView> ListarProduto(ProdutoFiltro filtro, PaginadorModel paginador)
        {

                using (UniCadDalRepositorio<Produto> repositorio = new UniCadDalRepositorio<Produto>())
                {
                    IQueryable<ProdutoView> query = GetQueryProduto(filtro, repositorio)
                                                            .Take(paginador.QtdeItensPagina * paginador.PaginaAtual)
                                                            .OrderBy(i => i.Nome)
                                                            .Skip(unchecked((int)paginador.InicioPaginacao));
                    return query.ToList();
                }

        }

        public int ListarProdutoCount(ProdutoFiltro filtro)
        {

                using (UniCadDalRepositorio<Produto> repositorio = new UniCadDalRepositorio<Produto>())
                {
                    IQueryable<ProdutoView> query = GetQueryProduto(filtro, repositorio);
                    return query.Count();
                }

        }

        private IQueryable<ProdutoView> GetQueryProduto(ProdutoFiltro filtro, IUniCadDalRepositorio<Produto> repositorio)
        {
            IQueryable<ProdutoView> query = (from app in repositorio.ListComplex<Produto>().AsNoTracking().OrderBy(i => i.Nome)
                                         join tipo in repositorio.ListComplex<TipoProduto>().AsNoTracking() on app.IDTipoProduto equals tipo.ID
                                            where (app.Nome.Contains(string.IsNullOrEmpty(filtro.Nome) ? app.Nome : filtro.Nome))
                                            && (app.Codigo.Contains(filtro.Codigo) || string.IsNullOrEmpty(filtro.Codigo))
                                            && (app.Status == filtro.Status || !filtro.Status.HasValue)
                                            && (filtro.IDTipoProduto == 0 || !filtro.IDTipoProduto.HasValue || (app.IDTipoProduto == filtro.IDTipoProduto))
                                            && (filtro.Pais == 0 || !filtro.Pais.HasValue || (tipo.Pais == filtro.Pais))
                                            select new ProdutoView { ID = app.ID, Codigo = app.Codigo, Densidade = app.Densidade, Status = app.Status, Nome = app.Nome, TipoProduto = tipo.Nome, Pais = tipo.Pais.ToString() });
            return query;
        }
    }
}


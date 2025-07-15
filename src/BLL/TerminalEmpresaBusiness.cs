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
    public class TerminalEmpresaBusiness : UniCadBusinessBase<TerminalEmpresa>
    {

        public List<TerminalEmpresa> ListarTerminalEmpresa(TerminalEmpresaFiltro filtro, PaginadorModel paginador)
        {


                using (UniCadDalRepositorio<TerminalEmpresa> repositorio = new UniCadDalRepositorio<TerminalEmpresa>())
                {
                    IQueryable<TerminalEmpresa> query = GetQueryTerminalEmpresa(filtro, repositorio)
                                                            .Take(paginador.QtdeItensPagina * paginador.PaginaAtual)
                                                            .OrderBy(i => i.Nome)
                                                            .Skip(unchecked((int)paginador.InicioPaginacao));
                    return query.ToList();
                }

        }

        public int ListarTerminalEmpresaCount(TerminalEmpresaFiltro filtro)
        {

                using (UniCadDalRepositorio<TerminalEmpresa> repositorio = new UniCadDalRepositorio<TerminalEmpresa>())
                {
                    IQueryable<TerminalEmpresa> query = GetQueryTerminalEmpresa(filtro, repositorio);
                    return query.Count();
                }

        }

        private IQueryable<TerminalEmpresa> GetQueryTerminalEmpresa(TerminalEmpresaFiltro filtro, IUniCadDalRepositorio<TerminalEmpresa> repositorio)
        {
            IQueryable<TerminalEmpresa> query = (from app in repositorio.ListComplex<TerminalEmpresa>().AsNoTracking().OrderBy(i => i.Nome)
                                          where (app.Nome.Contains(string.IsNullOrEmpty(filtro.Nome) ? app.Nome : filtro.Nome))
                                          && (app.ID == filtro.ID || !filtro.ID.HasValue)
                                          select app);
            return query;
        }
    }
}


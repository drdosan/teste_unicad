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
    public class TipoAgendaBusiness : UniCadBusinessBase<TipoAgenda>
    {

        public List<TipoAgendaView> ListarTipoAgenda(TipoAgendaFiltro filtro, PaginadorModel paginador)
        {

                using (UniCadDalRepositorio<TipoAgenda> repositorio = new UniCadDalRepositorio<TipoAgenda>())
                {
                    IQueryable<TipoAgendaView> query = GetQueryTipoAgenda(filtro, repositorio)
                                                            .Take(paginador.QtdeItensPagina * paginador.PaginaAtual)
                                                            .OrderBy(i => i.Nome)
                                                            .Skip(unchecked((int)paginador.InicioPaginacao));
                    return query.ToList();
                }

        }

        public int ListarTipoAgendaCount(TipoAgendaFiltro filtro)
        {

                using (UniCadDalRepositorio<TipoAgenda> repositorio = new UniCadDalRepositorio<TipoAgenda>())
                {
                    IQueryable<TipoAgendaView> query = GetQueryTipoAgenda(filtro, repositorio);
                    return query.Count();
                }

        }

        private IQueryable<TipoAgendaView> GetQueryTipoAgenda(TipoAgendaFiltro filtro, IUniCadDalRepositorio<TipoAgenda> repositorio)
        {
            IQueryable<TipoAgendaView> query = (from app in repositorio.ListComplex<TipoAgenda>().AsNoTracking().OrderBy(i => i.Nome)
                                            where (app.Nome.Contains(string.IsNullOrEmpty(filtro.Nome) ? app.Nome : filtro.Nome))
                                            && (app.Status == filtro.Status || !filtro.Status.HasValue)
                                            && (app.IDTipo == filtro.IDTipo || !filtro.IDTipo.HasValue)
                                             select new TipoAgendaView { ID = app.ID, IDTipo = app.IDTipo, Anexo = app.Anexo, Status = app.Status, Nome = app.Nome });
            return query;
        }
    }
}


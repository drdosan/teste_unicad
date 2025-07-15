using System.Collections.Generic;
using System.Linq;
using Raizen.Framework.Models;
using Raizen.UniCad.DAL;
using Raizen.UniCad.Model;
using Raizen.UniCad.Model.Filtro;

namespace Raizen.UniCad.BLL
{
    public class ConfiguracaoBusiness : UniCadBusinessBase<Configuracao>
    {
        public List<Configuracao> ListarConfiguracao(ConfiguracaoFiltro filtro, PaginadorModel paginador)
        {

            using (UniCadDalRepositorio<Configuracao> repositorio = new UniCadDalRepositorio<Configuracao>())
            {
                IQueryable<Configuracao> query = GetQueryConfiguracao(filtro, repositorio)
                                                        .Take(paginador.QtdeItensPagina * paginador.PaginaAtual)
                                                        .OrderBy(i => i.ID)
                                                        .Skip(unchecked((int)paginador.InicioPaginacao));
                return query.ToList();
            }

        }

        public int ListarConfiguracaoCount(ConfiguracaoFiltro filtro)
        {

            using (UniCadDalRepositorio<Configuracao> repositorio = new UniCadDalRepositorio<Configuracao>())
            {
                IQueryable<Configuracao> query = GetQueryConfiguracao(filtro, repositorio);
                return query.Count();
            }

        }

        private IQueryable<Configuracao> GetQueryConfiguracao(ConfiguracaoFiltro filtro, IUniCadDalRepositorio<Configuracao> repositorio)
        {
            IQueryable<Configuracao> query = repositorio.ListComplex<Configuracao>().AsNoTracking()
                                             .Include("Pais")
                                             .Where(app => app.NmVariavel.Contains(string.IsNullOrEmpty(filtro.NmVariavel) ? app.NmVariavel : filtro.NmVariavel)
                                             && (filtro.IdPais == 0 || filtro.IdPais == null || filtro.IdPais == app.IdPais))
                                             .OrderBy(i => i.ID);
            return query;

        }
    }
}

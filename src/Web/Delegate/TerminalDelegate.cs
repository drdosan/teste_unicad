using Raizen.UniCad.BLL;
using Raizen.UniCad.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Raizen.UniCad
{
    public class TerminalDelegate
    {
        public static List<Terminal> Listar()
        {
            var lista = new List<Terminal>();
            var dataHoje =DateTime.Now.Date;
            var listaIds = new AgendamentoTerminalBusiness().Listar(p => p.Ativo && p.Data >= dataHoje).Select(p => p.IDTerminal);
            var transp = new TerminalBusiness().Listar(p => listaIds.Contains(p.ID)).OrderBy(o => o.Nome).ToList();
            transp.ForEach(p => p.Nome = p.Sigla + " - " + p.Nome + (p.isPool ? " - POOL" : string.Empty));
            lista.AddRange(transp);

            return lista;
        }

    }
}
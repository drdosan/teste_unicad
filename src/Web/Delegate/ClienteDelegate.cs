using Raizen.UniCad.BLL;
using Raizen.UniCad.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Raizen.UniCad
{
    public class ClienteDelegate
    {
        public static List<Cliente> Listar()
        {
            var lista = new List<Cliente>();
            var transp = new ClienteBusiness().Listar().OrderBy(o => o.RazaoSocial).ToList();
            transp.ForEach(p => p.RazaoSocial = p.IBM + " - " + p.RazaoSocial);
            lista.AddRange(transp);

            return lista;
        }

    }
}
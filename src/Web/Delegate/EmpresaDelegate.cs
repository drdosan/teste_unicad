using Raizen.UniCad.BLL;
using Raizen.UniCad.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Raizen.UniCad
{ 
    public class EmpresaDelegate
    {
        public static List<Empresa> Listar()
        {
            var lista = new List<Empresa>();
            lista.AddRange(new EmpresaBusiness().Listar());

            return lista;
        }
    }
}
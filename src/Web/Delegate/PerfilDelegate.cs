using Raizen.UniCad.BLL;
using Raizen.UniCad.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Raizen.UniCad
{ 
    public class PerfilDelegate
    {
        public static List<Perfil> Listar(bool todos = false)
        {
            var lista = new List<Perfil>();
            lista.AddRange(new PerfilBusiness().ListarUserSystem(todos));

            return lista;
        }
    }
}
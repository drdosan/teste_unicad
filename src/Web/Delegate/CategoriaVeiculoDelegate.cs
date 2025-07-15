using Raizen.UniCad.BLL;
using Raizen.UniCad.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Raizen.UniCad
{ 
    public class CategoriaVeiculoDelegate
    {
        public static List<CategoriaVeiculo> Listar(bool todos = false)
        {
            var lista = new List<CategoriaVeiculo>();
            lista.AddRange(new CategoriaVeiculoBusiness().Listar(p => p.ID != 3 || todos));

            return lista;
        }

        public static List<CategoriaVeiculo> ListarVeiculo(bool todos = false)
        {
            var lista = new List<CategoriaVeiculo>();
            lista.AddRange(new CategoriaVeiculoBusiness().Listar(p => p.Tipo == 1 && (p.ID!= 3 || todos)));

            return lista;
        }
    }
}
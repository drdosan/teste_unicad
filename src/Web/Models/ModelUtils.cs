using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Raizen.Framework.Models;
using Raizen.Framework.Web.MVC.Utils;

namespace Raizen.UniCad.Web.Models
{
    public static class ModelUtils
    {
        public static List<FlatDados<int>> ListarConjuntoPaginas()
        {
            List<FlatDados<int>> dados = new List<FlatDados<int>>();
            dados.Add(new FlatDados<int>() { Nome = "10", Valor = 10 });
            dados.Add(new FlatDados<int>() { Nome = "15", Valor = 15 });
            dados.Add(new FlatDados<int>() { Nome = "20", Valor = 20 });
            dados.Add(new FlatDados<int>() { Nome = "25", Valor = 25 });
            dados.Add(new FlatDados<int>() { Nome = "30", Valor = 30 });
            dados.Add(new FlatDados<int>() { Nome = "35", Valor = 35 });
            dados.Add(new FlatDados<int>() { Nome = "40", Valor = 40 });
            dados.Add(new FlatDados<int>() { Nome = "45", Valor = 45 });
            dados.Add(new FlatDados<int>() { Nome = "50", Valor = 50 });
            dados.Add(new FlatDados<int>() { Nome = "60", Valor = 60 });
            dados.Add(new FlatDados<int>() { Nome = "70", Valor = 70 });
            dados.Add(new FlatDados<int>() { Nome = "80", Valor = 80 });
            dados.Add(new FlatDados<int>() { Nome = "90", Valor = 90 });
            dados.Add(new FlatDados<int>() { Nome = "100", Valor = 100 });

            return dados;
        }

        public static PaginadorModel IniciarPaginador(PaginadorModel PaginadorDados, Int64 qtdeRegistros, TipoPaginador tipoPaginador = TipoPaginador.Modelo_Linq, List<FlatDados<int>> conjuntoPaginas = null)
        {
            int qtdeItensPorPaginaAtual = 0;
            if (PaginadorDados == null)
            {
                PaginadorDados = new PaginadorModel();
            }
            else
            {
                qtdeItensPorPaginaAtual = PaginadorDados.QtdeItensPagina;
            }

            if (conjuntoPaginas == null)
            {
                PaginadorDados.ConjuntoPaginas = ListarConjuntoPaginas();
            }
            else
            {
                PaginadorDados.ConjuntoPaginas = conjuntoPaginas;
            }

            PaginadorDados.Status = EstadoPaginador.RenovandoConsulta;
            PaginadorDados.QtdeTotalRegistros = qtdeRegistros;
            PaginadorDados.QtdeItensPagina = (qtdeItensPorPaginaAtual > PaginadorDados.ConjuntoPaginas[0].Valor ? PaginadorDados.QtdeItensPagina : PaginadorDados.ConjuntoPaginas[0].Valor);
            PaginadorDados.PaginaAtual = 1;

            Paginador.CalcularPaginas(PaginadorDados, tipoPaginador);

            return PaginadorDados;
        }
    }    
}
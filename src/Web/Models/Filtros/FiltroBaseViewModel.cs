using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Raizen.UniCad.Web.Models.Filtros
{
    public abstract class FiltroBaseViewModel
    {
        protected FiltroBaseViewModel()
        {
            PaginaAtual = 1;
            TamanhoItensPagina = 50;
        }

        public int PaginaAtual { get; set; }
        public int TamanhoItensPagina { get; set; }
    }
}
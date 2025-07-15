
using System;
using System.Collections.Generic;
using Raizen.UniCad.Model;

namespace Raizen.UniCad.BLL
  {
    public class TipoComposicaoBusiness : UniCadBusinessBase<TipoComposicao>
    {
        public List<TipoComposicao> ListarPorPais(int pais)
        {
            return base.Listar(x => x.IdPais == pais);
        }
    }
}
  

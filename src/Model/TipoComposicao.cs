using System.Collections.Generic;
using Raizen.UniCad.Model.Base;

  namespace Raizen.UniCad.Model
  {
    public class TipoComposicao : TipoComposicaoBase
    {
        public List<TipoDocumentoTipoComposicao> TipoDocumentoTipoComposicao { get; set; }
    }
  }
  

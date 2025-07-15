using System.Collections.Generic;
using Raizen.UniCad.Model.Base;
using System.ComponentModel.DataAnnotations.Schema;
using Raizen.UniCad.Model.View;

namespace Raizen.UniCad.Model
  {
    public class TipoDocumento : TipoDocumentoBase
    {
        [NotMapped]
        public List<TipoDocumentoTipoProdutoView> TiposProduto { get; set; }

        [NotMapped]
        public List<TipoDocumentoTipoVeiculoView> TiposVeiculo { get; set; }

        [NotMapped]
        public List<TipoComposicao> TiposComposicao { get; set; }

        [NotMapped]
        public List<TipoDocumentoTipoComposicaoPlacaView> ComposicaoPlaca { get; set; }
        
        [NotMapped]
        public List<TipoDocumentoTipoComposicaoPlacaView> ComposicaoMotorista { get; set; }
        public List<TipoDocumentoTipoComposicao> TipoDocumentoTipoComposicao { get; set; }
    }
  }
  

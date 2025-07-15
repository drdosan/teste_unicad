using Raizen.UniCad.Model.Base;

namespace Raizen.UniCad.Model
{
    public class TipoDocumentoTipoComposicao : TipoDocumentoTipoComposicaoBase
    {
        public virtual TipoComposicao TipoComposicao { get; set; }
        public virtual TipoDocumento TipoDocumento { get; set; }
    }
}

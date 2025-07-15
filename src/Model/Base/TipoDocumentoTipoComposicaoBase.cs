using Raizen.Framework.Models;

namespace Raizen.UniCad.Model.Base
{
    public abstract class TipoDocumentoTipoComposicaoBase : BaseModel
    {
        public int ID { get; set; }
        public int IDTipoComposicao { get; set; }
        public int IDTipoDocumento { get; set; }
        public bool? Placa1 { get; set; }
        public bool? Placa2 { get; set; }
        public bool? Placa3 { get; set; }
        public bool? Placa4 { get; set; }
    }
}

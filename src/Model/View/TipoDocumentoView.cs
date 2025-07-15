using System;

namespace Raizen.UniCad.Model.View
{
    public class TipoDocumentoView
    {
        public virtual Int32 ID { get; set; }
        public virtual string Sigla { get; set; }
        public virtual string Descricao { get; set; }
        public virtual bool Status { get; set; }
        public virtual string Operacao { get; set; }
        public virtual DateTime DataAtualizacao { get; set; }
		public virtual int IDPais { get; set; }
		public virtual EnumPais Pais { get; set; }
    }
}
  


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raizen.UniCad.Model.Filtro
{
    public class SincronizacaoMotoristasFiltro
    {
        public virtual bool? IsOk { get; set; }
        public virtual string Mensagem { get; set; }

        public string CNH { get; set; }
        public string CPF { get; set; }
        public string RG { get; set; }
        public DateTime? DataInicio { get; set; }
        public DateTime? DataFim { get; set; }
    }
}

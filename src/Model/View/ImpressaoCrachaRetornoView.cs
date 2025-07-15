using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raizen.UniCad.Model.View
{
    public class ImpressaoCrachaRetornoView
    {
        public int IdMotorista { get; set; }
        public string NomeMotorista { get; set; }
        public string MensagemSituacao { get; set; }
        public bool AptoParaImpressaoDeCracha { get; set; }
        public string Justificativa { get; set; }
        public int IDStatus { get; set; }
        public List<MotoristaDocumentoView> Documentos { get; set; }


    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raizen.UniCad.Model.View
{
    public class TreinamentoView
    {
        public int IDMotorista { get; set; }
        public string Anexo { get; set; }
        public bool? TreinamentoAprovado { get; set; }
        public DateTime? dataValidade { get; set; }
        public string Justificativa { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raizen.UniCad.Model.View
{
    public class AgendamentoTreinamentoRetornoView
    {
        public int IdMotorista { get; set; }
        public string NomeMotorista { get; set; }
        public string Situacao { get; set; }
        public string DataValidadeAgendamento { get; set; }
    }
}

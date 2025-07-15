using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raizen.UniCad.Model.View
{
    public class ControlePresencaMotoristaView
    {
        public int idMotorista { get; set; }
        public int idAgendamentoTreinamento { get; set; }

        public int idAgendamentoHorario { get; set; }
        public virtual string Horario { get { return $"{HoraInicio:hh\\:mm}:{HoraFim:hh\\:mm}"; } }
        public TimeSpan HoraInicio { get; set; }
        public TimeSpan HoraFim { get; set; }
        public string LinhaNegocio { get; set; }
        public string Operacao { get; set; }
        public string Cpf { get; set; }
        public string Nome { get; set; }
        public int? Situacao { get; set; }
        public string Usuario { get; set; }
        public DateTime Data { get; set; }
        public int IDEmpresaCongenere { get; set; }
    }
}

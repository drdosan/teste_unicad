
using System.Collections.Generic;
using Raizen.UniCad.Model.Base;
using System.ComponentModel.DataAnnotations.Schema;
using Raizen.UniCad.Model.View;

namespace Raizen.UniCad.Model
{
    public class AgendamentoTerminal : AgendamentoTerminalBase
    {
        [NotMapped]
        public int IDEmpresa { get; set; }
        [NotMapped]
        public List<AgendamentoTerminalHorarioView> ListaAgendamentoTerminalHorario { get; set; }
        [NotMapped]
        public string Operacao { get; set; }
    }
}


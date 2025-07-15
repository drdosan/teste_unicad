
using Raizen.UniCad.Model.Base;
using Raizen.UniCad.Model.View;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Raizen.UniCad.Model
{
    public class AgendamentoTerminalHorario : AgendamentoTerminalHorarioBase
    {
        [NotMapped]
        public int IDEmpresaUsuario { get; set; }
        [NotMapped]
        public int idHoraAgenda { get; set; }
        [NotMapped]
        public string OperacaoUsuario { get; set; }
    }
}


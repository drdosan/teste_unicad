using System.Collections.Generic;
using Raizen.UniCad.Model.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace Raizen.UniCad.Model
{
    public class Terminal : TerminalBase
    {
        [NotMapped]
        public List<TerminalEmpresa> TerminalEmpresa {get;set; }
    }
}
  

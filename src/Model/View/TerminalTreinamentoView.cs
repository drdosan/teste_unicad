using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raizen.UniCad.Model.View
{
    public class TerminalTreinamentoView
    {
        public int ID { get; set; }
        public string Nome { get; set; }
        public string Usuario { get; set; }
        public string Anexo { get; set; }
        
        public string CodigoUsuario { get; set; }        
        public int IDTerminal { get; set; }
        public DateTime? dataValidade { get; set; }
    }
}

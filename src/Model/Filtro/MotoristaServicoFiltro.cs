using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raizen.UniCad.Model.Filtro
{
    public class MotoristaServicoFiltro
    {
        public string CPF { get; set; }
        public int? LinhaNegocio { get; set; }

        public string Operacao { get; set; }

        public DateTime DataAtualizacao { get; set; }
        public DateTime? DataFinal { get; set; }
        public string Terminal { get; set; }
    }
}

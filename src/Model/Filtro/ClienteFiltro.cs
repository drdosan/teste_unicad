using System.Collections.Generic;

namespace Raizen.UniCad.Model.Filtro
{
    public class ClienteFiltro
    {
        public int ID { get; set; }

        public int? IDEmpresa { get; set; }

        public string Nome { get; set; }

        public IList<string> Cnpjs { get; set; }
    }
}

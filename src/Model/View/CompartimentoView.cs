using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raizen.UniCad.Model.View
{
    public class CompartimentoView
    {
        public string Operacao { get; set; }
        public int seq { get; set; }
        public List<SetaView> setas { get; set; }
    }

    public class SetaView
    {
        public int seq { get; set; }
        public decimal? Volume { get; set; }

        public decimal? Lacres { get; set; }
        public bool isVolumeAlterado { get; set; }
        public string Operacao { get; set; }
        public bool Principal { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raizen.UniCad.Model.View
{
    public class CsonlineDriverRequestView
    {
        public string Cpf {  get; set; }
        public string Name { get; set; }
        public DateTime? DtMOPEExpire { get; set; }
        public DateTime? DtNR35Expire { get; set; }
        public DateTime? DtNR20Expire { get; set; }
    }
}

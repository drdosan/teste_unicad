using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raizen.UniCad.Model.View
{
    public class CsonlineDriverUpdateRequestView
    {
        public CsonlineDriverRequestView Driver { get; set; }
        public IEnumerable<string> Customers { get; set; }
    }
}

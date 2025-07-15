using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raizen.UniCad.Model.View
{
    public class CsonlineUpdateResponseView
    {
        public bool Success { get; set; }
        public IEnumerable<string> Error { get; set; }
    }
}

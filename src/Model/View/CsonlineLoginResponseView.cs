using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raizen.UniCad.Model.View
{
    public class CsonlineLoginResponseView
    {
        public string Code { get; set; }
        public string Token { get; set; }
        public string Name { get; set; }
        public bool IsStore { get; set; }
        public string SalesOrg { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using DataAnnotationsExtensions;
using Raizen.Framework.Models;
using Raizen.UniCad.Model.Base;
using Raizen.UniCad.Web.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace Raizen.UniCad.Model.View
{
    public class MotoristaTreinamentoPraticoServicoView
    {
        public virtual string SiglaTerminal { get; set; }
        public virtual DateTime Data { get; set; }
    }
}
  


using DataAnnotationsExtensions;
using Raizen.Framework.Models;
using Raizen.Framework.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raizen.UniCad.Model.Base
{
    public abstract class PaisBase : BaseModel
    {
        public virtual Int32 ID { get; set; }
        public virtual String Nome { get; set; }
        public virtual String Sigla { get; set; }
        public virtual String Bandeira { get; set; }
        public virtual String LinguagemPadrao { get; set; }
        public virtual Boolean StAtivo { get; set; }
    }
}

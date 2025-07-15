
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using DataAnnotationsExtensions;
using Raizen.Framework.Models;
using Raizen.Framework.Resources;

namespace Raizen.UniCad.Model.Base
{
    public abstract class ImportacaoBase : BaseModel
    {
        public virtual Int32 ID { get; set; }
        public virtual string Nome { get; set; }
        public virtual string Anexo { get; set; }
        public virtual int Tipo { get; set; }
        public virtual int Status { get; set; }
        public virtual DateTime Data { get; set; }

    }
}


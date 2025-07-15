
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
    public abstract class ErroImportacaoBase : BaseModel
    {
        public virtual Int32 ID { get; set; }
        public virtual Int32 IDImportacao { get; set; }
        public virtual Int32 Linha { get; set; }
        public virtual string Descricao { get; set; }
    }
}


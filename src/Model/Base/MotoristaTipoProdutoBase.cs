using DataAnnotationsExtensions;
using Raizen.Framework.Models;
using Raizen.Framework.Resources;
using Raizen.Framework.Utils.CustomAnnotation;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Raizen.UniCad.Model.Base
{
    public class MotoristaTipoProdutoBase
    {
        public virtual int ID { get; set; }
        public virtual int IDMotorista { get; set; }
        public virtual int IDTipoProduto { get; set; }
    }
}

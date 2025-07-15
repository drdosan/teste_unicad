
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
    public abstract class TipoVeiculoBase : BaseModel
    {
        [Required(ErrorMessageResourceType = typeof(MensagensPadrao), ErrorMessageResourceName = "CampoObrigatorio")]
        [Integer(ErrorMessageResourceType = typeof(MensagensPadrao), ErrorMessageResourceName = "FieldMustBeNumeric")]
        public virtual Int32 ID { get; set; }
        [StringLength(100, ErrorMessageResourceType = typeof(MensagensPadrao), ErrorMessageResourceName = "TamanhoIrregular")]
        public virtual string Nome { get; set; }
        [Required(ErrorMessageResourceType = typeof(MensagensPadrao), ErrorMessageResourceName = "CampoObrigatorio")]
        public virtual EnumPais Pais { get; set; }
    }
}


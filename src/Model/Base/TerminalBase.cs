
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
    public abstract class TerminalBase : BaseModel
    {
        [Required(ErrorMessageResourceType = typeof(MensagensPadrao), ErrorMessageResourceName = "CampoObrigatorio")]
        [Integer(ErrorMessageResourceType = typeof(MensagensPadrao), ErrorMessageResourceName = "FieldMustBeNumeric")]
        public virtual Int32 ID { get; set; }
        [Required(ErrorMessageResourceType = typeof(MensagensPadrao), ErrorMessageResourceName = "CampoObrigatorio")]
        [StringLength(200, ErrorMessageResourceType = typeof(MensagensPadrao), ErrorMessageResourceName = "TamanhoIrregular")]
        public virtual string Nome { get; set; }
        [StringLength(500, ErrorMessageResourceType = typeof(MensagensPadrao), ErrorMessageResourceName = "TamanhoIrregular")]
        public virtual string Cidade { get; set; }
        [StringLength(500, ErrorMessageResourceType = typeof(MensagensPadrao), ErrorMessageResourceName = "TamanhoIrregular")]
        public virtual string Endereco { get; set; }
        public virtual int? IDEstado { get; set; }
        public string Sigla { get; set; }
        [Required(ErrorMessageResourceType = typeof(MensagensPadrao), ErrorMessageResourceName = "CampoObrigatorio")]
        public bool isPool {get;set; }
    }
}


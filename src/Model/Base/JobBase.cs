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
    public abstract class JobBase : BaseModel
    {
        [Required(ErrorMessageResourceType = typeof(MensagensPadrao), ErrorMessageResourceName = "CampoObrigatorio")]
        [Integer(ErrorMessageResourceType = typeof(MensagensPadrao), ErrorMessageResourceName = "FieldMustBeNumeric")]
        public virtual Int32 ID { get; set; }
        [Required(ErrorMessageResourceType = typeof(MensagensPadrao), ErrorMessageResourceName = "CampoObrigatorio")]
        [StringLength(100, ErrorMessageResourceType = typeof(MensagensPadrao), ErrorMessageResourceName = "TamanhoIrregular")]
        public virtual string Nome { get; set; }
        [Required(ErrorMessageResourceType = typeof(MensagensPadrao), ErrorMessageResourceName = "CampoObrigatorio")]
        [Integer(ErrorMessageResourceType = typeof(MensagensPadrao), ErrorMessageResourceName = "FieldMustBeNumeric")]
        public virtual int NrPeriodicidadeMinutos { get; set; }
        [Required(ErrorMessageResourceType = typeof(MensagensPadrao), ErrorMessageResourceName = "CampoObrigatorio")]        
        public virtual DateTime DtUltimaExecucao { get; set; }
        [Required(ErrorMessageResourceType = typeof(MensagensPadrao), ErrorMessageResourceName = "CampoObrigatorio")]
        public virtual bool EmExecucao { get; set; }
        [Required(ErrorMessageResourceType = typeof(MensagensPadrao), ErrorMessageResourceName = "CampoObrigatorio")]
        public virtual DateTime DtInicioJob { get; set; }
        [Required(ErrorMessageResourceType = typeof(MensagensPadrao), ErrorMessageResourceName = "CampoObrigatorio")]
        public bool StAtivo { get; set; }
    }
}


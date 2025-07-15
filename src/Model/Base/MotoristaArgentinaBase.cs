using DataAnnotationsExtensions;
using Raizen.Framework.Models;
using Raizen.Framework.Resources;
using Raizen.Framework.Utils.CustomAnnotation;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Raizen.UniCad.Model.Base
{
    public class MotoristaArgentinaBase : BaseModel
    {
        [Required(ErrorMessageResourceType = typeof(MensagensPadrao), ErrorMessageResourceName = "CampoObrigatorio")]
        [Integer(ErrorMessageResourceType = typeof(MensagensPadrao), ErrorMessageResourceName = "FieldMustBeNumeric")]
        public virtual Int32 IDMotorista { get; set; }

        [Required(ErrorMessageResourceType = typeof(MensagensPadrao), ErrorMessageResourceName = "CampoObrigatorio")]
        public virtual string Apellido { get; set; }

        [Required(ErrorMessageResourceType = typeof(MensagensPadrao), ErrorMessageResourceName = "CampoObrigatorio")]
        public virtual string CUITTransportista { get; set; }

        [Required(ErrorMessageResourceType = typeof(MensagensPadrao), ErrorMessageResourceName = "CampoObrigatorio")]
        public virtual string LicenciaNacionalConducir { get; set; }

        [Required(ErrorMessageResourceType = typeof(MensagensPadrao), ErrorMessageResourceName = "CampoObrigatorio")]
        public virtual string LicenciaNacionalHabilitante { get; set; }

        [Required(ErrorMessageResourceType = typeof(MensagensPadrao), ErrorMessageResourceName = "CampoObrigatorio")]
        public virtual string DNI { get; set; }

        public virtual string Tarjeta { get; set; }
    }
}


using DataAnnotationsExtensions;
using Raizen.Framework.Resources;
using System;
using System.ComponentModel.DataAnnotations;

namespace Raizen.UniCad.Model.Base
{
    public abstract class PlacaDocumentoBaseModel : EntidadeDocumentoBaseModel
    {
        [Required(ErrorMessageResourceType = typeof(MensagensPadrao), ErrorMessageResourceName = "CampoObrigatorio")]
        [Integer(ErrorMessageResourceType = typeof(MensagensPadrao), ErrorMessageResourceName = "FieldMustBeNumeric")]
        public virtual Int32 ID { get; set; }
        [Required(ErrorMessageResourceType = typeof(MensagensPadrao), ErrorMessageResourceName = "CampoObrigatorio")]
        [Integer(ErrorMessageResourceType = typeof(MensagensPadrao), ErrorMessageResourceName = "FieldMustBeNumeric")]
        public virtual Int32 IDPlaca { get; set; }

        public virtual int? TipoAcaoVencimento { get; set; }
        public virtual string UsuarioAlterouStatus { get; set; }
    }
}


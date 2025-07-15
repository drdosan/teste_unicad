
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
    public abstract class EntidadeDocumentoBaseModel : BaseModel
    {
        [Required(ErrorMessageResourceType = typeof(MensagensPadrao), ErrorMessageResourceName = "CampoObrigatorio")]
        [Integer(ErrorMessageResourceType = typeof(MensagensPadrao), ErrorMessageResourceName = "FieldMustBeNumeric")]
        public virtual Int32 IDTipoDocumento { get; set; }
        public virtual string Anexo { get; set; }        
        public virtual DateTime? DataVencimento { get; set; }
     
        public virtual string Observacao { get; set; }
        [Required(ErrorMessageResourceType = typeof(MensagensPadrao), ErrorMessageResourceName = "CampoObrigatorio")]
        public virtual bool Vencido { get; set; }
        public virtual bool Alerta1Enviado { get; set; }
        public virtual bool Alerta2Enviado { get; set; }
        public virtual bool Bloqueado { get; set; }
        public virtual bool Processado { get; set; }
    }
}


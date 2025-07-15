
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
    public abstract class TipoDocumentoBase : BaseModel
    {
        [Required(ErrorMessageResourceType = typeof(MensagensPadrao), ErrorMessageResourceName = "CampoObrigatorio")]
        [Integer(ErrorMessageResourceType = typeof(MensagensPadrao), ErrorMessageResourceName = "FieldMustBeNumeric")]
        public virtual Int32 ID { get; set; }
        [Required(ErrorMessageResourceType = typeof(MensagensPadrao), ErrorMessageResourceName = "CampoObrigatorio")]
        [StringLength(20, ErrorMessageResourceType = typeof(MensagensPadrao), ErrorMessageResourceName = "TamanhoIrregular")]
        public virtual string Sigla { get; set; }
        [Required(ErrorMessageResourceType = typeof(MensagensPadrao), ErrorMessageResourceName = "CampoObrigatorio")]
        [StringLength(500, ErrorMessageResourceType = typeof(MensagensPadrao), ErrorMessageResourceName = "TamanhoIrregular")]
        public virtual string Descricao { get; set; }
        [Required(ErrorMessageResourceType = typeof(MensagensPadrao), ErrorMessageResourceName = "CampoObrigatorio")]
        [StringLength(5, ErrorMessageResourceType = typeof(MensagensPadrao), ErrorMessageResourceName = "TamanhoIrregular")]
        public virtual string Operacao { get; set; }
        [Required(ErrorMessageResourceType = typeof(MensagensPadrao), ErrorMessageResourceName = "CampoObrigatorio")]
        [Integer(ErrorMessageResourceType = typeof(MensagensPadrao), ErrorMessageResourceName = "FieldMustBeNumeric")]
        [Range(0, 100, ErrorMessageResourceType = typeof(MensagensPadrao), ErrorMessageResourceName = "CampoObrigatorio")]
        public virtual Int32 IDCategoriaVeiculo { get; set; }
        [Required(ErrorMessageResourceType = typeof(MensagensPadrao), ErrorMessageResourceName = "CampoObrigatorio")]
        public virtual bool Status { get; set; }
        [Required(ErrorMessageResourceType = typeof(MensagensPadrao), ErrorMessageResourceName = "CampoObrigatorio")]
        public virtual bool Obrigatorio { get; set; }
        [StringLength(1000, ErrorMessageResourceType = typeof(MensagensPadrao), ErrorMessageResourceName = "TamanhoIrregular")]
        public virtual string Observacao { get; set; }
        public virtual DateTime DataAtualizacao { get; set; }
        [Required(ErrorMessageResourceType = typeof(MensagensPadrao), ErrorMessageResourceName = "CampoObrigatorio")]
        [Integer(ErrorMessageResourceType = typeof(MensagensPadrao), ErrorMessageResourceName = "FieldMustBeNumeric")]
        public virtual int IDEmpresa { get; set; }
        public virtual int? BloqueioImediato { get; set; }
        [Integer(ErrorMessageResourceType = typeof(MensagensPadrao), ErrorMessageResourceName = "FieldMustBeNumeric")]
        public virtual int? QtdDiasBloqueio { get; set; }

        [Required(ErrorMessageResourceType = typeof(MensagensPadrao), ErrorMessageResourceName = "CampoObrigatorio")]
        [Integer(ErrorMessageResourceType = typeof(MensagensPadrao), ErrorMessageResourceName = "FieldMustBeNumeric")]
        public virtual Int32 Alerta1 { get; set; }
        [Required(ErrorMessageResourceType = typeof(MensagensPadrao), ErrorMessageResourceName = "CampoObrigatorio")]
        [Integer(ErrorMessageResourceType = typeof(MensagensPadrao), ErrorMessageResourceName = "FieldMustBeNumeric")]
        public virtual Int32 Alerta2 { get; set; }
        [Required(ErrorMessageResourceType = typeof(MensagensPadrao), ErrorMessageResourceName = "CampoObrigatorio")]
        [Integer(ErrorMessageResourceType = typeof(MensagensPadrao), ErrorMessageResourceName = "FieldMustBeNumeric")]
        public virtual Int32 qtdeAlertas { get; set; }
        [Range(1, 100, ErrorMessageResourceType = typeof(MensagensPadrao), ErrorMessageResourceName = "CampoObrigatorio")]
        [Required(ErrorMessageResourceType = typeof(MensagensPadrao), ErrorMessageResourceName = "CampoObrigatorio")]
        public virtual Int32 tipoCadastro { get; set; }
        public virtual Int32? MesesValidade {get;set; }

        public virtual int? TipoAcaoVencimento { get; set; }
        [Required(ErrorMessageResourceType = typeof(MensagensPadrao), ErrorMessageResourceName = "CampoObrigatorio")]
        [Integer(ErrorMessageResourceType = typeof(MensagensPadrao), ErrorMessageResourceName = "FieldMustBeNumeric")]
        public virtual Int32 IDPais { get; set; }
        public virtual bool DocumentoPossuiVencimento { get; set; }
    }
}


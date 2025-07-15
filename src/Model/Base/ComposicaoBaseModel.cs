
using DataAnnotationsExtensions;
using Raizen.Framework.Models;
using Raizen.Framework.Resources;
using Raizen.Framework.Utils.CustomAnnotation;
using Raizen.UniCad.Model.View;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Raizen.UniCad.Model.Base
{
    public class ComposicaoBaseModel : BaseModel
    {
        [Required(ErrorMessageResourceType = typeof(MensagensPadrao), ErrorMessageResourceName = "CampoObrigatorio")]
        [Integer(ErrorMessageResourceType = typeof(MensagensPadrao), ErrorMessageResourceName = "FieldMustBeNumeric")]
        public virtual Int32 ID { get; set; }
        [Required(ErrorMessageResourceType = typeof(MensagensPadrao), ErrorMessageResourceName = "CampoObrigatorio")]
        [Integer(ErrorMessageResourceType = typeof(MensagensPadrao), ErrorMessageResourceName = "FieldMustBeNumeric")]
        public virtual Int32 IDEmpresa { get; set; }
        public virtual string Operacao { get; set; }
        [Required(ErrorMessageResourceType = typeof(MensagensPadrao), ErrorMessageResourceName = "CampoObrigatorio")]
        [Integer(ErrorMessageResourceType = typeof(MensagensPadrao), ErrorMessageResourceName = "FieldMustBeNumeric")]
        public virtual Int32 IDTipoComposicao { get; set; }
        public virtual Nullable<Int32> IDTipoComposicaoEixo { get; set; }
        [Required(ErrorMessageResourceType = typeof(MensagensPadrao), ErrorMessageResourceName = "CampoObrigatorio")]
        [Integer(ErrorMessageResourceType = typeof(MensagensPadrao), ErrorMessageResourceName = "FieldMustBeNumeric")]
        public virtual Int32 IDCategoriaVeiculo { get; set; }

        public virtual int? TipoContratacao { get; set; }

        public int? IDComposicao { get; set; }
        public virtual int? IDPlaca1 { get; set; }
        public virtual int? IDPlaca2 { get; set; }
        public virtual int? IDPlaca3 { get; set; }
        public virtual int? IDPlaca4 { get; set; }
        public virtual string CPFCNPJ { get; set; }
        public virtual string CPFCNPJArrendamento { get; set; }
        public virtual string RazaoSocialArrendamento { get; set; }
        [StringLength(4000, ErrorMessageResourceType = typeof(MensagensPadrao), ErrorMessageResourceName = "TamanhoIrregular")]
        public virtual string Justificativa { get; set; }
        [Required(ErrorMessageResourceType = typeof(MensagensPadrao), ErrorMessageResourceName = "CampoObrigatorio")]
        [Integer(ErrorMessageResourceType = typeof(MensagensPadrao), ErrorMessageResourceName = "FieldMustBeNumeric")]
        public virtual int IDStatus { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [DateAttributeRaizen(ErrorMessageResourceType = typeof(MensagensPadrao), ErrorMessageResourceName = "FieldMustBeDate")]
        public virtual Nullable<DateTime> DataNascimento { get; set; }
        public virtual string RazaoSocial { get; set; }
        [Required(ErrorMessageResourceType = typeof(MensagensPadrao), ErrorMessageResourceName = "CampoObrigatorio")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [DateAttributeRaizen(ErrorMessageResourceType = typeof(MensagensPadrao), ErrorMessageResourceName = "FieldMustBeDate")]
        public virtual DateTime DataAtualizacao { get; set; }
        public virtual string Observacao { get; set; }
        public virtual string CodigoEasyQuery { get; set; }
        public virtual string CodigoSalesForce { get; set; }
        public string LoginUsuario { get; set; }
        public bool? isUtilizaPlacaChave { get; set; }

        public virtual string UsuarioAlterouStatus { get; set; }
        public int IdPais { get; set; }

        [NotMapped]
        public List<PlacaDocumentoView> Documentos { get; set; }

        [NotMapped]
        public double TaraComposicao { get; set; }
        [NotMapped]
        public int EixosComposicao { get; set; }
        [NotMapped]
        public int? EixosDistanciados { get; set; }
        [NotMapped]
        public int? EixosPneusDuplos { get; set; }
        [NotMapped]
        public List<ComposicaoEixo> ComposicaoEixo { get; set; }
        [NotMapped]
        public virtual string Placa1 { get; set; }
        [NotMapped]
        public virtual string Placa2 { get; set; }
        [NotMapped]
        public virtual string Placa3 { get; set; }
        [NotMapped]
        public virtual string Placa4 { get; set; }

    }
}



using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using DataAnnotationsExtensions;
using Raizen.Framework.Models;
using Raizen.Framework.Resources;
using Raizen.Framework.Utils.CustomAnnotation;

namespace Raizen.UniCad.Model.Base
{
    public abstract class PlacaBaseModel : BaseModel
    {
        [Required(ErrorMessageResourceType = typeof(MensagensPadrao), ErrorMessageResourceName = "CampoObrigatorio")]
        [Integer(ErrorMessageResourceType = typeof(MensagensPadrao), ErrorMessageResourceName = "FieldMustBeNumeric")]
        public virtual Int32 ID { get; set; }
        
        public virtual string PlacaVeiculo { get; set; }
        
        [Required(ErrorMessageResourceType = typeof(MensagensPadrao), ErrorMessageResourceName = "CampoObrigatorio")]
        public virtual string Operacao { get; set; }
        
        [Required(ErrorMessageResourceType = typeof(MensagensPadrao), ErrorMessageResourceName = "CampoObrigatorio")]
        [Integer(ErrorMessageResourceType = typeof(MensagensPadrao), ErrorMessageResourceName = "FieldMustBeNumeric")]
        public virtual Int32 IDTipoVeiculo { get; set; }
        
        public virtual string Marca { get; set; }
        
        public virtual string Modelo { get; set; }
        
        public virtual string Chassi { get; set; }
        
        [Required(ErrorMessageResourceType = typeof(MensagensPadrao), ErrorMessageResourceName = "CampoObrigatorio")]
        [Integer(ErrorMessageResourceType = typeof(MensagensPadrao), ErrorMessageResourceName = "FieldMustBeNumeric")]
        public virtual Int32 AnoFabricacao { get; set; }
        
        [Required(ErrorMessageResourceType = typeof(MensagensPadrao), ErrorMessageResourceName = "CampoObrigatorio")]
        [Integer(ErrorMessageResourceType = typeof(MensagensPadrao), ErrorMessageResourceName = "FieldMustBeNumeric")]
        public virtual Int32 AnoModelo { get; set; }
        
        public virtual string Cor { get; set; }
        
        public virtual string TipoRastreador { get; set; }
        
        public virtual string NumeroAntena { get; set; }
        
        public virtual string Versao { get; set; }

        public virtual bool CameraMonitoramento { get; set; }
        
        public virtual bool BombaDescarga { get; set; }
        
        [Range(0.00, 9999999999999999.99, ErrorMessageResourceType = typeof(MensagensPadrao), ErrorMessageResourceName = "CampoObrigatorio")]
        [Required(ErrorMessageResourceType = typeof(MensagensPadrao), ErrorMessageResourceName = "CampoObrigatorio")]
        public virtual Double Tara { get; set; }

        public virtual Int32 NumeroEixos { get; set; }
                
        public virtual bool EixosPneusDuplos { get; set; }
        
        public virtual bool PossuiAbs { get; set; }
        
        [Integer(ErrorMessageResourceType = typeof(MensagensPadrao), ErrorMessageResourceName = "FieldMustBeNumeric")]
        public virtual Nullable<Int32> NumeroEixosPneusDuplos { get; set; }
        
        public virtual bool EixosDistanciados { get; set; }
        
        [Integer(ErrorMessageResourceType = typeof(MensagensPadrao), ErrorMessageResourceName = "FieldMustBeNumeric")]
        public virtual Nullable<Int32> NumeroEixosDistanciados { get; set; }

        [Integer(ErrorMessageResourceType = typeof(MensagensPadrao), ErrorMessageResourceName = "FieldMustBeNumeric")]
        public virtual Int32? IDTipoProduto { get; set; }
        
        public virtual bool MultiSeta { get; set; }
        
        [Integer(ErrorMessageResourceType = typeof(MensagensPadrao), ErrorMessageResourceName = "FieldMustBeNumeric")]
        public virtual Int32? IDTipoCarregamento { get; set; }
                
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [DateAttributeRaizen(ErrorMessageResourceType = typeof(MensagensPadrao), ErrorMessageResourceName = "FieldMustBeDate")]
        public virtual Nullable<DateTime> DataNascimento { get; set; }
        
        public virtual string RazaoSocial { get; set; }
        
        public virtual Int32 IDCategoriaVeiculo { get; set; }

        public virtual string Observacao { get; set; }

        public virtual string Status { get; set; }

        public virtual string Anexo { get; set; }

        public virtual Nullable<DateTime> DataAtualizacao { get; set; }

        public virtual int? Principal { get; set; }

        public virtual int? IDTransportadora { get; set; }

        //Para operação AMBAS esse campo será EAB
        public virtual int? IDTransportadora2 { get; set; }

        public virtual EnumPais IDPais { get; set; }
    }
}


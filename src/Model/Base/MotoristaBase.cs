using DataAnnotationsExtensions;
using Raizen.Framework.Models;
using Raizen.Framework.Resources;
using Raizen.Framework.Utils.CustomAnnotation;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Raizen.UniCad.Model.Base
{
    public class MotoristaBase : BaseModel
    {
        [Required(ErrorMessageResourceType = typeof(MensagensPadrao), ErrorMessageResourceName = "CampoObrigatorio")]
        [Integer(ErrorMessageResourceType = typeof(MensagensPadrao), ErrorMessageResourceName = "FieldMustBeNumeric")]
        public virtual Int32 ID { get; set; }

        [Required(ErrorMessageResourceType = typeof(MensagensPadrao), ErrorMessageResourceName = "CampoObrigatorio")]
        [Integer(ErrorMessageResourceType = typeof(MensagensPadrao), ErrorMessageResourceName = "FieldMustBeNumeric")]
        public virtual Int32 IDEmpresa { get; set; }

        public virtual Int32? IDTransportadora { get; set; }

        [Required(ErrorMessageResourceType = typeof(MensagensPadrao), ErrorMessageResourceName = "CampoObrigatorio")]
        [Integer(ErrorMessageResourceType = typeof(MensagensPadrao), ErrorMessageResourceName = "FieldMustBeNumeric")]
        public virtual int IDStatus { get; set; }

        [Required(ErrorMessageResourceType = typeof(MensagensPadrao), ErrorMessageResourceName = "CampoObrigatorio")]
        public virtual string Nome { get; set; }
        
        [Required(ErrorMessageResourceType = typeof(MensagensPadrao), ErrorMessageResourceName = "CampoObrigatorio")]
        public virtual string Operacao { get; set; }

        [Required(ErrorMessageResourceType = typeof(MensagensPadrao), ErrorMessageResourceName = "CampoObrigatorio")]
        public virtual DateTime DataAtualizazao { get; set; }

        [Required(ErrorMessageResourceType = typeof(MensagensPadrao), ErrorMessageResourceName = "CampoObrigatorio")]
        public virtual bool Ativo { get; set; }

        //[Required(ErrorMessageResourceType = typeof(MensagensPadrao), ErrorMessageResourceName = "CampoObrigatorio")]
        //[RegularExpression(@"^\(?\d{2}\)?[\s-]?\d{5}-?\d{4}$", ErrorMessage = "Número de telefone inválido")]
        public virtual string Telefone { get; set; }

        //[Required(ErrorMessageResourceType = typeof(MensagensPadrao), ErrorMessageResourceName = "CampoObrigatorio")]
        //[EmailAddress(ErrorMessage = "E-mail inválido")]
        public virtual string Email { get; set; }

        public virtual string Justificativa { get; set; }

        public virtual string Anexo { get; set; }

        public virtual string CodigoEasyQuery { get; set; }

        public virtual string CodigoSalesForce { get; set; }

        public virtual int? IDMotorista { get; set; }

        public virtual string LoginUsuario { get; set; }

        public virtual string Observacao { get; set; }

        public virtual string PIS {get;set; }

        public virtual string UsuarioAlterouStatus { get; set; }

		public virtual EnumPais IdPais { get; set; }        
    }
}

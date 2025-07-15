using DataAnnotationsExtensions;
using Raizen.Framework.Models;
using Raizen.Framework.Resources;
using System;
using System.ComponentModel.DataAnnotations;

namespace Raizen.UniCad.Model.Base
{
    public abstract class PlacaArgentinaBaseModel : BaseModel
    {
        [Required(ErrorMessageResourceType = typeof(MensagensPadrao), ErrorMessageResourceName = "CampoObrigatorio")]
        [Integer(ErrorMessageResourceType = typeof(MensagensPadrao), ErrorMessageResourceName = "FieldMustBeNumeric")]
        public virtual Int32 IDPlaca { get; set; }

        public virtual string Vencimento { get; set; }

        public virtual string CUIT { get; set; }

        public virtual double? PBTC { get; set; }

        public virtual string NrMotor { get; set; }

        public virtual string SatelitalMarca { get; set; }

        public virtual string SatelitalModelo { get; set; }

        public virtual string SatelitalNrInterno { get; set; }

        public virtual string SatelitalEmpresa { get; set; }

        public virtual string Material { get; set; }

        public virtual string Potencia { get; set; }

    }
}

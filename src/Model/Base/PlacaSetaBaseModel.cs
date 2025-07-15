
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using DataAnnotationsExtensions;
using Raizen.Framework.Models;
using Raizen.Framework.Resources;
using System.ComponentModel.DataAnnotations.Schema;

namespace Raizen.UniCad.Model.Base
{
    public class PlacaSetaBaseModel : BaseModel
    {
        [Required(ErrorMessageResourceType = typeof(MensagensPadrao), ErrorMessageResourceName = "CampoObrigatorio")]
        [Integer(ErrorMessageResourceType = typeof(MensagensPadrao), ErrorMessageResourceName = "FieldMustBeNumeric")]
        public virtual Int32 ID { get; set; }
        [Required(ErrorMessageResourceType = typeof(MensagensPadrao), ErrorMessageResourceName = "CampoObrigatorio")]
        [Integer(ErrorMessageResourceType = typeof(MensagensPadrao), ErrorMessageResourceName = "FieldMustBeNumeric")]
        public virtual Int32 IDPlaca { get; set; }
        public virtual Nullable<decimal> Volume { get; set; }
        public virtual bool? CompartimentoPrincipal1 { get; set; }        
        public virtual Nullable<decimal> VolumeCompartimento1 { get; set; }
        public virtual bool? CompartimentoPrincipal2 { get; set; }
        public virtual Nullable<decimal> VolumeCompartimento2 { get; set; }
        public virtual bool? CompartimentoPrincipal3 { get; set; }
        public virtual Nullable<decimal> VolumeCompartimento3 { get; set; }
        public virtual bool? CompartimentoPrincipal4 { get; set; }
        public virtual Nullable<decimal> VolumeCompartimento4 { get; set; }
        public virtual bool? CompartimentoPrincipal5 { get; set; }
        public virtual Nullable<decimal> VolumeCompartimento5 { get; set; }
        public virtual bool? CompartimentoPrincipal6 { get; set; }
        public virtual Nullable<decimal> VolumeCompartimento6 { get; set; }
        public virtual bool? CompartimentoPrincipal7 { get; set; }
        public virtual Nullable<decimal> VolumeCompartimento7 { get; set; }
        public virtual bool? CompartimentoPrincipal8 { get; set; }
        public virtual Nullable<decimal> VolumeCompartimento8 { get; set; }
        public virtual bool? CompartimentoPrincipal9 { get; set; }
        public virtual Nullable<decimal> VolumeCompartimento9 { get; set; }
        public virtual bool? CompartimentoPrincipal10 { get; set; }
        public virtual Nullable<decimal> VolumeCompartimento10 { get; set; }
        public virtual Nullable<int> LacreCompartimento1 { get; set; }
        public virtual Nullable<int> LacreCompartimento2 { get; set; }
        public virtual Nullable<int> LacreCompartimento3 { get; set; }
        public virtual Nullable<int> LacreCompartimento4 { get; set; }
        public virtual Nullable<int> LacreCompartimento5 { get; set; }
        public virtual Nullable<int> LacreCompartimento6 { get; set; }
        public virtual Nullable<int> LacreCompartimento7 { get; set; }
        public virtual Nullable<int> LacreCompartimento8 { get; set; }
        public virtual Nullable<int> LacreCompartimento9 { get; set; }
        public virtual Nullable<int> LacreCompartimento10 { get; set; }
        public virtual bool? Compartimento1IsInativo { get; set; }
        public virtual bool? Compartimento2IsInativo { get; set; }
        public virtual bool? Compartimento3IsInativo { get; set; }
        public virtual bool? Compartimento4IsInativo { get; set; }
        public virtual bool? Compartimento5IsInativo { get; set; }
        public virtual bool? Compartimento6IsInativo { get; set; }
        public virtual bool? Compartimento7IsInativo { get; set; }
        public virtual bool? Compartimento8IsInativo { get; set; }
        public virtual bool? Compartimento9IsInativo { get; set; }
        public virtual bool? Compartimento10IsInativo { get; set; }

        [NotMapped]
        public int Colunas { get; set; }

        [NotMapped]
        public int Sequencial { get; set; }

        [NotMapped]
        public bool Multiseta { get; set; }
        [NotMapped]
        public bool SomenteVisualizacao { get; set; }

        #region Constructors

        public PlacaSetaBaseModel()
        {

        }

        public PlacaSetaBaseModel(int colunas, int sequencial, bool multiSeta)
        {
            this.Colunas = colunas;
            this.Sequencial = sequencial;
            this.Multiseta = multiSeta;
        }

        #endregion
    }
}


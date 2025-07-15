using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using DataAnnotationsExtensions;
using Raizen.Framework.Models;
using Raizen.UniCad.Model.Base;

namespace Raizen.UniCad.Model.View
{
    public class PlacaSetaView
    {
        public virtual Int32 ID { get; set; }
        public virtual Int32 IDPlaca { get; set; }
        public virtual Nullable<decimal> Volume { get; set; }
        public virtual Nullable<decimal> VolumeCompartimento1 { get; set; }
        public virtual Nullable<decimal> VolumeCompartimento2 { get; set; }
        public virtual Nullable<decimal> VolumeCompartimento3 { get; set; }
        public virtual Nullable<decimal> VolumeCompartimento4 { get; set; }
        public virtual Nullable<decimal> VolumeCompartimento5 { get; set; }
        public virtual Nullable<decimal> VolumeCompartimento6 { get; set; }
        public virtual Nullable<decimal> VolumeCompartimento7 { get; set; }
        public virtual Nullable<decimal> VolumeCompartimento8 { get; set; }
        public virtual Nullable<decimal> VolumeCompartimento9 { get; set; }
        public virtual Nullable<decimal> VolumeCompartimento10 { get; set; }

        public int Colunas { get; set; }

        public int Sequencial { get; set; }

        public bool Multiseta { get; set; }
    }
}
  


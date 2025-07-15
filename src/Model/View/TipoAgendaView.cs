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
    public class TipoAgendaView
    {
        public virtual Int32 ID { get; set; }
        public virtual string Nome { get; set; }
        public virtual Int32 IDTipo { get; set; }
        public virtual bool Status { get; set; }
        public virtual string Anexo { get; set; }
    }
}
  


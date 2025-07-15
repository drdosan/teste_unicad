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
    public class ProdutoView
    {
        public virtual Int32 ID { get; set; }
        public virtual string Nome { get; set; }
        public virtual string Codigo { get; set; }
        public virtual bool Status { get; set; }
        public virtual string TipoProduto { get; set; }
        public virtual double Densidade { get; set; }
        public virtual string Pais { get; set; }
    }
}
  


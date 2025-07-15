
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
    public class TipoDocumentoTipoProdutoView
    {
        public int ID { get; set; }
        public int IDTipoDocumento { get; set; }
        public int IDTipoProduto { get; set; }
        public string Nome { get; set; }
    }
}
  


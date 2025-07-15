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
    public class ClienteTransportadoraView
    {
        public int ID { get; set; }
        public int? IDClienteTransportadora { get; set; }
        public int? IDUsuario { get; set; }
        public string RazaoSocial { get; set; }
        public string IBM { get; set; }
        public string CPF_CNPJ { get; set; }
        
    }
}
  


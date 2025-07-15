
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using DataAnnotationsExtensions;
using Raizen.Framework.Models;
 
using Raizen.UniCad.Model.Base;
using System.ComponentModel.DataAnnotations.Schema;
using Raizen.UniCad.Model.View;

namespace Raizen.UniCad.Model
  {
    public class Usuario : UsuarioBase
    {
        [NotMapped]
        public List<UsuarioClienteView> Clientes { get; set; }
        [NotMapped]
        public List<UsuarioTransportadoraView> Transportadoras { get; set; }

    }
  }
  


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

namespace Raizen.UniCad.Model
  {
    public class Configuracao : ConfiguracaoBase
    {
        public Pais Pais { get; set; }
    }
}
  

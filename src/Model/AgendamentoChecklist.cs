
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
    public class AgendamentoChecklist : AgendamentoChecklistBase
    {
        
        [NotMapped]
        public string Placa { get; set; }

        [NotMapped]
        public string Placas { get; set; }

        [NotMapped]
        public string OperacaoUsuario { get; set; }
        [NotMapped]
        public int IDEmpresaUsuario { get; set; }
    }
}
  

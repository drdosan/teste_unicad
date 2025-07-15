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
    public class MotoristaClienteView
    {
        public string CNPJCPF { get; set; }
        public int Colunas { get; set; }
        public string IBM { get; set; }
        public int ID { get; set; }
        public int IDCliente { get; set; }
        public int IDMotorista { get; set; }
        public string RazaoSocial { get; set; }
        public DateTime? DataAprovacao { get; set; }
    }
}
  


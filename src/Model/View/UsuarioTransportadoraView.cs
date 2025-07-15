
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
    public class UsuarioTransportadoraView
    {
        public string IBM { get; set; }
        public int ID { get; set; }
        public int IDTransportadora { get; set; }
        public int IDUsuario { get; set; }
        public string Nome { get; set; }
        public string RazaoSocial { get; set; }
        public string CNPJCPF { get; set; }
    }
}



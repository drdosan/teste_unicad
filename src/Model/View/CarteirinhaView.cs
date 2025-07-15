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
    public class CarteirinhaView
    {
        public DateTime? dataVencimentoCdds { get; set; }

        public DateTime? dataVencimentoCNH { get; set; }
        public DateTime? dataVencimentoMOPP { get; set; }
        public DateTime? dataVencimentoCIP { get; set; }
        public DateTime? dataVencimentoNR20 { get; set; }
        public DateTime? dataVencimentoNr35 { get; set; }


        public string Nome { get; set; }
        public string Transportadora { get; set; }
        public string CPF { get; set; }
        public string CPFComMascara { get; set; }
        public string MOPPVencimento { get; set; }
        public string CNHVencimento { get; set; }
        public string CIPVencimento { get; set; }
        public string CDDSVencimento { get; set; }
        public string NR20Vencimento { get; set; }
        public string NR35Vencimento { get; set; }
        public string DataEmissao { get; set; }
    }
}



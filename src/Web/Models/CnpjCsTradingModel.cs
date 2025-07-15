using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Raizen.UniCad.Web.Models
{
    public class CnpjCsTradingModel
    {
        public string Cnpj { get; set; }
        public string CompanyName { get; set; }
        public int NetworkId { get; set; }
        public string NetworkDescription { get; set; }
        public int IsActive { get; set; }
    }
}
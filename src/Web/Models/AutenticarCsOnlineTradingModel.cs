using Raizen.UniCad.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Raizen.UniCad.Web.Models
{
    public class AutenticarCsOnlineTradingModel
    {
        public string Lan { get; set; }
        public string Tipo { get; set; }
        public string Dv { get; set; }
        public string Token { get; set; }
        public IList<string> Cnpjs { get; set; }
    }
}
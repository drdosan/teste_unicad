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
    public class AgendamentoTerminalView
    {
        public virtual Int32 ID { get; set; }
        public virtual int IdTerminal {get;set; }
        public int IdTipoAgenda { get; set; }
        public virtual DateTime Data { get; set; }
        public virtual int IdTipoTipoAgenda {get;set; }
        public virtual string Terminal { get; set; }
        public virtual string TipoAgenda { get; set; }
        public virtual bool Status { get; set; }
        public string Nome {get;set; }
        public string LinhaNegocio { get; set; }
        public string Operacao { get; set; }
        public string Vagas { get; set; }
        public string VagasDisponiveis { get; set; }
        public int Linhas { get; set; }
        public int IdEmpresa {get;set; }
        public string NomeEnderecoTerminal { get; set; }
        public List<ControlePresencaMotoristaView> listaControles {get;set; }
    }
}



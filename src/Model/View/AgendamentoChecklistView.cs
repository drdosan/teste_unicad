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
    public class AgendamentoChecklistView
    {
        public Nullable<Int32> ID { get; set; }
        public String Placa1 { get; set; }
        public String Placa2 { get; set; }
        public String Placa3 { get; set; }
        public String Placa4 { get; set; }
        public string Empresa { get; set; }
        public string Operacao { get; set; }
        public string TipoComposicao { get; set; }
        public string SiglaTerminal { get; set; }
        public string Terminal { get; set; }
        public DateTime Data { get; set; }
        public string Horario { get; set; }
        public string Usuario { get; set; }
        public DateTime DataAgendamento { get; set; }
        public string EnderecoTerminal { get; set; }
        public string CidadeTerminal { get; set; }
        public string EstadoTerminal { get; set; }

        public string Mensagem { get; set; }
        public string Placas { get; set; }
        public int IDComposicao { get; set; }
        public int Linhas { get; set; }
        public TimeSpan HoraInicio { get; set; }
    }
}
  


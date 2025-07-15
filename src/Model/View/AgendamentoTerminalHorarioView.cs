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
    public class AgendamentoTerminalHorarioView
    {
        public virtual Int32 ID { get; set; }
        public virtual string Horario { get { return $"{HoraInicio:hh\\:mm}:{HoraFim:hh\\:mm}";}}
        public virtual int IdLinhaNegocios { get; set; }
        public virtual string Endereco { get; set; }
        public virtual string Cidade { get; set; }
        public virtual string LinhaNegocios { get; set; }
        public virtual string Operacao { get; set; }
        public virtual int NumVagas { get; set; }
        public int Vagas {get;set; }
        public DateTime Data { get; set; }
        public TimeSpan HoraInicio {get;set; }
        public TimeSpan HoraFim {get;set; }
        public string Anexo { get; set; }
        public bool Bloqueado { get; set; }
        public string TipoComposicao { get; set; }
        public string Placa1 { get; set; }
        public string Placa2 { get; set; }
        public string Placa3 { get; set; }
        public string Placa4 { get; set; }
        public string CPF { get; set; }
        public string Nome { get; set; }
        public int? IdSituacao { get; set; }
        public int IDTerminal { get; set; }
        public int IDEmpresa { get; set; }
        public int IDTipoAgendamento { get; set; }
        public string TipoTreinamento { get; set; }
    }
}



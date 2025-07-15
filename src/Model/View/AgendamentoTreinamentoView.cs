using System;
using Raizen.Framework.Utils.Extensions;
using System.Collections.Generic;

namespace Raizen.UniCad.Model.View
{
    public class AgendamentoTreinamentoView
    {
        public int? ID { get; set; }
        public string Usuario {get;set; }
        public string Empresa { get; set; }
        public string TipoTreinamento { get; set; }
        public string CPF { get; set; }
        public int? idSituacao { get; set; }
        public string Operacao { get; set; }
        public string Terminal { get; set; }
        public DateTime Data { get; set; }
        public virtual string Horario { get { return $"{HoraInicio:hh\\:mm}"; } }
        public string EnderecoTerminal { get; set; }
        public string CidadeTerminal { get; set; }
        public string EstadoTerminal { get; set; }
        public string Motorista { get; set; }
        public TimeSpan HoraInicio { get; set; }
        public TimeSpan HoraFim { get; set; }
        public string LinhaNegocios { get; set; }
        public int NumVagas { get; set; }
        public string Endereco { get; set; }
        public string Anexo { get; set; }
        public string Cidade { get; set; }
        public int Linhas { get; set; }
        public int IDMotorista { get; set; }
        public int IDAgendamentoTerminalHorario { get; set; }
        public int IDEmpresa { get; set; }
        public int IdTipoAgenda { get; set; }
        public int IDTerminal { get; set; }
        public int IDTipoTreinamento { get; set; }
        public bool IsInscrito { get; set; }
        public string Nome { get; set; }
        public List<ControlePresencaMotoristaView> listaControles { get; set; }
        public int IDEmpresaMotorista { get; set; }
        public string OperacaoMotorista { get; set; }
        public int IdTipo { get; set; }
        public int IDEmpresaCongenere {get;set; }
    }
}



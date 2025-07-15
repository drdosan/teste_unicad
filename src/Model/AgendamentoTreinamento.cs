using Raizen.UniCad.Model.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace  Raizen.UniCad.Model
{
    public class AgendamentoTreinamento : AgendamentoTreinamentoBase
    {
        [NotMapped]
        public int IDEmpresa { get; set; }
        [NotMapped]
        public string Operacao { get; set; }
        [NotMapped]
        public int IDTerminal { get; set; }
        [NotMapped]
        public string CPF { get; set; }
        [NotMapped]
        public int IDTipoTreinamento {get;set; }
        [NotMapped]
        public int IDTipo {get;set; }
        [NotMapped]
        public bool IsEdicao { get; set; }
        [NotMapped]
        public int IDEmpresaUsuario { get; set; }
        [NotMapped]
        public string OperacaoUsuario { get; set; }
    }
}
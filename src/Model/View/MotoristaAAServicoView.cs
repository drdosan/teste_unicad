using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Raizen.UniCad.Model.View
{
    public class MotoristaAAServicoView
    {
        [IgnoreDataMember]
        public int ID { get; set; }
        public string CPF { get; set; }
        public string Nome { get; set; }
        public int LinhaNegocio {get;set; }
        public string Operacao { get; set; }
        public string RG { get; set; }
        public string OrgaoEmissor { get; set; }
        public string CNH { get; set; }
        public string CategoriaCNH { get; set; }
        public string EmissorCNH { get; set; }
        public string LocalNascimento { get; set; }
        public string Telefone { get; set; }
        public string Email { get; set; }
        public string Observacao { get; set; }
        public DateTime DataAtualizacao { get; set; }
        public DateTime? DataTreinamentoTeorico { get; set; }
        public bool? IsTreinamentoTeoricoAprovado {get;set; }
        public string PIS {get;set; }
        public DateTime Nascimento { get;set; }
        public string UsuarioTreinamentoTeorico { get;set; }
        public string UsuarioTreinamentoPratico { get; set; }
        public int Status {get;set; }
        public List<MotoristaDocumentoAAServicoView> ListaDocumentos { get; set; }
        public List<MotoristaPermissaoServicoView> ListaPermissoes { get; set; }
        public List<MotoristaTreinamentoPraticoServicoView> ListaTreinamentosPraticos { get; set; }

    }
}



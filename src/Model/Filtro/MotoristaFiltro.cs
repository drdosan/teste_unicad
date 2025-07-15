using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raizen.UniCad.Model.Filtro
{
    public class MotoristaFiltro
    {
        public int ID { get; set; }
        public string CPF { get; set; }
        public string RG { get; set; }
        public string CNH { get; set; }
        public string CategoriaCNH { get; set; }
        public string Nome { get; set; }        
        public string Chamado { get; set; }
        public int? IDEmpresa { get; set; }
        public string Operacao { get; set; }
        public int? IDStatus { get; set; }
        public DateTime? DataInicio { get; set; }
        public DateTime? DataFim { get; set; }
        public Boolean? Ativo { get; set; }
        public int? IDUsuarioTransportadora { get; set; }
        public int? IDUsuarioCliente { get; set; }
        public int? IDEmpresaUsuario { get; set; }
        public string OperacaoUsuario { get; set; }
        public int? IDTransportadora { get;set; }
        public string TransportadoraNome {get;set; }
        public bool UsuarioExterno { get; set; }
        public int IdTipoProduto { get; set; }
        public int IdTipoComposicao { get; set; }

        public int? IdPais { get; set; }

        public int? IDCliente { get; set; }
        public string ClienteNome { get; set; }

        public string Apellido { get; set; }
        
        public string DNI { get; set; }


    }
}

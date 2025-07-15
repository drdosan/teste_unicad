#region Cabeçalho do Arquivo

// <Summary>
// File Name          : Acao.cs
// Class Name         : Acao
// Author             : Alexandre Spreafico Novaes
// Creation Date      : 03/11/2016
// Template Version   : 2.3
// </Summary>

// <RevisionHistory>
// <SNo value=1>
//	Modified By             : 
//	Date of Modification    : 
//	Reason for modification : 
//	Modification Done       : 
//	Defect Id (If any)      : 
// <SNo>
// </RevisionHistory>

#endregion Cabeçalho do Arquivo

#region Namespaces

using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

#endregion Namespaces

namespace Raizen.UniCad.Model.Filtro
{
	/// <summary>
	/// Entitade Representada por Acao
	/// </summary>	
	public  class ComposicaoFiltro
	{
		#region Propriedades
		public Nullable<Int32> Id { get; set; }
        public int? IDEmpresa { get; set; }
        public int? IDTransportadora{ get; set; }
        public int? IDStatus { get; set; }
        public int? IDCliente { get; set; }
        public int? IDUsuarioTransportadora { get; set; }
        public int? IDUsuarioCliente { get; set; }
        public DateTime? DataInicio { get; set; }
        public DateTime? DataFim { get; set; }
        public String Chamado { get; set; }
        public String Placa { get; set; }        
        public String Operacao { get; set; }
        public string ComposicaoVeiculo { get; set; }
        public bool? Status { get; set; }
        public int? IDTipoComposicao { get; set; }
        public string CodigoEasyQuery { get; set; }
        public int? IdPlacaOficial { get; set; }
        public int? IDEmpresaUsuario { get; set; }
        public string OperacaoUsuario { get; set; }
        public string ClienteNome { get; set; }
        public string TransportadoraNome { get; set; }
        public bool UsuarioExterno { get; set; }
        public int? IdPais { get; set; }
        #endregion Propriedades
    }
}


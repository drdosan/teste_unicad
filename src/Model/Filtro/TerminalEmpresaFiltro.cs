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
	public  class TerminalEmpresaFiltro
	{
		#region Propriedades
		
		/// <summary>
		/// Propriedade Id
		/// </summary>
		public Nullable<Int32> ID { get; set; }
		/// <summary>
		/// Propriedade Nome
		/// </summary>
		public String Nome { get; set; }
        #endregion Propriedades
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Raizen.UniCad.Web.Models
{
	public class PlacaClientesAlteradosView
	{
		public virtual bool IsClientesAlterados { get; set; }
		public virtual bool IsOutrosDadosAlterados { get; set; }
	}
}
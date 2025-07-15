using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using Raizen.UniCad.BLL;
using System.Text;
using Raizen.UniCad.Model;
using Raizen.UniCad.Model.Filtro;
using Raizen.UniCad.Model.View;

namespace Raizen.UniCad.Web.Models
{
	public class ModelProduto : BaseModel, IValidatableObject
	{
		#region Constantes
		public ProdutoFiltro Filtro { get; set; }
		public Produto Produto { get; set; }
		public List<ProdutoView> ListaProduto { get; set; }
		#endregion

		#region Validação de Integridade
		public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
		{
			var results = new List<ValidationResult>();

			if (this.Produto != null)
			{
				ProdutoBusiness appBll = new ProdutoBusiness();
				TipoProdutoBusiness tipoProdutoBusiness = new TipoProdutoBusiness();

				if (!string.IsNullOrEmpty(this.Produto.Nome))
				{
					var idTiposPais = tipoProdutoBusiness.Listar(item => item.Pais == Filtro.Pais).Select(item => item.ID).ToList();
					Produto produto = appBll.Selecionar(item => item.Nome.Equals(this.Produto.Nome, StringComparison.OrdinalIgnoreCase) && idTiposPais.Contains(item.IDTipoProduto));

					if (string.IsNullOrEmpty(this.ChavePrimaria))
					{
						if (produto != null && produto.ID > 0)
						{
							results.Add(new ValidationResult("Já existe Produto com esse nome.", new string[] { "Produto_Nome" }));
							return results;
						}
					}
					else
					{
						Produto ProdutoOld = appBll.Selecionar(int.Parse(this.ChavePrimaria));

						if (produto != null && produto.ID != ProdutoOld.ID)
						{
							results.Add(new ValidationResult("Já existe Produto com esse nome.", new string[] { "Produto_Nome" }));
							return results;
						}

					}
				}
				else
				{
					if (Filtro.IDTipoProduto == null)
						Filtro.IDTipoProduto = 0;
				}

				if (this.Filtro.IDTipoProduto == 0)
				{
					results.Add(new ValidationResult("Preenchimento Obrigatório!", new string[] { "Filtro_IDTipoProduto" }));
				}

				if (this.Filtro.Pais == null)
				{
					results.Add(new ValidationResult("Preenchimento Obrigatório!", new string[] { "Filtro_Pais" }));
				}

				if (double.Equals(this.Produto.Densidade, 0))
				{
					results.Add(new ValidationResult("Preenchimento Obrigatório!", new string[] { "Produto_Densidade" }));
				}
			}

			return results;
		}
		#endregion
	}
}
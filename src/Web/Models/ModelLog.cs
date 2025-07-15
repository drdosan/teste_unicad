using Raizen.UniCad.Model;
using Raizen.UniCad.Model.Filtro;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Raizen.UniCad.Web.Models
{
    public class ModelLog : BaseModel, IValidatableObject
    {
        public List<Framework.Log.Model.LogErro> ListaLogs { get; set; }

        public Framework.Log.Model.LogErro Log { get; set; }

        public LogFiltro Filtro { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var results = new List<ValidationResult>();

            //Validação dos filtros de entrada.
            if (this.Filtro != null && this.Operacao == Framework.Models.OperacoesCRUD.List)
            {
                if (this.Filtro.DataInicial.HasValue && this.Filtro.DataFinal.HasValue
                    && this.Filtro.DataInicial > this.Filtro.DataFinal)
                {
                    results.Add(new ValidationResult("O período de ocorrência selecionado é inválido.", new string[] { "Filtro_DataInicial" }));
                }
            }

            return results;
        }
    }
}
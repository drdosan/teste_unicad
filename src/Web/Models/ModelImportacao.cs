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
    public class ModelImportacao : BaseModel, IValidatableObject
    {
        #region Constantes
        public ImportacaoFiltro Filtro { get; set; }
        public Importacao Importacao { get; set; }
        public List<ImportacaoView> ListaImportacao { get; set; }
        #endregion

        #region Validação de Integridade
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var results = new List<ValidationResult>();

            if (this.Importacao != null)
            {
               
            }

            return results;
        }

        #endregion

    }
}
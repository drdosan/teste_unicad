using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Raizen.UniCad.Model;
using Raizen.UniCad.Model.Filtro;
using Raizen.UniCad.Model.View;
using Raizen.UniCad.BLL;

namespace Raizen.UniCad.Web.Models
{
    public class ModelSincronizacaoMotoritas : BaseModel, IValidatableObject
    {
        #region Constantes
        public SincronizacaoMotoristasFiltro Filtro { get; set; }
        public List<Motorista> ListaMotoritas { get; set; }
        public List<SincronizacaoMotoritasView> ListaSincronizacaoMotoritas {get;set; }
        #endregion

        #region Validação de Integridade
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var results = new List<ValidationResult>();
            return results;
        }
        #endregion

    }
}
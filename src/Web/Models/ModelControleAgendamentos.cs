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
using Raizen.UniCad.Extensions;

namespace Raizen.UniCad.Web.Models
{
    public class ModelControleAgendamentos : BaseModel, IValidatableObject
    {
        #region Constantes
        public AgendamentoTerminalFiltro Filtro { get; set; }
        public List<AgendamentoTerminalView> ListaAgendamentoTerminal { get; set; }
      

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
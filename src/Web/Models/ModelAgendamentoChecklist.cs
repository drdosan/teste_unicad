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
    public class ModelAgendamentoChecklist : BaseModel, IValidatableObject
    {
        #region Constantes
        public AgendamentoChecklistFiltro Filtro { get; set; }
        public AgendamentoChecklist AgendamentoChecklist { get; set; }
        public List<AgendamentoChecklistView> ListaAgendamentoChecklist { get; set; }
        public List<AgendamentoTerminalHorarioView> ListaAgendamentoTerminalHorario { get; set; }
        public int? IDComposicao { get; set; }
        public string Placas { get; set; }
        public string Mensagem { get; set; }
        #endregion

        #region Validação de Integridade
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var results = new List<ValidationResult>();

            if (this.AgendamentoChecklist != null)
            {
                AgendamentoChecklistBusiness appBll = new AgendamentoChecklistBusiness();
            }

            return results;
        }
        #endregion

    }
}
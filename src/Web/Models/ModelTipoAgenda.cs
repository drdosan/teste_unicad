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
    public class ModelTipoAgenda : BaseModel, IValidatableObject
    {
        #region Constantes
        public TipoAgendaFiltro Filtro { get; set; }
        public TipoAgenda TipoAgenda { get; set; }
        public List<TipoAgendaView> ListaTipoAgenda { get; set; }
        #endregion

        #region Validação de Integridade
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var results = new List<ValidationResult>();

            if (this.TipoAgenda != null)
            {
                TipoAgendaBusiness appBll = new TipoAgendaBusiness();

                if (!string.IsNullOrEmpty(this.TipoAgenda.Nome))
                {
                    if (string.IsNullOrEmpty(this.ChavePrimaria))
                    {
                        TipoAgenda TipoAgenda = appBll.Selecionar(item => item.Nome.Equals(this.TipoAgenda.Nome, StringComparison.OrdinalIgnoreCase));
                        if (TipoAgenda != null && TipoAgenda.ID > 0)
                        {
                            results.Add(new ValidationResult("Já existe Tipo de Agenda com esse nome.", new string[] { "TipoAgenda_Nome" }));
                            return results;
                        }
                    }
                    if (!string.IsNullOrEmpty(this.ChavePrimaria))
                    {
                        TipoAgenda TipoAgendaOld = appBll.Selecionar(int.Parse(this.ChavePrimaria));

                        if (!TipoAgendaOld.Nome.Equals(this.TipoAgenda.Nome))
                        {
                            TipoAgenda TipoAgenda = appBll.Selecionar(item => item.Nome.Equals(this.TipoAgenda.Nome));

                            if (TipoAgenda != null)
                            {
                                results.Add(new ValidationResult("Já existe Tipo de Agenda com esse nome.", new string[] { "TipoAgenda_Nome" }));
                                return results;
                            }

                        }
                    }
                }
            }

            return results;
        }
        #endregion

    }
}
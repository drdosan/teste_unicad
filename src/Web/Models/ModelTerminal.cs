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
    public class ModelTerminal : BaseModel, IValidatableObject
    {
        #region Constantes
        public TerminalFiltro Filtro { get; set; }
        public Terminal Terminal { get; set; }
        public List<Terminal> ListaTerminal { get; set; }
        #endregion

        #region Validação de Integridade
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var results = new List<ValidationResult>();

            if (this.Terminal != null)
            {
                TerminalBusiness appBll = new TerminalBusiness();

                if (!string.IsNullOrEmpty(this.Terminal.Nome))
                {
                    if (string.IsNullOrEmpty(this.ChavePrimaria))
                    {
                        Terminal Terminal = appBll.Selecionar(item => item.Nome.Equals(this.Terminal.Nome, StringComparison.OrdinalIgnoreCase));
                        if (Terminal != null && Terminal.ID > 0)
                        {
                            results.Add(new ValidationResult("Já existe Terminal com esse nome.", new string[] { "Terminal_Nome" }));
                            return results;
                        }
                    }
                    if (!string.IsNullOrEmpty(this.ChavePrimaria))
                    {
                        Terminal TerminalOld = appBll.Selecionar(int.Parse(this.ChavePrimaria));

                        if (!TerminalOld.Nome.Equals(this.Terminal.Nome))
                        {
                            Terminal Terminal = appBll.Selecionar(item => item.Nome.Equals(this.Terminal.Nome));

                            if (Terminal != null)
                            {
                                results.Add(new ValidationResult("Já existe Terminal com esse nome.", new string[] { "Terminal_Nome" }));
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
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
    public class ModelTerminalEmpresa : BaseModel, IValidatableObject
    {
        #region Constantes
        public TerminalEmpresaFiltro Filtro { get; set; }
        public TerminalEmpresa TerminalEmpresa { get; set; }
        public List<TerminalEmpresa> ListaTerminalEmpresa { get; set; }
        #endregion

        #region Validação de Integridade
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var results = new List<ValidationResult>();

            if (this.TerminalEmpresa != null)
            {
                TerminalEmpresaBusiness appBll = new TerminalEmpresaBusiness();

                if (!string.IsNullOrEmpty(this.TerminalEmpresa.Nome))
                {
                    if (string.IsNullOrEmpty(this.ChavePrimaria))
                    {
                        TerminalEmpresa TerminalEmpresa = appBll.Selecionar(item => item.Nome.Equals(this.TerminalEmpresa.Nome, StringComparison.OrdinalIgnoreCase) && item.IDTerminal == this.TerminalEmpresa.IDTerminal);
                        if (TerminalEmpresa != null && TerminalEmpresa.ID > 0)
                        {
                            results.Add(new ValidationResult("Já existe Empresa com esse nome.", new string[] { "TerminalEmpresa_Nome" }));
                            return results;
                        }
                    }
                    if (!string.IsNullOrEmpty(this.ChavePrimaria))
                    {
                        TerminalEmpresa TerminalEmpresaOld = appBll.Selecionar(int.Parse(this.ChavePrimaria));

                        if (!TerminalEmpresaOld.Nome.Equals(this.TerminalEmpresa.Nome))
                        {
                            TerminalEmpresa TerminalEmpresa = appBll.Selecionar(item => item.Nome.Equals(this.TerminalEmpresa.Nome) && item.IDTerminal == this.TerminalEmpresa.IDTerminal);

                            if (TerminalEmpresa != null)
                            {
                                results.Add(new ValidationResult("Já existe Empresa com esse nome.", new string[] { "TerminalEmpresa_Nome" }));
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
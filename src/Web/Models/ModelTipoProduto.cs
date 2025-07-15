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
using System.Web.Mvc;

namespace Raizen.UniCad.Web.Models
{
    public class ModelTipoDocumento : BaseModel, IValidatableObject
    {
        #region Constantes
        public TipoDocumentoFiltro Filtro { get; set; }
        public TipoDocumento TipoDocumento { get; set; }
        public List<TipoDocumentoView> ListaTipoDocumento { get; set; }
        #endregion

        #region Validação de Integridade
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var results = new List<ValidationResult>();

            if (this.TipoDocumento != null)
            {
                var appBll = new TipoDocumentoBusiness();

                if ((this.TipoDocumento.TipoAcaoVencimento != (int)EnumTipoAcaoVencimento.SemAcao) &&
                    (this.TipoDocumento.BloqueioImediato == (int)EnumTipoBloqueioImediato.Nao && !this.TipoDocumento.QtdDiasBloqueio.HasValue)
                   )
                {
                    results.Add(new ValidationResult("Campo obrigatório.", new string[] { "TipoDocumento_QtdDiasBloqueio" }));
                    return results;
                }

                if(this.TipoDocumento.BloqueioImediato == (int)EnumTipoBloqueioImediato.Nao && this.TipoDocumento.QtdDiasBloqueio.HasValue && TipoDocumento.QtdDiasBloqueio.Value == 0)
                {
                    results.Add(new ValidationResult("O valor precisa ser maior que zero.", new string[] { "TipoDocumento_QtdDiasBloqueio" }));
                    return results;
                }

                if (!string.IsNullOrEmpty(this.TipoDocumento.Sigla))
                {
                    if (!string.IsNullOrEmpty(this.ChavePrimaria))
                    {
                        TipoDocumento TipoDocumentoOld = appBll.Selecionar(int.Parse(this.ChavePrimaria));

                        if (!TipoDocumentoOld.Sigla.Equals(this.TipoDocumento.Sigla))
                        {
                            TipoDocumento TipoDocumento = appBll.Selecionar(item => item.Sigla.Equals(this.TipoDocumento.Sigla));

                            if (TipoDocumento != null)
                            {
                                results.Add(new ValidationResult("Já existe TipoDocumento com essa Sigla.", new string[] { "TipoDocumento_Sigla" }));
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
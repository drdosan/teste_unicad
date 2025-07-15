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
    public class ModelLogDocumentos : BaseModel, IValidatableObject
    {
        #region Constantes

        public LogDocumentosFiltro Filtro { get; set; }
        public LogDocumentos LogDocumentos { get; set; }
        public List<LogDocumentosView> ListaLogDocumentos { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            return new List<ValidationResult>();
        }

        #endregion
    }
}
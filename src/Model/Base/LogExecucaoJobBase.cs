#region Detalhes Gerador

// Assembly: Raizen.Gerador.UI
// Versão: 1.1.0.14463
// Data Geração: 15/03/2019 11:47:31

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using DataAnnotationsExtensions;

using Raizen.Framework.Models;
using Raizen.Framework.Resources;
using Raizen.Framework.Utils.CustomAnnotation;

namespace Raizen.UniCad.Model.Base
{
    public abstract class LogExecucaoJobBase : BaseModel
    {
        [Required(ErrorMessageResourceType = typeof(MensagensPadrao), ErrorMessageResourceName = "CampoObrigatorio")]
		[Integer(ErrorMessageResourceType = typeof(MensagensPadrao), ErrorMessageResourceName = "FieldMustBeNumeric")]
		public virtual int Id { get; set; }

		[Required(ErrorMessageResourceType = typeof(MensagensPadrao), ErrorMessageResourceName = "CampoObrigatorio")]
		public virtual DateTime Data { get; set; }

		[Required(ErrorMessageResourceType = typeof(MensagensPadrao), ErrorMessageResourceName = "CampoObrigatorio")]
		[StringLength(255, ErrorMessageResourceType = typeof(MensagensPadrao), ErrorMessageResourceName = "TamanhoIrregular")]
		public virtual string Job { get; set; }

		[Required(ErrorMessageResourceType = typeof(MensagensPadrao), ErrorMessageResourceName = "CampoObrigatorio")]
		[StringLength(255, ErrorMessageResourceType = typeof(MensagensPadrao), ErrorMessageResourceName = "TamanhoIrregular")]
		public virtual string Titulo { get; set; }

		[Required(ErrorMessageResourceType = typeof(MensagensPadrao), ErrorMessageResourceName = "CampoObrigatorio")]
		[StringLength(2000, ErrorMessageResourceType = typeof(MensagensPadrao), ErrorMessageResourceName = "TamanhoIrregular")]
		public virtual string Descricao { get; set; }

		[Required(ErrorMessageResourceType = typeof(MensagensPadrao), ErrorMessageResourceName = "CampoObrigatorio")]
		[Integer(ErrorMessageResourceType = typeof(MensagensPadrao), ErrorMessageResourceName = "FieldMustBeNumeric")]
		public virtual int Codigo { get; set; }

		
    }
}

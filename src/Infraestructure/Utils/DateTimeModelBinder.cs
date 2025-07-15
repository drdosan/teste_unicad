using Raizen.Framework.Resources;
using System;
using System.Globalization;
using System.Web.Mvc;

namespace Raizen.UniCad.Web
{
    public class DateTimeModelBinder : IModelBinder
    {
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var valueResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            if (valueResult == null)
                return null;
            DateTime dt;
            if (!DateTime.TryParse(valueResult.AttemptedValue, CultureInfo.CurrentCulture, DateTimeStyles.None, out dt))
            {
                //Tratamento para incluir erro de validação de data.
                if (!string.IsNullOrEmpty(valueResult.AttemptedValue))
                    bindingContext.ModelState.AddModelError(bindingContext.ModelName, MensagensPadrao.FieldMustBeDate);
                return null;
            }
            return dt;
        }
    }
}

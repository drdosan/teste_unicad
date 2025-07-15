using System;
using System.Globalization;
using System.Web.Mvc;

namespace Raizen.UniCad.Web
{
    public class DoubleModelBinder : IModelBinder
    {
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var valueResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            var modelState = new ModelState { Value = valueResult };
            object actualValue = null;

            try
            {
                if(!string.IsNullOrEmpty(valueResult.AttemptedValue))
                actualValue = Convert.ToDouble(valueResult.AttemptedValue, CultureInfo.CurrentCulture);
            }
            catch (FormatException)
            {
                modelState.Errors.Add("Formato inválido!");
            }

            bindingContext.ModelState.Add(bindingContext.ModelName, modelState);
            return actualValue;
        }
    }
}

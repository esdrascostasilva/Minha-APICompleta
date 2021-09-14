using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace DevDe.Api.Extensions
{
    public class JsonWithFilesFormDataModelBinder : IModelBinder
    {
        private readonly FormFileModelBinder _formFileModelBinder;
        private readonly IOptions<JsonOptions> _jsonOptions;

        public JsonWithFilesFormDataModelBinder(IOptions<JsonOptions> jsonOptions, ILoggerFactory loggerFactory)
        {
            _formFileModelBinder = new FormFileModelBinder(loggerFactory);
            _jsonOptions = jsonOptions;
        }

        public async Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
                throw new ArgumentNullException(nameof(bindingContext));

            // Retrieve the form part containing the Json
            var valueResult = bindingContext.ValueProvider.GetValue(bindingContext.FieldName);
            if (valueResult == ValueProviderResult.None)
            {
                // The Json was not found
                var message = bindingContext.ModelMetadata.ModelBindingMessageProvider.MissingBindRequiredValueAccessor(bindingContext.FieldName);
                bindingContext.ModelState.TryAddModelError(bindingContext.ModelName, message);
                return;
            }

            var rawValue = valueResult.FirstValue; //SerializerSettings

            // Deserialize the Json
            var model = JsonConvert.DeserializeObject(rawValue, bindingContext.ModelType);//, _jsonOptions.Value.JsonSerializerOptions); //

            foreach (var property in bindingContext.ModelMetadata.Properties)
            {
                if (property.ModelType != typeof(IFormFile))
                    continue;

                var fieldName = property.BinderModelName ?? property.PropertyName;
                var modelName = fieldName;
                var propertyModel = property.PropertyGetter(bindingContext.Model);
                ModelBindingResult propertyResult;
                
                using(bindingContext.EnterNestedScope(property, fieldName, modelName, propertyModel))
                {
                    await _formFileModelBinder.BindModelAsync(bindingContext);
                    propertyResult = bindingContext.Result;
                }

                if (propertyResult.IsModelSet)
                {
                    property.PropertySetter(model, propertyResult.Model);
                }
                else if(property.IsBindingRequired)
                {
                    var message = property.ModelBindingMessageProvider.MissingBindRequiredValueAccessor(fieldName);
                    bindingContext.ModelState.TryAddModelError(modelName, message);
                }

            }

            bindingContext.Result = ModelBindingResult.Success(model);
        }
    }
}

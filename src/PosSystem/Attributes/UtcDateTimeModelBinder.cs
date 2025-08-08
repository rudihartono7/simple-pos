using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace PosSystem.Attributes
{
    public class UtcDateTimeModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
                throw new ArgumentNullException(nameof(bindingContext));

            var modelType = bindingContext.ModelType;
            var isNullableDateTime = modelType == typeof(DateTime?) || modelType == typeof(DateTime);

            if (!isNullableDateTime)
            {
                return Task.CompletedTask;
            }

            var value = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            if (value == ValueProviderResult.None)
            {
                return Task.CompletedTask;
            }

            bindingContext.ModelState.SetModelValue(bindingContext.ModelName, value);

            var stringValue = value.FirstValue;
            if (string.IsNullOrEmpty(stringValue))
            {
                if (modelType == typeof(DateTime?))
                {
                    bindingContext.Result = ModelBindingResult.Success(null);
                }
                return Task.CompletedTask;
            }

            if (DateTime.TryParse(stringValue, out var dateTime))
            {
                // Convert to UTC
                var utcDateTime = dateTime.Kind == DateTimeKind.Unspecified
                    ? DateTime.SpecifyKind(dateTime, DateTimeKind.Utc)
                    : dateTime.ToUniversalTime();

                bindingContext.Result = ModelBindingResult.Success(utcDateTime);
            }
            else
            {
                bindingContext.ModelState.TryAddModelError(bindingContext.ModelName, "Invalid date format");
                bindingContext.Result = ModelBindingResult.Failed();
            }

            return Task.CompletedTask;
        }
    }

    public class UtcDateTimeModelBinderProvider : IModelBinderProvider
    {
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context.Metadata.ModelType == typeof(DateTime) || 
                context.Metadata.ModelType == typeof(DateTime?))
            {
                return new UtcDateTimeModelBinder();
            }

            return null;
        }
    }
}
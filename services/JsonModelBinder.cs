using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;

public class JsonModelBinder : IModelBinder
{
    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        var val = bindingContext.ValueProvider.GetValue(bindingContext.ModelName).FirstValue;

        if (string.IsNullOrEmpty(val))
        {
            bindingContext.Result = ModelBindingResult.Success(null);
            return Task.CompletedTask;
        }

        try
        {
            var result = JsonConvert.DeserializeObject(val, bindingContext.ModelType);
            bindingContext.Result = ModelBindingResult.Success(result);
        }
        catch (Exception)
        {
            bindingContext.ModelState.TryAddModelError(bindingContext.ModelName, "Invalid JSON.");
        }

        return Task.CompletedTask;
    }
}

using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace VideoCollabServer.Utils;

public static class ModelStateUtils
{
    public static List<string> GetErrorsList(this ModelStateDictionary modelState)
    {
        var errors = new List<string>();
        
        foreach (var val in modelState.Values)
        {
            errors.AddRange(val.Errors.Select(er => er.ErrorMessage));
        }
        return errors;
    } 
}
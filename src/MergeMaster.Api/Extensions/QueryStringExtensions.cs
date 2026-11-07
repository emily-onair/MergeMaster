using Microsoft.AspNetCore.WebUtilities;

namespace MergeMaster.Api.Extensions;
public static class QueryStringExtensions
{
    public static string UpdateQueryString(this HttpRequest request, 
        params (string key, string? value)[] changes)
    {
        var queryString = QueryHelpers.ParseQuery(request.QueryString.Value);
        foreach (var (key, value) in changes)
        {
            if (value is null) queryString.Remove(key);
            else queryString[key] = value;
        }
        return request.Path + QueryString.Create(queryString);
    }
}
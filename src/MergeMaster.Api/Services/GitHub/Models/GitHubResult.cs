namespace MergeMaster.Api.Services.GitHub.Models;

using MergeMaster.Api.Models;

public class GitHubResult<T>
{
    public T? Value { get; }
    public HttpError? Error { get; }
    public bool Proceed { get; }

    private GitHubResult(T? value, HttpError? error, bool proceed)
    {
        if (!proceed && error is null) throw new ArgumentException("Error must be provided if Proceed is false");
        Value = value;
        Error = error;
        Proceed = proceed;
    }
    public static GitHubResult<T> Success(T value) => 
        new(value, null, true);

    public static GitHubResult<T> ProceedDespiteError(T? fallbackValue) => 
        new(fallbackValue, null, true);

    public static GitHubResult<T> Fail(HttpError error) 
    {
        ArgumentNullException.ThrowIfNull(error);
        return new(default, error, false);
    }
}
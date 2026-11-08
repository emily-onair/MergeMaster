namespace MergeMaster.Api.Services.GitHub.Models;

using System.Diagnostics.CodeAnalysis;
using MergeMaster.Api.Models;

public class GitHubResult<T>
{
    public T? Value { get; }
    public HttpError? Error { get; }

    [MemberNotNullWhen(false,nameof(Error))]
    public bool Proceed { get; }


    private GitHubResult(T? value, HttpError? error, bool proceed)
    {
        Value = value;
        Error = error;
        Proceed = proceed;
    }
    public static GitHubResult<T> Success(T value) => 
        new(value, null, true);

    public static GitHubResult<T> ProceedDespiteError() => 
        new(default, null, true);

    public static GitHubResult<T> Fail(HttpError error) 
    {
        ArgumentNullException.ThrowIfNull(error);
        return new(default, error, false);
    }
}
namespace MergeMaster.Api.Services.GitHub.Models;

using System.Diagnostics.CodeAnalysis;
using MergeMaster.Api.Models;

public class GitHubResult<T>
{
    public T? Value { get; }
    public HttpError? Error { get; }

    [MemberNotNullWhen(false,nameof(Error))]
    public bool Proceed { get; }

    public bool IsUnavailable { get; }
    private GitHubResult(T? value, HttpError? error, bool proceed, bool isUnavailable)
    {
        Value = value;
        Error = error;
        Proceed = proceed;
        IsUnavailable = isUnavailable;
    }
    public static GitHubResult<T> Success(T value) => 
        new(value, null, true, false);

    public static GitHubResult<T> ProceedDespiteError(bool isUnavailable = false) => 
        new(default, null, true, isUnavailable);

    public static GitHubResult<T> Fail(HttpError error, bool isUnavailable = false) 
    {
        ArgumentNullException.ThrowIfNull(error);
        return new(default, error, false, isUnavailable);
    }
}
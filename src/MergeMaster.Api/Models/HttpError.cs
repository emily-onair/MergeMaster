using System.Net;

namespace MergeMaster.Api.Models;

public class HttpError (HttpStatusCode statusCode, string message)
{
    public HttpStatusCode StatusCode { get; } = statusCode;
    public string Message { get; } = message;
}

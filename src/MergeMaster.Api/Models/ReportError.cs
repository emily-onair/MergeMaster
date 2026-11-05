namespace MergeMaster.Api.Models;

public class ReportError
{
    public string? Message { get; set;}
    public DateTimeOffset Time { get; set; }
    public ReportErrorTypes ErrorType { get; set; }
}
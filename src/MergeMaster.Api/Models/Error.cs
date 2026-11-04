namespace MergeMaster.Api.Models;

public class ReportError
{
    public string? Message = null;
    public DateTimeOffset Time;
    public ReportErrorTypes ErrorType;
}
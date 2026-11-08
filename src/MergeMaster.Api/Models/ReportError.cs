namespace MergeMaster.Api.Models;

public class ReportSyncError
{
    public string? Message { get; set;}
    public DateTimeOffset Time { get; set; }
    public ReportSyncErrorTypes ErrorType { get; set; }
}
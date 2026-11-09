namespace MergeMaster.Api.Models;

public class ReportSyncFlagEntity
{
    public int Id { get; set; }
    public int ReportId { get; set; }
    public string? Message { get; set;}
    public DateTime DetectedAtUtc { get; set; }
    public ReportSyncFlagTypes FlagType { get; set; }
}
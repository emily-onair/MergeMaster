using MergeMaster.Api.Models;

namespace MergeMaster.Api.DAL.Models;

public class SyncFlagEntity
{
    public int Id { get; init; }
    public int ReportId { get; init; }
    public SyncFlagTypes FlagType { get; init; }
    public DateTime DetectedAtUtc { get; set; }
    // public string? Details { get; set;}
}
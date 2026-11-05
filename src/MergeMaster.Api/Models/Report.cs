namespace MergeMaster.Api.Models;

public sealed class Report : ReportBase
{
    public DateTimeOffset CreatedAt { get; init; }
    public DateTimeOffset? CommitDate { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
    public DateTimeOffset? ArchivedAt { get; set; }
};
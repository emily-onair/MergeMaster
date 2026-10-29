namespace MergeMaster.Api.Data;

public sealed class ReportEntity
{
    public int Id { get; init; }
    public DateTime CreatedAtUtc { get; init; }
    public string Repository { get; init; } = "";
    public string Branch { get; init; } = "";
    public string CommitSha { get; init; } = "";
    public string? CommitMessage { get; set; }
    public DateTime? CommitDateUtc { get; set; }
    public string AuthorName { get; init; } = "";
    public string AuthorEmail { get; init; } = "";
    public bool Urgent { get; set; }
    public string Tags { get; set; } = "";
    public string? Notes { get; set; }
    public DateTime? UpdatedAtUtc { get; set; }
    public bool Validated { get; set; }
    public DateTime? ArchivedAtUtc { get; set; }
}
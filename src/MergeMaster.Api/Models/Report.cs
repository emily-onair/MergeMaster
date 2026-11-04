namespace MergeMaster.Api.Models;

public sealed class Report
{
    public int Id { get; init; } 
    // bool Synced,
    public DateTimeOffset CreatedAt { get; init; }
    public string Repository { get; init; } = "";
    public string Branch { get; init; } = ""; 
    public string CommitSha { get; init; } = "";
    public string? CommitMessage { get; set; }
    public DateTimeOffset? CommitDate { get; set; }
    public string AuthorName { get; init; } = "";
    public string AuthorEmail { get; init; } = "";
    public bool Urgent { get; set; }
    public string Tags { get; set; } = "";
    public string? Notes { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
    public bool Validated { get; set; }
    public DateTimeOffset? ArchivedAt { get; set; }
    public ReportError? Error { get; set; }
};
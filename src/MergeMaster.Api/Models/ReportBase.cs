namespace MergeMaster.Api.Models;

public abstract class ReportBase
{
    public int Id { get; init; }
    public string Repository { get; init; } = "";
    public string Branch { get; set; } = "";
    public string CommitSha { get; init; } = "";
    public string? CommitMessage { get; set; }
    public string AuthorName { get; init; } = "";
    public string AuthorEmail { get; init; } = "";
    public bool Urgent { get; set; }
    public string Tags { get; set; } = "";
    public string? Notes { get; set; }
    public bool Validated { get; set; }
    // public ReportError? Error { get; set; }
}
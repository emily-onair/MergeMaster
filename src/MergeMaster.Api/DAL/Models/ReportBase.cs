namespace MergeMaster.Api.DAL.Models;

public abstract class ReportBase
{
    public int Id { get; init; }
    public string CommitSha { get; init; } = "";
    public string? CommitMessage { get; set; }
    public bool Urgent { get; set; }
}
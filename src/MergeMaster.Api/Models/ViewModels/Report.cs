using MergeMaster.Api.DAL.Models;

namespace MergeMaster.Api.Models.ViewModels;

public sealed class Report : ReportBase
{
    public string AuthorName { get; set; } = "";
    public string Branch { get; set; } = "";
    public string Repo { get; set; } = "";
    public List<string> Tags { get; set; } = [];
    // public List<ReportFlag> Flags { get; set; } = [];
    public bool Flagged { get; set; }
    public DateTimeOffset CreatedAt { get; init; }
    public DateTimeOffset? CommitDate { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
    public DateTimeOffset? ArchivedAt { get; set; }
};
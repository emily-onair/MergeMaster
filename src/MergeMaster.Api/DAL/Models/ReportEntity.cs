using MergeMaster.Api.Models.ViewModels;

namespace MergeMaster.Api.DAL.Models;

public class ReportEntity : ReportBase
{
    public int BranchId { get; set; }
    public BranchEntity Branch { get; set; } = null!;
    public int AuthorId { get; init; }
    public AuthorEntity Author { get; set; } = null!;
    public List<TagEntity> Tags { get; set; } = [];
    public DateTime CreatedAtUtc { get; init; }
    public DateTime? CommitDateUtc { get; set; }
    public DateTime? UpdatedAtUtc { get; set; }
    public DateTime? ArchivedAtUtc { get; set; }
    // public int RepositoryId {get; set;}

    public List<SyncFlagEntity> SyncFlags { get; set; } = [];
    public Report ToReport() => new()
    {
        Id = Id,
        CreatedAt = new DateTimeOffset(CreatedAtUtc, TimeSpan.Zero),
        Repo = Branch.Repo.Repository,
        Branch = Branch.Name,
        CommitSha = CommitSha,
        CommitMessage = CommitMessage,
        CommitDate = CommitDateUtc is null ? null : new DateTimeOffset(CommitDateUtc.Value, TimeSpan.Zero),
        AuthorName = Author.Name,
        Urgent = Urgent,
        Tags = [.. Tags.Select(t => t.Name)],
        Flags = [.. SyncFlags.Select(f => 
            new ReportFlag(new DateTimeOffset(f.DetectedAtUtc), f.FlagType))],
        // Notes = Notes,
        UpdatedAt = UpdatedAtUtc is null ? null : new DateTimeOffset(UpdatedAtUtc.Value, TimeSpan.Zero),
        ArchivedAt = ArchivedAtUtc is null ? null : new DateTimeOffset(ArchivedAtUtc.Value, TimeSpan.Zero)
    };
}
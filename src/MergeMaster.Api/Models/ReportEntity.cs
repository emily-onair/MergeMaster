namespace MergeMaster.Api.Models;

public sealed class ReportEntity : ReportBase
{
    public DateTime CreatedAtUtc { get; init; }
    public DateTime? CommitDateUtc { get; set; }
    public DateTime? UpdatedAtUtc { get; set; }
    public DateTime? ArchivedAtUtc { get; set; }
    public Report ToReport() => new()
    {
        Id = Id,
        CreatedAt = new DateTimeOffset(CreatedAtUtc, TimeSpan.Zero),
        Repository = Repository,
        Branch = Branch,
        CommitSha = CommitSha,
        CommitMessage = CommitMessage,
        CommitDate = CommitDateUtc is null ? null : new DateTimeOffset(CommitDateUtc.Value, TimeSpan.Zero),
        AuthorName = AuthorName,
        AuthorEmail = AuthorEmail,
        Urgent = Urgent,
        Tags = Tags,
        Notes = Notes,
        UpdatedAt = UpdatedAtUtc is null ? null : new DateTimeOffset(UpdatedAtUtc.Value, TimeSpan.Zero),
        Validated = Validated,
        ArchivedAt = ArchivedAtUtc is null ? null : new DateTimeOffset(ArchivedAtUtc.Value, TimeSpan.Zero)
    };
}
namespace MergeMaster.Api;

public sealed record AddReportRequestBody(
    string OwnerRepo,
    string Repository,
    string Branch,
    string CommitSha,
    bool Urgent,
    string Tags,
    string? Notes
);
public sealed record NewReport(
    string Repository, 
    string Branch, 
    string CommitSha,
    string CommitMessage,
    DateTimeOffset CommitDate,
    string AuthorName,
    string AuthorEmail,
    bool Urgent,
    string Tags,
    string? Notes
);

public sealed record Report(
    int Id, 
    // bool Synced,
    DateTimeOffset CreatedAt,
    string Repository, 
    string Branch, 
    string CommitSha,
    string CommitMessage,
    DateTimeOffset CommitDate,
    string AuthorName,
    string AuthorEmail,
    bool Urgent,
    string Tags,
    string? Notes,
    DateTimeOffset? UpdatedAt
);

// Lives as long as the app (singleton), so it is shared by all requests
// and needs a lock. Chapter 6 replaces this with EF Core.
public sealed class ReportStore
{
    private readonly Lock _lock = new();
    private readonly List<Report> _reports = [];

    public Report Add(NewReport input)
    {
        lock (_lock)
        {
            var report = new Report(
                _reports.Count + 1,
                DateTimeOffset.UtcNow,
                input.Repository, 
                input.Branch, 
                input.CommitSha,
                input.CommitMessage,
                input.CommitDate,
                input.AuthorName,
                input.AuthorEmail,
                input.Urgent,
                input.Tags,
                input.Notes,
                null
            );
            _reports.Add(report);
            return report;
        }
    }

    public IReadOnlyList<Report> All()
    {
        lock (_lock)
        {
            return _reports.ToList();
        }
    }
}
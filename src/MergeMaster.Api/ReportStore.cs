namespace MergeMaster.Api;

// Placeholder shapes. Replace once your design note defines the real report.
public sealed record NewReport(
    string Repository, 
    string Branch, 
    string CommitSha,
    string CommitMessage,
    string AuthorName,
    string AuthorEmail,
    bool Urgent,
    string Tags,
    string? Notes
);

public sealed record Report(
    int Id, 
    bool Synced,
    DateTimeOffset ReceivedAt,
    string Repository, 
    string Branch, 
    string CommitSha,
    string CommitMessage,
    string AuthorName,
    string AuthorEmail,
    bool Urgent,
    string Tags,
    string? Notes
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
                false,
                DateTimeOffset.UtcNow,
                input.Repository, 
                input.Branch, 
                input.CommitSha,
                input.CommitMessage,
                input.AuthorName,
                input.AuthorEmail,
                input.Urgent,
                input.Tags,
                input.Notes
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
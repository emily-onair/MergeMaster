namespace MergeMaster.Api.Data;
using MergeMaster.Api.Models;

// Lives as long as the app (singleton), so it is shared by all requests
// and needs a lock. Chapter 6 replaces this with EF Core.
public sealed class ReportStore
{
    private readonly Lock _lock = new();
    private readonly List<Report> _reports = [];

    // Loads existing reports as-is (keeps their Ids and dates). Dev only.
    public void Seed(IEnumerable<Report> reports)
    {
        lock (_lock)
        {
            _reports.AddRange(reports);
        }
    }

    public Report Add(NewReport input)
    {
        lock (_lock)
        {
            var report = new Report
            {
                Id = _reports.Count + 1,
                CreatedAt = DateTimeOffset.UtcNow,
                Repository = input.Repository, 
                Branch = input.Branch, 
                CommitSha = input.CommitSha,
                CommitMessage = input.CommitMessage,
                CommitDate = input.CommitDate,
                AuthorName = input.AuthorName,
                AuthorEmail = input.AuthorEmail,
                Urgent = input.Urgent,
                Tags = input.Tags,
                Notes = input.Notes,
                UpdatedAt = null
            };
            _reports.Add(report);
            return report;
        }
    }

    public bool ReportUpdated(NewReport newReport)
    {
        var report = _reports.Find(r => r.Branch == newReport.Branch);
        if (report is not null)
        {
            report.CommitSha = newReport.CommitSha;
            report.CommitMessage = newReport.CommitMessage;
            report.CommitDate = newReport.CommitDate;
            report.AuthorName = newReport.AuthorName;
            report.AuthorEmail = newReport.AuthorEmail;
            report.Urgent = newReport.Urgent;
            report.Tags = newReport.Tags;
            report.UpdatedAt = DateTimeOffset.UtcNow;
            return true;
        }
        return false;
    }

    public bool ReportForBranchExists(string branch)
    {
        var inList = _reports.Count(r => r.Branch == branch);
        return inList > 0;
    }

    public bool ReportIsDuplicate(string commit)
    {
        var inList = _reports.Count(r => r.CommitSha == commit);
        return inList > 0;
    }

    public IReadOnlyList<Report> All()
    {
        lock (_lock)
        {
            return [.. _reports];
        }
    }

    public IReadOnlyList<Report> UrgentOnly()
    {
        lock (_lock)
        {
            return [.. _reports.Where(r => r.Urgent == true)];
        }
    }
}
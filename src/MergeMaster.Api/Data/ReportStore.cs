namespace MergeMaster.Api.Data;
using MergeMaster.Api.Models;

public sealed class ReportStore(ReportDb db)
{
    // public void Seed(IEnumerable<Report> reports)
    // {
    //     lock (_lock)
    //     {
    //         _reports.AddRange(reports);
    //     }
    // }

    // public Report Add(NewReport input)
    // {
    //     {
    //         var report = new Report
    //         {
    //             CreatedAt = DateTimeOffset.UtcNow,
    //             Repository = input.Repository, 
    //             Branch = input.Branch, 
    //             CommitSha = input.CommitSha,
    //             CommitMessage = input.CommitMessage ?? "",
    //             CommitDate = input.CommitDate ?? default,
    //             AuthorName = input.AuthorName,
    //             AuthorEmail = input.AuthorEmail ?? "",
    //             Urgent = input.Urgent,
    //             Tags = input.Tags,
    //             Notes = input.Notes,
    //             UpdatedAt = null,
    //             Validated = input.Validated
    //         };
    //         _reports.Add(report);
    //         return report;
    //     }
    // }

    // public bool CheckForDuplicate(string commit)
    // {
    //     // NOTE: remove global filters to include archived reports for this check
    //     var inList = _reports.Count(r => r.CommitSha == commit);
    //     return inList > 0;
    // }

    // public IReadOnlyList<Report> FindAll()
    // {
    //     {
    //         return [.. _reports];
    //     }
    // }

    // public Report? FindById(int id)
    // {
    //     {
    //         return _reports.Find(r => r.Id == id);
    //     }
    // }

    // public IReadOnlyList<Report> FindUrgentOnly()
    // {
    //     {
    //         return [.. _reports.Where(r => r.Urgent == true)];
    //     }
    // }

    // public bool Delete(int id)
    // {
    //     {
    //         var toDelete = FindById(id);
    //         if (toDelete is not null)
    //             return _reports.Remove(toDelete);
    //         return false;
    //     }
    // }
}
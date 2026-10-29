// TODO: delete

// namespace MergeMaster.Api.Data;
// using MergeMaster.Api.Models;

// // Lives as long as the app (singleton), so it is shared by all requests
// // and needs a lock. Chapter 6 replaces this with EF Core.
// public sealed class ReportStoreTest
// {
//     private readonly Lock _lock = new();
//     private readonly List<Report> _reports = [];

//     // Loads existing reports as-is (keeps their Ids and dates). Dev only.
//     public void Seed(IEnumerable<Report> reports)
//     {
//         lock (_lock)
//         {
//             _reports.AddRange(reports);
//         }
//     }

//     public Report Add(NewReport input)
//     {
//         lock (_lock)
//         {
//             var report = new Report
//             {
//                 Id = _reports.Count + 1,
//                 CreatedAt = DateTimeOffset.UtcNow,
//                 Repository = input.Repository, 
//                 Branch = input.Branch, 
//                 CommitSha = input.CommitSha,
//                 CommitMessage = input.CommitMessage ?? "",
//                 CommitDate = input.CommitDate ?? default,
//                 AuthorName = input.AuthorName,
//                 AuthorEmail = input.AuthorEmail ?? "",
//                 Urgent = input.Urgent,
//                 Tags = input.Tags,
//                 Notes = input.Notes,
//                 UpdatedAt = null,
//                 Validated = input.Validated
//             };
//             _reports.Add(report);
//             return report;
//         }
//     }

//     public bool CheckForDuplicate(string commit)
//     {
//         // NOTE: remove global filters to include archived reports for this check
//         var inList = _reports.Count(r => r.CommitSha == commit);
//         return inList > 0;
//     }

//     public IReadOnlyList<Report> FindAll()
//     {
//         lock (_lock)
//         {
//             return [.. _reports];
//         }
//     }

//     public Report? FindById(int id)
//     {
//         lock (_lock)
//         {
//             return _reports.Find(r => r.Id == id);
//         }
//     }

//     public IReadOnlyList<Report> FindUrgentOnly()
//     {
//         lock (_lock)
//         {
//             return [.. _reports.Where(r => r.Urgent == true)];
//         }
//     }

//     public bool Delete(int id)
//     {
//         lock (_lock)
//         {
//             var toDelete = FindById(id);
//             if (toDelete is not null)
//                 return _reports.Remove(toDelete);
//             return false;
//         }
//     }
// }
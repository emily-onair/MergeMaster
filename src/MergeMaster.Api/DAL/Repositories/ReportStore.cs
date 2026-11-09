namespace MergeMaster.Api.DAL.Repositories;

using System.Linq.Expressions;
using MergeMaster.Api.DAL.Models;
using MergeMaster.Api.Models.ViewModels;
using Microsoft.EntityFrameworkCore;

public sealed class ReportStore(ReportDb db, RepoStore repos, BranchStore branches,
    AuthorStore authors, TagStore tags) : IReportStore
{
    public async Task<Report> AddAsync(NewReport newReport, CancellationToken ct)
    {
        var repo = await repos.GetOrCreateAsync(newReport.Owner, newReport.Repository, ct);
        var branch = await branches.GetOrCreateAsync(repo, newReport.Branch, ct);
        var author = await authors.GetOrCreateAsync(newReport.AuthorName, ct);
        var tagList = await tags.GetOrCreateAsync(newReport.Tags, ct);

        var report = new ReportEntity
        {
            CreatedAtUtc = newReport.CreatedAtUtc,
            CommitSha = newReport.CommitSha,
            CommitMessage = newReport.CommitMessage,
            CommitDateUtc = newReport.CommitDateUtc,
            Urgent = newReport.Urgent,
            Branch = branch,
            Author = author,
            Tags = tagList,
        };
        db.Reports.Add(report);
        await db.SaveChangesAsync(ct);
        return report.ToReport();
    }

    public async Task<bool> FindDuplicateAsync(string commit, CancellationToken ct) =>
        await db.Reports.AnyAsync(r => r.CommitSha == commit, ct);

    public async Task<IReadOnlyList<Report>> FindAllAsync(CancellationToken ct)
    {
        // TODO: validation can be here as private method
        return await db.Reports.AsNoTracking()
            .Populate()
            .Select(r => r.ToReport())
            // .Include(Repository) // joins to other table in db
            .ToListAsync(ct);
    }

    public async Task<PaginationResult> FindAllFilteredPagesAsync(
        bool archived, bool? descending, bool? urgentOnly, List<string> authors, 
        List<string> tags, string? search, int page, int pageSize, CancellationToken ct)
    {
        var query = db.Reports.InArchive(archived);

        if (urgentOnly is true) query = query.Where(r => r.Urgent);
        
        if (authors.Count > 0) query = query.Where(r => authors.Contains(r.Author.Name));

        var searchTerm = search?.Trim();
        if (!string.IsNullOrEmpty(searchTerm)) {
            searchTerm = searchTerm.ToLowerInvariant();
            // pragma to suppress warning for string comparison
            // sqlite provider can't translate in this context
            #pragma warning disable CA1862
                query = query.Where(r =>
                    r.Branch.Name.ToLower().Contains(searchTerm) || 
                    r.CommitSha.ToLower().Contains(searchTerm));
            #pragma warning restore CA1862
        }

        if (tags.Count > 0) query = query.Where(r => r.Tags.Any(t => tags.Contains(t.Name)));

        query = descending is true
            ? query.OrderByDescending(r => r.CreatedAtUtc)
            : query.OrderBy(r => r.CreatedAtUtc);

        // #if DEBUG
        // Console.WriteLine("QUERY STRING ======> " + query.ToQueryString());
        // #endif

        var total = await query.CountAsync(ct);

        var filteredReports = await query.AsNoTracking()
            .Populate()
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(r => r.ToReport())
            .ToListAsync(ct);
        return new PaginationResult(filteredReports, total, page, pageSize);
    }

    public async Task<Report?> FindByIdAsync(int id, CancellationToken ct)
    {
        var report = await db.Reports
            .Populate()
            .FirstOrDefaultAsync(r => r.Id == id, ct);
        return report?.ToReport();
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct)
    {
        var toDelete = await db.Reports.FindAsync([id], ct);
        if (toDelete is not null) 
        {
            db.Reports.Remove(toDelete);
            await db.SaveChangesAsync(ct);
            return true;
        }
        return false;
    }

    public async Task<bool> ArchiveAsync(int id, CancellationToken ct)
    {
        var updated = await db.Reports
            .Where(r => r.Id == id)
            .ExecuteUpdateAsync(s => s
                .SetProperty(u => u.ArchivedAtUtc, DateTime.UtcNow)
                .SetProperty(u => u.UpdatedAtUtc, DateTime.UtcNow), ct); 
        return updated > 0;
    }

    public async Task<bool> RestoreFromArchiveAsync(int id, CancellationToken ct)
    {
        var updated = await db.Reports
            .Where(r => r.Id == id)
            .ExecuteUpdateAsync(s => s
                .SetProperty(u => u.ArchivedAtUtc, (DateTime?)null)
                .SetProperty(u => u.UpdatedAtUtc, DateTime.UtcNow), ct); 
        return updated > 0;
    }

    public async Task<DropdownOptionLists> FindDropdownOptionsAsync(bool archived, CancellationToken ct)
    {
        return new DropdownOptionLists(
            Authors: await authors.GetNamesAsync(archived, ct),
            Tags: await tags.GetNamesAsync(archived, ct),
            Branches: await branches.GetNamesAsync(archived, ct),
            Commits: await FindCommitsAsync(archived, ct)
        );
    }

    public async Task<List<string>> FindCommitsAsync(bool archived, CancellationToken ct)
    {
        var query = db.Reports.InArchive(archived);
        return await query.Select(r => r.CommitSha)
            .Distinct()
            .OrderBy(x => x)
            .ToListAsync(ct);
    }
}
namespace MergeMaster.Api.DAL.Repositories;

using MergeMaster.Api.DAL.Interfaces;
using MergeMaster.Api.DAL.Models;
using MergeMaster.Api.Models.ViewModels;
using Microsoft.EntityFrameworkCore;

public sealed class ReportStore(ReportDb db, IRepoStore repos, IBranchStore branches,
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

    public async Task<bool> CheckIfExistsAsync(string commit, CancellationToken ct) =>
        await db.Reports.AnyAsync(r => r.CommitSha == commit, ct);

    public async Task<List<ReportEntity>> GetActiveEntitiesAsync(CancellationToken ct)
    {
        var query = db.Reports.InArchive(false);

        return await query.AsNoTracking()
            .Include(r => r.Branch)
            .ThenInclude(b => b.Repo)
            .Include(r => r.SyncFlags)
            .ToListAsync(ct);
    }

    public async Task<PaginationResult> FilterAsync(FilterQueryParams queryParams, 
        CancellationToken ct)
    {
        var query = db.Reports.InArchive(queryParams.Archived);

        if (queryParams.UrgentOnly is true) query = query.Where(r => r.Urgent);
        
        if (queryParams.Authors.Count > 0) query = query.Where(r => queryParams.Authors.Contains(r.Author.Name));

        var searchTerm = queryParams.Search?.Trim();
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

        if (queryParams.Tags.Count > 0) query = query.Where(r => r.Tags.Any(t => queryParams.Tags.Contains(t.Name)));

        query = queryParams.Descending is true
            ? query.OrderByDescending(r => r.CreatedAtUtc)
            : query.OrderBy(r => r.CreatedAtUtc);

        // #if DEBUG
        // Console.WriteLine("QUERY STRING ======> " + query.ToQueryString());
        // #endif

        var total = await query.CountAsync(ct);

        var filteredReports = await query.AsNoTracking()
            .Populate()
            .Skip((queryParams.Page - 1) * queryParams.PageSize)
            .Take(queryParams.PageSize)
            .Select(r => r.ToReport())
            .ToListAsync(ct);
        return new PaginationResult(filteredReports, total, queryParams.Page, queryParams.PageSize);
    }

    public async Task<Report?> FindByIdAsync(int id, CancellationToken ct)
    {
        var report = await db.Reports
            .Populate()
            .FirstOrDefaultAsync(r => r.Id == id, ct);
        return report?.ToReport();
    }

    public async Task<DropdownOptionLists> GetDropdownOptionsAsync(bool archived, CancellationToken ct)
    {
        return new DropdownOptionLists(
            Authors: await authors.GetNamesAsync(archived, ct),
            Tags: await tags.GetNamesAsync(archived, ct),
            Branches: await branches.GetNamesAsync(archived, ct),
            Commits: await GetCommitsAsync(archived, ct)
        );
    }

    private async Task<List<string>> GetCommitsAsync(bool archived, CancellationToken ct)
    {
        var query = db.Reports.InArchive(archived);
        return await query.Select(r => r.CommitSha)
            .Distinct()
            .OrderBy(x => x)
            .ToListAsync(ct);
    }
}
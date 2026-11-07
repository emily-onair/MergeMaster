namespace MergeMaster.Api.Data;

using System.Linq.Expressions;
using MergeMaster.Api.Models;
using MergeMaster.Api.Models.ViewModels;
using Microsoft.EntityFrameworkCore;

public sealed class ReportStore(ReportDb db) : IReportStore
{
    public async Task<Report> AddAsync(ReportEntity newReport, CancellationToken ct)
    {
        db.Add(newReport);
        await db.SaveChangesAsync(ct);
        return newReport.ToReport();
    }

    public async Task<bool> FindDuplicateAsync(string commit, CancellationToken ct)
    {
        // remove global filters to include archived reports
        return await db.Reports.IgnoreQueryFilters()
            .AnyAsync(r => r.CommitSha == commit, ct);
    }

    public async Task<IReadOnlyList<Report>> FindAllAsync(CancellationToken ct)
    {
        // TODO: validation can be here as private method
        return await db.Reports.AsNoTracking()
            .Select(r => r.ToReport())
            .ToListAsync(ct);
    }

    public async Task<PaginationResult> FindAllFilteredPagesAsync(
        bool archived, bool? descending, bool? urgentOnly, List<string> authors, 
        List<string> tags, string? search, int page, int pageSize, CancellationToken ct)
    {
        var query = archived
            ? db.Reports.IgnoreQueryFilters().Where(r => r.ArchivedAtUtc != null)
            : db.Reports;

        if (urgentOnly is true) query = query.Where(r => r.Urgent);
        
        if (authors.Count > 0) query = query.Where(r => authors.Contains(r.AuthorName));

        var searchTerm = search?.Trim();
        if (!string.IsNullOrEmpty(searchTerm)) {
            searchTerm = searchTerm.ToLowerInvariant();
            // suppress warning for string comparison
            // sqlite provider can't translate in this context
            #pragma warning disable CA1862
                query = query.Where(r =>
                    r.Branch.ToLower().Contains(searchTerm) || 
                    r.CommitSha.ToLower().Contains(searchTerm));
            #pragma warning restore CA1862
        }

        if (tags.Count > 0) query = query.Where(r => tags.Any(t => ("," + r.Tags + ",")
            .Contains("," + t + ",")));

        query = descending is true
            ? query.OrderByDescending(r => r.CreatedAtUtc)
            : query.OrderBy(r => r.CreatedAtUtc);

        var total = await query.CountAsync(ct);

        var filteredReports = await query.AsNoTracking()
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(r => r.ToReport())
            .ToListAsync(ct);
        return new PaginationResult(filteredReports, total, page, pageSize);
    }

    public async Task<Report?> FindByIdAsync(int id, CancellationToken ct)
    {
        return (await db.Reports.FindAsync([id], ct))?.ToReport();
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
            .IgnoreQueryFilters()
            .Where(r => r.Id == id)
            .ExecuteUpdateAsync(s => s
                .SetProperty(u => u.ArchivedAtUtc, (DateTime?)null)
                .SetProperty(u => u.UpdatedAtUtc, DateTime.UtcNow), ct); 
        return updated > 0;
    }

    public async Task<DropdownOptionLists> FindDropdownOptionsAsync(bool archived, CancellationToken ct)
    {
        return new DropdownOptionLists(
            Authors: await FindAuthorsAsync(archived, ct),
            Tags: await FindTagsAsync(archived, ct),
            Branches: await FindBranchesAsync(archived, ct),
            Commits: await FindCommitsAsync(archived, ct)
        );
    }

    public async Task<List<string>> FindAuthorsAsync(bool archived, CancellationToken ct) =>
        await FindDistinctAsync(r => r.AuthorName, archived, ct);

    public async Task<List<string>> FindBranchesAsync(bool archived, CancellationToken ct) =>
        await FindDistinctAsync(r => r.Branch, archived, ct);

    public async Task<List<string>> FindCommitsAsync(bool archived, CancellationToken ct) =>
        await FindDistinctAsync(r => r.CommitSha, archived, ct);

    private async Task<List<string>> FindDistinctAsync(
        Expression<Func<ReportEntity, string>> selector, bool archived, CancellationToken ct)
    {
        var query = archived
            ? db.Reports.IgnoreQueryFilters().Where(r => r.ArchivedAtUtc != null)
            : db.Reports;

        return await query.Select(selector)
            .Distinct()
            .OrderBy(x => x)
            .ToListAsync(ct);
    } 

    public async Task<List<string>> FindTagsAsync(bool archived, CancellationToken ct)
    {
        var query = archived
            ? db.Reports.IgnoreQueryFilters().Where(r => r.ArchivedAtUtc != null)
            : db.Reports;
        var tagStrings = await query.AsNoTracking()
            .Select(r => r.Tags).ToListAsync(ct);
        return [.. tagStrings
            .SelectMany(t => t.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            .Distinct()];
    }
}
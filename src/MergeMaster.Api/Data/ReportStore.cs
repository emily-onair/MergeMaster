namespace MergeMaster.Api.Data;
using MergeMaster.Api.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

public sealed class ReportStore(ReportDb db)
{
    // public void Seed(IEnumerable<Report> reports)
    // {
    //     lock (_lock)
    //     {
    //         _reports.AddRange(reports);
    //     }
    // }

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
        return await db.Reports.AsNoTracking()
            .Select(r => r.ToReport())
            .ToListAsync(ct);
    }

    public async Task<Report?> FindByIdAsync(int id, CancellationToken ct)
    {
        return (await db.Reports.FindAsync([id], ct))?.ToReport();
    }

    public async Task<IReadOnlyList<Report>> FindUrgentOnlyAsync(CancellationToken ct)
    {
        return await db.Reports.AsNoTracking()
            .Where(r => r.Urgent == true)
            .Select(r => r.ToReport())
            .ToListAsync(ct);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct)
    {
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
    }
}
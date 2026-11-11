using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using MergeMaster.Api.DAL.Models;
using MergeMaster.Api.Models;
using MergeMaster.Api.Models.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace MergeMaster.Api.DAL.Repositories;

public class SyncFlagStore(ReportDb db)
{
    public async Task<List<ReportFlag>> FindByReportAsync(int reportId, CancellationToken ct)
    {
        return await db.SyncFlags.AsNoTracking()
            .Where(f => f.ReportId == reportId)
            .OrderByDescending(f => f.DetectedAtUtc)
            .Select(f => new ReportFlag(
                new DateTimeOffset(f.DetectedAtUtc, TimeSpan.Zero),
                f.FlagType
            )).ToListAsync(ct);
    }
    public async Task AddSingleAsync(int reportId, SyncFlagTypes type, CancellationToken ct)
    {
        var newFlag = new SyncFlagEntity()
        {
            ReportId = reportId,
            FlagType = type,
            DetectedAtUtc = DateTime.UtcNow
        };
        db.SyncFlags.Add(newFlag);
        await db.SaveChangesAsync(ct);
    }

    public async Task<int> AddManyAsync(SyncFlagTypes type, 
        Expression<Func<ReportEntity, bool>> selector, CancellationToken ct)
    {
        var affectedReportIds = await db.Reports.Where(selector)
            .Where(r => !r.SyncFlags.Any(f => f.FlagType == type))
            .Select(r => r.Id)
            .ToListAsync(ct);

        var now = DateTime.UtcNow;
        var newFlags = affectedReportIds.Select(id => new SyncFlagEntity
        {
            ReportId = id,
            FlagType = type,
            DetectedAtUtc = now
        });

        db.SyncFlags.AddRange(newFlags);
        await db.SaveChangesAsync(ct);
        return affectedReportIds.Count;
    }
}

// Expression<Func<ReportEntity, string>> selector
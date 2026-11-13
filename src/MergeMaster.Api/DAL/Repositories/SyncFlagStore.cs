using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

using MergeMaster.Api.DAL.Models;
using MergeMaster.Api.Models;
using MergeMaster.Api.Models.ViewModels;
using MergeMaster.Api.DAL.Interfaces;
using MergeMaster.Api.Extensions;

namespace MergeMaster.Api.DAL.Repositories;

public class SyncFlagStore(ReportDb db) : ISyncFlagStore
{
    public async Task<List<ReportFlag>> GetByReportAsync(int reportId, CancellationToken ct)
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

    public async Task<int> AddForRepoAsync(int repoId, SyncFlagTypes type, CancellationToken ct) => 
        await AddManyAsync(type, r => r.Branch.RepoId == repoId, ct);
    public async Task<int> AddForBranchAsync(int branchId, SyncFlagTypes type, CancellationToken ct) =>
        await AddManyAsync(type, r => r.BranchId == branchId, ct);
    private async Task<int> AddManyAsync(SyncFlagTypes type, 
        Expression<Func<ReportEntity, bool>> selector, CancellationToken ct)
    {
        var affectedReportIds = await db.Reports
            .InArchive(false)
            .Where(selector)
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
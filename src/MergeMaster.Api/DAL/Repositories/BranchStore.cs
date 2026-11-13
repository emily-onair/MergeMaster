using MergeMaster.Api.DAL.Interfaces;
using MergeMaster.Api.DAL.Models;
using MergeMaster.Api.Extensions;
using Microsoft.EntityFrameworkCore;

namespace MergeMaster.Api.DAL.Repositories;

public class BranchStore(ReportDb db) : IBranchStore
{

    public async Task<bool> UpdateNameAsync(string newName, int id, CancellationToken ct)
    {
        var updated = await db.Branches
            .Where(b => b.Id == id)
            .ExecuteUpdateAsync(b => b
                .SetProperty(u => u.Name, newName)
                .SetProperty(u => u.UpdatedAtUtc, DateTime.UtcNow), ct);
        
        return updated > 0;
    }
    public async Task<List<string>> GetNamesAsync(bool archived, CancellationToken ct)
    {
        var reports = db.Reports.InArchive(archived);

        return await db.Branches
            .Where(b => reports.Any(r => r.BranchId == b.Id))
            .Select(b => b.Name)
            .Distinct()
            .OrderBy(n => n)
            .ToListAsync(ct);
    }

    public async Task<List<BranchEntity>> GetActiveEntitiesAsync(CancellationToken ct)
    {
        var reports = db.Reports.InArchive(false);

        return await db.Branches
            .Include(b => b.Repo)
            .Where(b => reports.Any(r => r.BranchId == b.Id))
            .ToListAsync(ct);
    }
    
    /// <inheritdoc/>
    public async Task<BranchEntity> GetOrCreateAsync(RepoEntity repo, string name, CancellationToken ct)
    {
        name = name.Trim();
        // A repo created in this request has no Id yet, so it can't have branches in the DB
        if (repo.Id != 0)
        {
            var branch = await db.Branches
                .FirstOrDefaultAsync(b => b.RepoId == repo.Id && b.Name == name, ct);
            if (branch is not null) return branch;
        }
        var newBranch = new BranchEntity { Name = name, Repo = repo, CreatedAtUtc = DateTime.UtcNow };
        db.Branches.Add(newBranch);
        return newBranch;
    }
}

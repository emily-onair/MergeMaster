using MergeMaster.Api.DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace MergeMaster.Api.DAL.Repositories;

public class BranchStore(ReportDb db)
{
    public async Task<List<string>> GetNamesAsync(bool archived, CancellationToken ct)
    {
        var reports = db.Reports.InArchive(archived);

        return await db.Branches
            .Where(b => reports.Any(r => r.BranchId == b.Id))
            .Select(b => b.Name)
            .OrderBy(n => n)
            .ToListAsync(ct);
    }
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

using MergeMaster.Api.DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace MergeMaster.Api.DAL.Repositories;

public class RepoStore(ReportDb db)
{
    public async Task<List<string>> GetNamesAsync(bool archived, CancellationToken ct)
    {
        var reports = db.Reports.InArchive(archived);

        return await db.Repos
            .Where(r => reports.Any(rp => rp.Branch.RepoId == r.Id))
            .Select(r => r.Repository)
            .Distinct()
            .OrderBy(n => n)
            .ToListAsync(ct);
    }

    public async Task<List<RepoEntity>> GetReposAsync(bool archived, CancellationToken ct)
    {
        var reports = db.Reports.InArchive(archived);

        return await db.Repos
            .Where(r => reports.Any(rp => rp.Branch.RepoId == r.Id))
            .Select(r => r)
            .OrderBy(n => n)
            .ToListAsync(ct);
    }

    public async Task<int> UpdateOwnerAsync(string newOwner, string oldOwner)
    {
        return await db.Repos
            .Where(r => r.Owner == oldOwner)
            .ExecuteUpdateAsync(r => r
                .SetProperty(u => u.Owner, newOwner)
                .SetProperty(u => u.UpdatedAtUtc, DateTime.UtcNow));
    }

    public async Task<int> UpdateRepoNameAsync(string newName, string oldName)
    {
        return await db.Repos
            .Where(r => r.Repository == oldName)
            .ExecuteUpdateAsync(r => r
                .SetProperty(u => u.Repository, newName)
                .SetProperty(u => u.UpdatedAtUtc, DateTime.UtcNow));
    }
    public async Task<RepoEntity> GetOrCreateAsync(string owner, string repository, CancellationToken ct)
    {
        owner = owner.Trim();
        repository = repository.Trim();

        var repo = await db.Repos.FirstOrDefaultAsync(a => 
            a.Owner == owner && a.Repository == repository, ct);
        if (repo is not null) return repo;

        repo = new RepoEntity { 
            Owner = owner, Repository = repository, CreatedAtUtc = DateTime.UtcNow };
        db.Repos.Add(repo);          // staged; ReportStore saves
        return repo;
    }
}

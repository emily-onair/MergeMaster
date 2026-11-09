using MergeMaster.Api.DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace MergeMaster.Api.DAL.Repositories;

public class RepoStore(ReportDb db)
{
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

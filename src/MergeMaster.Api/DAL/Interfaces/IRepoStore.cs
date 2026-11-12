using MergeMaster.Api.DAL.Models;

namespace MergeMaster.Api.DAL.Interfaces;

public interface IRepoStore
{
    Task<List<string>> GetNamesAsync(bool archived, CancellationToken ct);
    Task<List<RepoEntity>> GetActiveEntitiesAsync(CancellationToken ct);
    Task<bool> UpdateOwnerAsync(string newOwner, int id, CancellationToken ct);
    Task<bool> UpdateNameAsync(string newName, int id, CancellationToken ct);

    /// <summary>
    /// Returns the existing repo, or stages a new one.
    /// Does not save: the caller must call SaveChangesAsync.
    /// </summary>
    Task<RepoEntity> GetOrCreateAsync(string owner, string repository, CancellationToken ct);
}

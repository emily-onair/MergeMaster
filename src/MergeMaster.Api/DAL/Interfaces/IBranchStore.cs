using MergeMaster.Api.DAL.Models;

namespace MergeMaster.Api.DAL.Interfaces;

public interface IBranchStore
{
    Task<List<string>> GetNamesAsync(bool archived, CancellationToken ct);
    Task<List<BranchEntity>> GetActiveEntitiesAsync(CancellationToken ct);
    Task<bool> UpdateNameAsync(string newName, int id, CancellationToken ct);
    
    /// <summary>
    /// Returns the existing branch for this repo, or stages a new one.
    /// Does not save: the caller must call SaveChangesAsync.
    /// </summary>
    Task<BranchEntity> GetOrCreateAsync(RepoEntity repo, string name, CancellationToken ct);
}

using MergeMaster.Api.DAL.Models;

namespace MergeMaster.Api.DAL.Interfaces;

public interface IRepoStore
{
    public Task<List<string>> GetNamesAsync(bool archived, CancellationToken ct);
    public Task<RepoEntity> GetOrCreateAsync(string owner, string repository, CancellationToken ct);
    public Task<List<RepoEntity>> GetActiveEntitiesAsync(CancellationToken ct);
    public Task<bool> UpdateOwnerAsync(string newOwner, int id, CancellationToken ct);
    public Task<bool> UpdateNameAsync(string newName, int id, CancellationToken ct);
}

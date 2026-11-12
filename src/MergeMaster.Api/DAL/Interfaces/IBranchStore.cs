using MergeMaster.Api.DAL.Models;

namespace MergeMaster.Api.DAL.Interfaces;

public interface IBranchStore
{
    public Task<List<string>> GetNamesAsync(bool archived, CancellationToken ct);
    public Task<BranchEntity> GetOrCreateAsync(RepoEntity repo, string name, CancellationToken ct);
    public Task<List<BranchEntity>> GetActiveEntitiesAsync(CancellationToken ct);
    public Task<bool> UpdateNameAsync(string newName, int id, CancellationToken ct);
}

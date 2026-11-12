namespace MergeMaster.Api.Services.GitHub;

using MergeMaster.Api.Services.GitHub.Models;

public interface IGitHubApi
{
    Task<GitHubResult<bool>> CheckForCommitOnBranchAsync(string ownerRepo, string branch, string commit, bool unavailableAllowed, CancellationToken ct);
    Task<GitHubResult<bool>> CheckForCommitAlreadyMergedAsync(string ownerRepo, string commit, bool unavailableAllowed, CancellationToken ct);
    Task<GitHubResult<string>> CheckForRepoExistenceAsync(string owner, string repo, bool unavailableAllowed, CancellationToken ct);
    Task<GitHubResult<string>> CheckForBranchExistenceAsync(string owner, string repo, string branch, bool unavailableAllowed, CancellationToken ct);
    Task<GitHubResult<bool>> CheckForCommitExistenceAsync(string ownerRepo, string commit, bool unavailableAllowed, CancellationToken ct);
    Task<GitHubResult<ResultCommit>> GetCommitDetailsAsync(string ownerRepo, string commit, bool unavailableAllowed, CancellationToken ct);
}
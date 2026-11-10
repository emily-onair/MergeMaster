namespace MergeMaster.Api.Services.GitHub;

using MergeMaster.Api.Services.GitHub.Models;

public interface IGitHubApi
{
    public Task<GitHubResult<bool>> CheckForCommitOnBranchAsync(string ownerRepo, string branch, string commit, bool unavailableAllowed, CancellationToken ct);

    public Task<GitHubResult<bool>> CheckForCommitAlreadyMerged(string ownerRepo, string commit, bool unavailableAllowed, CancellationToken ct);

    public Task<GitHubResult<ResultCommit>> GetCommitDetails(string ownerRepo, string commit, bool unavailableAllowed, CancellationToken ct);

    public Task<GitHubResult<string>> CheckForRepoExistence(string owner, string repo, bool unavailableAllowed, CancellationToken ct);
    public Task<GitHubResult<string>> CheckForBranchExistence(string owner, string repo, string branch, bool unavailableAllowed, CancellationToken ct);
    public Task<GitHubResult<bool>> CheckForCommitExistence(string ownerRepo, string commit, bool unavailableAllowed, CancellationToken ct);
}
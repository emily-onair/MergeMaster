namespace MergeMaster.Api.Services.GitHub;

using MergeMaster.Api.Services.GitHub.Models;

public interface IGitHubApi
{
    public Task<GitHubResult<bool>> CheckForCommitOnBranchAsync(string ownerRepo, string branch, string commit, bool unavailableAllowed, CancellationToken ct);

    public Task<GitHubResult<bool>> CheckForCommitAlreadyMerged(string ownerRepo, string commit, bool unavailableAllowed, CancellationToken ct);

    public Task<GitHubResult<ResultCommit>> GetCommitDetails(string ownerRepo, string commit, bool unavailableAllowed, CancellationToken ct);

    public Task<GitHubResult<bool>> CheckForRepoExistence(string ownerRepo, bool unavailableAllowed, CancellationToken ct);
    public Task<GitHubResult<string>> CheckForBranchExistence(string ownerRep0, string branch, bool unavailableAllowed, CancellationToken ct);
}
namespace MergeMaster.Api.Services.GitHub;

using MergeMaster.Api.Models;
using MergeMaster.Api.Services.GitHub.Models;

public interface IGitHubApi
{
    public Task<GitHubResult<bool>> CheckForCommitOnBranchAsync(string ownerRepo, string branch, string commit, CancellationToken ct);

    public Task<GitHubResult<bool>> CheckForCommitAlreadyMerged(string ownerRepo, string commit, CancellationToken ct);

    public Task<GitHubResult<ResultCommit>> GetCommitDetails(string ownerRepo, string commit, CancellationToken ct);

    public bool ShouldProceed(HttpResponseMessage response);

    public Task<List<ReportSyncError>> SynchronizeWithGitHub();
}
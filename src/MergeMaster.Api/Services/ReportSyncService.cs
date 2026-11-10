using System.Net;
using System.Text.Json;
using MergeMaster.Api.DAL.Models;
using MergeMaster.Api.DAL.Repositories;
using MergeMaster.Api.Models;
using MergeMaster.Api.Models.ViewModels;
using MergeMaster.Api.Services.GitHub;

namespace MergeMaster.Api.Services;

public class ReportSyncService (GitHubApi github, SyncFlagStore flags,
    RepoStore repos, BranchStore branches) : IReportSyncService
{
    private static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web)
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
    };
    public async Task<List<SyncUpdate>> SyncWithGitHubAsync(CancellationToken ct)
    {
        var updates = new List<SyncUpdate>();

        var repoList = await repos.GetReposAsync(false, ct);
        var repoSyncCompleted = await SyncReposAsync(repoList, updates, ct);
        if (repoSyncCompleted is false)
            return updates;
        
        var branchList = await branches.GetBranchesAsync(false, ct);
        var branchSyncCompleted = await SyncBranchesAsync(branchList, updates, ct);
        
        updates.Add(new SyncUpdate(false, false, "Database synchronization completed"));
        return updates;
    }

    private async Task<bool> SyncBranchesAsync(List<BranchEntity> branches, List<SyncUpdate> updates, CancellationToken ct)
    {
        foreach (var branch in branches)
        {
            var branchExists = await github.CheckForBranchExistence(
                branch.Repo.Owner, branch.Repo.Repository, branch.Name, false, ct);
            
        }
        return true;
    }

    private async Task<bool> SyncReposAsync(List<RepoEntity> repoList, List<SyncUpdate> updates, CancellationToken ct)
    {
        foreach (RepoEntity repo in repoList)
        {
            var repoExists = await github.CheckForRepoExistence(repo.Owner, repo.Repository, false, ct);
            
            if (repoExists.Value is not null && repoExists.Value != $"{repo.Owner}/{repo.Repository}")
            {
                var parts = repoExists.Value.Split('/');
                var newOwner = parts[0];
                var newRepoName = parts[1];
                var toastPrefix = $"{repo.Owner}/{repo.Repository} updated with new repository";
                if (repo.Owner != newOwner)
                {
                    var updated = await repos.UpdateOwnerAsync(newOwner, repo.Id);
                    updates.Add(new SyncUpdate(false, false, $"{toastPrefix} owner: {newOwner}"));
                }
                if (repo.Repository != newRepoName)
                {
                    var updated = await repos.UpdateRepoNameAsync(newRepoName, repo.Id);
                    updates.Add(new SyncUpdate(false, false, $"{toastPrefix} name: {newRepoName}"));
                }
            }

            if (repoExists.IsUnavailable)
            {
                updates.Add(new SyncUpdate(true, false, "GitHub could not be reached. Synchronization incomplete"));
                return false;
            }

            if (repoExists.Error?.StatusCode == HttpStatusCode.NotFound)
            {
                var updated = await flags.AddManyAsync(SyncFlagTypes.RepoDoesNotExist, 
                    r => r.Branch.RepoId == repo.Id, ct);
                if (updated > 0)
                {
                    updates.Add(new SyncUpdate(false, true, 
                        $"{updated} reports flagged for non-existent repository: {repo.Owner}/{repo.Repository}"));
                }
            } 
            else if (repoExists.Error is not null)
            {
                updates.Add(new SyncUpdate(true, false, repoExists.Error.Message));
                return false;
            }
        }
        // nothing went wrong or needed updating
        return true;
    }
}
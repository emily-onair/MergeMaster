using System.Net;
using System.Text.Json;
using MergeMaster.Api.DAL.Models;
using MergeMaster.Api.DAL.Repositories;
using MergeMaster.Api.Models;
using MergeMaster.Api.Models.ViewModels;
using MergeMaster.Api.Services.GitHub;

namespace MergeMaster.Api.Services;

public class ReportSyncService (GitHubApi github, SyncFlagStore flags,
    RepoStore repos) : IReportSyncService
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
        
        // var branchList = await branches.GetBranchesAsync(false, ct);
        // if (updates.Count == 0)
        
        updates.Add(new SyncUpdate(false, false, "Database synchronized"));
        return updates;
    }

    // private async Task<bool> SyncBranchesAsync(List<string> branchNames, List<SyncUpdate> updates, CancellationToken ct)
    // {
    //     foreach (string branchName in branchNames)
    //     {
    //         var branchExists = await github.CheckForBranchExistence()
    //     }
    // }

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
                var prev = $"{repo.Owner}/{repo.Repository}";
                if (repo.Owner != newOwner)
                {
                    var updated = await repos.UpdateOwnerAsync(newOwner, repo.Id);
                    updates.Add(new SyncUpdate(false, false, 
                        $"{prev} updated with new repository owner: {newOwner}"));
                }
                if (repo.Repository != newRepoName)
                {
                    var updated = await repos.UpdateRepoNameAsync(newRepoName, repo.Id);
                    updates.Add(new SyncUpdate(false, false, 
                        $"{prev} updated with new repository name: {newOwner}"));
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
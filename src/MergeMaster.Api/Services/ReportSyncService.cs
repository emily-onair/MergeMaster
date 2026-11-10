using System.Net;
using System.Text.Json;
using MergeMaster.Api.DAL.Models;
using MergeMaster.Api.DAL.Repositories;
using MergeMaster.Api.Services.GitHub;

namespace MergeMaster.Api.Services;

public class ReportSyncService (GitHubApi github, SyncFlagStore flags,
    ReportStore reports, RepoStore repos) : IReportSyncService
{
    private static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web)
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
    };
    public async Task<List<SyncUpdate>> SyncWithGitHubAsync(CancellationToken ct)
    {
        var updates = new List<SyncUpdate>();

        var repoList = await repos.GetReposAsync(false, ct);
        var repoSyncCompleted = await SyncRepoAsync(repoList, updates, ct);
        if (repoSyncCompleted is false)
            return updates;
        // var repoCheck = await github.CheckForRepoExistence(
        //     ownerRepo,
        //     unavailableAllowed: false, ct);
        // if (repoCheck.Value is false)
        // {
        //     updates.Add(new SyncUpdate(true, $"Repo {ownerRepo} could not be synchronized"));
        //     // return updates; // if single repo, stop here
        //     // all reports with this repo would need to be flagged
        // }

        // var branches = await store.FindBranchesAsync(false, ct);
        // branches.ForEach(async b =>
        // {
        //     var update = await SyncBranchAsync(b);
        //     if (update is not null) updates.Add(update);
        // });
        return updates;
    }

    private async Task<bool> SyncRepoAsync(List<RepoEntity> repoList, List<SyncUpdate> updates, CancellationToken ct)
    {
        foreach (RepoEntity repo in repoList)
        {
            var ownerRepoString = $"{repo.Owner}/{repo.Repository}";
            var repoExists = await github.CheckForRepoExistence($"{repo.Owner}/{repo.Repository}", false, ct);
            
            if (repoExists.Value is not null && repoExists.Value != ownerRepoString)
            {
                var parts = repoExists.Value.Split('/');
                var newOwner = parts[0];
                var newRepoName = parts[1];
                if (repo.Owner != newOwner)
                {
                    var updated = await repos.UpdateOwnerAsync(newOwner, repo.Owner);
                    updates.Add(new SyncUpdate(false, false, 
                        $"{updated} reports updated with new repository owner: {newOwner}"));
                }
                if (repo.Repository != newRepoName)
                {
                    var updated = await repos.UpdateRepoNameAsync(newRepoName, repo.Repository);
                    updates.Add(new SyncUpdate(false, false, 
                        $"{updated} reports updated with new repository name: {newOwner}"));
                }
            }

            if (repoExists.Error?.StatusCode == HttpStatusCode.NotFound)
            {
                
            } 
            
            else if (repoExists.IsUnavailable)
            {
                updates.Add(new SyncUpdate(true, false, "GitHub could not be reached. Synchronization not incomplete"));
                return false;
            }
        }
        // nothing went wrong or needed updating
        return true;
    }
}
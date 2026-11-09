using MergeMaster.Api.DAL.Repositories;
using MergeMaster.Api.Services.GitHub;

namespace MergeMaster.Api.Services;

public class ReportSyncService (GitHubApi github, ReportStore store)
{
    public async Task<List<SyncUpdate>> SyncWithGitHubAsync(CancellationToken ct)
    {
        var updates = new List<SyncUpdate>();
        // const string ownerRepo = "emily-onair/TestRepo"; // get this from db if multiple repos allowed, or app settings
        
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

    private async Task<SyncUpdate?> SyncBranchAsync(string branch)
    {

        return null;
    }
}
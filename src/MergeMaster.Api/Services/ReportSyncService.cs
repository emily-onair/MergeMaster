using System.Net;
using System.Text.Json;
using MergeMaster.Api.DAL.Models;
using MergeMaster.Api.DAL.Interfaces;
using MergeMaster.Api.Models;
using MergeMaster.Api.Models.ViewModels;
using MergeMaster.Api.Services.GitHub;
using MergeMaster.Api.Services.GitHub.Models;

namespace MergeMaster.Api.Services;

public class ReportSyncService (IGitHubApi github, ISyncFlagStore flags,
    IRepoStore repos, IBranchStore branches, IReportStore reports) : IReportSyncService
{
    public async Task<List<SyncUpdate>> SyncWithGitHubAsync(CancellationToken ct)
    {
        var updates = new List<SyncUpdate>();

        var repoList = await repos.GetActiveEntitiesAsync(ct);
        var repoSyncCompleted = await SyncReposAsync(repoList, updates, ct);
        if (!repoSyncCompleted)
            return updates;
        
        var branchList = await branches.GetActiveEntitiesAsync(ct);
        var branchSyncCompleted = await SyncBranchesAsync(branchList, updates, ct);
        if (!branchSyncCompleted)
            return updates;

        var reportList = await reports.GetActiveEntitiesAsync(ct);
        var commitSyncCompleted = await SyncCommitsAsync(reportList, updates, ct);
        if (!commitSyncCompleted)
            return updates;
        
        
        updates.Add(new SyncUpdate(false, false, "Database synchronization completed"));
        return updates;
    }

    private async Task<bool> SyncCommitsAsync(
        List<ReportEntity> reportList, List<SyncUpdate> updates, CancellationToken ct)
    {
        foreach(var report in reportList)
        {
            // save api tokens by avoiding pointless requests
            if (report.SyncFlags.Any(f => 
                f.FlagType == SyncFlagTypes.RepoDoesNotExist
                || f.FlagType == SyncFlagTypes.CommitDoesNotExist
                || f.FlagType == SyncFlagTypes.CommitAlreadyMerged))
                continue;

            var toastPrefix = $"Commit {report.CommitSha[..5]} from branch {report.Branch.Name}" 
                + $" on {report.Branch.Repo.Repository}";

            var commitMerged = await github.CheckForCommitAlreadyMergedAsync(
                $"{report.Branch.Repo.Owner}/{report.Branch.Repo.Repository}",
                report.CommitSha, false, ct);
            
            // main should always exist, 404 can only mean commit not found
            if (commitMerged.Error?.StatusCode == HttpStatusCode.NotFound)
            {
                updates.Add(new SyncUpdate(false, true, $"{toastPrefix} does not exist."));
                await flags.AddSingleAsync(report.Id, SyncFlagTypes.CommitDoesNotExist, ct);
                continue;
            }

            // already merged
            if (commitMerged.Error?.StatusCode == HttpStatusCode.BadRequest)
            {
                updates.Add(new SyncUpdate(false, true, $"{toastPrefix} has already been merged."));
                    await flags.AddSingleAsync(report.Id, SyncFlagTypes.CommitAlreadyMerged, ct);
                continue;
            }

            if (!ErrorChecks(commitMerged, [HttpStatusCode.NotFound, HttpStatusCode.BadRequest], updates)) 
                return false;

            // save api tokens by avoiding pointless requests
            if (report.SyncFlags.Any(f => 
                f.FlagType == SyncFlagTypes.CommitNotOnBranch
                || f.FlagType == SyncFlagTypes.BranchDoesNotExist))
                continue;
            
            var onBranch = await github.CheckForCommitOnBranchAsync(
                $"{report.Branch.Repo.Owner}/{report.Branch.Repo.Repository}",
                report.Branch.Name, report.CommitSha, false, ct);

            if (onBranch.Error?.StatusCode == HttpStatusCode.BadRequest)
            {
                updates.Add(new SyncUpdate(false, true, $"{toastPrefix} is not on its branch."));
                await flags.AddSingleAsync(report.Id, SyncFlagTypes.CommitNotOnBranch, ct);
            }

            if (!ErrorChecks(onBranch, [HttpStatusCode.BadRequest], updates)) return false;
        }
        return true;
    }

    private async Task<bool> SyncBranchesAsync(
        List<BranchEntity> branchList, List<SyncUpdate> updates, CancellationToken ct)
    {
        foreach (var branch in branchList)
        {
            var branchExists = await github.CheckForBranchExistenceAsync(
                branch.Repo.Owner, branch.Repo.Repository, branch.Name, false, ct);
            
            if (branchExists.Value is not null && branchExists.Value != branch.Name)
            {
                var toastPrefix = $"Branch {branch.Name} name updated to";
                var updated = await branches.UpdateNameAsync(branchExists.Value, branch.Id, ct);
                if (updated)
                    updates.Add(new SyncUpdate(false, false, $"{toastPrefix}: {branchExists.Value}"));
            }

            if (branchExists.Error?.StatusCode == HttpStatusCode.NotFound)
            {
                var updated = await flags.AddForBranchAsync(branch.Id, SyncFlagTypes.BranchDoesNotExist, ct);
                if (updated > 0)
                {
                    updates.Add(new SyncUpdate(false, true, 
                        $"{updated} reports flagged for non-existent branch: {branch.Name}"));
                }
            }

            // break early if something went wrong
            if (!ErrorChecks(branchExists, [HttpStatusCode.NotFound], updates)) return false;
        }
        return true;
    }

    private async Task<bool> SyncReposAsync(
        List<RepoEntity> repoList, List<SyncUpdate> updates, CancellationToken ct)
    {
        foreach (RepoEntity repo in repoList)
        {
            var repoExists = await github.CheckForRepoExistenceAsync(repo.Owner, repo.Repository, false, ct);
            
            if (repoExists.Value is not null && repoExists.Value != $"{repo.Owner}/{repo.Repository}")
            {
                var parts = repoExists.Value.Split('/');
                var newOwner = parts[0];
                var newRepoName = parts[1];
                var toastPrefix = $"{repo.Owner}/{repo.Repository} updated with new repository";
                if (repo.Owner != newOwner)
                {
                    var updated = await repos.UpdateOwnerAsync(newOwner, repo.Id, ct);
                    if (updated)
                        updates.Add(new SyncUpdate(false, false, $"{toastPrefix} owner: {newOwner}"));
                }
                if (repo.Repository != newRepoName)
                {
                    var updated = await repos.UpdateNameAsync(newRepoName, repo.Id, ct);
                    if (updated)
                        updates.Add(new SyncUpdate(false, false, $"{toastPrefix} name: {newRepoName}"));
                }
            }

            if (repoExists.Error?.StatusCode == HttpStatusCode.NotFound)
            {
                var updated = await flags.AddForRepoAsync(repo.Id, SyncFlagTypes.RepoDoesNotExist, ct);
                if (updated > 0)
                    updates.Add(new SyncUpdate(false, true, 
                        $"{updated} reports flagged for non-existent repository: {repo.Owner}/{repo.Repository}"));
            } 
            
            // break early if something went wrong
            if (!ErrorChecks(repoExists, [HttpStatusCode.NotFound], updates)) return false;
        }
        return true;
    }

    private static bool ErrorChecks<T>(
        GitHubResult<T> result, IEnumerable<HttpStatusCode> ignoreCodes, List<SyncUpdate> updates)
    {
        if (result.IsUnavailable)
        {
            updates.Add(new SyncUpdate(true, false, "Synchronization incomplete: GitHub could not be reached."));
            return false;
        }
        if (result.Error is not null && !ignoreCodes.Contains(result.Error.StatusCode))
        {
            updates.Add(new SyncUpdate(true, false, $"Synchronization incomplete: {result.Error.Message}"));
            return false;
        }
        return true;
    }
}
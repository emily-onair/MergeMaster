using System.Net;
using System.Text.Json;

namespace MergeMaster.Api.Services.GitHub;

using MergeMaster.Api.Models;
using MergeMaster.Api.Services.GitHub.Models;

public sealed class GitHubApi(HttpClient http) : IGitHubApi
{
    private static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web)
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
    };

    public async Task<GitHubResult<bool>> CheckForCommitOnBranchAsync(string ownerRepo, string branch, string commit, CancellationToken ct)
    {  
        return await NetworkGuardAsync(async () => {
            using var response = await http.GetAsync($"/repos/{ownerRepo}/compare/{branch}...{commit}", ct);
            if (!response.IsSuccessStatusCode)
                return await ErrorResultAsync<bool>(response);
            
            var json = await response.Content.ReadFromJsonAsync<ResultCompare>(Json, ct);
            var isOnBranch = json is not null && (json.Status == "behind" || json.Status == "identical");
            if (isOnBranch)
                return GitHubResult<bool>.Success(isOnBranch);
            return GitHubResult<bool>.Fail(new HttpError(HttpStatusCode.BadRequest, $"Commit {commit} not on branch {branch}"));         
        }, ct);
    }

    public async Task<GitHubResult<bool>> CheckForCommitAlreadyMerged(string ownerRepo, string commit, CancellationToken ct)
    {  
        return await NetworkGuardAsync(async () => {
            // check if commit exists on main branch
            using var resCompare = await http.GetAsync($"/repos/{ownerRepo}/compare/main...{commit}", ct);
            if (!resCompare.IsSuccessStatusCode)
                return await ErrorResultAsync<bool>(resCompare);

            var jsonCompare = await resCompare.Content.ReadFromJsonAsync<ResultCompare>(Json, ct);
            var isOnMain = jsonCompare is not null && (jsonCompare.Status == "behind" || jsonCompare.Status == "identical");
            if (isOnMain)
                return GitHubResult<bool>.Fail(
                    new HttpError(HttpStatusCode.BadRequest, 
                    $"Commit {commit} has already been merged into main"));

            // check for squash merge - only shows if performed via pull request
            using var resPulls = await http.GetAsync($"/repos/{ownerRepo}/commits/{commit}/pulls", ct);
            if (!resPulls.IsSuccessStatusCode)
                return await ErrorResultAsync<bool>(resPulls);

            var jsonPulls = await resPulls.Content.ReadFromJsonAsync<List<ResultPull>>(Json, ct) ?? [];
            var IsSquashedViaPull = jsonPulls.FirstOrDefault(p => p.MergedAt is not null && p.Base.Ref == "main");
            if (IsSquashedViaPull is not null) {
                return GitHubResult<bool>.Fail(
                    new HttpError(HttpStatusCode.BadRequest, 
                    $"Commit {commit} has already been merged into main"));
            }
            
            return GitHubResult<bool>.Success(false);
        }, ct);
    }

    public async Task<GitHubResult<ResultCommit>> GetCommitDetails(string ownerRepo, string commit, CancellationToken ct)
    {
        return await NetworkGuardAsync(async () => {
            using var response = await http.GetAsync($"/repos/{ownerRepo}/commits/{commit}", ct);
            if (!response.IsSuccessStatusCode)
                return await ErrorResultAsync<ResultCommit>(response);
            
            var json = await response.Content.ReadFromJsonAsync<ResultCommit>(Json, ct);
            if (json is not null) return GitHubResult<ResultCommit>.Success(json);
            else return GitHubResult<ResultCommit>.ProceedDespiteError();
        }, ct);
    }

    public async Task<List<ReportSyncError>> SynchronizeWithGitHub()
    {
        // repo check - 404 could be token lost access
        // general error
        
        // resend same ETag with IfNoneMatch header for 304 NotModified res
        // saves token rate

        // get branches from unarchived reports
        // check each branch name matches db - rename will be caught here

        // get commits
        // if exists but not on branch = rebase or amend?
        // check if merged into main
        // if not, could still be a squash merge - check commit for pulls for merged_at
        return [];
    }

    public bool ShouldProceed(HttpResponseMessage response)
    {
        if (response.StatusCode == HttpStatusCode.BadGateway
            || response.StatusCode == HttpStatusCode.ServiceUnavailable
            || response.StatusCode == HttpStatusCode.TooManyRequests)
        {
            // github down or rate limit reached
            return true;
        } 
        if (response.StatusCode == HttpStatusCode.Forbidden
            && response.Headers.TryGetValues("X-RateLimit-Remaining", out var remaining)
            && remaining.FirstOrDefault() == "0")
        {
            // 403 forbidden triggered by rate limit
            return true;
        } 
        // any other error
        return false;
    }

    private static async Task<string> ExtractErrorMessageAsync(HttpResponseMessage response)
    {
        var genericFallback = $"GitHub error: {response.ReasonPhrase}";
        var content = await response.Content.ReadAsStringAsync();
        if (string.IsNullOrEmpty(content)) return genericFallback;
        try {
            using var json = JsonDocument.Parse(content);
            if (json.RootElement.TryGetProperty("message", out var message)) {
                var str = message.GetString();
                return !string.IsNullOrEmpty(str) ? str : genericFallback;
            }
            return content.Length < 200 ? content : genericFallback;
        } catch (JsonException) { 
            // if it's not JSON, but short text, return the raw content
            return content.Length < 200 ? content : genericFallback; 
        }
    }

    private async Task<GitHubResult<T>> ErrorResultAsync<T>(HttpResponseMessage response)
    {
        if (ShouldProceed(response))
            return GitHubResult<T>.ProceedDespiteError();
        var message = await ExtractErrorMessageAsync(response);
        return GitHubResult<T>.Fail(new HttpError(response.StatusCode, message));
    }

    private static async Task<GitHubResult<T>> NetworkGuardAsync<T>(
        Func<Task<GitHubResult<T>>> call, CancellationToken ct
    )
    {
        try
        {
            return await call();
        } catch (Exception ex) 
            when (!ct.IsCancellationRequested &&
                ex is HttpRequestException
                or OperationCanceledException   // HttpClient timeout
                or Polly.ExecutionRejectedException)
        {
            return GitHubResult<T>.ProceedDespiteError();
        }

    }
}
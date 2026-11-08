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
        using var response = await http.GetAsync($"/repos/{ownerRepo}/compare/{branch}...{commit}", ct);
        if (response.IsSuccessStatusCode) {
            var json = await response.Content.ReadFromJsonAsync<ResultCompare>(Json, ct);
            var isMatch = json is not null && (json.Status == "behind" || json.Status == "identical");
            if (isMatch)
                return GitHubResult<bool>.Success(isMatch);
            return GitHubResult<bool>.Fail(new HttpError(HttpStatusCode.BadRequest, $"Commit {commit} not on branch {branch}"));
        }
        // TODO: refactor this section in this and IsCommitAlreadyMerged functions
        var message = await ExtractErrorMessageAsync(response);
        if (ShouldProceed(response))
            return GitHubResult<bool>.ProceedDespiteError(false);
        return GitHubResult<bool>.Fail(new HttpError(response.StatusCode, message));            
    }

    public async Task<GitHubResult<bool>> CheckForCommitAlreadyMerged(string ownerRepo, string commit, CancellationToken ct)
    {  
        using var response = await http.GetAsync($"/repos/{ownerRepo}/compare/main...{commit}", ct);
        if (response.IsSuccessStatusCode) {
            var json = await response.Content.ReadFromJsonAsync<ResultCompare>(Json, ct);
            var isMatch = json is not null && (json.Status == "behind" || json.Status == "identical");
            if (isMatch)
                return GitHubResult<bool>.Fail(new HttpError(HttpStatusCode.BadRequest, $"Commit {commit} has already been merged into main"));
            return GitHubResult<bool>.Success(isMatch);
        }
        var message = await ExtractErrorMessageAsync(response);
        if (ShouldProceed(response))
            return GitHubResult<bool>.ProceedDespiteError(false);
        return GitHubResult<bool>.Fail(new HttpError(response.StatusCode, message));            
    }

    public async Task<GitHubResult<ResultCommit>> GetCommitDetails(string ownerRepo, string commit, CancellationToken ct)
    {
        using var response = await http.GetAsync($"/repos/{ownerRepo}/commits/{commit}", ct);
        if (response.IsSuccessStatusCode)
        {
            var json = await response.Content.ReadFromJsonAsync<ResultCommit>(Json, ct);
            if (json is not null) return GitHubResult<ResultCommit>.Success(json);
            else return GitHubResult<ResultCommit>.ProceedDespiteError(json);
        }
        if (ShouldProceed(response))
            return GitHubResult<ResultCommit>.ProceedDespiteError(null);
        var message = await ExtractErrorMessageAsync(response);
        return GitHubResult<ResultCommit>.Fail(new HttpError(response.StatusCode, message));
        // response.EnsureSuccessStatusCode();
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
        using var json = JsonDocument.Parse(content);
        if (json.RootElement.TryGetProperty("message", out var message)) {
            var str = message.GetString();
            return !string.IsNullOrEmpty(str) ? str : genericFallback;
        }
        // if it's not JSON, but short text, return the raw content
        return content.Length < 200 ? content : genericFallback;
    }

    public async Task<List<ReportSyncError>> SynchronizeWithGitHub()
    {
        
        return [];
    }
}
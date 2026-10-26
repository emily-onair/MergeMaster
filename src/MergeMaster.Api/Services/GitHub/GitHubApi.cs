using System.Net;
using System.Text.Json;

namespace MergeMaster.Api.Services.GitHub;
using MergeMaster.Api.Services.GitHub.Models;

public sealed class GitHubApi(HttpClient http)
{
    private static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web)
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
    };

    /// <summary>Compares two refs (branch or SHA). Returns null if GitHub says 404.</summary>
    public async Task<bool?> IsCommitOnBranchAsync(string ownerRepo, string branch, string commit, CancellationToken ct)
    {  
        using var response = await http.GetAsync($"/repos/{ownerRepo}/compare/{branch}...{commit}", ct);
        if (response.StatusCode == HttpStatusCode.NotFound) return null;
        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadFromJsonAsync<ResultCompare>(Json, ct);
        return json is not null && (json.Status == "behind" || json.Status == "identical");
    }

    public async Task<ResultCommit?> GetReportDetails(string ownerRepo, string commit, CancellationToken ct)
    {
        using var response = await http.GetAsync($"/repos/{ownerRepo}/commits/{commit}", ct);
        if (response.StatusCode == HttpStatusCode.NotFound) return null;
        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadFromJsonAsync<ResultCommit>(Json, ct);
        return json;
    }
}
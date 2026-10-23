using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace MergeMaster.Api.Github;

public sealed class GitHubApi(HttpClient http)
{
    private static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web)
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
    };

    /// <summary>Compares two refs (branch or SHA). Returns null if GitHub says 404.</summary>
    public async Task<bool?> CommitOnBranchAsync(string ownerRepo, string branch, string commit, CancellationToken ct)
    {  
        using var response = await http.GetAsync($"/repos/{ownerRepo}/compare/{branch}...{commit}", ct);
        if (response.StatusCode == HttpStatusCode.NotFound) return null;
        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadFromJsonAsync<CompareResult>(Json, ct);
        return json is not null && (json.Status == "behind" || json.Status == "identical");
    }
}



public sealed record CompareResult(string Status, int AheadBy, int BehindBy);
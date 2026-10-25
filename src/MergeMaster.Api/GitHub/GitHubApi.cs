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
    public async Task<bool?> IsCommitOnBranchAsync(string ownerRepo, string branch, string commit, CancellationToken ct)
    {  
        using var response = await http.GetAsync($"/repos/{ownerRepo}/compare/{branch}...{commit}", ct);
        if (response.StatusCode == HttpStatusCode.NotFound) return null;
        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadFromJsonAsync<CompareResult>(Json, ct);
        return json is not null && (json.Status == "behind" || json.Status == "identical");
    }

    public async Task<CommitResult?> GetReportDetails(string ownerRepo, string commit, CancellationToken ct)
    {
        using var response = await http.GetAsync($"/repos/{ownerRepo}/commits/{commit}", ct);
        if (response.StatusCode == HttpStatusCode.NotFound) return null;
        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadFromJsonAsync<CommitResult>(Json, ct);
        return json;
    }
}

public sealed record CompareResult(string Status, int AheadBy, int BehindBy);
public sealed record CommitAuthor(string Name, string Email, DateTimeOffset Date);
public sealed record Commit(CommitAuthor Author, string Message);

public sealed record CommitResult(Commit Commit);
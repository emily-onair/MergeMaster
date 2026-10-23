using System.ComponentModel.DataAnnotations;

namespace MergeMaster.Api.GitHub;

public sealed class GitHubOptions
{
    public const string Section = "GitHub";

    [Required] public string Token { get; set; } = "";
    [Required] public string BaseUrl { get; set; } = "https://api.github.com";
    [Required] public string ApiVersion { get; set; } = "2026-03-10";
}
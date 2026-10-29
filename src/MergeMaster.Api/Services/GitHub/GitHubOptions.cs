using System.ComponentModel.DataAnnotations;

namespace MergeMaster.Api.Services.GitHub;

public sealed class GitHubOptions
{
    public const string Section = "GitHub";

    [Required] public required string Token { get; init; }
    [Required] public required string BaseUrl { get; init; }
    [Required] public required string ApiVersion { get; init; }
}
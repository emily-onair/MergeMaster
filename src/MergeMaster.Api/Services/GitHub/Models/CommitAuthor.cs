namespace MergeMaster.Api.Services.GitHub.Models;

public sealed record CommitAuthor(string Name, string Email, DateTimeOffset Date);
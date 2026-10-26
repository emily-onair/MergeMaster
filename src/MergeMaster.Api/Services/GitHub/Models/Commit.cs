namespace MergeMaster.Api.Services.GitHub.Models;

public sealed record Commit(CommitAuthor Author, string Message);
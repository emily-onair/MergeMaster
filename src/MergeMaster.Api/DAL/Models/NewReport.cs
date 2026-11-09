namespace MergeMaster.Api.DAL.Models;

public sealed record NewReport(
    DateTime CreatedAtUtc, string Owner, string Repository, 
    string Branch, string CommitSha, string AuthorName, bool Urgent, 
    string Tags, DateTime CommitDateUtc, string? CommitMessage = null);
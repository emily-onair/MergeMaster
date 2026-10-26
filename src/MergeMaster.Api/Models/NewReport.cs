namespace MergeMaster.Api.Models;

public sealed record NewReport(
    string Repository, 
    string Branch, 
    string CommitSha,
    string CommitMessage,
    DateTimeOffset CommitDate,
    string AuthorName,
    string AuthorEmail,
    bool Urgent,
    string Tags,
    string? Notes
);
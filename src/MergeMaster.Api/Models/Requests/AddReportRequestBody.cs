namespace MergeMaster.Api.Models.Requests;

public sealed record AddReportRequestBody(
    string Owner,
    string Repository,
    string Branch,
    string CommitSha,
    DateTimeOffset CommitDate,
    string AuthorName,
    bool Urgent,
    string Tags,
    string? CommitMessage = null
);
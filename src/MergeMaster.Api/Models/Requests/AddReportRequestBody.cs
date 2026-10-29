namespace MergeMaster.Api.Models.Requests;

public sealed record AddReportRequestBody(
    string OwnerRepo,
    string Repository,
    string Branch,
    string AuthorName,
    string AuthorEmail,
    string CommitSha,
    bool Urgent,
    string Tags,
    string? Notes
);
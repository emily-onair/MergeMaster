
using Microsoft.AspNetCore.Mvc;

using MergeMaster.Api.Services.GitHub;
using MergeMaster.Api.Data;
using MergeMaster.Api.Models.Requests;

namespace MergeMaster.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class ReportsController(GitHubApi github, ReportStore store) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAllReports(CancellationToken ct)
    {
        var reports = await store.FindAllAsync(ct);
        // TODO: revalidate each row before sending to frontend
        // TODO: pagination 
        return Ok(reports);
    }

    [HttpPost("add")]
    public async Task<IActionResult> AddAsync(AddReportRequestBody body, CancellationToken ct)
    {
        Console.WriteLine($"REQUEST BODY: {body}");

        var isDuplicate = await store.FindDuplicateAsync(body.CommitSha, ct);
        if (isDuplicate)
            return Problem(statusCode: 400, detail: "A report already exists for this commit");

        var isCommitOnBranch = await github.CheckForCommitOnBranchAsync(body.OwnerRepo, body.Branch, body.CommitSha, ct);
        if (!isCommitOnBranch.Proceed)
            return Problem(
                statusCode: (int)isCommitOnBranch.Error.StatusCode, 
                detail: isCommitOnBranch.Error.Message);

        var IsAlreadyMerged = await github.CheckForCommitAlreadyMerged(body.OwnerRepo, body.CommitSha, ct);
        if (!IsAlreadyMerged.Proceed)
            return Problem(
                statusCode: (int)IsAlreadyMerged.Error.StatusCode, 
                detail: IsAlreadyMerged.Error.Message);

        var newReport = new ReportEntity() {
            CreatedAtUtc = DateTime.UtcNow,
            Repository = body.Repository,
            Branch = body.Branch,
            CommitSha = body.CommitSha,
            AuthorName = body.AuthorName,
            AuthorEmail = body.AuthorEmail,
            Urgent = body.Urgent,
            Tags = body.Tags,
            Notes = body.Notes
        };

        var missingDetails = await github.GetCommitDetails(body.OwnerRepo, body.CommitSha, ct);
        if (!missingDetails.Proceed)
            return Problem(
                statusCode: (int)missingDetails.Error.StatusCode, 
                detail: missingDetails.Error.Message);
        if (missingDetails.Value is not null) {
            newReport.CommitMessage = missingDetails.Value.Commit.Message;
            newReport.CommitDateUtc = missingDetails.Value.Commit.Author.Date;
            newReport.Validated = true;
        }

        await store.AddAsync(newReport, ct);
        return Ok("New report successfully created");
    }
}
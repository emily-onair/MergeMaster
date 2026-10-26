
using Microsoft.AspNetCore.Mvc;

using MergeMaster.Api.Services.GitHub;
using MergeMaster.Api.Data;
using MergeMaster.Api.Models.Requests;
using MergeMaster.Api.Models;

namespace MergeMaster.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class ReportsController(GitHubApi github, ReportStore store) : ControllerBase
{
    [HttpPost("add")]
    public async Task<IActionResult> AddAsync(AddReportRequestBody body, CancellationToken ct)
    {
        Console.WriteLine($"REQUEST BODY: {body}");
        // TODO: handle no github connection
        var problem = await CheckAgainstGitHubAsync(body, ct);
        if (problem is not null) return problem;
        var missingDetails = await github.GetReportDetails(body.OwnerRepo, body.CommitSha, ct);
        if (missingDetails is null) {
            return Problem(statusCode: 404, detail: "GitHub could not provide details");
        }
        var newReport = new NewReport(
            body.Repository, 
            body.Branch, 
            body.CommitSha,
            missingDetails.Commit.Message,
            missingDetails.Commit.Author.Date,
            body.AuthorName,
            missingDetails.Commit.Author.Email,
            body.Urgent,
            body.Tags,
            body.Notes
        );
        var isDuplicate = store.ReportIsDuplicate(body.CommitSha);
        if (isDuplicate)
            return Problem(statusCode: 400, detail: "A report already exists for this commit on this branch");
        store.Add(newReport);
        return Ok("New report successfully created");
    }

    private async Task<ObjectResult?> CheckAgainstGitHubAsync(AddReportRequestBody body, CancellationToken ct)
    {
        var commitOnBranch = await github.IsCommitOnBranchAsync(body.OwnerRepo, body.Branch, body.CommitSha, ct);
        if (commitOnBranch is null)
            return Problem(statusCode: 404, detail: "GitHub could not find branch or commit");
        if (commitOnBranch is false)
            return Problem(statusCode: 400, detail: "This commit does not exist on the branch");
        var alreadyMerged = await github.IsCommitOnBranchAsync(body.OwnerRepo, "main", body.CommitSha, ct);
        if (alreadyMerged is null)
            return Problem(statusCode: 404, detail: "GitHub could not find main branch");
        if (alreadyMerged is true)
            return Problem(statusCode: 400, detail: "This commit has already been merged into main");
        return null;
    }
}


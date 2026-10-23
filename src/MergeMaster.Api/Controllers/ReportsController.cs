
using Microsoft.AspNetCore.Mvc;
using MergeMaster.Api.Github;

namespace MergeMaster.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class ReportsController(GitHubApi github, ReportStore store) : ControllerBase
{
    [HttpPost("add")]
    public async Task<IActionResult> Add(AddReportRequestBody body, CancellationToken ct)
    {
        Console.WriteLine($"REQUEST BODY: {body}");
        // TODO: handle no github connection
        var commitOnBranch = await github.CommitOnBranchAsync(body.OwnerRepo, body.Branch, body.CommitSha, ct);
        if (commitOnBranch is null)
            return Problem(statusCode: 404, detail: "GitHub could not find branch or commit");
        if (commitOnBranch is false)
            return Problem(statusCode: 400, detail: "This commit does not exist on the branch");
        var alreadyMerged = await github.CommitOnBranchAsync(body.OwnerRepo, "main", body.CommitSha, ct);
        if (alreadyMerged is null)
            return Problem(statusCode: 404, detail: "GitHub could not find main branch");
        if (alreadyMerged is true)
            return Problem(statusCode: 400, detail: "This commit has already been merged into main");
        var missingDetails = await github.GetReportDetails(body.OwnerRepo, body.CommitSha, ct);
        if (missingDetails is null) {
            return Problem(statusCode: 404, detail: "GitHub could not provide details");
        }
        var newReport = new NewReport(
            body.Repository, 
            body.Branch, 
            body.CommitSha,
            missingDetails.Message,
            missingDetails.Author.Date,
            missingDetails.Author.Name,
            missingDetails.Author.Email,
            body.Urgent,
            body.Tags,
            body.Notes
        );
        var branchExists = store.ReportForBranchExists(body.Branch);
        if (branchExists)
        {
            var isDuplicate = store.ReportIsDuplicate(body.CommitSha);
            if (isDuplicate)
                return Problem(statusCode: 400, detail: "A report already exists for this commit on this branch");
            var updateSuccessful = store.ReportUpdated(newReport);
            return updateSuccessful
                ? Ok($"Existing report for branch {newReport.Branch} updated")
                : Problem(statusCode: 500, detail: "Record exists for this branch, but could not be updated");
        }
        store.Add(newReport);
        return Ok("New report successfully created");
    }
}

// { OwnerRepo = emily-onair/MergeMaster, Repository = MergeMaster, Branch = main, CommitSha = a3a28c25d023822f763b4c5dd7a4a95e37a6f765, Urgent = False, Tags = chore,docs, Notes =  }

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

        var isDuplicate = store.ReportIsDuplicate(body.CommitSha);
        if (isDuplicate)
            return Problem(statusCode: 400, detail: "A report already exists for this commit");

        var isCommitOnBranch = await github.IsCommitOnBranchAsync(body.OwnerRepo, body.Branch, body.CommitSha, ct);
        if (!isCommitOnBranch.Proceed)
            return Problem(
                statusCode: (int)isCommitOnBranch.Error!.StatusCode, 
                detail: isCommitOnBranch.Error.Message);

        var alreadyMerged = await github.IsCommitAlreadyMerged(body.OwnerRepo, body.CommitSha, ct);
        if (!alreadyMerged.Proceed)
            return Problem(
                statusCode: (int)alreadyMerged.Error!.StatusCode, 
                detail: alreadyMerged.Error.Message);

        var newReport = new NewReport(
            Repository: body.Repository,
            Branch: body.Branch,
            CommitSha: body.CommitSha,
            AuthorName: body.AuthorName,
            Urgent: body.Urgent,
            Tags: body.Tags,
            Notes: body.Notes
        );

        var missingDetails = await github.GetCommitDetails(body.OwnerRepo, body.CommitSha, ct);
        if (missingDetails.Proceed && missingDetails.Value is not null) {
            newReport = newReport with
            {
                AuthorEmail = missingDetails.Value.Commit.Author.Email,
                CommitMessage = missingDetails.Value.Commit.Message,
                CommitDate = missingDetails.Value.Commit.Author.Date,
                Validated = true
            };
        }

        store.Add(newReport);
        return Ok("New report successfully created");
    }
}
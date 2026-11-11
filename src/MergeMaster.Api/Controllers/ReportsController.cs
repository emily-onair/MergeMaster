
using Microsoft.AspNetCore.Mvc;

using MergeMaster.Api.Services.GitHub;
using MergeMaster.Api.Models.Requests;
using MergeMaster.Api.DAL.Repositories;
using MergeMaster.Api.DAL.Models;

namespace MergeMaster.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class ReportsController(GitHubApi github, ReportStore store) : ControllerBase
{
    // [HttpGet]
    // public async Task<IActionResult> GetAllReports(CancellationToken ct)
    // {
    //     var reports = await store.FindAllAsync(ct);
    //     // TODO: revalidate each row before sending to frontend
    //     // TODO: pagination 
    //     return Ok(reports);
    // }

    [HttpPost("add")]
    public async Task<IActionResult> AddAsync(AddReportRequestBody body, CancellationToken ct)
    {
        Console.WriteLine($"REQUEST BODY: {body}");

        var isDuplicate = await store.FindDuplicateAsync(body.CommitSha, ct);
        if (isDuplicate)
            return Problem(statusCode: 400, detail: "A report already exists for this commit");

        var ownerRepo = $"{body.Owner}/{body.Repository}";

        var isCommitOnBranch = await github.CheckForCommitOnBranchAsync(ownerRepo, body.Branch, body.CommitSha, true, ct);
        if (!isCommitOnBranch.Proceed)
            return Problem(
                statusCode: (int)isCommitOnBranch.Error.StatusCode, 
                detail: isCommitOnBranch.Error.Message);

        var IsAlreadyMerged = await github.CheckForCommitAlreadyMerged(ownerRepo, body.CommitSha, true, ct);
        if (!IsAlreadyMerged.Proceed)
            return Problem(
                statusCode: (int)IsAlreadyMerged.Error.StatusCode, 
                detail: IsAlreadyMerged.Error.Message);

        var newReport = new NewReport(
            CreatedAtUtc: DateTime.UtcNow,
            Owner: body.Owner,
            Repository: body.Repository,
            Branch: body.Branch,
            CommitSha: body.CommitSha,
            AuthorName: body.AuthorName,
            Urgent: body.Urgent,
            Tags: body.Tags,
            CommitDateUtc: body.CommitDate.UtcDateTime
        );

        var missingDetails = await github.GetCommitDetails(ownerRepo, body.CommitSha, true, ct);
        if (!missingDetails.Proceed)
            return Problem(
                statusCode: (int)missingDetails.Error.StatusCode, 
                detail: missingDetails.Error.Message);
        if (missingDetails.Value is not null) {
            newReport = newReport 
                with { CommitMessage = missingDetails.Value.Commit.Message };
        }

        await store.AddAsync(newReport, ct);
        return Ok("New report successfully created");
    }
}
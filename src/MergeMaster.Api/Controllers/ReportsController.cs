
using Microsoft.AspNetCore.Mvc;
using MergeMaster.Api.Github;
using Polly.Fallback;

namespace MergeMaster.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class ReportsController(GitHubApi github) : ControllerBase
{
    [HttpPost("add")]
    public async Task<IActionResult> Add(AddReportRequestBody body, CancellationToken ct)
    {
        Console.WriteLine(body);
        // Compare branch + sha
        var commitOnBranch = await github.CommitOnBranchAsync(body.OwnerRepo, body.Branch, body.CommitSha, ct);
        if (commitOnBranch is null)
            return Problem(statusCode: 404, detail: "GitHub could not find branch or commit");
        if (commitOnBranch is false)
            return Problem(statusCode: 422, detail: "This commit does not exist on the branch");
        var alreadyCommitted = await github.CommitOnBranchAsync(body.OwnerRepo, "main", body.CommitSha, ct);
        if (alreadyCommitted is null)
            return Problem(statusCode: 404, detail: "GitHub could not find main branch");
        if (alreadyCommitted is true)
            return Problem(statusCode: 422, detail: "This commit has already been merged into main");
        
        
        return Ok("would create ");
    }
}

// { OwnerRepo = emily-onair/MergeMaster, Repository = MergeMaster, Branch = main, CommitSha = a3a28c25d023822f763b4c5dd7a4a95e37a6f765, Urgent = False, Tags = chore,docs, Notes =  }
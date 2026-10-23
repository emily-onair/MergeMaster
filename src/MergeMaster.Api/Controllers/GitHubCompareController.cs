using MergeMaster.Api.Github;
using MergeMaster.Api.GitHub;
using Microsoft.AspNetCore.Mvc;

namespace MergeMaster.Api.Controllers;

// [ApiController]
// [Route("api/github")]
// public sealed class GitHubCompareController(GitHubApi github) : ControllerBase
// {
//     [HttpGet("compare/{owner}/{repo}")]
//     public async Task<IActionResult> Compare(
//         string owner, 
//         string repo, 
//         [FromQuery] string branch, 
//         [FromQuery] string commit, 
//         CancellationToken ct
//     )
//     {
//         var result = await github.CompareBranchToCommitAsync(owner repo, branch, commit, ct);
//         return result is null ? NotFound() : Ok(result);
//     }
// }
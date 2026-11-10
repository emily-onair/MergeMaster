using MergeMaster.Api.Models;

namespace MergeMaster.Api.Extensions;

public static class SyncFlagMessages
{
    public static string ToMessage(this SyncFlagTypes flag) => flag switch
    {
        SyncFlagTypes.RepoDoesNotExist    => "Repository does not exist on GitHub",
        SyncFlagTypes.BranchDoesNotExist  => "Branch does not exist on GitHub",
        SyncFlagTypes.CommitDoesNotExist  => "Commit not found in repository",
        SyncFlagTypes.CommitNotOnBranch   => "Commit is not on the reported branch",
        SyncFlagTypes.CommitAlreadyMerged => "Commit has already been merged",
        _ => "Out of sync with GitHub"
    };
}

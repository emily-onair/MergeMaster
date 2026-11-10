namespace MergeMaster.Api.Models;

public enum SyncFlagTypes
{
    RepoDoesNotExist,
    BranchDoesNotExist,
    CommitDoesNotExist,
    CommitNotOnBranch,
    CommitAlreadyMerged
}
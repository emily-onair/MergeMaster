namespace MergeMaster.Api.Models;

public enum ReportSyncFlagTypes
{
    RepoDoesNotExist,
    BranchDoesNotExist,
    CommitDoesNotExist,
    CommitNotOnBranch,
    CommitAlreadyMerged,
    Other
}
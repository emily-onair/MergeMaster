namespace MergeMaster.Api.Models;

public enum ReportSyncErrorTypes
{
    CommitDoesNotExist,     // deleted
    BranchDoesNotExist,     // deleted, renamed
    CommitAlreadyMerged,
    CommitNotOnBranch,
    Unknown
}
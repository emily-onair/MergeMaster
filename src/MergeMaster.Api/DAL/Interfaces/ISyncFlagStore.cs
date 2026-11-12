using MergeMaster.Api.Models;
using MergeMaster.Api.Models.ViewModels;

namespace MergeMaster.Api.DAL.Interfaces;

public interface ISyncFlagStore
{
    Task<List<ReportFlag>> GetByReportAsync(int reportId, CancellationToken ct);
    Task AddSingleAsync(int reportId, SyncFlagTypes type, CancellationToken ct);
    Task<int> AddForRepoAsync(int repoId, SyncFlagTypes type, CancellationToken ct);
    Task<int> AddForBranchAsync(int branchId, SyncFlagTypes type, CancellationToken ct);
}
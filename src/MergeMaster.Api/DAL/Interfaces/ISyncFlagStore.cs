using MergeMaster.Api.Models;
using MergeMaster.Api.Models.ViewModels;

namespace MergeMaster.Api.DAL.Interfaces;

public interface ISyncFlagStore
{
    public Task<List<ReportFlag>> GetByReportAsync(int reportId, CancellationToken ct);
    public Task AddSingleAsync(int reportId, SyncFlagTypes type, CancellationToken ct);
    public Task<int> AddForRepoAsync(int repoId, SyncFlagTypes type, CancellationToken ct);
    public Task<int> AddForBranchAsync(int branchId, SyncFlagTypes type, CancellationToken ct);
}
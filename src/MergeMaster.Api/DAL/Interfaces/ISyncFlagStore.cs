using System.Linq.Expressions;
using MergeMaster.Api.DAL.Models;
using MergeMaster.Api.Models;
using MergeMaster.Api.Models.ViewModels;

namespace MergeMaster.Api.DAL.Interfaces;

public interface ISyncFlagStore
{
    public Task<List<ReportFlag>> GetByReportAsync(int reportId, CancellationToken ct);
    public Task AddSingleAsync(int reportId, SyncFlagTypes type, CancellationToken ct);
    public Task<int> AddManyAsync(SyncFlagTypes type, 
        Expression<Func<ReportEntity, bool>> selector, CancellationToken ct);
}
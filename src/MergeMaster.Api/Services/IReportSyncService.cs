using MergeMaster.Api.Models.ViewModels;

namespace MergeMaster.Api.Services;

public interface IReportSyncService
{
    Task<List<SyncUpdate>> SyncWithGitHubAsync(CancellationToken ct);
}
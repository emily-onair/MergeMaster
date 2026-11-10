namespace MergeMaster.Api.Services;

public interface IReportSyncService
{
    public Task<List<SyncUpdate>> SyncWithGitHubAsync(CancellationToken ct);
}
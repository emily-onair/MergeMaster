using Microsoft.AspNetCore.Mvc;

using MergeMaster.Api.DAL.Repositories;
using MergeMaster.Api.Models.ViewModels;
using MergeMaster.Api.Services;
using System.Text.Json;

namespace MergeMaster.Api.Pages;

public sealed class IndexModel(ReportStore store, IReportSyncService syncService) : ReportsPageModel(store)
{
    public override bool IsArchive => false;
    [TempData]
    public string? SyncUpdatesJson { get; set; }
    public List<SyncUpdate> SyncUpdates => SyncUpdatesJson is null
        ? [] : JsonSerializer.Deserialize<List<SyncUpdate>>(SyncUpdatesJson) ?? [];
    public async Task<IActionResult> OnPostArchiveAsync(int id, CancellationToken ct)
    {
        await Store.ArchiveAsync(id, ct);
        return RedirectWithFilters;
    }

    public async Task<IActionResult> OnPostSynchronizeAsync(CancellationToken ct)
    {
        var updates = await syncService.SyncWithGitHubAsync(ct);
        SyncUpdatesJson = JsonSerializer.Serialize(updates);
        return RedirectWithFilters;
    }
}
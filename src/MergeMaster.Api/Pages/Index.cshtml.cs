using Microsoft.AspNetCore.Mvc;

namespace MergeMaster.Api.Pages;
using MergeMaster.Api.Data;
using MergeMaster.Api.Models.ViewModels;

public sealed class IndexModel(ReportStore store) : ReportsPageModel(store)
{
    public override bool IsArchive => false;
    public async Task<IActionResult> OnPostArchiveAsync(int id, CancellationToken ct)
    {
        await Store.ArchiveAsync(id, ct);
        return RedirectWithFilters;
    }
}
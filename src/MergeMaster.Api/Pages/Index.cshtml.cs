using Microsoft.AspNetCore.Mvc;

using MergeMaster.Api.DAL.Repositories;
using MergeMaster.Api.Models.ViewModels;

namespace MergeMaster.Api.Pages;

public sealed class IndexModel(ReportStore store) : ReportsPageModel(store)
{
    public override bool IsArchive => false;
    public async Task<IActionResult> OnPostArchiveAsync(int id, CancellationToken ct)
    {
        await Store.ArchiveAsync(id, ct);
        return RedirectWithFilters;
    }
}
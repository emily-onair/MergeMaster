using Microsoft.AspNetCore.Mvc;

using MergeMaster.Api.DAL.Repositories;
using MergeMaster.Api.Models.ViewModels;

namespace MergeMaster.Api.Pages;

public class ArchivedModel(ReportStore store) : ReportsPageModel(store)
{
    public override bool IsArchive => true;

    public async Task<IActionResult> OnPostRestoreAsync(int id, CancellationToken ct)
    {
        await Store.RestoreFromArchiveAsync(id, ct);
        return RedirectWithFilters;
    }
}
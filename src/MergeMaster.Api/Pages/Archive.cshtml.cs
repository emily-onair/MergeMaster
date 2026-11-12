using Microsoft.AspNetCore.Mvc;

using MergeMaster.Api.Models.ViewModels;
using MergeMaster.Api.DAL.Interfaces;

namespace MergeMaster.Api.Pages;

public class ArchivedModel(IReportStore store) : ReportsPageModel(store)
{
    public override bool IsArchive => true;

    public async Task<IActionResult> OnPostRestoreAsync(int id, CancellationToken ct)
    {
        await Store.RestoreFromArchiveAsync(id, ct);
        return RedirectWithFilters;
    }
}
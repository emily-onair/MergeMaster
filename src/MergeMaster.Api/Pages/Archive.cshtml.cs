using MergeMaster.Api.Data;
using MergeMaster.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace MergeMaster.Api.Pages;

public class ArchivedModel(ReportStore store) : ReportsPageModel(store)
{
    public override bool IsArchive => true;
    protected override async Task<IReadOnlyList<Report>> LoadReportsAsync(CancellationToken ct)
    {
        return await Store.FindAllFilteredPagesAsync(IsArchive, UrgentOnly, SelectedAuthors,SelectedTags, ct);
    }

    public async Task<IActionResult> OnPostRestoreAsync(int id, CancellationToken ct)
    {
        await Store.RestoreFromArchiveAsync(id, ct);
        return RedirectToPage(new { UrgentOnly, Descending, SelectedAuthors, SelectedTags });
    }
}
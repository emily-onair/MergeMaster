using Microsoft.AspNetCore.Mvc;

namespace MergeMaster.Api.Pages;
using MergeMaster.Api.Data;
using MergeMaster.Api.Models;
using MergeMaster.Api.Models.ViewModels;

public sealed class IndexModel(ReportStore store) : ReportsPageModel(store)
{
    public override bool IsArchive => false;
    protected override Task<IReadOnlyList<Report>> LoadReportsAsync(CancellationToken ct)
    {
        return Store.FindAllFilteredPagesAsync(IsArchive, UrgentOnly, SelectedAuthors, SelectedTags, SearchValue, ct);
    }
    public async Task<IActionResult> OnPostArchiveAsync(int id, CancellationToken ct)
    {
        await Store.ArchiveAsync(id, ct);
        return RedirectToPage(new { UrgentOnly, Descending, SelectedAuthors, SelectedTags, SearchValue });
    }
}
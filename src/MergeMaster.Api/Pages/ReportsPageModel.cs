using MergeMaster.Api.Data;
using MergeMaster.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MergeMaster.Api.Pages;

public abstract class ReportsPageModel(ReportStore store) : PageModel
{
    protected ReportStore Store { get; } = store;
    [BindProperty(SupportsGet = true)]
    public bool UrgentOnly { get; set; }
    [BindProperty(SupportsGet = true)]
    public bool Descending { get; set; }
    public IReadOnlyList<Report> Reports { get; private set; } = [];
    public abstract bool IsArchive { get; }

    protected abstract Task<IReadOnlyList<Report>> LoadReportsAsync(CancellationToken ct);
    public async Task OnGetAsync(CancellationToken ct)
    {
        var list = await LoadReportsAsync(ct);
        var sorted = Descending ? list.OrderByDescending(r => r.CreatedAt)
                                : list.OrderBy(r => r.CreatedAt);
        Reports = [.. sorted];
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id, CancellationToken ct)
    {
        await Store.DeleteAsync(id, ct);
        return RedirectToPage(new { UrgentOnly, Descending });
    }
}
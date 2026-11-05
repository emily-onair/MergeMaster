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
    public bool Descending { get; set; } = false;
    [BindProperty(SupportsGet = true)]
    public List<string> SelectedAuthors { get; set; } = [];
    [BindProperty(SupportsGet = true)]
    public List<string> SelectedTags { get; set; } = [];
    public List<string> Authors { get; private set; } = [];
    public List<string> Tags { get; private set; } = [];
    public IReadOnlyList<Report> Reports { get; private set; } = [];
    public abstract bool IsArchive { get; }

    protected abstract Task<IReadOnlyList<Report>> LoadReportsAsync(CancellationToken ct);
    public async Task OnGetAsync(CancellationToken ct)
    {
        var list = await LoadReportsAsync(ct);
        var sorted = Descending ? list.OrderByDescending(r => r.CreatedAt)
                                : list.OrderBy(r => r.CreatedAt);
        Authors = await Store.FindAuthorsAsync(IsArchive, ct);
        Tags = await Store.FindTagsAsync(IsArchive, ct);
        Reports = [.. sorted];
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id, CancellationToken ct)
    {
        await Store.DeleteAsync(id, ct);
        return RedirectToPage(new { UrgentOnly, Descending, SelectedAuthors, SelectedTags });
    }
}
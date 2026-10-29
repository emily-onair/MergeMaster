using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MergeMaster.Api.Pages;
using MergeMaster.Api.Data;
using MergeMaster.Api.Models;

public sealed class IndexModel(ReportStore store) : PageModel
{
    [BindProperty(SupportsGet = true)]
    public bool UrgentOnly { get; set; }

    [BindProperty(SupportsGet = true)]
    public bool Descending { get; set; }
    
    public IReadOnlyList<Report> Reports { get; private set; } = [];

    public Dictionary<string, string> CategoryClasses { get; } = new()
    {
        ["bugfix"] = "text-bg-success",
        ["hotfix"] = "text-bg-danger",
        ["feature"] = "text-bg-primary",
        ["extend"] = "text-bg-info",
        ["security"] = "text-bg-security",
        ["chore"] = "text-bg-secondary",
        ["docs"] = "text-bg-light",
        ["claude"] = "claude",
        ["other"] = "text-bg-dark"
    };

    public async Task OnGetAsync(CancellationToken ct)
    {
        var list = UrgentOnly ? await store.FindUrgentOnlyAsync(ct) : await store.FindAllAsync(ct);
        var sorted = Descending  ? list.OrderByDescending(r => r.CreatedAt)
                                 : list.OrderBy(r => r.CreatedAt);
        Reports = [.. sorted];
    } 

    public async Task<IActionResult> OnPostDeleteAsync(int id, CancellationToken ct)
    {
        await store.DeleteAsync(id, ct);
        return RedirectToPage(new { UrgentOnly, Descending });   
    }
}
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

    public void OnGet()
    {
        var list = UrgentOnly ? store.UrgentOnly() : store.All();
        var sorted = Descending  ? list.OrderByDescending(r => r.CreatedAt)
                                 : list.OrderBy(r => r.CreatedAt);
        Reports = [.. sorted];
    } 

    public IActionResult OnPostDelete(int id)
    {
        Console.WriteLine("ID IS :" + id);
        store.Delete(id);
        return RedirectToPage(new { UrgentOnly, Descending });   
    }
}
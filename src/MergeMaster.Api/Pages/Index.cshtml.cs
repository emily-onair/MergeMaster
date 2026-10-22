using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MergeMaster.Api.Pages;

public sealed class IndexModel(ReportStore store) : PageModel
{
    public IReadOnlyList<Report> Reports { get; private set; } = [];

    public void OnGet() => Reports = store.All();
}
using MergeMaster.Api.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MergeMaster.Api.Models.ViewModels;
using MergeMaster.Api.Extensions;

public abstract class ReportsPageModel(ReportStore store) : PageModel
{
    protected ReportStore Store { get; } = store;

    public abstract bool IsArchive { get; }
    // Name = "" binds without a prefix: ?IsUrgentOnly=true instead of ?Filter.IsUrgentOnly=true.
    [BindProperty(SupportsGet = true, Name = "")]
    public ReportFilterOptions FilterOptions { get; set; } = new();
    protected IActionResult RedirectWithFilters => RedirectToPage(FilterOptions);
    public DropdownOptionLists DropdownOptions { get; private set; } = new([], [], [], []);
    public const int PageSize = 2;
    [BindProperty(SupportsGet = true)]
    public int PageNumber { get; set; } = 1;
    public PaginationResult Results { get; private set; } = new([], 0, 1, PageSize);
    public IReadOnlyList<Report> Reports => Results.FilteredReports;
    public async Task OnGetAsync(CancellationToken ct)
    {
        Results = await LoadReportsAsync(ct);
        DropdownOptions = await Store.FindDropdownOptionsAsync(IsArchive, ct);
    }

    private async Task<PaginationResult> LoadReportsAsync(CancellationToken ct)
    {
        return await Store.FindAllFilteredPagesAsync(
            archived: IsArchive, 
            descending: FilterOptions.IsDescending, 
            urgentOnly: FilterOptions.IsUrgentOnly, 
            authors: FilterOptions.SelectedAuthors, 
            tags: FilterOptions.SelectedTags, 
            search: FilterOptions.SearchValue, 
            page: PageNumber,
            pageSize: PageSize,
            ct);
    }

    public string PageUrl(int pageNumber) => Request.UpdateQueryString(("PageNumber", pageNumber.ToString()));

    public string SortUrl => Request.UpdateQueryString(
        ("IsDescending", (!FilterOptions.IsDescending).ToString().ToLowerInvariant()),
        ("PageNumber", null));

    public async Task<IActionResult> OnPostDeleteAsync(int id, CancellationToken ct)
    {
        await Store.DeleteAsync(id, ct);
        return RedirectWithFilters;
    }
}
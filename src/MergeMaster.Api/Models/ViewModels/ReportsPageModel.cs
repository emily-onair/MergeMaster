using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using MergeMaster.Api.Extensions;
using MergeMaster.Api.DAL.Interfaces;
using MergeMaster.Api.DAL.Models;

namespace MergeMaster.Api.Models.ViewModels;

public abstract class ReportsPageModel(IReportStore store) : PageModel
{
    protected IReportStore Store { get; } = store;
    public abstract bool IsArchive { get; }
    // Name = "" binds without a prefix: ?IsUrgentOnly=true instead of ?Filter.IsUrgentOnly=true.
    [BindProperty(SupportsGet = true, Name = "")]
    public ReportFilterOptions FilterOptions { get; set; } = new();
    protected IActionResult RedirectWithFilters => RedirectToPage(FilterOptions);
    public DropdownOptionLists DropdownOptions { get; private set; } = new([], [], [], []);
    public const int PageSize = 25;
    [BindProperty(SupportsGet = true)]
    public int PageNumber { get; set; } = 1;
    public PaginationResult Results { get; private set; } = new([], 0, 1, PageSize);
    public IReadOnlyList<Report> Reports => Results.FilteredReports;
    public async Task OnGetAsync(CancellationToken ct)
    {
        Results = await LoadReportsAsync(ct);
        DropdownOptions = await Store.GetDropdownOptionsAsync(IsArchive, ct);
    }

    public async Task<IActionResult> OnGetFlagsAsync(
        int id, [FromServices] ISyncFlagStore flagStore, CancellationToken ct)
        // [FromServices] method injection from DI container (scoped/singleton)
    {
        var flags = await flagStore.GetByReportAsync(id, ct);
        return Partial("_ModalContentFlags", flags);
    }


    private async Task<PaginationResult> LoadReportsAsync(CancellationToken ct)
    {
        return await Store.FilterAsync(new FilterQueryParams(
        Archived: IsArchive,
        Descending: FilterOptions.IsDescending,
        UrgentOnly: FilterOptions.IsUrgentOnly,
        Authors: FilterOptions.SelectedAuthors,
        Tags: FilterOptions.SelectedTags,
        Search: FilterOptions.SearchValue,
        Page: PageNumber,
        PageSize: PageSize), ct);
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
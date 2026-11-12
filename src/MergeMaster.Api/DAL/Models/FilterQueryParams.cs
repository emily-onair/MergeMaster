namespace MergeMaster.Api.DAL.Models;

public sealed record FilterQueryParams(
    bool Archived,
    bool Descending,
    bool UrgentOnly,
    IReadOnlyList<string> Authors,
    IReadOnlyList<string> Tags,
    string? Search,
    int Page,
    int PageSize);

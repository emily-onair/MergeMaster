namespace MergeMaster.Api.Models.ViewModels;

public sealed record PaginationResult(
    IReadOnlyList<Report> FilteredReports, int TotalCount, int Page, int PageSize)
{
    public int TotalPages => (TotalCount + PageSize - 1) / PageSize;
    public bool HasPrev => Page > 1;
    public bool HasNext => Page < TotalPages;
}
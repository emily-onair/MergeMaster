namespace MergeMaster.Api.Models.ViewModels;

public sealed record PaginationControlViewModel(
    PaginationResult Results,
    Func<int, string> PageUrl)
{
    public string PrevUrl => PageUrl(Results.Page - 1);
    public string NextUrl => PageUrl(Results.Page + 1);
};
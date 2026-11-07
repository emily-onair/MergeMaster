namespace MergeMaster.Api.Models.ViewModels;

public sealed record ReportsTableViewModel(
    IReadOnlyList<Report> Reports,
    bool IsDescending,
    bool IsArchive,
    string SortUrl);
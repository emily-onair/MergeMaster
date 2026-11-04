namespace MergeMaster.Api.Models.ViewModels;

public sealed record ReportsTableViewModel(
    IReadOnlyList<Report> Reports,
    bool Descending,
    bool UrgentOnly,
    bool IsArchive);

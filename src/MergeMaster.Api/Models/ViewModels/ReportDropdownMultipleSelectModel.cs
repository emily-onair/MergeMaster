namespace MergeMaster.Api.Models.ViewModels;

public sealed record ReportDropdownMultipleSelectModel(
    IReadOnlyList<string> Selectable,
    IReadOnlyList<string> Selected,
    string Label
);
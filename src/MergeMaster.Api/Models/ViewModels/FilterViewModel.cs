namespace MergeMaster.Api.Models.ViewModels;

public sealed record FilterViewModel(
    DropdownOptionLists DropdownOptions,
    ReportFilterOptions FilterOptions
);
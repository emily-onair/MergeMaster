namespace MergeMaster.Api.Models.ViewModels;

public sealed record DropdownOptionLists(
    List<string> Authors, List<string> Tags, List<string> Branches, List<string> Commits);
